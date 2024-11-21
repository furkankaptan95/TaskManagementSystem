using AuthAPI.DTOs;
using AuthAPI.Entities;
using AuthAPI.Enums;
using IdentityModel;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthAPI.Services;
public class AuthService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MongoDbService _mongoDbService;

    public AuthService(MongoDbService mongoDbService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _mongoDbService = mongoDbService;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public async Task<RegistrationResult> RegisterAsync(RegisterDto dto)
    {
        var emailFilter = Builders<UserEntity>.Filter.Eq(user => user.Email, dto.Email);
        var usernameFilter = Builders<UserEntity>.Filter.Eq(user => user.Username, dto.Username);

        var isEmailAlreadyTaken = await _mongoDbService.GetUserAsync(emailFilter);
        var isUsernameAlreadyTaken = await _mongoDbService.GetUserAsync(usernameFilter);

        if (isEmailAlreadyTaken is not null && isUsernameAlreadyTaken is not null)
        {
            return new RegistrationResult(false, "Email and Username already taken!", RegistrationError.BothTaken);
        }

        else if (isEmailAlreadyTaken is null && isUsernameAlreadyTaken is not null)
        {
            return new RegistrationResult(false, "Username already taken!", RegistrationError.UsernameTaken);
        }

        else if (isEmailAlreadyTaken is not null && isUsernameAlreadyTaken is null)
        {
            return new RegistrationResult(false, "Email already taken!", RegistrationError.EmailTaken);
        }

        var userEntity = new UserEntity();

        byte[] passwordHash, passwordSalt;

        HashingHelper.CreatePasswordHash(dto.Password, out passwordHash, out passwordSalt);

        userEntity.Email = dto.Email;
        userEntity.Username = dto.Username;
        userEntity.PasswordHash = passwordHash;
        userEntity.PasswordSalt = passwordSalt;

        await _mongoDbService.CreateUserAsync(userEntity);

        var token = Guid.NewGuid().ToString().Substring(0, 6);

        var userVerification = new UserVerificationEntity
        {
            UserId = userEntity.Id.ToString(),
            Token = token
        };

        await _mongoDbService.CreateUserVerificationAsync(userVerification);

        return new RegistrationResult(true, "Registered successfully. Please visit your Email to verif your account.", RegistrationError.None);
    }

    public async Task<ServiceResult<TokensDto>> LoginAsync(LoginDto dto)
    {
        var user = await _mongoDbService.GetUserWithTokensAsync(dto.Email);

        if (user == null)
        {
            return new ServiceResult<TokensDto>(false, "User not found.");
        }

        if (!HashingHelper.VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
        {
            return new ServiceResult<TokensDto>(false, "Wrong password.");
        }

        await _mongoDbService.UpdateAllTokensToRevokedAsync(user.Id.ToString());

        string jwt = GenerateJwtToken(user);

        string refreshTokenString = GenerateRefreshToken();

        var refreshToken = new RefreshTokenEntity
        {
            Token = refreshTokenString,
            UserId = user.Id.ToString(),
            ExpireDate = DateTime.UtcNow.AddDays(7),
        };

        await _mongoDbService.CreateRefreshTokenAsync(refreshToken);

        var tokensDto = new TokensDto
        {
            JwtToken = jwt,
            RefreshToken = refreshTokenString
        };

        return new ServiceResult<TokensDto>(true, "Logged in successfully.", tokensDto);
    }

    public async Task<ServiceResult<TokensDto>> RefreshTokenAsync(string token)
    {
        var refreshToken = await _mongoDbService.GetRefreshTokenAsync(token);

        if (refreshToken == null)
        {
            return new ServiceResult<TokensDto>(false, "No valid token exists.");
        }

        var newJwt = GenerateJwtToken(refreshToken.User);
        var newRefreshTokenString = GenerateRefreshToken();

        var newRefreshToken = new RefreshTokenEntity
        {
            Token = newRefreshTokenString,
            UserId = refreshToken.UserId,
            ExpireDate = DateTime.UtcNow.AddDays(7),
        };

        await _mongoDbService.CreateRefreshTokenAsync(newRefreshToken);

        var tokens = new TokensDto
        {
            JwtToken = newJwt,
            RefreshToken = newRefreshTokenString
        };

        return new ServiceResult<TokensDto>(true, "Refresh Token used successfuly.", tokens);
    }

    public async Task<ServiceResult> VerifyEmailAsync(VerifyEmailDto dto)
    {
        var userVerificationFilter = Builders<UserVerificationEntity>.Filter.Eq(rt => rt.Token, dto.Token);

        var userVerification = await _mongoDbService.GetUserVerificationAsync(userVerificationFilter);

        if (userVerification is null)
        {
            return new ServiceResult(false, "User Verification Entity not found");
        }

        if (userVerification.Expiration < DateTime.UtcNow || userVerification.User.Email != dto.Email)
        {
            return new ServiceResult(false, "No Valid User Verification found");
        }

        var userFilter = Builders<UserEntity>.Filter.Eq(user => user.Id, new ObjectId(userVerification.UserId));

        var updateUser = Builders<UserEntity>.Update
            .Set(user => user.IsActive, true);

         await _mongoDbService.UpdateUserAsync(userFilter, updateUser);

        var deleteUserVerificationFilter = Builders<UserVerificationEntity>.Filter.Eq(uv => uv.Id, userVerification.Id);

        await _mongoDbService.DeleteUserVerificationAsync(deleteUserVerificationFilter);

        return new ServiceResult(true,"Account verified successfully.");
    }

    public ServiceResult ValidateToken(string token)
    {
        var parts = token.Split('.');

        if (parts.Length != 3)
        {
            return new ServiceResult(false,"Wrong JWT Format");
        }

        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token))
        {
            return new ServiceResult(false, "Token cannot be read or Wrong JWT Format");
        }

        var header = parts[0];
        var payload = parts[1];
        var signature = parts[2];

        var computedSignature = CreateSignature(header, payload, _configuration["Jwt:Key"]);

        if (computedSignature == signature)
        {
            return new ServiceResult(true,"Valid JWT Token");
        }

        return new ServiceResult(false, "Invalid Token");
    }

    private string GenerateJwtToken(UserEntity user)
    {
        var claims = new List<Claim>
           {
                new Claim(JwtClaimTypes.Subject,user.Id.ToString()),
                new Claim(JwtClaimTypes.Email,user.Email),
                new Claim(JwtClaimTypes.Role, user.Role),
                new Claim(JwtClaimTypes.Name,user.Username),
            };

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        var jwt = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
        claims = claims,
        notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpireMinutes")),
            signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
            );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(jwt);

        return tokenString;
    }

    private string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }

    private static string CreateSignature(string header, string payload, string secret)
    {
        var key = Encoding.UTF8.GetBytes(secret);

        using (var algorithm = new HMACSHA256(key))
        {
            var data = Encoding.UTF8.GetBytes(header + "." + payload);
            var hash = algorithm.ComputeHash(data);
            return Base64UrlEncode(hash);
        }
    }

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

}

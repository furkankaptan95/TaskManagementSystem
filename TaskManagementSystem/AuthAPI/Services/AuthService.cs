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
        userEntity.Firstname = dto.Firstname;
        userEntity.Lastname = dto.Lastname;
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
        var userFilter = Builders<UserEntity>.Filter.Eq(user => user.Email, dto.Email);
        var user = await _mongoDbService.GetUserAsync(userFilter);

        if (user == null)
        {
            return new ServiceResult<TokensDto>(false, "User not found.");
        }

        if(user.IsActive is false)
        {
            return new ServiceResult<TokensDto>(false,"Please visit your email and verify your account to log in.");
        }

        if (!HashingHelper.VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
        {
            return new ServiceResult<TokensDto>(false, "Wrong password.");
        }

        var filter = Builders<RefreshTokenEntity>.Filter.Eq(token => token.UserId, user.Id.ToString());
        var update = Builders<RefreshTokenEntity>.Update.Set(token => token.IsRevoked, true);

        await _mongoDbService.UpdateManyRefreshTokensAsync(filter, update);

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
        var filterBuilder = Builders<RefreshTokenEntity>.Filter;
        var filter = filterBuilder.Eq(rt => rt.Token, token) &
                     filterBuilder.Gt(rt => rt.ExpireDate, DateTime.UtcNow) &
                     filterBuilder.Eq(rt => rt.IsRevoked, false) &
                     filterBuilder.Eq(rt => rt.IsUsed, false);

        var refreshToken = await _mongoDbService.GetRefreshTokenAsync(filter);

        if (refreshToken == null)
        {
            return new ServiceResult<TokensDto>(false, "No valid token exists.");
        }

        var updateFilter = Builders<RefreshTokenEntity>.Filter.Eq(rt => rt.Token, refreshToken.Token);

        var update = Builders<RefreshTokenEntity>.Update.Set(rt => rt.IsUsed, true);

        await _mongoDbService.UpdateSingleRefreshTokenAsync(updateFilter, update);

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
            return new ServiceResult(false, "User Verification not found");
        }

        if (userVerification.Expiration < DateTime.UtcNow || userVerification.User.Email != dto.Email)
        {
            return new ServiceResult(false, "No valid User Verification found");
        }

        var userFilter = Builders<UserEntity>.Filter.Eq(user => user.Id, new ObjectId(userVerification.UserId));

        var updateUser = Builders<UserEntity>.Update
            .Set(user => user.IsActive, true);

        await _mongoDbService.UpdateUserAsync(userFilter, updateUser);

        var deleteUserVerificationFilter = Builders<UserVerificationEntity>.Filter.Eq(uv => uv.Id, userVerification.Id);

        await _mongoDbService.DeleteUserVerificationAsync(deleteUserVerificationFilter);

        return new ServiceResult(true, "Account verified successfully. Now you can log in.");
    }

    public ServiceResult ValidateToken(string token)
    {
        var parts = token.Split('.');

        if (parts.Length != 3)
        {
            return new ServiceResult(false, "Wrong JWT Format");
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
            return new ServiceResult(true, "Valid JWT Token");
        }

        return new ServiceResult(false, "Invalid Token");
    }

    public async Task<ServiceResult> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        var userFilter = Builders<UserEntity>.Filter.Eq(u => u.Email, forgotPasswordDto.Email);
        var userEntity = await _mongoDbService.GetUserAsync(userFilter);

        if (userEntity is null)
        {
            return new ServiceResult(false, "User not found");
        }

        var token = Guid.NewGuid().ToString().Substring(0, 6);

        var forgotPasswordEntity = new UserVerificationEntity
        {
            UserId = userEntity.Id.ToString(),
            Token = token,
        };

        await _mongoDbService.CreateUserVerificationAsync(forgotPasswordEntity);

        return new ServiceResult(true, "Please check your Email to renew your password.");
    }

    public async Task<ServiceResult<string>> RenewPasswordVerifyEmailAsync(RenewPasswordDto dto)
    {
        var userVerificationFilter = Builders<UserVerificationEntity>.Filter.Eq(uv => uv.Token, dto.Token);

        var userVerification = await _mongoDbService.GetUserVerificationAsync(userVerificationFilter);

        if (userVerification == null || userVerification.Expiration < DateTime.UtcNow || userVerification.User.Email != dto.Email)
        {
            return new ServiceResult<string>(false, "No valid User Verification.");
        }

        return new ServiceResult<string>(true, "Successfully verified. You can change password.",dto.Token);
    }

    public async Task<ServiceResult> NewPasswordAsync(NewPasswordDto dto)
    {
        var userVerificationFilter = Builders<UserVerificationEntity>.Filter.Eq(uv => uv.Token, dto.Token);

        var userVerification = await _mongoDbService.GetUserVerificationAsync(userVerificationFilter);

        if (userVerification == null || userVerification.Expiration < DateTime.UtcNow || userVerification.User.Email != dto.Email)
        {
            return new ServiceResult(false, "No valid User Verification.");
        }

        var userFilter = Builders<UserEntity>.Filter.Eq(u => u.Email, dto.Email); 
        var user = await _mongoDbService.GetUserAsync(userFilter);

        if (user is null)
        {
            return new ServiceResult(false, "User not found.");
        }

        byte[] passwordHash, passwordSalt;

        HashingHelper.CreatePasswordHash(dto.Password, out passwordHash, out passwordSalt);

        var update = Builders<UserEntity>.Update
        .Set(u => u.PasswordHash, passwordHash)
        .Set(u => u.PasswordSalt, passwordSalt);

        await _mongoDbService.UpdateUserAsync(userFilter, update);

        var deleteUserVerificationFilter = Builders<UserVerificationEntity>.Filter.Eq(uv => uv.Id, userVerification.Id);

        await _mongoDbService.DeleteUserVerificationAsync(deleteUserVerificationFilter);

        return new ServiceResult(true, "Password changed successfully.");
    }

    public async Task<ServiceResult> NewVerificationAsync(NewVerificationMailDto dto)
    {
        var userFilter = Builders<UserEntity>.Filter.Eq(u => u.Email, dto.Email);
        var user = await _mongoDbService.GetUserAsync(userFilter);

        if (user is null)
        {
            return new ServiceResult(false, "User not found.");
        }

        var token = Guid.NewGuid().ToString().Substring(0, 6);

        var newVerification = new UserVerificationEntity
        {
            UserId = user.Id.ToString(),
            Token = token,
        };

        await _mongoDbService.CreateUserVerificationAsync(newVerification);

        return new ServiceResult(true, "Mail sended successfully. Please visit your Email to verif your account.");
    }

    public async Task<ServiceResult> RevokeTokenAsync(string token)
    {
        var refreshTokenFilter = Builders<RefreshTokenEntity>.Filter.Eq(rt => rt.Token, token);
        var refreshToken = await _mongoDbService.GetRefreshTokenAsync(refreshTokenFilter);

        if (refreshToken is null)
        {
            return new ServiceResult(false, "Token to revoke is not found.");
        }

        var update = Builders<RefreshTokenEntity>.Update.Set(rt => rt.IsRevoked, true);

        await _mongoDbService.UpdateSingleRefreshTokenAsync(refreshTokenFilter, update);

        return new ServiceResult(true, "Token revoked successfully.");
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

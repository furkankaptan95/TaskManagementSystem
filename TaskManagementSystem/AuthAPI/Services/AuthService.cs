using AuthAPI.DTOs;
using AuthAPI.Entities;
using IdentityModel;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
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

    public AuthService(MongoDbService mongoDbService,IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _mongoDbService = mongoDbService;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public async Task<ServiceResult<TokensDto>> LoginAsync(LoginDto dto)
    {
        var user = await _mongoDbService.GetUserWithTokensAsync(dto.Email);

        if (user == null)
        {
            return new ServiceResult<TokensDto>(false,"User not found.");
        }

        if (!HashingHelper.VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
        {
            return new ServiceResult<TokensDto>(false,"Wrong password.");
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

        return new ServiceResult<TokensDto>(true,"Logged in successfully.",tokensDto);
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

        return new ServiceResult<TokensDto>(true,"Refresh Token used successfuly.",tokens);
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

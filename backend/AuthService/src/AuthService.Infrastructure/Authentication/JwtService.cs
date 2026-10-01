using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthService.Application;
using AuthService.Application.Interfaces;
using AuthService.Domain.Users;
using CSharpFunctionalExtensions;
using FileService.Contracts.Shared;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Infrastructure.Authentication;

public class JwtService : IJwtService
{
    private readonly JwtOptions _options;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public JwtService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public Result<string, Error> GenerateAccessToken(AppUser user)
    {
        SymmetricSecurityKey? key = GetSigningKey();
        if (key == null)
            return AuthServiceErrors.InvalidJwtConfiguration();

        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Name, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(JwtRegisteredClaimNames.EmailVerified, user.EmailConfirmed.ToString()),
        };
        
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_options.AccessTokenExpirationMinutes),
            signingCredentials: signingCredentials);

        return _tokenHandler.WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken(Guid userId)
    {
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
            .TrimEnd('=')
            .TrimEnd('+', '-')
            .Replace('/', '_');
        
        var expiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenExpirationDays);
        
        return RefreshToken.Create(userId, refreshToken, expiresAt);
    }

    public Result<bool, Error> ValidateAccessToken(string accessToken)
    {
        var key = GetSigningKey();
        if (key == null)
            return AuthServiceErrors.InvalidJwtConfiguration();

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,
            ValidateAudience = true,
            ValidAudience = _options.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        try
        {
            _tokenHandler.ValidateToken(accessToken, parameters, out SecurityToken validatedToken);
            return true;
        }
        catch (SecurityTokenException ex)
        {
            return AuthServiceErrors.InvalidToken();
        }
    }

    private SymmetricSecurityKey? GetSigningKey()
    {
        if (string.IsNullOrEmpty(_options.Secret))
            return null;
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        return key.KeySize >= 256 ? key : null;
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MakeSport.Auth.JwtValidation;
using Microsoft.IdentityModel.Tokens;

namespace VenuesService.IntegrationTests.Infrastructure;

public static class TestJwtFactory
{
    public const string Issuer = "makesport";
    public const string Audience = "makesport-api";
    public const string Secret = "generate_strong_secret_key_for_jwt_options";

    public static string Create(Guid userId, string displayName, string role = AuthRoles.User)
    {
        var claims = new Claim[]
        {
            new("sub", userId.ToString()),
            new("display_name", displayName),
            new(ClaimTypes.Role, role),
        };
        
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)),
            SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: signingCredentials);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
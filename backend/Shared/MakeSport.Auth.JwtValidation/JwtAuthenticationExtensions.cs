using System.Security.Claims;
using System.Text;
using MakeSport.Auth.JwtValidation.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace MakeSport.Auth.JwtValidation;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        IConfigurationSection jwtSection = configuration.GetSection("Jwt");
        var jwt = jwtSection.Get<JwtValidationOptions>() ?? new JwtValidationOptions();
        
        if (string.IsNullOrEmpty(jwt.Secret))
            throw new ApplicationException("Secret is empty or less than 256");

        var keyBytes = Encoding.UTF8.GetBytes(jwt.Secret);
        var key = new SymmetricSecurityKey(keyBytes);
        if (key.KeySize < 256)
            throw new ApplicationException("Secret is empty or less than 256");
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = "sub",
                };
            });

        services.AddAuthorization(o =>
        {
            o.AddPolicy(AuthPolicies.ModeratorOrAbove, p => p.RequireRole(AuthRoles.Moderator, AuthRoles.Admin));
            o.AddPolicy(AuthPolicies.AdminOnly, p => p.RequireRole(AuthRoles.Admin));
        });

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        return services;
    }
}
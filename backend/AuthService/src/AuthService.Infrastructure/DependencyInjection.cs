using AuthService.Application.Interfaces;
using AuthService.Infrastructure.Authentication;
using AuthService.Infrastructure.Persistence.Database;
using AuthService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContext>(
            options => options.UseNpgsql(configuration.GetConnectionString("Postgres")));

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        services.AddScoped<IRefreshTokensRepository, RefreshTokensRepository>();

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"))
            .AddScoped<IJwtService, JwtService>();

        return services;
    }
}
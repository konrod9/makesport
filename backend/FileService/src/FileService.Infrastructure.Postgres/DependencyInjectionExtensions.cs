using FileService.Application;
using FileService.Infrastructure.Postgres.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.Infrastructure.Postgres;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructurePostgres(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IMediaAssetsRepository, MediaAssetsRepository>();

        services.AddDbContextPool<FileServiceDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString("Postgres");
            options.UseNpgsql(connectionString);
        });
        
        services.AddDbContextPool<IReadDbContext, FileServiceDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString("Postgres");
            options.UseNpgsql(connectionString);
        });

        return services;
    }
}
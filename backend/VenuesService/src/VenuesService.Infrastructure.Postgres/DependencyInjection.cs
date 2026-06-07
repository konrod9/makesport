using VenuesService.Application;
using VenuesService.Application.UseCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VenuesService.Infrastructure.Postgres.Database;

namespace VenuesService.Infrastructure.Postgres;

public static class DependencyInjection
{
    public static IServiceCollection AddVenueStorage(this IServiceCollection services, string? dbConnectionString)
    {
        services
            .AddScoped<IVenuesRepository, VenuesRepository>()
            .AddScoped<IGuidFactory, GuidFactory>()
            .AddDbContextPool<IVenuesReadDbContext, VenuesDbContext>(options => 
                options.UseNpgsql(dbConnectionString, o => o.UseNetTopologySuite()));
        
        return services;
    }
}
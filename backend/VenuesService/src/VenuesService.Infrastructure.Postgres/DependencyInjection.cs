using VenuesService.Application;
using VenuesService.Application.UseCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VenuesService.Application.UseCases.GetVenues;
using VenuesService.Infrastructure.Postgres.Database;

namespace VenuesService.Infrastructure.Postgres;

public static class DependencyInjection
{
    public static IServiceCollection AddVenueStorage(this IServiceCollection services, string? dbConnectionString)
    {
        services
            .AddScoped<IGuidFactory, GuidFactory>()
            .AddScoped<IVenueSortingService, VenueSortingService>()
            .AddDbContextPool<VenuesDbContext>(options =>
                options.UseNpgsql(dbConnectionString, o => o.UseNetTopologySuite()))
            .AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<VenuesDbContext>())
            .AddScoped<IApplicationReadDbContext>(sp => sp.GetRequiredService<VenuesDbContext>());
        
        return services;
    }
}
using MakeSport.Application.UseCases.CreateVenue;
using MakeSport.Application.UseCases.GetVenues;
using MakeSport.Infrastructure.Postgres;
using MakeSport.Infrastructure.Postgres.Storages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MakeSport.Infrastructure.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVenueStorage(this IServiceCollection services, string? dbConnectionString)
    {
        services
            .AddScoped<IGetVenuesStorage, GetVenuesStorage>()
            .AddScoped<ICreateVenueStorage, CreateVenueStorage>()
            .AddScoped<IGuidFactory, GuidFactory>()
            .AddDbContextPool<VenueDbContext>(options => options.UseNpgsql(dbConnectionString));
        
        return services;
    }
}
using MakeSport.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MakeSport.Infrastructure.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVenueStorage(this IServiceCollection services, string? dbConnectionString)
    {
        services
            .AddDbContextPool<VenueDbContext>(options => options.UseNpgsql(dbConnectionString));
        
        return services;
    }
}
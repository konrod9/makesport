using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MakeSport.Infrastructure.Postgres;

public static class DependencyInjection
{
    public static IServiceCollection AddVenueStorage(this IServiceCollection services, string? dbConnectionString)
    {
        services
            // TODO: зарегистрировать репозиторий
            .AddScoped<IGuidFactory, GuidFactory>()
            .AddDbContextPool<VenueDbContext>(options => 
                options.UseNpgsql(dbConnectionString, o => o.UseNetTopologySuite()));
        
        return services;
    }
}
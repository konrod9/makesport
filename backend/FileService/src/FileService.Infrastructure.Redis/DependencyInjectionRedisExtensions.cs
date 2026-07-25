using FileService.Application.Redis;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.Infrastructure.Redis;

public static class DependencyInjectionRedisExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RedisOptions>(configuration.GetSection(nameof(RedisOptions)));
        
        var redisOptions = configuration.GetSection(nameof(RedisOptions)).Get<RedisOptions>()!;
        
        services.AddStackExchangeRedisCache(setup =>
        {
            setup.Configuration = redisOptions.ServiceUrl;
        });
        
        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions()
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(redisOptions.LocalCacheExpirationMinutes),
                Expiration = TimeSpan.FromMinutes(redisOptions.ExpirationMinutes)
            };
        });

        return services;
    }
}
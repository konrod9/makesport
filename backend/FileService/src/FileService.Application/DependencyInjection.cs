using FileService.Application.UseCases.CompleteMultipartUpload;
using FileService.Application.UseCases.GetByOwners;
using FileService.Application.UseCases.GetMediaAssetInfo;
using FileService.Application.UseCases.GetMediaAssets;
using FileService.Application.UseCases.StartMultipartUpload;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<StartMultipartUploadUseCase>();
        services.AddScoped<CompleteMultipartUploadUseCase>();
        services.AddScoped<GetMediaAssetsUseCase>();
        services.AddScoped<GetMediaAssetInfoUseCase>();
        services.AddScoped<GetByOwnersUseCase>();

        services.AddStackExchangeRedisCache(setup =>
        {
            setup.Configuration = "localhost:6379";
        });
        
        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions()
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(5),
                Expiration = TimeSpan.FromMinutes(30)
            };
        });
        
        return services;
    }
}
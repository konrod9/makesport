using FileService.Application.UseCases.CompleteMultipartUpload;
using FileService.Application.UseCases.GetMediaAssetInfo;
using FileService.Application.UseCases.GetMediaAssets;
using FileService.Application.UseCases.StartMultipartUpload;
using FluentValidation;
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

        return services;
    }
}
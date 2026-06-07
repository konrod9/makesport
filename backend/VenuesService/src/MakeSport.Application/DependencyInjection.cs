using FileService.Contracts.HttpCommunication;
using FluentValidation;
using MakeSport.Application.UseCases.CreateVenue;
using MakeSport.Application.UseCases.GetVenues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MakeSport.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddVenuesServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddScoped<GetVenuesUseCase>()
            .AddScoped<CreateVenueUseCase>();
        
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddFileServiceHttpCommunication(configuration);

        return services;
    }
}
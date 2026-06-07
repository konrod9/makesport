using FileService.Contracts.HttpCommunication;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VenuesService.Application.UseCases.CreateVenue;
using VenuesService.Application.UseCases.GetVenues;

namespace VenuesService.Application;

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
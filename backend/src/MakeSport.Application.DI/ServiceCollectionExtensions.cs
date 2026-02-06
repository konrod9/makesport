using FluentValidation;
using MakeSport.Application.UseCases.CreateVenue;
using MakeSport.Application.UseCases.GetVenues;
using Microsoft.Extensions.DependencyInjection;

namespace MakeSport.Application.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVenuesServices(this IServiceCollection services)
    {
        services
            .AddScoped<IGetVenuesUseCase, GetVenuesUseCase>()
            .AddScoped<ICreateVenueUseCase, CreateVenueUseCase>();
        
        services
            .AddValidatorsFromAssemblyContaining<CreateVenueCommand>(includeInternalTypes: true);

        return services;
    }
}
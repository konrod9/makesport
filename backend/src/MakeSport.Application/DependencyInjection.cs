using MakeSport.Application.UseCases.CreateVenue;
using MakeSport.Application.UseCases.GetVenues;
using Microsoft.Extensions.DependencyInjection;

namespace MakeSport.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddVenuesServices(this IServiceCollection services)
    {
        services
            .AddScoped<GetVenuesUseCase>()
            .AddScoped<CreateVenueUseCase>();
        
        // TODO: Зарегестрировать валидаторы

        return services;
    }
}
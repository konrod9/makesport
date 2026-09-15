using AuthService.Application.UseCases.Register;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //TODO: services.AddValidatorsFromAssembly()

        services.AddScoped<RegisterUseCase>();

        return services;
    }
}
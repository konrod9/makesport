using AuthService.Application.UseCases.GetCurrentUser;
using AuthService.Application.UseCases.Login;
using AuthService.Application.UseCases.Logout;
using AuthService.Application.UseCases.Refresh;
using AuthService.Application.UseCases.Register;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<RegisterUseCase>();
        services.AddScoped<LoginUseCase>();
        services.AddScoped<RefreshUseCase>();
        services.AddScoped<LogoutUseCase>();
        services.AddScoped<GetCurrentUserUseCase>();

        return services;
    }
}
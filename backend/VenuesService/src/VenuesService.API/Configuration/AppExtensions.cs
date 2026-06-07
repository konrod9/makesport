using VenuesService.API.Middlewares;
using Serilog;

namespace VenuesService.API.Configuration;

public static class AppExtensions
{
    public static IApplicationBuilder ConfigureApp(this WebApplication app)
    {
        app.UseExceptionMiddleware();

        app.UseRequestCorrelationId();
        app.UseSerilogRequestLogging();
        
        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapControllers();

        return app;
    }
}
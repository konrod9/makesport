using AuthService.API.Middlewares;
using Serilog;

namespace AuthService.API.Configuration;

public static class AppExtensions
{
    public static IApplicationBuilder ConfigureApp(this WebApplication app)
    {
        app.UseCors(builder =>
        {
            builder.WithOrigins(
                    "http://localhost:3000",
                    "http://localhost:3001",
                    "http://localhost",
                    "http://frontend:3000",
                    "https://make-sport.ru",
                    "https://www.make-sport.ru")
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
        
        app.UseExceptionMiddleware();
        app.UseRequestCorrelationId();
        app.UseSerilogRequestLogging();
        
        app.UseSwagger();
        app.UseSwaggerUI();
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}
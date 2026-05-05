using MakeSport.API.Middlewares;

namespace MakeSport.API.Configuration;

public static class AppExtensions
{
    public static IApplicationBuilder ConfigureApp(this WebApplication app)
    {
        app.UseExceptionMiddleware();
        
        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapControllers();

        return app;
    }
}
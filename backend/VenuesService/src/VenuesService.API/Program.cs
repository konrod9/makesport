using System.Globalization;
using MakeSport.Auth.JwtValidation;
using Microsoft.OpenApi;
using VenuesService.API.Configuration;
using VenuesService.Application;
using VenuesService.Infrastructure.Postgres;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting application");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilogLogging(builder.Configuration);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Вставить access token из /auth/login (без префикса)",
        });
        options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", doc)] = []
        });
    });

    builder.Services.AddJwtAuthentication(builder.Configuration);
    
    builder.Services.AddVenuesServices(builder.Configuration);
    builder.Services.AddVenueStorage(builder.Configuration.GetConnectionString("Postgres"));

    var app = builder.Build();

    app.ConfigureApp();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

namespace VenuesService.API
{
    public partial class Program;
}
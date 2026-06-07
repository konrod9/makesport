using System.Globalization;
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
    builder.Services.AddSwaggerGen();

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
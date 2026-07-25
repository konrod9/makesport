using System.Globalization;
using FileService.API.Configuration;
using FileService.Application;
using FileService.Infrastructure.Postgres;
using FileService.Infrastructure.Redis;
using FileService.Infrastructure.S3;
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
    
    builder.Services.AddCors();

    builder.Services.AddApplication(builder.Configuration);
    builder.Services.AddInfrastructurePostgres(builder.Configuration);
    builder.Services.AddS3(builder.Configuration);
    builder.Services.AddRedisCache(builder.Configuration);

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

namespace FileService.API
{
    public partial class Program;
}
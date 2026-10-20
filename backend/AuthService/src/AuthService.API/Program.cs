using System.Globalization;
using AuthService.API.Configuration;
using AuthService.Application;
using AuthService.Domain.Users;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Persistence.Database;
using Microsoft.OpenApi;
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

    builder.Services.AddAuthStorage(builder.Configuration);
    builder.Services.AddIdentityCore<AppUser>(options =>
        {
            options.Password.RequiredLength = UserConstraints.PasswordMinLength;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredUniqueChars = 1;
        })
        .AddEntityFrameworkStores<AuthDbContext>();
    
    builder.Services.AddJwtBearer(builder.Configuration);
    builder.Services.AddApplication();

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

namespace AuthService.API
{
    public partial class Program;
}
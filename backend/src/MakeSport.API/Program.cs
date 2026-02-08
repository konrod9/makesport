using MakeSport.API.Middlewares;
using MakeSport.Application.DI;
using MakeSport.Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddVenuesServices();
builder.Services.AddVenueStorage(builder.Configuration.GetConnectionString("Postgres"));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.Run();
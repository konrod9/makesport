using MakeSport.API.Configuration;
using MakeSport.Application;
using MakeSport.Infrastructure.Postgres;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddVenuesServices();
builder.Services.AddVenueStorage(builder.Configuration.GetConnectionString("Postgres"));

var app = builder.Build();

app.ConfigureApp();

app.Run();
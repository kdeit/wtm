using AssistanceSF.BusConsumers;
using Microsoft.EntityFrameworkCore;
using OtusKdeBus;
using WTM.AssistanceDAL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IBusConsumer, BusConsumer>();
builder.Services.AddScoped<IBusProducer, BusProducer>();
builder.Services.AddScoped<AssistanceBusConsumer>();


var connectionString = "Host=localhost;Database=wtm_assistance;Username=postgres;Password=postgres;Port=5432";
if (!builder.Environment.IsDevelopment())
{
    var DB_HOST = Environment.GetEnvironmentVariable("ASSISTANCE_DB_HOST");
    var DB_PORT = Environment.GetEnvironmentVariable("ASSISTANCE_DB_PORT");
    var DB_NAME = Environment.GetEnvironmentVariable("ASSISTANCE_DB_NAME");
    var DB_USER = Environment.GetEnvironmentVariable("ASSISTANCE_DB_USER");
    var DB_PASSWORD = Environment.GetEnvironmentVariable("ASSISTANCE_DB_PASSWORD");
    connectionString =
        $"Host={DB_HOST};Database={DB_NAME};Username={DB_USER};Password={DB_PASSWORD};Port={DB_PORT}";
}
builder.Services.AddDbContext<AssistanceContext>(
    opt => opt.UseNpgsql(connectionString)
);

var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var consumer1 = services.GetService<AssistanceBusConsumer>();
consumer1.Init();

app.Run();
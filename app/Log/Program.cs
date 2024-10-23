using Log.BusConsumers;
using Microsoft.EntityFrameworkCore;
using OtusKdeBus;
using WTM.LogDAL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IBusConsumer, BusConsumer>();
builder.Services.AddScoped<IBusProducer, BusProducer>();
builder.Services.AddScoped<AssistanceBusConsumer>();


var connectionString = "Host=localhost;Database=wtm_log23;Username=postgres;Password=postgres;Port=5432";
if (!builder.Environment.IsDevelopment())
{
    var DB_HOST = Environment.GetEnvironmentVariable("LOG_2023_DB_HOST");
    var DB_PORT = Environment.GetEnvironmentVariable("LOG_2023_DB_PORT");
    var DB_NAME = Environment.GetEnvironmentVariable("LOG_2023_DB_NAME");
    var DB_USER = Environment.GetEnvironmentVariable("LOG_2023_DB_USER");
    var DB_PASSWORD = Environment.GetEnvironmentVariable("DB_PASSWORD");
    connectionString =
        $"Host={DB_HOST};Database={DB_NAME};Username={DB_USER};Password={DB_PASSWORD};Port={DB_PORT}";
}
builder.Services.AddDbContext<Log2023Context>(
    opt => opt.UseNpgsql(connectionString)
);

var connectionString2 = "Host=localhost;Database=wtm_log24;Username=postgres;Password=postgres;Port=5432";
if (!builder.Environment.IsDevelopment())
{
    var DB_HOST = Environment.GetEnvironmentVariable("LOG_2024_DB_HOST");
    var DB_PORT = Environment.GetEnvironmentVariable("LOG_2024_DB_PORT");
    var DB_NAME = Environment.GetEnvironmentVariable("LOG_2024_DB_NAME");
    var DB_USER = Environment.GetEnvironmentVariable("LOG_2024_DB_USER");
    var DB_PASSWORD = Environment.GetEnvironmentVariable("DB_PASSWORD");
    connectionString =
        $"Host={DB_HOST};Database={DB_NAME};Username={DB_USER};Password={DB_PASSWORD};Port={DB_PORT}";
}
builder.Services.AddDbContext<Log2024Context>(
    opt => opt.UseNpgsql(connectionString2)
);

var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context2023 = services.GetRequiredService<Log2023Context>();
var context2024 = services.GetRequiredService<Log2024Context>();

context2023.Database.EnsureDeleted();
context2023.Database.EnsureCreated();

context2024.Database.EnsureDeleted();
context2024.Database.EnsureCreated();

var consumer1 = services.GetService<AssistanceBusConsumer>();
consumer1.Init();

app.Run();
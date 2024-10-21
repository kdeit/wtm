using Assistance;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OtusKdeBus;
using OtusKdeDAL.BusConsumers;
using WTM.AssistanceDAL;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddScoped<IBusConsumer, BusConsumer>();
builder.Services.AddScoped<IBusProducer, BusProducer>();
builder.Services.AddScoped<AssistanceBusConsumer>();
builder.Services.AddScoped<IncidentCreatedSaga>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost";
    options.InstanceName = "wtm";
});

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

builder.Services.AddOpenTelemetry().WithMetrics(builder =>
{
    builder.AddPrometheusExporter();
    builder.AddMeter("Microsoft.AspNetCore.Hosting",
        "Microsoft.AspNetCore.Server.Kestrel");
    builder.AddView("http.server.request.duration",
        new ExplicitBucketHistogramConfiguration
        {
            Boundaries = new double[]
            {
                0, 0.005, 0.01, 0.025, 0.05,
                0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10
            }
        });
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<AssistanceContext>();
//context.Database.EnsureDeleted();
context.Database.EnsureCreated();

var transaction = services.GetService<IncidentCreatedSaga>();
transaction.handle();

app.MapControllers();
app.MapPrometheusScrapingEndpoint();
Console.WriteLine("Start «Assistance» service");
app.Run();
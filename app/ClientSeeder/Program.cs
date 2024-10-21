using Bogus;
using Microsoft.EntityFrameworkCore;
using WTM.ClientDAL;
using WTM.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Host=localhost;Database=otus;Username=postgres;Password=postgres;Port=5432";
if (!builder.Environment.IsDevelopment())
{
    var DB_HOST = Environment.GetEnvironmentVariable("DB_HOST");
    var DB_PORT = Environment.GetEnvironmentVariable("DB_PORT");
    var DB_NAME = Environment.GetEnvironmentVariable("DB_NAME");
    var DB_USER = Environment.GetEnvironmentVariable("DB_USER");
    var DB_PASSWORD = Environment.GetEnvironmentVariable("DB_PASSWORD");
    connectionString =
        $"Host={DB_HOST};Database={DB_NAME};Username={DB_USER};Password={DB_PASSWORD};Port={DB_PORT}";
}

builder.Services.AddDbContext<ClientContext>(
    opt => opt.UseNpgsql(connectionString)
);


var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<ClientContext>();
context.Database.EnsureDeleted();
context.Database.EnsureCreated();

var faker = new Faker<User>()
    .RuleFor(u => u.Firstname, (f) => f.Name.FirstName())
    .RuleFor(u => u.Lastname, (f) => f.Name.LastName())
    .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.Lastname))
    .FinishWith((f, u) =>
{
    Console.WriteLine("User Created! Email={0}", u.Email);
});

context.AddRange(faker.Generate(1000));
await context.SaveChangesAsync();

Console.Write("Task complete");

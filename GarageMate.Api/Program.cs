using GarageMate.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var host = Environment.GetEnvironmentVariable("PG_HOST");
var port = Environment.GetEnvironmentVariable("PG_PORT");
var db = Environment.GetEnvironmentVariable("PG_DB");
var user = Environment.GetEnvironmentVariable("PG_USER");
var password = Environment.GetEnvironmentVariable("PG_PASSWORD");

var connectionString = $"Host={host};Port={port};Database={db};Username={user};Password={password}";

builder.Services.AddDbContext<GarageMateContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

app.MapGet("/", () => "Hello GarageMate!");

app.Run();

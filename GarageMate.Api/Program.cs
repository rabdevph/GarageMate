using System.Text.Json.Serialization;
using GarageMate.Api.Data;
using GarageMate.Api.Endpoints;
using Microsoft.AspNetCore.Http.Json;
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
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(null, allowIntegerValues: false));
});

var app = builder.Build();

app.MapCustomerEndpoints();

app.Run();

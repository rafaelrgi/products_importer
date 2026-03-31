using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Products.Application.Services;
using Products.Domain.Contracts;
using Products.Infra;
using Products.Infra.Repositories;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//DB
var connectionStringTemplate = builder.Configuration.GetConnectionString(Db.ConnectionName)!;
var dbPassword = builder.Configuration["DbPassword"];
var connectionString = string.Format(connectionStringTemplate, dbPassword);
builder.Services.AddDbContext<Db>(options =>
{
  options.UseNpgsql(connectionString);
});

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());

// Add services to the container.
builder.Services.AddTransient<HttpClient, HttpClient>();
builder.Services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductImporterService, ProductImporterService>();

// Authorization
builder.Services.AddRsaJwtAuthentication(builder.Configuration);

//Require authorization by default for all requests
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

//Cors configuration
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAngularApp",
      builder =>
      {
        builder.WithOrigins("http://localhost:4200")
                 .AllowAnyHeader()
                 .AllowAnyMethod();
      });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("AllowAngularApp");
app.UseAuthorization();
app.MapControllers();
app.Run();

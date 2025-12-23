using cs_api;
using cs_api.Controllers;
using cs_api.Data;
using cs_api.Repositories;
using cs_api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Db>(options =>
{
  options.UseMySQL(builder.Configuration.GetConnectionString(Db.ConnectionName)!);
});

// Add services to the container.
builder.Services.AddScoped<HttpClient, HttpClient>();
builder.Services.AddScoped<DefaultController, DefaultController>();
builder.Services.AddScoped<ExchangeRateRepository, ExchangeRateRepository>();
builder.Services.AddScoped<ExchangeRateService, ExchangeRateService>();
builder.Services.AddTransient<ExchangeRateController, ExchangeRateController>();
builder.Services.AddScoped<ProductRepository, ProductRepository>();
builder.Services.AddScoped<ProductService, ProductService>();
builder.Services.AddTransient<ProductController, ProductController>();

// Configure the HTTP request pipeline.
var app = builder.Build();

app.MapRoutes();

app.Run();

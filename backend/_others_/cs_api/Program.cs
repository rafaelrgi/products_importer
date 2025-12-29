using cs_api.src.Application.Services;
using cs_api.src.Infra;
using cs_api.src.Infra.Repositories;
using cs_api.src.Web;
using cs_api.src.Web.Controllers;
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

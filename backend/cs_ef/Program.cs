using cs_ef.src.Application.Services;
using cs_ef.src.Data;
using cs_ef.src.Domain.Contracts;
using cs_ef.src.Infra.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<Db>(options =>
{
  options.UseMySQL(builder.Configuration.GetConnectionString(Db.ConnectionName)!);
});

// Add services to the container.
builder.Services.AddTransient<HttpClient, HttpClient>();
builder.Services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.Run();

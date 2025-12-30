using cs_ef.src.Application.Services;
using cs_ef.src.Data;
using cs_ef.src.Domain.Contracts;
using cs_ef.src.Infra.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<Db>(options =>
{
  options.UseMySQL(builder.Configuration.GetConnectionString(Db.ConnectionName)!);
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
var publicKey = builder.Configuration["RsaKeys:PublicKey"] ?? "";
var rsa = RSA.Create();
rsa.ImportFromPem(publicKey.ToCharArray());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
  //bearer.SaveToken = true;
  options.RequireHttpsMetadata = false;
  options.Authority = JwtBearerDefaults.AuthenticationScheme;
  options.Audience = JwtBearerDefaults.AuthenticationScheme;
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new RsaSecurityKey(rsa),
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
  };
});

//Require authorization by default for all requests
builder.Services.AddAuthorization(options =>
{
  options.FallbackPolicy = new AuthorizationPolicyBuilder()
      .RequireAuthenticatedUser()
      .Build();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.Run();

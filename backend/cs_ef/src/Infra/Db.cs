using cs_ef.src.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace cs_ef.src.Data
{
  public class Db : DbContext
  {
    private const string CONNECTION_NAME = "DefaultConnection";
    private const string DOCKER_CONNECTION_NAME = "DockerConnection";

    public static string ConnectionName { get => Environment.GetEnvironmentVariable("IS_DOCKER") == "true" ? DOCKER_CONNECTION_NAME : CONNECTION_NAME; }
    public Db(DbContextOptions<Db> options) : base(options) { }

    public DbSet<ExchangeRate> ExchangeRates { get; set; }
    public DbSet<Product> Products { get; set; }

  }
}

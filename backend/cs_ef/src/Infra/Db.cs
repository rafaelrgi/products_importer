using cs_ef.src.Domain.Entities;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Product>().HasQueryFilter(p => p.DeletedAt == null);
    }

    public override int SaveChanges()
    {
      AddTimestamps();
      return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
    {
      AddTimestamps();
      return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
      AddTimestamps();
      return await base.SaveChangesAsync(cancellationToken);
    }

    private void AddTimestamps()
    {
      var entities = ChangeTracker
          .Entries()
          .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

      foreach (var entity in entities)
      {
        var now = DateTime.Now; //UtcNow ???
        if (entity.State == EntityState.Added)
          ((BaseEntity)entity.Entity).CreatedAt = now;

        ((BaseEntity)entity.Entity).UpdatedAt = now;
      }
    }
  }
}

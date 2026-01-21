using cs_ef.src.Data;
using cs_ef.src.Domain.Entities;
using cs_ef.src.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Tests
{
  [TestClass]
  public sealed class Test_ProductRepository
  {
    [TestMethod]
    public async Task Test_FindAll()
    {
      // Arrange
      var options = new DbContextOptionsBuilder<Db>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

      List<Product> result = [];
      using (var context = new Db(options))
      {
        context.Products.Add(new Product { Id = 1, Name = "Laptop", Quantity = 16, Expiration = DateTime.Now, Price = 256 });
        context.Products.Add(new Product { Id = 2, Name = "Mouse", Quantity = 64, Expiration = DateTime.Now, Price = 8 });
        context.SaveChanges();

        // Act      
        var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);
        result = await repository.FindAll("name", "asc");
      }

      // Assert
      Assert.IsNotNull(result);
      Assert.HasCount(2, result);
      Assert.IsInstanceOfType<List<Product>>(result);
      Assert.IsNotNull(result[0]);
      Assert.IsNotNull(result[1]);
    }
  }
}

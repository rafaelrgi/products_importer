using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Products.Domain.Common;
using Products.Domain.Entities;
using Products.Infra;
using Products.Infra.Repositories;
using Xunit;

namespace Tests
{
  public sealed class ProductRepositoryTests: IDisposable
  {
    private ProductRepository? _repository;
    private Db? _context;

    public ProductRepositoryTests()
    {
      // Arrange
      var options = new DbContextOptionsBuilder<Db>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

      _context = new Db(options);
      _context.Database.EnsureCreated();
      _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
      _context.Products.Add(new Product { Id = 01, Name = "Laptop", Quantity = 16, Expiration = DateTime.UtcNow, Price = 512 });
      _context.Products.Add(new Product { Id = 02, Name = "Mouse", Quantity = 64, Expiration = DateTime.UtcNow, Price = 2 });
      _context.Products.Add(new Product { Id = 03, Name = "Keyboard", Quantity = 32, Expiration = DateTime.UtcNow, Price = 8 });
      _context.Products.Add(new Product { Id = 04, Name = "Headset", Quantity = 8, Expiration = DateTime.UtcNow, Price = 8 });
      _context.Products.Add(new Product { Id = 05, Name = "Ram 16 GB", Quantity = 8, Expiration = DateTime.UtcNow, Price = 16 });
      _context.Products.Add(new Product { Id = 06, Name = "SSD 1 TB M2", Quantity = 8, Expiration = DateTime.UtcNow, Price = 64 });
      _context.Products.Add(new Product { Id = 07, Name = "Webcam", Quantity = 2, Expiration = DateTime.UtcNow, Price = 2 });
      _context.Products.Add(new Product { Id = 08, Name = "Desktop", Quantity = 16, Expiration = DateTime.UtcNow, Price = 512 });
      _context.Products.Add(new Product { Id = 09, Name = "Monitor", Quantity = 32, Expiration = DateTime.UtcNow, Price = 128 });
      _context.Products.Add(new Product { Id = 10, Name = "SmartPhone", Quantity = 16, Expiration = DateTime.UtcNow, Price = 256 });
      _context.Products.Add(new Product { Id = 11, Name = "Deleted", Quantity = 1, Expiration = DateTime.UtcNow, Price = 32, DeletedAt = DateTime.UtcNow });
      _context.SaveChanges();
      _context.ChangeTracker.Clear();

      _repository = new ProductRepository(_context, NullLogger<ProductRepository>.Instance);

      Assert.NotNull(_context);
      Assert.NotNull(_repository);
    }

    public void Dispose()
    {
      _context!.Dispose();
      _context = null;
      _repository = null;
    }


    [Fact]
    public async Task Test_FindAll()
    {
      //Arrange
      var repository = _repository!;

      // Act
      var result = await repository.FindAll("name", "asc");

      // Assert
      Assert.NotNull(result);
      Assert.Equal(10, result.Count);
      Assert.IsType<List<Product>>(result);
      Assert.NotNull(result[0]);
      Assert.NotNull(result[9]);
    }

    [Fact]
    public async Task Test_Order()
    {
      //Arrange
      var repository = _repository!;

      // Act
      var asc = await repository.FindAll("name", "asc");
      var desc = await repository.FindAll("name", "desc");

      // Assert
      Assert.Equal("Desktop", asc[0].Name);
      Assert.Equal("Headset", asc[1].Name);
      Assert.Equal("SSD 1 TB M2", asc[8].Name);
      Assert.Equal("Webcam", asc[9].Name);

      Assert.Equal("Webcam", desc[0].Name);
      Assert.Equal("SSD 1 TB M2", desc[1].Name);
      Assert.Equal("Headset", desc[8].Name);
      Assert.Equal("Desktop", desc[9].Name);
    }

    [Fact]
    public async Task Test_Sort()
    {
      //Arrange
      var repository = _repository!;

      // Act
      var price = await repository.FindAll("price", "desc");
      var qty = await repository.FindAll("quantity", "asc");

      // Assert
      Assert.Equal(512, price[0].Price);
      Assert.Equal(2, price[9].Price);

      Assert.Equal(2, qty[0].Quantity);
      Assert.Equal(64, qty[9].Quantity);
    }

    [Fact]
    public async Task Test_Paginated()
    {
      //Arrange
      var repository = _repository!;

      // Act
      var page1 = await repository.FindAllPaginated(1, 2, "id", "asc");
      var page2 = await repository.FindAllPaginated(2, 2, "id", "asc");
      var page5 = await repository.FindAllPaginated(5, 2, "id", "asc");
      var page6 = await repository.FindAllPaginated(6, 2, "id", "asc");

      // Assert
      Assert.NotNull(page1);
      Assert.IsType<Pagination<Product>>(page2);
      Assert.IsType<Pagination<Product>>(page5);
      Assert.IsType<List<Product>>(page5.Data);

      Assert.Equal(10, page1.RecordCount);
      Assert.Equal(10, page5.RecordCount);
      Assert.Equal(10, page6.RecordCount);
      Assert.Equal(5, page1.PageCount);
      Assert.Equal(2, page2.Page);
      Assert.Equal(5, page5.Page);
      Assert.Equal(6, page6.Page);

      Assert.True(page1.HasData);
      Assert.True(page2.HasData);
      Assert.True(page5.HasData);
      Assert.False(page6.HasData);
      Assert.NotNull(page1.Data);
      Assert.NotNull(page2.Data);
      Assert.NotNull(page5.Data);

      Assert.Equal(1, page1.Data![0].Id);
      Assert.Equal(4, page2.Data![1].Id);
      Assert.Equal(9, page5.Data![0].Id);
      Assert.Equal(10, page5.Data![1].Id);
    }

    [Fact]
    public async Task Test_Find()
    {
      //Arrange
      var repository = _repository!;

      // Act
      var prod4 = await repository.Find(4);
      var prod7 = await repository.Find(7);
      var prod0 = await repository.Find(11, false);
      var prod11 = await repository.Find(11, true);

      // Assert
      Assert.Null(prod0);
      Assert.NotNull(prod4);
      Assert.NotNull(prod7);
      Assert.NotNull(prod11);

      Assert.Equal(4, prod4.Id);
      Assert.Equal(7, prod7.Id);
      Assert.Equal(11, prod11.Id);
    }

    [Fact]
    public async Task Test_Save()
    {
      //Arrange
      var repository = _repository!;

      // Act
      //{ Id = 04, Name = "Headset", Quantity = 8, Expiration = DateTime.Now, Price = 8 }
      var prod = await repository.Find(4);

      //Check the product, just to be shure it was really changed
      Assert.NotNull(prod);
      Assert.Equal("Headset", prod.Name);
      Assert.Equal(8, prod.Quantity);
      Assert.Equal(8m, prod.Price);

      //update
      prod.Price = 12.99m;
      prod.Quantity = 6;
      prod.Name = "HeadphoneS";
      await repository.Save(prod);

      //insert
      var prod3 = new Product()
      {
        Name = "Inserted",
        Quantity = 7,
        Expiration = DateTime.Now,
        Price = 9.5m
      };
      await repository.Save(prod3);

      var other = await repository.Find(4);
      var other2 = await repository.Find(12);
      //Assert
      Assert.NotNull(other);
      Assert.Equal("HeadphoneS", other.Name);
      Assert.Equal(6, other.Quantity);
      Assert.Equal(12.99m, other.Price);

      Assert.NotNull(other2);
      Assert.Equal("Inserted", other2.Name);
      Assert.Equal(7, other2.Quantity);
      Assert.Equal(9.5m, other2.Price);
    }

    [Fact]
    public async Task Test_Delete()
    {
      //Arrange
      var repository = _repository!;

      // Act
      var before = await repository.FindAll("name", "asc");
      var row = await repository.Find(3);
      Assert.NotNull(row);

      var ok = await repository.Delete(row);
      var after = await repository.FindAll("name", "asc");

      // Assert
      Assert.Equal(10, before.Count);
      Assert.Equal(9, after.Count);
      Assert.True(ok);
      Assert.Null(await repository.Find(3, false));
    }

    [Fact]
    public async Task Test_UnDelete()
    {
      //Arrange
      var repository = _repository!;

      // Act
      var beforeDelete = (await repository.FindAll("name", "asc")).Count;
      var row = await repository.Find(7);
      Assert.NotNull(row);

      var okDelete = await repository.Delete(row);
      var afterDelete1 = (await repository.FindAll("name", "asc")).Count;
      var deleted = await repository.Find(7);
      row = null;

      row = await repository.Find(4);
      Assert.NotNull(row);
      await repository.Delete(row);
      var afterDelete2 = (await repository.FindAll("name", "asc")).Count;

      var row2 = await repository.Find(7, true);
      Assert.NotNull(row2);
      var okRestore = await repository.UnDelete(row2);
      var afterRestore = (await repository.FindAll("name", "asc")).Count;
      var restored = await repository.Find(7);

      // Assert
      Assert.Equal(10, beforeDelete);
      Assert.Equal(9, afterDelete1);
      Assert.Equal(8, afterDelete2);
      Assert.Equal(9, afterRestore);
      Assert.True(okDelete);
      Assert.True(okRestore);
      Assert.Null(await repository.Find(4, false));
      Assert.NotNull(await repository.Find(4, true));
      Assert.NotNull(await repository.Find(7, false));
    }

    [Fact]
    public async Task Test_SaveProducts()
    {
      //Arrange
      var repository = _repository!;

      // Act
      var before = (await repository.FindAll("name", "asc")).Count;

      var rows = new Product[]
      {
        new Product { Name = "Inserted 1", Quantity = 7, Expiration = DateTime.Now, Price = 9.5m },
        new Product { Name = "Inserted 2", Quantity = 9, Expiration = DateTime.Now, Price = 7.5m },
        new Product { Name = "Inserted 3", Quantity = 5, Expiration = DateTime.Now, Price = 5m },
        new Product { Name = "Inserted 4", Quantity = 3, Expiration = DateTime.Now, Price = 3.33m },
      };

      var ok = await repository.SaveProducts(rows, 3);

      var after = (await repository.FindAll("name", "asc")).Count;
      var row13 = await repository.Find(13);

      // Assert
      Assert.True(ok);
      Assert.Equal(10, before);
      Assert.Equal(13, after);
      Assert.NotNull(row13);
      Assert.Equal("Inserted 2", row13.Name);
      Assert.Equal(9, row13.Quantity);
    }
  }
}

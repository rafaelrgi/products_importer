using cs_ef.src.Data;
using cs_ef.src.Domain.Core;
using cs_ef.src.Domain.Entities;
using cs_ef.src.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Tests
{
  [TestClass]
  public sealed class Test_ProductRepository
  {
    private ProductRepository? _repository;
    private Db? _context;

    [TestInitialize]
    public void Setup()
    {
      // Arrange
      var options = new DbContextOptionsBuilder<Db>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

      _context = new Db(options);
      _context.Database.EnsureCreated();
      _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
      _context.Products.Add(new Product { Id = 01, Name = "Laptop", Quantity = 16, Expiration = DateTime.Now, Price = 512 });
      _context.Products.Add(new Product { Id = 02, Name = "Mouse", Quantity = 64, Expiration = DateTime.Now, Price = 2 });
      _context.Products.Add(new Product { Id = 03, Name = "Keyboard", Quantity = 32, Expiration = DateTime.Now, Price = 8 });
      _context.Products.Add(new Product { Id = 04, Name = "Headset", Quantity = 8, Expiration = DateTime.Now, Price = 8 });
      _context.Products.Add(new Product { Id = 05, Name = "Ram 16 GB", Quantity = 8, Expiration = DateTime.Now, Price = 16 });
      _context.Products.Add(new Product { Id = 06, Name = "SSD 1 TB M2", Quantity = 8, Expiration = DateTime.Now, Price = 64 });
      _context.Products.Add(new Product { Id = 07, Name = "Webcam", Quantity = 2, Expiration = DateTime.Now, Price = 2 });
      _context.Products.Add(new Product { Id = 08, Name = "Desktop", Quantity = 16, Expiration = DateTime.Now, Price = 512 });
      _context.Products.Add(new Product { Id = 09, Name = "Monitor", Quantity = 32, Expiration = DateTime.Now, Price = 128 });
      _context.Products.Add(new Product { Id = 10, Name = "SmartPhone", Quantity = 16, Expiration = DateTime.Now, Price = 256 });
      _context.Products.Add(new Product { Id = 11, Name = "Deleted", Quantity = 1, Expiration = DateTime.Now, Price = 32, DeletedAt = DateTime.Now });
      _context.SaveChanges();
      _context.ChangeTracker.Clear();

      _repository = new ProductRepository(_context, NullLogger<ProductRepository>.Instance);

      Assert.IsNotNull(_context);
      Assert.IsNotNull(_repository);
    }

    [TestCleanup]
    public void Teardown()
    {
      _context!.Dispose();
      _context = null;
      _repository = null;
    }


    [TestMethod]
    public async Task Test_FindAll()
    {
      //Arrange
      var repository = _repository!;

      // Act
      var result = await repository.FindAll("name", "asc");

      // Assert
      Assert.IsNotNull(result);
      Assert.HasCount(10, result);
      Assert.IsInstanceOfType<List<Product>>(result);
      Assert.IsNotNull(result[0]);
      Assert.IsNotNull(result[9]);
    }

    [TestMethod]
    public async Task Test_Order()
    {
      //Arrange
      var repository = _repository!;

      // Act
      var asc = await repository.FindAll("name", "asc");
      var desc = await repository.FindAll("name", "desc");

      // Assert
      Assert.AreEqual("Desktop", asc[0].Name);
      Assert.AreEqual("Headset", asc[1].Name);
      Assert.AreEqual("SSD 1 TB M2", asc[8].Name);
      Assert.AreEqual("Webcam", asc[9].Name);

      Assert.AreEqual("Webcam", desc[0].Name);
      Assert.AreEqual("SSD 1 TB M2", desc[1].Name);
      Assert.AreEqual("Headset", desc[8].Name);
      Assert.AreEqual("Desktop", desc[9].Name);
    }

    [TestMethod]
    public async Task Test_Sort()
    {
      //Arrange
      var repository = _repository!;

      // Act
      var price = await repository.FindAll("price", "desc");
      var qty = await repository.FindAll("quantity", "asc");

      // Assert
      Assert.AreEqual(512, price[0].Price);
      Assert.AreEqual(2, price[9].Price);

      Assert.AreEqual(2, qty[0].Quantity);
      Assert.AreEqual(64, qty[9].Quantity);
    }

    [TestMethod]
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
      Assert.IsNotNull(page1);
      Assert.IsInstanceOfType<Pagination<Product>>(page2);
      Assert.IsInstanceOfType<Pagination<Product>>(page5);
      Assert.IsInstanceOfType<List<Product>>(page5.Data);

      Assert.AreEqual(10, page1.RecordCount);
      Assert.AreEqual(10, page5.RecordCount);
      Assert.AreEqual(10, page6.RecordCount);
      Assert.AreEqual(5, page1.PageCount);
      Assert.AreEqual(2, page2.Page);
      Assert.AreEqual(5, page5.Page);
      Assert.AreEqual(6, page6.Page);

      Assert.IsTrue(page1.HasData);
      Assert.IsTrue(page2.HasData);
      Assert.IsTrue(page5.HasData);
      Assert.IsFalse(page6.HasData);
      Assert.IsNotNull(page1.Data);
      Assert.IsNotNull(page2.Data);
      Assert.IsNotNull(page5.Data);

      Assert.AreEqual(1, page1.Data![0].Id);
      Assert.AreEqual(4, page2.Data![1].Id);
      Assert.AreEqual(9, page5.Data![0].Id);
      Assert.AreEqual(10, page5.Data![1].Id);
    }

    [TestMethod]
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
      Assert.IsNull(prod0);
      Assert.IsNotNull(prod4);
      Assert.IsNotNull(prod7);
      Assert.IsNotNull(prod11);

      Assert.AreEqual(4, prod4.Id);
      Assert.AreEqual(7, prod7.Id);
      Assert.AreEqual(11, prod11.Id);
    }

    [TestMethod]
    public async Task Test_Save()
    {
      //Arrange
      var repository = _repository!;

      // Act
      //{ Id = 04, Name = "Headset", Quantity = 8, Expiration = DateTime.Now, Price = 8 }
      var prod = await repository.Find(4);

      //Check the product, just to be shure it was really changed 
      Assert.IsNotNull(prod);
      Assert.AreEqual("Headset", prod.Name);
      Assert.AreEqual(8, prod.Quantity);
      Assert.AreEqual(8m, prod.Price);

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
      Assert.IsNotNull(other);
      Assert.AreEqual("HeadphoneS", other.Name);
      Assert.AreEqual(6, other.Quantity);
      Assert.AreEqual(12.99m, other.Price);

      Assert.IsNotNull(other2);
      Assert.AreEqual("Inserted", other2.Name);
      Assert.AreEqual(7, other2.Quantity);
      Assert.AreEqual(9.5m, other2.Price);
    }

    [TestMethod]
    public async Task Test_Delete()
    {
      //Arrange
      var repository = _repository!;

      // Act
      var before = await repository.FindAll("name", "asc");
      var row = await repository.Find(3);
      Assert.IsNotNull(row);

      var ok = await repository.Delete(row);
      var after = await repository.FindAll("name", "asc");

      // Assert
      Assert.HasCount(10, before);
      Assert.HasCount(9, after);
      Assert.IsTrue(ok);
      Assert.IsNull(await repository.Find(3, false));
    }

    [TestMethod]
    public async Task Test_UnDelete()
    {
      //Arrange
      var repository = _repository!;

      // Act
      var beforeDelete = (await repository.FindAll("name", "asc")).Count;
      var row = await repository.Find(7);
      Assert.IsNotNull(row);

      var okDelete = await repository.Delete(row);
      var afterDelete1 = (await repository.FindAll("name", "asc")).Count;
      var deleted = await repository.Find(7);
      row = null;

      row = await repository.Find(4);
      Assert.IsNotNull(row);
      await repository.Delete(row);
      var afterDelete2 = (await repository.FindAll("name", "asc")).Count;

      var row2 = await repository.Find(7, true);
      Assert.IsNotNull(row2);
      var okRestore = await repository.UnDelete(row2);
      var afterRestore = (await repository.FindAll("name", "asc")).Count;
      var restored = await repository.Find(7);

      // Assert
      Assert.AreEqual(10, beforeDelete);
      Assert.AreEqual(9, afterDelete1);
      Assert.AreEqual(8, afterDelete2);
      Assert.AreEqual(9, afterRestore);
      Assert.IsTrue(okDelete);
      Assert.IsTrue(okRestore);
      Assert.IsNull(await repository.Find(4, false));
      Assert.IsNotNull(await repository.Find(4, true));
      Assert.IsNotNull(await repository.Find(7, false));
    }

    [TestMethod]
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
      Assert.IsTrue(ok);
      Assert.AreEqual(10, before);
      Assert.AreEqual(13, after);
      Assert.IsNotNull(row13);
      Assert.AreEqual("Inserted 2", row13.Name);
      Assert.AreEqual(9, row13.Quantity);
    }
  }
}

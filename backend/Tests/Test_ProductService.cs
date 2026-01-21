using cs_ef.src.Application.Dtos;
using cs_ef.src.Application.Services;
using cs_ef.src.Domain.Contracts;
using cs_ef.src.Domain.Entities;
using Moq;

namespace Tests
{
  [TestClass]
  public sealed class Test_ProductService
  {
    [TestMethod]
    public async Task Test_FindAll()
    {
      // Arrange
      var mockRepository = new Mock<IProductRepository>();
      var expectedProducts = new List<Product>
      {
        new() { Id = 1, Name = "Laptop", Quantity = 16, Expiration = DateTime.Now, Price = 256 },
        new() { Id = 2, Name = "Mouse", Quantity = 64, Expiration = DateTime.Now, Price = 8 }
      };

      mockRepository.Setup(repo => repo
        .FindAll("name", "asc"))
        .ReturnsAsync(expectedProducts);

      var productService = new ProductService(mockRepository.Object);


      // Act
      var result = await productService.FindAll("name", "asc");


      // Assert
      Assert.HasCount(2, result);
      Assert.IsInstanceOfType<List<ProductDto>>(result);
      Assert.IsNotNull(result[0]);
      Assert.IsNotNull(result[1]);
      mockRepository.Verify(repo => repo.FindAll("name", "asc"), Times.Once());
    }


  }
}

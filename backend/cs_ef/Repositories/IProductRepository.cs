using cs_ef.Dtos;
using cs_ef.Models;

namespace cs_ef.Repositories
{
  public interface IProductRepository
  {
    public Task<PaginationDto<Product>> FindAll(int page, int perPage, string? sort, string? order, string? name, decimal? priceMin, decimal? priceMax, DateTime? expirationMin, DateTime? expirationMax);
    public Task<bool> SaveProducts(Product[] products, int count);

  }
}
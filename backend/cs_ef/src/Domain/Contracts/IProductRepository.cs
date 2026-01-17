using cs_ef.src.Domain.Core;
using cs_ef.src.Domain.Entities;

namespace cs_ef.src.Domain.Contracts
{
  public interface IProductRepository
  {
    Task<Product?> Find(int id, bool showDeleted = true);
    Task<List<Product>> FindAll(string sort, string order, string? name, decimal? priceMin, decimal? priceMax, DateTime? expirationMin, DateTime? expirationMax);
    Task<Pagination<Product>> FindAllPaginated(int page, int perPage, string sort, string order, string? name, decimal? priceMin, decimal? priceMax, DateTime? expirationMin, DateTime? expirationMax, bool showDeleted);
    Task<bool> SaveProducts(Product[] products, int count);
    Task<bool> Delete(Product row);
    Task<bool> UnDelete(Product row);
    Task<Product> Save(Product row);
  }
}
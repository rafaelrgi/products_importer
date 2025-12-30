using cs_ef.src.Domain.Core;
using cs_ef.src.Domain.Entities;

namespace cs_ef.src.Domain.Contracts
{
  public interface IProductService
  {
    Task<Product?> Find(int id);
    Task<Pagination<Product>> FindAll(int page, int perPage, string? sort, string? order, string? name, decimal? priceMin, decimal? priceMax, DateTime? expirationMin, DateTime? expirationMax);
    Task<bool> Save(Product row);
    Task<bool> Delete(int id);
    Task<bool> UnDelete(int id);
  }
}
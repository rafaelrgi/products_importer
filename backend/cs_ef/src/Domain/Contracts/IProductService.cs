using cs_ef.src.Domain.Core;
using cs_ef.src.Domain.Models;

namespace cs_ef.src.Domain.Contracts
{
  public interface IProductService
  {
    public Task<Pagination<Product>> FindAll(int page, int perPage, string? sort, string? order, string? name, decimal? priceMin, decimal? priceMax, DateTime? expirationMin, DateTime? expirationMax);
    public Task<(int processed, int rejected)> ImportCsv(IFormFile file);    
  }
}
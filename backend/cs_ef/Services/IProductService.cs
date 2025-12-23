using cs_ef.Dtos;
using cs_ef.Models;

namespace cs_ef.Services
{
  public interface IProductService
  {
    public Task<PaginationDto<Product>> FindAll(int page, int perPage, string? sort, string? order, string? name, decimal? priceMin, decimal? priceMax, DateTime? expirationMin, DateTime? expirationMax);
    public Task<(int processed, int rejected)> ImportCsv(IFormFile file);    
  }
}
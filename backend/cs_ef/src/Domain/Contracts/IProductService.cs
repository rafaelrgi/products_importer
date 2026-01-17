using cs_ef.src.Application.Dtos;
using cs_ef.src.Domain.Common;
using cs_ef.src.Domain.Core;

namespace cs_ef.src.Domain.Contracts
{
  public interface IProductService
  {
    Task<ProductDto?> Find(int id, bool showDeleted = true);
    Task<List<ProductDto>> FindAll(string? sort, string? order, string? name, decimal? priceMin, decimal? priceMax, DateTime? expirationMin, DateTime? expirationMax);
    Task<Pagination<ProductDto>> FindAllPaginated(int page, int perPage, string? sort, string? order, string? name, decimal? priceMin, decimal? priceMax, DateTime? expirationMin, DateTime? expirationMax, bool showDeleted);
    Task<Result<ProductDto>> Save(ProductDto dto, int id = 0);
    Task<bool> Delete(int id);
    Task<bool> UnDelete(int id);
  }
}
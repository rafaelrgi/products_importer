using Products.Application.Dtos;
using Products.Domain.Common;

namespace Products.Domain.Contracts
{
  public interface IProductService
  {
    Task<ProductDto?> Find(int id, bool showDeleted = false);
    Task<List<ProductDto>> FindAll(string? sort, string? order, string? name = null, decimal? priceMin = null, decimal? priceMax = null, DateTime? expirationMin = null, DateTime? expirationMax = null);
    Task<Pagination<ProductDto>> FindAllPaginated(int page, int perPage, string? sort, string? order, string? name = null, decimal? priceMin = null, decimal? priceMax = null, DateTime? expirationMin = null, DateTime? expirationMax = null, bool showDeleted = false);
    Task<Result<ProductDto>> Save(ProductDto dto, int id = 0);
    Task<bool> Delete(int id);
    Task<bool> UnDelete(int id);
  }
}

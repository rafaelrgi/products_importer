using cs_ef.src.Domain.Contracts;
using cs_ef.src.Domain.Core;
using cs_ef.src.Domain.Entities;

namespace cs_ef.src.Application.Services
{
  public class ProductService : IProductService
  {
    readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
      _repository = repository;
    }

    public async Task<Product?> Find(int id)
    {
      var row = await _repository.Find(id);
      return row;
    }

    public async Task<Pagination<Product>> FindAll(int page, int perPage, string? sort, string? order, string? name, decimal? priceMin, decimal? priceMax, DateTime? expirationMin, DateTime? expirationMax)
    {
      page = Math.Max(page, 1);
      perPage = Math.Max(perPage, 2);
      return await _repository.FindAll(page, perPage, sort, order, name, priceMin, priceMax, expirationMin, expirationMax);
    }

    public async Task<bool> Delete(int id)
    {
      var row = await _repository.Find(id);
      if (row == null)
        return false;

      return await _repository.Delete(row);
    }

    public async Task<bool> UnDelete(int id)
    {
      var row = await _repository.Find(id, false);
      if (row == null)
        return false;

      return await _repository.UnDelete(row);
    }
  }
}

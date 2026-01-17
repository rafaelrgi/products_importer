using cs_ef.src.Application.Dtos;
using cs_ef.src.Domain.Common;
using cs_ef.src.Domain.Contracts;
using cs_ef.src.Domain.Core;
using cs_ef.src.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace cs_ef.src.Application.Services
{
  public class ProductService : IProductService
  {
    readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
      _repository = repository;
    }

    public async Task<ProductDto?> Find(int id, bool showDeleted = true)
    {
      var row = await _repository.Find(id, showDeleted);
      return ProductToDto(row);
    }

    public async Task<List<ProductDto>> FindAll(
        string? sort, string? order,
        string? name, decimal? priceMin, decimal? priceMax, DateTime? expirationMin, DateTime? expirationMax)
    {
      (sort, order) = SortOrderParams(sort, order);
      var rows = await _repository.FindAll(sort, order, name, priceMin, priceMax, expirationMin, expirationMax);
      return rows.Select(x => ProductToDto(x)).ToList();
    }

    public async Task<Pagination<ProductDto>> FindAllPaginated(
        int page, int perPage, string? sort, string? order,
        string? name, decimal? priceMin, decimal? priceMax, DateTime? expirationMin, DateTime? expirationMax, bool showDeleted
    )
    {
      //pagination
      page = Math.Max(page, 1);
      perPage = Math.Max(perPage, 2);

      (sort, order) = SortOrderParams(sort, order);

      if (!IsValidSort(sort))
        throw new ArgumentException("Invalid sorting: " + sort);

      var rows = await _repository.FindAllPaginated(page, perPage, sort, order, name, priceMin, priceMax, expirationMin, expirationMax, showDeleted);
      var result = ProductsToDto(rows);
      return result;
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
      var row = await _repository.Find(id, true);
      if (row == null)
        return false;

      return await _repository.UnDelete(row);
    }

    public async Task<Result<ProductDto>> Save(ProductDto dto, int id = 0)
    {
      Product? row = (id > 0) ? await _repository.Find(id) : new Product();
      ProductFromDto(row, dto);
      row!.Id = id;

      //UNDONE: base service? base entity?
      var results = new List<ValidationResult>();
      var context = new ValidationContext(row, serviceProvider: null, items: null);
      if (!Validator.TryValidateObject(row, context, results, validateAllProperties: true))
      {
        string s = string.Join(" \r\n", results);
        if (string.IsNullOrWhiteSpace(s))
          s = "The object is invalid.";
        return new(null, false, false, s);
      }

      var result = ProductToDto(await _repository.Save(row));
      return new(result, true);
    }

    private Pagination<ProductDto> ProductsToDto(Pagination<Product> rows)
    {
      var result = new Pagination<ProductDto>()
      {
        Page = rows.Page,
        PerPage = rows.PerPage,
        PageCount = rows.PageCount,
        RecordCount = rows.RecordCount,
        Data = rows.Data?.Select(x => ProductToDto(x)).ToList(),
      };
      return result;
    }

    private ProductDto ProductToDto(Product? row)
    {
      if (row == null)
        throw new NullReferenceException();

      return new ProductDto(
        row.Id,
        row.Name,
        row.Quantity,
        row.Price,
        row.Expiration,
        row.BRL,
        row.EUR,
        row.CAD,
        row.MXN,
        row.GBP,
        row.IsDeleted,
        row.CreatedAt,
        row.UpdatedAt
        );

    }
    private void ProductFromDto(Product? row, ProductDto dto)
    {
      if (row == null)
        throw new NullReferenceException();

      row.Name = dto.Name;
      row.Quantity = dto.Quantity;
      row.Price = dto.Price;
      row.Expiration = dto.Expiration;
      row.BRL = dto.BRL;
      row.EUR = dto.EUR;
      row.CAD = dto.CAD;
      row.MXN = dto.MXN;
      row.GBP = dto.GBP;

      if (row.IsDeleted != dto.IsDeleted)
        row.DeletedAt = dto.IsDeleted ? DateTime.Now : null;
    }

    private bool IsValidSort(string? sort)
    {
      if (string.IsNullOrEmpty(sort))
        return true;

      return sort switch
      {
        "id" or "name" or "quantity" or "price" or "expiration" => true,
        _ => false,
      };
    }

    private (string sort, string order) SortOrderParams(string? sort, string? order)
    {
      var sortNonNull = (sort ?? "id").Trim().ToLower();
      var orderNonNull = (order ?? "asc").Trim().ToLower();
      if (orderNonNull != "desc")
        orderNonNull = "asc";
      return (sortNonNull, orderNonNull);
    }

  }
}


using cssharp.Dtos;
using cssharp.Models;
using cssharp.Repositories;
using System.Globalization;
using System.Text.RegularExpressions;

namespace cssharp.Services
{

  public interface IProductService
  {
    public char CsvSeparator { get; }
    public Task<PaginationDto<Product>> FindAll(int page, int perPage, string? sort, string? order, string? name, double? priceMin, double? priceMax, DateTime? expirationMin, DateTime? expirationMax);
    public Task<bool> importCsvLine(string[] values);
  }

  public class ProductService : IProductService
  {
    ProductRepository _repository;
    IExchangeRateService _rates;

    public char CsvSeparator { get => ';'; }

    public ProductService(IProductRepository repository, IExchangeRateService rates)
    {
      _repository = (ProductRepository)repository;
      _rates = rates;
    }

    public async Task<PaginationDto<Product>> FindAll(int page, int perPage, string? sort, string? order, string? name, double? priceMin, double? priceMax, DateTime? expirationMin, DateTime? expirationMax)
    {
      page = Math.Max(page, 1);
      perPage = Math.Min(perPage, 50);

      var result = new PaginationDto<Product>();

      result.Data = await _repository.FindAll(page, perPage, sort, order, name, priceMin, priceMax, expirationMin, expirationMax);
      result.RecordCount = await _repository.Count(page, perPage, sort, order, name, priceMin, priceMax, expirationMin, expirationMax);
      result.Page = page;
      result.PerPage = perPage;
      result.PageCount = (result.PerPage > 0 ? result.RecordCount / result.PerPage : 0) + (result.HasData ? 1 : 0);

      return result;
    }

    public async Task<bool> importCsvLine(string[] values)
    {
      if (!values.Any() || values.Length != 3)
        return false;

      string name = values[0].Trim();
      if (string.IsNullOrEmpty(name))
        return false;

      decimal d;
      decimal? price = (decimal.TryParse(Regex.Replace(values[1], @"[^0-9.-]", ""), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out d)) ? d : null;
      if (!price.HasValue || price < 0)
        return false;

      DateTime dt;
      DateTime? expiration = (DateTime.TryParse(values[2], out dt)) ? dt : null;
      if (!price.HasValue)
        return false;

      Product product = new Product()
      { 
        Id = 0,
        Name = name,
        Price = price ?? 0,
        Expiration = expiration ?? DateTime.MinValue,
        CreatedAt = DateTime.Now,
      };

      var rates = await _rates.GetToday5Rates();
      if (rates.Any())
      {
        foreach (var rate in rates)
        {
          switch (rate.Abbreviation)
          {
            case "BRL":
              product.BRL = rate.Rate * product.Price; 
              break;
            case "EUR":
              product.EUR = rate.Rate * product.Price;
              break;
            case "CAD":
              product.CAD = rate.Rate * product.Price;
              break;
            case "MXN":
              product.MXN = rate.Rate * product.Price;
              break;
            case "GBP":
              product.GBP = rate.Rate * product.Price;
              break;
          }
        }
      }

      return (await _repository.Save(product) > 0);
    }

    
  }
}

using cs_api.Dtos;
using cs_api.Models;
using cs_api.Repositories;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;

namespace cs_api.Services
{
  public class ProductService
  {
    const int BUFFER_SIZE = 1024;
    const string CSV_SEPARATOR = ";";

    private ProductRepository _repository;
    private ExchangeRateService _rates;
    private Product[] _buffer = [];
    private int _bufferPointer;

    public ProductService(ProductRepository repository, ExchangeRateService rates)
    {
      _repository = repository;
      _rates = rates;
    }

    public async Task<PaginationDto<Product>> FindAll(int page, int perPage, string? sort, string? order, string? name, decimal? priceMin, decimal? priceMax, DateTime? expirationMin, DateTime? expirationMax)
    {
      page = Math.Max(page, 1);
      perPage = Math.Min(perPage, 50);
      return await _repository.FindAll(page, perPage, sort, order, name, priceMin, priceMax, expirationMin, expirationMax);
    }

    public async Task<(int processed, int rejected)> ImportCsv(IFormFile file)
    {
      int processed = 0, rejected = 0;

      var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
      {
        Delimiter = CSV_SEPARATOR,
      };

      var stream = file.OpenReadStream();
      using (var reader = new StreamReader(stream))
      {
        using (var csv = new CsvReader(reader, csvConfig))
        {
          //skip the header
          csv.Read();
          csv.ReadHeader();

          while (csv.Read())
          {
            var row = csv.GetRecord<CsvRecord>();
            processed++;
            if (!await ImportCsvLine(row))
              rejected++;
          }
        }
      }

      return (processed, rejected);
    }

    private async Task<bool> ImportCsvLine(CsvRecord row)
    {
      row.name = (row.name ?? "").Trim();
      row.price = (row.price ?? "").Trim();
      row.expiration = (row.expiration ?? "").Trim();
      if (string.IsNullOrEmpty(row.name) || string.IsNullOrEmpty(row.price) || string.IsNullOrEmpty(row.expiration))
        return false;

      _SetupBufferIfNeeded();

      decimal d;
      decimal? price = (decimal.TryParse(Regex.Replace(row.price, @"[^0-9.-]", ""), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out d)) ? d : null;
      if (!price.HasValue || price < 0)
        return false;

      DateTime dt;
      DateTime? expiration = (DateTime.TryParse(row.expiration, out dt)) ? dt : null;
      if (!price.HasValue)
        return false;

      Product product = new Product()
      {
        Id = 0,
        Name = row.name,
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

      _buffer[_bufferPointer++] = product;
      if (_bufferPointer >= BUFFER_SIZE)
        return await _FlushBuffer();

      return true;
    }    

    private void _SetupBufferIfNeeded()
    {
      if (_buffer.Length > 0)
        return;

      _buffer = new Product[BUFFER_SIZE];
      _bufferPointer = 0;
    }

    private async Task<bool> _FlushBuffer()
    {
      bool result = await _repository.SaveProducts(_buffer, _bufferPointer);
      _bufferPointer = 0;
      return result;
    }

  }

  //--------------------------------------
  public class CsvRecord
  {
    public string name { get; set; } = "";
    public string price { get; set; } = "";
    public string expiration { get; set; } = "";
  }
  //--------------------------------------
}
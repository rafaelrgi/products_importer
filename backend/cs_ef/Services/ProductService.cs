using cs_ef.Dtos;
using cs_ef.Models;
using cs_ef.Repositories;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace cs_ef.Services
{
  public class ProductService : IProductService
  {
    const int BUFFER_SIZE = 256;
    const char CSV_SEPARATOR = ';'; 

    private ProductRepository _repository;
    private IExchangeRateService _rates;
    private Product[] _buffer = [];
    private int _bufferPointer;


    public ProductService(IProductRepository repository, IExchangeRateService rates)
    {
      _repository = (ProductRepository)repository;
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
      // Use the stream directly to process the CSV data
      using (var streamReader = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
      {
        while (!streamReader.EndOfStream)
        {
          var row = await streamReader.ReadLineAsync();
          //skip headers line
          if (row == null || processed++ == 0)
            continue;

          var values = row.Split(CSV_SEPARATOR);
          if (!await ImportCsvLine(values))
            rejected++;
        }
      }

      //flush any items left in the buffer
      if (_bufferPointer > 0)
        await _FlushBuffer();

      return (processed, rejected);
    }

    private async Task<bool> ImportCsvLine(string[] values)
    {
      _SetupBufferIfNeeded();

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
}

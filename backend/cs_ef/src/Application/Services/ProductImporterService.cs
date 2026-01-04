using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using cs_ef.src.Domain.Contracts;
using cs_ef.src.Domain.Entities;
using cs_ef.src.Infra.Repositories;

namespace cs_ef.src.Application.Services
{
  public class ProductImporterService : IProductImporterService
  {
    const int BUFFER_SIZE = 1024;
    const char CSV_SEPARATOR = ';';

    readonly ProductRepository _repository;
    readonly IExchangeRateService _rates;
    readonly ILogger<ProductImporterService> _logger;

    private Product[] _buffer = [];
    private int _bufferPointer;


    public ProductImporterService(IProductRepository repository, IExchangeRateService rates, ILogger<ProductImporterService> logger)
    {
      _repository = (ProductRepository)repository;
      _rates = rates;
      _logger = logger;
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
          var msg = await ImportCsvLine(values);
          if (msg != "")
          {
            _logger.LogWarning($"Line {processed + 1}: {msg} :: {row}");
            //string.Concat("Line ".AsSpan(), (processed + 1).ToString().AsSpan(), ": ".AsSpan(), msg.AsSpan(), " :: ".AsSpan(), row.AsSpan());
            rejected++;
          }

        }
      }

      //flush any items left in the buffer
      if (_bufferPointer > 0)
        await _FlushBuffer();

      return (processed, rejected);
    }

    private async Task<string> ImportCsvLine(string[] values)
    {
      _SetupBufferIfNeeded();

      if (!values.Any() || values.Length != 3)
        return "Misformatted line";

      string name = values[0].Trim();
      if (string.IsNullOrEmpty(name))
        return "Name is missing";

      decimal d;
      decimal? price = decimal.TryParse(Regex.Replace(values[1], @"[^0-9.-]", ""), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out d) ? d : null;
      if (!price.HasValue || price < 0)        
        return "Price is missing";

      DateTime dt;
      DateTime? expiration = DateTime.TryParse(values[2], out dt) ? dt : null;
      if (!price.HasValue)
        return "Expiration is missing";

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
        return await _FlushBuffer()? "" : $"Error flushing buffer (buffer_pointer: {_bufferPointer})";

      return "";
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

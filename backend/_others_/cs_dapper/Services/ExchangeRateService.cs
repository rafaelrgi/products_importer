using cssharp.Models;
using cssharp.Repositories;
using System.Text.Json;

namespace cssharp.Services
{
  public interface IExchangeRateService
  {
    public Task<IEnumerable<ExchangeRate>> FindAll();

    public Task<IEnumerable<ExchangeRate>> GetToday5Rates();
  }

  public class ExchangeRateService : IExchangeRateService
  {
    private const string URL_API = "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies/usd.json";
    private const string URL_API_2 = "https://latest.currency-api.pages.dev/v1/currencies/usd.json";

    IExchangeRateRepository _repository;
    HttpClient _httpClient;

    public ExchangeRateService(IExchangeRateRepository repository, HttpClient httpClient)
    {
      _repository = repository;
      _httpClient = httpClient;
    }

    public async Task<IEnumerable<ExchangeRate>> FindAll()
    {
      return await _repository.FindAll();
    }

    public async Task<IEnumerable<ExchangeRate>> GetToday5Rates()
    {
      var rates = await _GetToday5RatesDb();
      if (rates != null && rates.Any())
        return rates;
      rates = await _GetToday5RatesApi(URL_API);
      if (rates != null && rates.Any())
      {
        await _SaveRatesAsync(rates);
        return rates;
      }
      rates = await _GetToday5RatesApi(URL_API_2);
      await _SaveRatesAsync(rates);
      return rates;
    }

    private async Task _SaveRatesAsync(IEnumerable<ExchangeRate> rates)
    {
      foreach (var rate in rates)
      {
        await _repository.Save(rate);
      }
    }

    private async Task<IEnumerable<ExchangeRate>> _GetToday5RatesApi(string url)
    {
      HttpResponseMessage response = await _httpClient.GetAsync(url);
      if (!response.IsSuccessStatusCode)
        return Enumerable.Empty<ExchangeRate>();
      string body = await response.Content.ReadAsStringAsync();

      using JsonDocument doc = JsonDocument.Parse(body);
      JsonElement root = doc.RootElement;
      DateTime date = root.GetProperty("date").GetDateTime();
      JsonElement usd = root.GetProperty("usd");

      ExchangeRate[] rates = new ExchangeRate[5];
      rates[0] = _GetRateFromJson(usd, "brl", date!);
      rates[1] = _GetRateFromJson(usd, "eur", date!);
      rates[2] = _GetRateFromJson(usd, "cad", date!);
      rates[3] = _GetRateFromJson(usd, "mxn", date!);
      rates[4] = _GetRateFromJson(usd, "gbp", date!);

      return rates;
    }

    private ExchangeRate _GetRateFromJson(JsonElement usd, string abbreviation, DateTime date)
    {
      var rate = new ExchangeRate()
      {
        Id = 0,
        CreatedAt = DateTime.Now,
        Date = date,
        Abbreviation = abbreviation.ToUpper(),
        Rate = usd.GetProperty(abbreviation).GetDecimal(),
      };
      return rate;
    }

    private async Task<IEnumerable<ExchangeRate>> _GetToday5RatesDb()
    {
      return await _repository.FindByDate(DateTime.Now);
    }
  }
}

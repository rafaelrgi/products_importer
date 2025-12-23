using cssharp.Models;

namespace cs_ef.Services
{
  public interface IExchangeRateService
  {
    public Task<List<ExchangeRate>> FindAll();
    public Task<List<ExchangeRate>> GetToday5Rates();

  }
}
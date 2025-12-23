using cssharp.Models;

namespace cs_ef.Repositories
{
  public interface IExchangeRateRepository
  {
    public Task<List<ExchangeRate>> FindAll();
    public Task<List<ExchangeRate>> FindByDate(DateTime dt);
    public Task<bool> SaveRates(ExchangeRate[] rows);
  }
}
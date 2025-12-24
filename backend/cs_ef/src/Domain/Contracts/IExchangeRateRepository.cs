using cs_ef.src.Domain.Models;

namespace cs_ef.src.Domain.Contracts
{
  public interface IExchangeRateRepository
  {
    public Task<List<ExchangeRate>> FindAll();
    public Task<List<ExchangeRate>> FindByDate(DateTime dt);
    public Task<bool> SaveRates(ExchangeRate[] rows);
  }
}
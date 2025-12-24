using cs_ef.src.Domain.Models;

namespace cs_ef.src.Domain.Contracts
{
  public interface IExchangeRateService
  {
    public Task<List<ExchangeRate>> FindAll();
    public Task<List<ExchangeRate>> GetToday5Rates();

  }
}
using Products.Domain.Entities;

namespace Products.Domain.Contracts
{
  public interface IExchangeRateService
  {
    public Task<List<ExchangeRate>> FindAll();
    public Task<List<ExchangeRate>> GetToday5Rates();

  }
}
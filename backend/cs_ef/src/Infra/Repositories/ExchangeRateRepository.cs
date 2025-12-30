using cs_ef.src.Data;
using cs_ef.src.Domain.Contracts;
using cs_ef.src.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace cs_ef.src.Infra.Repositories
{
  public class ExchangeRateRepository : IExchangeRateRepository
  {
    private readonly Db _db;
    readonly ILogger<ExchangeRateRepository> _logger;

    public ExchangeRateRepository(Db db, ILogger<ExchangeRateRepository> logger)
    {
      _db = db;
      _logger = logger;
    }

    public async Task<List<ExchangeRate>> FindAll()
    {
      var rows = await _db.ExchangeRates.ToListAsync();
      return rows;
    }

    public async Task<List<ExchangeRate>> FindByDate(DateTime dt)
    {
      var rows = await _db.ExchangeRates
                        .Where(e => e.Date.Date == dt.Date)
                        .AsNoTracking()
                        .ToListAsync();
      if (!rows.Any())
        rows = await _db.ExchangeRates
                        .Where(e => e.CreatedAt.Date == dt.Date)
                        .AsNoTracking()
                        .ToListAsync();
      return rows;
    }

    public async Task<bool> SaveRates(ExchangeRate[] rows)
    {
      try
      {
        foreach (var row in rows)
        {
          if (row.Id == 0)
            await _db.ExchangeRates.AddAsync(row);
          else
            _db.ExchangeRates.Update(row);
        }
        await _db.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.ToString());
        return false;
      }
      return true;
    }

  }
}

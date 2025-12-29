using cs_api.src.Domain.Entities;
using cs_api.src.Infra;
using Microsoft.EntityFrameworkCore;

namespace cs_api.src.Infra.Repositories
{
  public class ExchangeRateRepository
  {
    readonly Db _db;

    public ExchangeRateRepository(Db db)
    {
      _db = db;
    }

    public async Task<List<ExchangeRate>> FindAll()
    {
      var rows = await _db.ExchangeRates.AsNoTracking().ToListAsync();
      return rows;
    }

    public async Task<List<ExchangeRate>> FindByDate(DateTime dt)
    {
      var rows = await _db.ExchangeRates
                        .Where(e => e.Date.Date == dt.Date)
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
      catch (Exception)
      {
        return false;
      }
      return true;  
    }

  }
}

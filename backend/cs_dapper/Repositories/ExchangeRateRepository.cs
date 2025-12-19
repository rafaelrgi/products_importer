using cssharp.Models;
using Dapper;
using MySqlConnector;

namespace cssharp.Repositories
{
  public interface IExchangeRateRepository  {
    public Task<IEnumerable<ExchangeRate>> FindAll();
    public Task<IEnumerable<ExchangeRate>> FindByDate(DateTime dt);
    public Task<int> Save(ExchangeRate record);
  }
  public class ExchangeRateRepository : BaseRepository<ExchangeRate>, IExchangeRateRepository
  {
    public async Task<IEnumerable<ExchangeRate>> FindAll()
    {
      using var connection = new MySqlConnection(connectionString);
      var sql = "Select * From exchange_rate";
      var rows = await connection.QueryAsync<ExchangeRate>(sql);
      return rows;
    }

    public async Task<IEnumerable<ExchangeRate>> FindByDate(DateTime dt)
    {
      using var connection = new MySqlConnection(connectionString);
      var sql = "Select * From exchange_rate Where Date=@date";
      var rows = await connection.QueryAsync<ExchangeRate>(sql, dt);
      if (! rows.Any())
      {
        sql = "Select * From exchange_rate Where CreatedAt=@date";
        rows = await connection.QueryAsync<ExchangeRate>(sql, dt);
      }

      return rows;
    }

    public async Task<int> Save(ExchangeRate record)
    {      
      string sql = (record.Id > 0) ?
          "Update exchange_rate Set Id=@Id, Date=@Date, CreatedAt=@CreatedAt, Abbreviation=@Abbreviation, Rate=@Rate Where id=@Id"
          :
          "Insert into exchange_rate (Date, CreatedAt, Abbreviation, Rate ) Values (@Date, @CreatedAt, @Abbreviation, @Rate); SELECT LAST_INSERT_ID();";

      //do it!!!
      using (var connection = new MySqlConnection(connectionString))
      {
        var result = await connection.ExecuteScalarAsync<int>(sql, record);
        return result;
      }
    }

  }
}

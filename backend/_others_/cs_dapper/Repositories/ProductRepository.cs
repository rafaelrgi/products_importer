using cssharp.Models;
using Dapper;
using MySqlConnector;
using System.Text;

namespace cssharp.Repositories
{
  public interface IProductRepository
  {
    public Task<IEnumerable<Product>> FindAll(int page, int perPage, string? sort, string? order, string? name, double? priceMin, double? priceMax, DateTime? expirationMin, DateTime? expirationMax);
    public Task<int> Count(int page, int perPage, string? sort, string? order, string? name, double? priceMin, double? priceMax, DateTime? expirationMin, DateTime? expirationMax);
    public Task<int> Save(Product product);
  }

  public class ProductRepository : BaseRepository<Product>, IProductRepository
  {
    public async Task<IEnumerable<Product>> FindAll(int page, int perPage, string? sort, string? order, string? name, double? priceMin, double? priceMax, DateTime? expirationMin, DateTime? expirationMax)
    {
      var sql = _Select("*", page, perPage, sort, order, name, priceMin, priceMax, expirationMin, expirationMax);
      int skip = page * perPage - perPage;
      var parameters = _selectParameters(perPage, name, priceMin, priceMax, expirationMin, expirationMax, skip);

      //order
      if (sort != null && order != null)
        sql.Append(_OrderBy(sort, order));

      //pagination
      sql.Append(" LIMIT @PerPage OFFSET @Skip ");

      //do it!!!
      using (var connection = new MySqlConnection(connectionString))
      {
        var rows = await connection.QueryAsync<Product>(sql.ToString(), parameters);
        return rows;
      }
    }

    public async Task<int> Count(int page, int perPage, string? sort, string? order, string? name, double? priceMin, double? priceMax, DateTime? expirationMin, DateTime? expirationMax)
    {
      var sql = _Select("Count(1)", page, perPage, sort, order, name, priceMin, priceMax, expirationMin, expirationMax);
      var parameters = _selectParameters(perPage, name, priceMin, priceMax, expirationMin, expirationMax, 0);

      //do it!!!
      using (var connection = new MySqlConnection(connectionString))
      {
        var rows = await connection.ExecuteScalarAsync<int>(sql.ToString(), parameters);
        return rows;
      }
    }

    public async Task<int> Save(Product product)
    {
      string sql = (product.Id > 0) ?
          "Update product Set name=@Name, price=@Price, expiration=@Expiration, brl=@BRL, eur=@EUR, cad=@CAD, mxn=@MXN, gbp=@GBP Where id=@Id"
          :
          "Insert into product (name, price, expiration, CreatedAt, brl, eur, cad, mxn, gbp) Values (@Name, @Price, @Expiration, @CreatedAt, @BRL, @EUR, @CAD, @MXN, @GBP); SELECT LAST_INSERT_ID();";

      //do it!!!
      using (var connection = new MySqlConnection(connectionString))
      {
        var result = await connection.ExecuteScalarAsync<int>(sql, product);
        return result;
      }
    }

    private object _selectParameters(int perPage, string? name, double? priceMin, double? priceMax, DateTime? expirationMin, DateTime? expirationMax, int skip)
    {
      return new
      {
        Skip = skip,
        PerPage = perPage,
        Name = name,
        PriceMin = priceMin,
        PriceMax = priceMax,
        ExpirationMin = expirationMin,
        ExpirationMax = expirationMax,
      };
    }

    private StringBuilder _Select(string columns, int page, int perPage, string? sort, string? order, string? name, double? priceMin, double? priceMax, DateTime? expirationMin, DateTime? expirationMax)
    {
      //select
      var sql = new StringBuilder($"Select {columns} From product ");

      //filters
      bool hasWhere = false;
      if (name != null)
      {
        sql.Append(" where name Like @Name% ");
        hasWhere = true;
      }
      if (priceMin != null)
      {
        sql.Append(hasWhere ? " and " : " where ").Append(" price >= @PriceMin ");
        hasWhere = true;
      }
      if (priceMax != null)
      {
        sql.Append(hasWhere ? " and " : " where ").Append(" price <= @PriceMax ");
        hasWhere = true;
      }
      if (expirationMin != null)
      {
        sql.Append(hasWhere ? " and " : " where ").Append(" expiration >= @ExpirationMin ");
        hasWhere = true;
      }
      if (expirationMax != null)
      {
        sql.Append(hasWhere ? " and " : " where ").Append(" expiration <= @ExpirationMax ");
        hasWhere = true;
      }

      return sql;
    }

    private string _OrderBy(string sort, string order)
    {
      sort = sort.ToLower().Trim();
      order = order.ToLower().Trim();
      var sb = new StringBuilder();

      switch (sort)
      {
        case "name":
          sb.Append(" Order by name ");
          break;
        case "price":
          sb.Append(" Order by price ");
          break;
        case "expiration":
          sb.Append(" Order by expiration ");
          break;
      }

      if (order == "desc")
        sb.Append(" desc ");
      else
        sb.Append(" asc ");

      return sb.ToString();
    }
  }

}

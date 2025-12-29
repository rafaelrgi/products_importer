using cs_api.src.Domain.Core;
using cs_api.src.Domain.Entities;
using cs_api.src.Infra;
using Microsoft.EntityFrameworkCore;

namespace cs_api.src.Infra.Repositories
{
  public class ProductRepository
  {
    readonly Db _db;

    public ProductRepository(Db db)
    {
      _db = db;
    }

    public async Task<Pagination<Product>> FindAll(int page, int perPage, string? sort, string? order, string? name, decimal? priceMin, decimal? priceMax, DateTime? expirationMin, DateTime? expirationMax)
    {
      var qry = _db.Products.AsQueryable();
      qry = _ApplyFilters(name, priceMin, priceMax, expirationMin, expirationMax, qry);

      //order
      qry = _ApplyOrderBy(sort, order, qry);

      //pagination
      int totalRecords = await qry.CountAsync();
      int skip = page * perPage - perPage;
      qry = qry
        .Skip(skip)
        .Take(perPage);

      //Console.WriteLine(qry.ToQueryString());
      var rows = await qry.AsNoTracking().ToListAsync();

      var result = new Pagination<Product>();
      result.Data = rows;
      result.RecordCount = totalRecords;
      result.Page = page;
      result.PerPage = perPage;
      result.PageCount = (result.PerPage > 0 ? result.RecordCount / result.PerPage : 0) + (result.HasData ? 1 : 0);
      return result;
    }

    private static IQueryable<Product> _ApplyOrderBy(string? sort, string? order, IQueryable<Product> qry)
    {
      if (sort == null || order == null || sort.Length < 4)
        return qry.OrderBy(x => x.Id);

      sort = sort[0].ToString().ToUpper() + sort.Substring(1).ToLower();
      if (order.Equals("desc", StringComparison.OrdinalIgnoreCase))
        qry = qry.OrderByDescending(x => EF.Property<object>(x, sort)).ThenBy(x => x.Id);
      else
        qry = qry.OrderBy(x => EF.Property<object>(x, sort)).ThenBy(x => x.Id);

      return qry;
    }

    private static IQueryable<Product> _ApplyFilters(string? name, decimal? priceMin, decimal? priceMax, DateTime? expirationMin, DateTime? expirationMax, IQueryable<Product> qry)
    {
      if (name != null)
        qry = qry.Where(x => EF.Functions.Like(x.Name, name + '%'));

      if (priceMin != null)
        qry = qry.Where(x => x.Price >= priceMin);
      if (priceMax != null)
        qry = qry.Where(x => x.Price <= priceMax);
      if (expirationMin != null)
        qry = qry.Where(x => x.Expiration >= expirationMin);
      if (expirationMax != null)
        qry = qry.Where(x => x.Expiration >= expirationMax);
      return qry;
    }

    public async Task<bool> SaveProducts(Product[] products, int count)
    {
      int n = 0;
      foreach (var product in products)
      {
        if (product == null)
          continue;
        if (product.Id == 0)
          await _db.Products.AddAsync(product);
        else
          _db.Products.Update(product);
        if (++n >= count)
          break;
      }

      try
      {
        await _db.SaveChangesAsync();
        return true;
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        return false;
      }
    }

  }

}

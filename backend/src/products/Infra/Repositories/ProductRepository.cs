using Microsoft.EntityFrameworkCore;
using products.Domain.Common;
using products.Domain.Contracts;
using products.Domain.Entities;
using products.Infra;

namespace products.Infra.Repositories
{
  public class ProductRepository : IProductRepository
  {
    readonly Db _db;
    readonly ILogger<ProductRepository> _logger;

    public ProductRepository(Db db, ILogger<ProductRepository> logger)
    {
      _db = db;
      _logger = logger;
    }

    public async Task<List<Product>> FindAll(
        string sort, string order,
        string? name = null, decimal? priceMin = null, decimal? priceMax = null, DateTime? expirationMin = null, DateTime? expirationMax = null
    )
    {
      var qry = _db.Products.AsQueryable();
      qry = _ApplyFilters(name, priceMin, priceMax, expirationMin, expirationMax, qry);

      //order
      qry = _ApplyOrderBy(sort, order, qry);

      //_logger.LogInformation(qry.ToQueryString());
      var rows = await qry.AsNoTracking().ToListAsync();

      return rows;
    }

    public async Task<Pagination<Product>> FindAllPaginated(
        int page, int perPage, string sort, string order,
        string? name = null, decimal? priceMin = null, decimal? priceMax = null, DateTime? expirationMin = null, DateTime? expirationMax = null, bool showDeleted = false
    )
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

      //includeDeleted?
      if (showDeleted)
        qry = qry.IgnoreQueryFilters();

      //_logger.LogInformation(qry.ToQueryString());
      var rows = await qry.AsNoTracking().ToListAsync();

      var result = new Pagination<Product>();
      result.Data = rows;
      result.RecordCount = totalRecords;
      result.Page = page;
      result.PerPage = perPage;
      result.PageCount = result.RecordCount / result.PerPage;
      return result;
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
        _db.ChangeTracker.Clear();
        return true;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.ToString());
        return false;
      }
    }

    public async Task<Product?> Find(int id, bool showDeleted = false)
    {
      var qry = _db.Products.AsQueryable();
      if (showDeleted)
        qry = qry.IgnoreQueryFilters();

      return await qry.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> Delete(Product row)
    {
      row.DeletedAt = DateTime.Now;
      _db.Products.Update(row);
      var result = (await _db.SaveChangesAsync() > 0);
      _db.Entry(row).State = EntityState.Detached;
      return result;
    }

    public async Task<bool> UnDelete(Product row)
    {
      row.DeletedAt = null;
      _db.Products.Update(row);
      var result = (await _db.SaveChangesAsync() > 0);
      _db.Entry(row).State = EntityState.Detached;
      return result;
    }

    public async Task<Product> Save(Product row)
    {
      if (row.Id == 0)
        await _db.Products.AddAsync(row);
      else
        _db.Products.Update(row);

      await _db.SaveChangesAsync();
      _db.Entry(row).State = EntityState.Detached;
      return row;
    }

    private static IQueryable<Product> _ApplyOrderBy(string sort, string order, IQueryable<Product> qry)
    {
      //if (sort == null || order == null || sort.Length < 4) return qry.OrderBy(x => x.Id);
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

  }

}

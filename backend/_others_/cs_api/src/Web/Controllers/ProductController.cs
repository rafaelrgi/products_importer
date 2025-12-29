using cs_api.src.Application.Services;
using System.Globalization;

namespace cs_api.src.Web.Controllers
{
  public class ProductController
  {
    readonly ProductService _service;

    public ProductController(ProductService service)
    {
      _service = service;
    }

    public async Task<IResult> Index(HttpContext context)
    {
      //pagination
      if (!int.TryParse(context.Request.Query["page"], out int page))
        page = 1;
      if (!int.TryParse(context.Request.Query["perPage"], out int perPage))
        perPage = 10;
      page = Math.Max(page, 1);
      perPage = Math.Min(perPage, 50);

      //sort and order
      string? sort = context.Request.Query["sort"];
      string? order = context.Request.Query["order"];
      if (string.IsNullOrEmpty(sort))
        order = sort;
      else
      {
        sort = sort.ToLower();
        order = (order ?? "asc").ToLower();
        if (order != "desc")
          order = "asc";
      }

      //filters: name, PriceMin, PriceMax, ExpirationMin, ExpirationMax
      string? name = context.Request.Query["name"];

      decimal d;
      decimal? priceMin = decimal.TryParse(context.Request.Query["priceMin"], NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out d) ? d : null;
      decimal? priceMax = decimal.TryParse(context.Request.Query["priceMax"], NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out d) ? d : null;

      DateTime dt;
      DateTime? expirationMin = DateTime.TryParse(context.Request.Query["expirationMin"], out dt) ? dt : null;
      DateTime? expirationMax = DateTime.TryParse(context.Request.Query["expirationMax"], out dt) ? dt : null;

      var result = await _service.FindAll(page, perPage, sort, order, name, priceMin, priceMax, expirationMin, expirationMax);
      if (!result.HasData)
        return Results.NotFound();

      return Results.Ok(result);
    }

    public async Task<IResult> Upload(HttpRequest request)
    {
      if (request.Form == null || request.Form.Files == null || !request.Form.Files.Any())
        return Results.BadRequest("No valid file found");

      var file = request.Form.Files[0];
      if (file == null || file.Length == 0)
        return Results.BadRequest("No valid file found");

      int lines = 0;
      int rejected = 0;
      try
      {
        (lines, rejected) = await _service.ImportCsv(file);
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        return Results.StatusCode(500);
      }

      return Results.Ok($"{lines} rows processed :: {rejected} rows rejected");
    }
  }
}
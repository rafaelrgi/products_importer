using cs_ef.src.Application.Services;
using cs_ef.src.Domain.Contracts;
using cs_ef.src.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;

namespace cs_ef.src.Web.Controllers
{
  [Route("api/products")]
  [ApiController]
  public class ProductController : Controller
  {
    private readonly IProductService _service;
    public ProductController(IProductService service)
    {
      _service = (ProductService)service;
    }

    [HttpGet]
    public async Task<ActionResult<List<Product>>> Index()
    {
      //pagination
      if (!int.TryParse(HttpContext.Request.Query["page"], out int page))
        page = 1;
      if (!int.TryParse(HttpContext.Request.Query["perPage"], out int perPage))
        perPage = 10;
      page = Math.Max(page, 1);
      perPage = Math.Min(perPage, 50);

      //sort and order
      string? sort = HttpContext.Request.Query["sort"];
      string? order = HttpContext.Request.Query["order"];
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
      string? name = HttpContext.Request.Query["name"];

      decimal d;
      decimal? priceMin = decimal.TryParse(HttpContext.Request.Query["priceMin"], NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out d) ? d : null;
      decimal? priceMax = decimal.TryParse(HttpContext.Request.Query["priceMax"], NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out d) ? d : null;

      DateTime dt;
      DateTime? expirationMin = DateTime.TryParse(HttpContext.Request.Query["expirationMin"], out dt) ? dt : null;
      DateTime? expirationMax = DateTime.TryParse(HttpContext.Request.Query["expirationMax"], out dt) ? dt : null;

      var result = await _service.FindAll(page, perPage, sort, order, name, priceMin, priceMax, expirationMin, expirationMax);
      if (!result.HasData)
        return NotFound();

      return Ok(result);
    }

    [HttpGet("test")]
    public ActionResult<string> Test()
    {
      return Ok("Api is ready!");
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<string>> Upload(IFormFile file)
    {
      if (file == null || file.Length == 0)
        return BadRequest("No valid file found");

      int lines = 0;
      int rejected = 0;
      try
      {
        (lines, rejected) = await _service.ImportCsv(file);
      }
      catch (Exception e)
      {
        return StatusCode(500, e.Message);
      }

      return Ok($"{lines - 1} rows processed :: {rejected} rows rejected");
    }
  }
}

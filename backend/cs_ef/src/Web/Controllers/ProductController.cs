using cs_ef.src.Domain.Contracts;
using cs_ef.src.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace cs_ef.src.Web.Controllers
{
  [Route($"{ROUTE}")]
  [ApiController]
  public class ProductController : Controller
  {
    const string ROUTE = "api/products";

    readonly IProductService _service;
    readonly IProductImporterService _importer;
    readonly ILogger<ProductController> _logger;

    public ProductController(IProductService service, IProductImporterService importer, ILogger<ProductController> logger)
    {
      _service = service;
      _importer = importer;
      _logger = logger;
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

      decimal? priceMin = decimal.TryParse(HttpContext.Request.Query["priceMin"], NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out decimal d) ? d : null;
      decimal? priceMax = decimal.TryParse(HttpContext.Request.Query["priceMax"], NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out d) ? d : null;

      DateTime dt;
      DateTime? expirationMin = DateTime.TryParse(HttpContext.Request.Query["expirationMin"], out dt) ? dt : null;
      DateTime? expirationMax = DateTime.TryParse(HttpContext.Request.Query["expirationMax"], out dt) ? dt : null;

      var result = await _service.FindAll(page, perPage, sort, order, name, priceMin, priceMax, expirationMin, expirationMax);
      if (!result.HasData)
        return NotFound();

      return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product?>> Details(int id)
    {
      Product? row = await _service.Find(id);
      if (row == null)
        return NotFound();

      return Ok(row);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Create([FromBody] Product row)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState.Values.SelectMany(v => v.Errors));

      try
      {
        if (! await _service.Save(row))
          return BadRequest("Unable to create record");

        var uri = new Uri($"{ROUTE}/{row.Id}", UriKind.Relative);
        return Created(uri, row);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.ToString());
        throw new Exception(ex.Message);
      }
    }     

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]    
    public async Task<ActionResult> Edit(int id, [FromBody] Product row)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState.Values.SelectMany(v => v.Errors));

      row.Id = id;
      try
      {
        if (! await _service.Save(row))
          return BadRequest("Unable to save record");

        return Ok(row);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.ToString());
        throw new NotImplementedException();
      }
    } 

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
      if (!await _service.Delete(id))
        return NotFound();
      return NoContent();
    }

    [HttpPatch("activate/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UnDelete(int id)
    {
      if (!await _service.UnDelete(id))
        return NotFound();
      return NoContent();
    }
    
    [HttpPost("upload")]
    [Authorize(Roles = "Admin")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<string>> Upload(IFormFile file)
    {
      if (file == null || file.Length == 0)
        return BadRequest("No valid file found");

      int lines = 0;
      int rejected = 0;
      try
      {
        (lines, rejected) = await _importer.ImportCsv(file);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.ToString());
        return StatusCode(500, ex.Message);
      }

      var s = $"{file.FileName}: {lines - 1} rows processed :: {rejected} rows rejected";
      _logger.LogInformation(s);
      return Ok(s);
    }
    
    //UNDONE: Pdf report (filters = Index)

  }
}

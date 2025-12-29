using cs_ef.src.Domain.Contracts;
using cs_ef.src.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace cs_ef.src.Web.Controllers
{
  [Route("api/products")]
  [ApiController]
  public class ProductController : Controller
  {
    readonly IProductService _service;
    readonly IImportProductService _importer;

    public ProductController(IProductService service, IImportProductService importer)
    {
      _service = service;
      _importer = importer;
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
    [HttpGet("test")]
    public ActionResult<string> Test()
    {
      return Ok("Api is ready!");
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
      catch (Exception e)
      {
        return StatusCode(500, e.Message);
      }

      return Ok($"{lines - 1} rows processed :: {rejected} rows rejected");
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

    //UNDONE: Pdf report (filters = Index)
    //UNDONE: Create
    /*
    [HttpPost]
    [Authorize(Roles = "Admin")]
    //[AllowAnonymous]
    public async Task<ActionResult> Create([FromBody] User user)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState.Values.SelectMany(v => v.Errors));

      try
      {
        var dto = await _service.Save(user);
        var uri = new Uri($"api/users/{dto.Id}", UriKind.Relative);
        return Created(uri, dto);
      }
      catch (Exception e)
      {
        Console.WriteLine(e.ToString());
        throw new Exception(e.Message);
      }
    }     
     */
    //UNDONE: Update
    /*
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Edit(int id, [FromBody] User user)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState.Values.SelectMany(v => v.Errors));

      user.Id = id;
      try
      {
        var dto = await _service.Save(user);
        return Ok(dto);
      }
      catch (Exception e)
      {
        Console.WriteLine(e.ToString());
        throw new NotImplementedException();
      }
    } 
    */

    //UNDONE: logs
  }
}

using Microsoft.AspNetCore.Mvc;
using products.Application.Services;
using products.Domain.Contracts;
using products.Domain.Entities;

namespace products.Web.Controllers
{
  [ApiController]
  [Route("api/exchange-rates")]
  public class ExchangeRateController : Controller
  {
    readonly IExchangeRateService _service;

    public ExchangeRateController(IExchangeRateService service)
    {
      _service = (ExchangeRateService)service;
    }

    [HttpGet]
    public async Task<ActionResult<List<ExchangeRate>>> Index()
    {
      var rates = await _service.FindAll();
      if (!rates.Any())
        return NotFound();

      return Ok(rates);
    }
        
  }

}

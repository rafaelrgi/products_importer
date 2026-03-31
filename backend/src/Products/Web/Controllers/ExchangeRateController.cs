using Microsoft.AspNetCore.Mvc;
using Products.Application.Services;
using Products.Domain.Contracts;
using Products.Domain.Entities;

namespace Products.Web.Controllers
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

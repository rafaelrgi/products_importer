using cssharp.Models;
using cssharp.Services;
using Microsoft.AspNetCore.Mvc;

namespace cssharp.Controllers
{
  [Route("api/exchange-rates")]
  [ApiController]
  public class ExchangeRateController : Controller
  {
    private readonly IExchangeRateService _service;
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

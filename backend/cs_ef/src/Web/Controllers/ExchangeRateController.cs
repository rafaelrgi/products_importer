using cs_ef.src.Application.Services;
using cs_ef.src.Domain.Contracts;
using cs_ef.src.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace cs_ef.src.Web.Controllers
{
  [ApiController]
  [Route("api/exchange-rates")]
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

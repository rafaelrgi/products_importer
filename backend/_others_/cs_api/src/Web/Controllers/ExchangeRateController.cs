using cs_api.src.Application.Services;

namespace cs_api.src.Web.Controllers
{
  public class ExchangeRateController
  {
    readonly ExchangeRateService _service;

    public ExchangeRateController(ExchangeRateService service)
    {
      _service = service;
    }

    public async Task<IResult> Index()
    {
      var rates = await _service.FindAll();
      if (!rates.Any())
        return Results.NotFound();

      return Results.Ok(rates);
    }
  }

}

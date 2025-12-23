using cs_api.Services;

namespace cs_api.Controllers
{
  public class ExchangeRateController
  {
    private readonly ExchangeRateService _service;

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

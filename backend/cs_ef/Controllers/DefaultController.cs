using Microsoft.AspNetCore.Mvc;

namespace cs_ef.Controllers
{
  [Route("api/")]
  [ApiController]
  public class DefaultController : Controller
  {
    public DefaultController()
    {

    }

    [HttpGet]
    public ActionResult<string> Index()
    {
      bool isRunningInDocker = Environment.GetEnvironmentVariable("IS_DOCKER") == "true";
      string inDocker = isRunningInDocker ? "Yes" : "No";
      return Ok($" API is ready! \r\n\r\n Running in Docker? {inDocker}");
    }
  }
}

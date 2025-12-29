using cssharp.Models;
using cssharp.Services;
using Microsoft.AspNetCore.Mvc;

namespace cssharp.Controllers
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
      bool isRunningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_DOCKER") == "true";
      string inDocker = isRunningInDocker ? "Yes" : "No";
      return Ok($" API is ready! \r\n\r\n Running in Docker? {inDocker}");
    }
  }
}

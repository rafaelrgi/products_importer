using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace cs_ef.src.Web.Controllers
{
  [AllowAnonymous]
  [Route("api/")]
  [ApiController]
  public class HomeController : Controller
  {
    public HomeController() { }

    [HttpGet]
    public ActionResult<string> Index()
    {
      bool isDocker = Environment.GetEnvironmentVariable("IS_DOCKER") == "true";

      bool isAuth = (User.Identity != null && User.Identity.IsAuthenticated);

      var result = new
      {
        Status = "API is ready!",
        IsDocker = isDocker,
        IsAuth = isAuth,
        User = !isAuth ? null : new
        {
          Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
          Name = User.Identity!.Name,
          Admin = User.IsInRole("Admin"),
          AuthType = User.Identity.AuthenticationType,
        }
      };

      return Ok(result);
    }
  }
}

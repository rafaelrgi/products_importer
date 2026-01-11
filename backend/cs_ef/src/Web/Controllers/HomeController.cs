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
#if DEBUG
        BaseDir = AppContext.BaseDirectory,
#endif
        User = !isAuth ? null : new
        {
#if DEBUG
          Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
          AuthType = User.Identity.AuthenticationType,
#endif
          Name = User.Identity!.Name,
          Admin = User.IsInRole("Admin"),
        }
      };

      return Ok(result);
    }
  }
}

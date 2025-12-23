using Microsoft.AspNetCore.Http.HttpResults;

namespace cs_api.Controllers
{
  public class DefaultController
  {
    public IResult Index()
    {
      bool isRunningInDocker = Environment.GetEnvironmentVariable("IS_DOCKER") == "true";
      string inDocker = isRunningInDocker ? "Yes" : "No";
      return Results.Ok($" API is ready!    Running in Docker? {inDocker}");
    }
       

  }
}

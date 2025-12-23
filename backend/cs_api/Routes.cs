using cs_api.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace cs_api
{
  public static class Routes
  {
    public static WebApplication MapRoutes(this WebApplication app)
    {
      //app.MapGet("/users/{id}", (int id, IUserService userService) => ... );

      app.MapGet("/api", (DefaultController ctrl) => ctrl.Index());

      app.MapGet("/api/exchange-rates", async (ExchangeRateController ctrl) => await ctrl.Index());
      
      app.MapGet("/api/products", async (ProductController ctrl, HttpContext context) => await ctrl.Index(context));
      app.MapPost("/api/products/upload", async (ProductController ctrl, HttpRequest request) => await ctrl.Upload(request));
      
      return app;
    }


  }
}

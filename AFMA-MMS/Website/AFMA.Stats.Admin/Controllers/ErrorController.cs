namespace AFMA.Stats.Admin.Controllers
{
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Diagnostics;
  using Microsoft.AspNetCore.Mvc;
  using Models;

  public class ErrorController : Controller
  {
    [Route("Error")]
    [AllowAnonymous]
    public IActionResult Error()
    {
      var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
      var error = new ErrorViewModel
      {
        ErrorTitle = "Error loading the requested page",
        ExceptionMessage = exceptionDetails.Error.Message,
        InnerExceptionMessage = exceptionDetails.Error.InnerException?.Message,
        Path = exceptionDetails.Path,
        Referrer = Request.Headers["Referrer"].ToString(),
      };

      return View("Error", error);
    }

    [Route("Error/{statusCode}")]
    [AllowAnonymous]

    public IActionResult PageError(int statusCode)
    {
      switch (statusCode)
      {
        case 404:
          var err = "Page Not Found";
          break;

        default:
          var err1 = "Generic Error";
          break;
      }

      var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
      var error = new ErrorViewModel
      {
        ErrorTitle = "Error loading the requested page",
        ExceptionMessage = exceptionDetails.Error.Message,
        InnerExceptionMessage = exceptionDetails.Error.InnerException?.Message,
        Path = exceptionDetails.Path,
        Referrer = Request.Headers["Referrer"].ToString(),
      };

      return View("Error", error);
    }
  }
}

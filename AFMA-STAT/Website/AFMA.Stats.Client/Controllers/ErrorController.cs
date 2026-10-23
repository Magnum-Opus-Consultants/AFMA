namespace AFMA.Stats.Client.Controllers
{
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Diagnostics;
  using Microsoft.AspNetCore.Mvc;
  using Models;

  [Authorize(Roles = "User")]
  public class ErrorController : Controller
  {
    [Route("Error")]
    [AllowAnonymous]
    public IActionResult Error()
    {
      // Null when /Error is requested directly (no exception in context) — guard against it
      // so the error page itself never throws.
      var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
      var error = new ErrorViewModel
      {
        ErrorTitle = "Error loading the requested page",
        ExceptionMessage = exceptionDetails?.Error?.Message,
        InnerExceptionMessage = exceptionDetails?.Error?.InnerException?.Message,
        Path = exceptionDetails?.Path,
        Referrer = Request.Headers["Referrer"].ToString(),
      };

      return View("Error", error);
    }
  }
}

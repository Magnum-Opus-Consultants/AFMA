namespace AFMA.Shared.Services.Maintenance;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

/// <summary>
/// Redirects Salmonella-module actions to the module's maintenance page while
/// <see cref="SalmonellaMaintenanceSettings.Enabled"/> is true, unless the visitor
/// holds a valid bypass cookie. Applied with <c>[ServiceFilter(typeof(SalmonellaMaintenanceFilter))]</c>.
/// Recovered from the production build of 2026-07-21; this code was never committed.
/// </summary>
public class SalmonellaMaintenanceFilter : IActionFilter
{
  private readonly IOptionsMonitor<SalmonellaMaintenanceSettings> _options;

  public SalmonellaMaintenanceFilter(IOptionsMonitor<SalmonellaMaintenanceSettings> options)
  {
    _options = options;
  }

  public void OnActionExecuting(ActionExecutingContext context)
  {
    var settings = _options.CurrentValue;

    if (!settings.Enabled)
    {
      return;
    }

    // Never redirect the maintenance page itself.
    if (string.Equals(context.ActionDescriptor.RouteValues["action"], "Maintenance", StringComparison.OrdinalIgnoreCase))
    {
      return;
    }

    // A valid bypass cookie lets the visitor use the module normally.
    if (!string.IsNullOrEmpty(settings.BypassToken)
        && context.HttpContext.Request.Cookies.TryGetValue(settings.BypassCookieName, out var cookieToken)
        && string.Equals(cookieToken, settings.BypassToken, StringComparison.Ordinal))
    {
      return;
    }

    context.Result = new RedirectToActionResult("Maintenance", "Salmonella", null);
  }

  public void OnActionExecuted(ActionExecutedContext context)
  {
  }
}

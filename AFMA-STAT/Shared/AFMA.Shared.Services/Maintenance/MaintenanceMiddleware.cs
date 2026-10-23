namespace AFMA.Shared.Services.Maintenance;

using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

/// <summary>
/// Short-circuits the pipeline with a maintenance page when
/// <see cref="MaintenanceSettings.Enabled"/> is true, unless the
/// request is from a bypassed visitor (cookie or token).
/// </summary>
public class MaintenanceMiddleware
{
  private readonly RequestDelegate _next;
  private readonly IOptionsMonitor<MaintenanceSettings> _options;

  public MaintenanceMiddleware(RequestDelegate next, IOptionsMonitor<MaintenanceSettings> options)
  {
    _next = next;
    _options = options;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    var settings = _options.CurrentValue;

    if (!settings.Enabled)
    {
      await _next(context);
      return;
    }

    // 1. Token in query string → set bypass cookie, redirect to the same URL without the token.
    var queryToken = context.Request.Query[settings.BypassQueryParameter].ToString();
    if (!string.IsNullOrEmpty(settings.BypassToken)
        && !string.IsNullOrEmpty(queryToken)
        && FixedTimeEquals(queryToken, settings.BypassToken))
    {
      context.Response.Cookies.Append(
        settings.BypassCookieName,
        settings.BypassToken,
        new CookieOptions
        {
          HttpOnly = true,
          Secure = context.Request.IsHttps,
          SameSite = SameSiteMode.Lax,
          Expires = settings.BypassCookieDays > 0
            ? DateTimeOffset.UtcNow.AddDays(settings.BypassCookieDays)
            : null,
        });

      var cleanQuery = string.Join(
        "&",
        context.Request.Query
          .Where(kv => !string.Equals(kv.Key, settings.BypassQueryParameter, StringComparison.OrdinalIgnoreCase))
          .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value.ToString())}"));

      var cleanUrl = context.Request.Path + (string.IsNullOrEmpty(cleanQuery) ? string.Empty : "?" + cleanQuery);
      context.Response.Redirect(cleanUrl);
      return;
    }

    // 2. Existing bypass cookie → let the request through to the live site.
    if (!string.IsNullOrEmpty(settings.BypassToken)
        && context.Request.Cookies.TryGetValue(settings.BypassCookieName, out var cookieToken)
        && FixedTimeEquals(cookieToken, settings.BypassToken))
    {
      await _next(context);
      return;
    }

    // 3. Otherwise — serve the maintenance page.
    context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
    context.Response.Headers["Retry-After"] = settings.RetryAfterSeconds.ToString();
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(RenderHtml(settings.Heading, settings.Message));
  }

  // Length-stable comparison so an attacker can't time-bomb the token char-by-char.
  private static bool FixedTimeEquals(string? a, string? b)
  {
    if (a is null || b is null) return false;
    if (a.Length != b.Length) return false;
    var diff = 0;
    for (var i = 0; i < a.Length; i++)
    {
      diff |= a[i] ^ b[i];
    }
    return diff == 0;
  }

  private static string RenderHtml(string heading, string message)
  {
    var safeHeading = WebUtility.HtmlEncode(heading);
    var safeMessage = WebUtility.HtmlEncode(message);

    return $@"<!doctype html>
<html lang=""en"">
<head>
  <meta charset=""utf-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <title>{safeHeading} | AFMA Statistics Portal</title>
  <link rel=""shortcut icon"" href=""/favicon.ico"">
  <link rel=""preconnect"" href=""https://fonts.googleapis.com"">
  <link rel=""preconnect"" href=""https://fonts.gstatic.com"" crossorigin>
  <link rel=""stylesheet"" href=""https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap"">
  <style>
    *, *::before, *::after {{ box-sizing: border-box; }}
    body {{ margin: 0; font-family: 'Inter', system-ui, -apple-system, sans-serif; background: linear-gradient(135deg, #f4f6f9 0%, #e9eef5 100%); color: #495057; min-height: 100vh; display: flex; align-items: center; justify-content: center; padding: 2rem; }}
    .card {{ max-width: 640px; width: 100%; background: #fff; border-radius: 12px; box-shadow: 0 8px 32px rgba(0,0,0,0.08); padding: 3rem 2.5rem; text-align: center; }}
    .logo {{ max-width: 220px; margin: 0 auto 1.75rem; display: block; }}
    h1 {{ font-size: 1.625rem; font-weight: 600; color: #2b3a55; margin: 0 0 0.75rem; letter-spacing: -0.01em; }}
    p {{ font-size: 1rem; line-height: 1.6; margin: 0 0 1rem; color: #495057; }}
    .meta {{ margin-top: 2rem; padding-top: 1.5rem; border-top: 1px solid #e9ecef; font-size: 0.85rem; color: #868e96; }}
  </style>
</head>
<body>
  <main class=""card"" role=""status"" aria-live=""polite"">
    <img src=""/assets/images/afma-full.png"" alt=""AFMA"" class=""logo"" onerror=""this.onerror=null;this.src='/img/afma-full.png';"">
    <h1>{safeHeading}</h1>
    <p>{safeMessage}</p>
    <div class=""meta"">AFMA Statistics Portal</div>
  </main>
</body>
</html>";
  }
}

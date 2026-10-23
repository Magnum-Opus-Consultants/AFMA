namespace AFMA.Shared.Services.Maintenance;

/// <summary>
/// Configuration for the maintenance-mode middleware. Bound from the
/// <c>MaintenanceSettings</c> section of appsettings.json.
/// </summary>
public class MaintenanceSettings
{
  public const string SectionName = "MaintenanceSettings";

  /// <summary>When true, all non-bypassed requests receive the maintenance page (HTTP 503).</summary>
  public bool Enabled { get; set; }

  /// <summary>Headline message shown on the maintenance page.</summary>
  public string Heading { get; set; } = "System Maintenance";

  /// <summary>Body message shown on the maintenance page.</summary>
  public string Message { get; set; } =
    "The AFMA Statistics Portal is currently undergoing scheduled maintenance. Please check back later.";

  /// <summary>
  /// Secret token. A visitor reaching the site with <c>?{BypassQueryParameter}={BypassToken}</c>
  /// is granted a bypass cookie and is then allowed to use the live site normally.
  /// Leave blank to disable the bypass mechanism entirely.
  /// </summary>
  public string? BypassToken { get; set; }

  /// <summary>Name of the cookie used to remember a successful bypass.</summary>
  public string BypassCookieName { get; set; } = "afma_maintenance_bypass";

  /// <summary>Query-string parameter that carries the bypass token (e.g. <c>?preview=xxxxx</c>).</summary>
  public string BypassQueryParameter { get; set; } = "preview";

  /// <summary>How long the bypass cookie should last. Set to 0 for a session cookie.</summary>
  public int BypassCookieDays { get; set; } = 7;

  /// <summary>Value used for the HTTP <c>Retry-After</c> response header.</summary>
  public int RetryAfterSeconds { get; set; } = 3600;
}

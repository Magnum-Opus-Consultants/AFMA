namespace AFMA.Shared.Services.Maintenance;

/// <summary>
/// Configuration for the Salmonella-module maintenance mode. Bound from the
/// <c>SalmonellaMaintenanceSettings</c> section of appsettings.json.
/// Recovered from the production build of 2026-07-21; this code was never committed.
/// </summary>
public class SalmonellaMaintenanceSettings
{
  public const string SectionName = "SalmonellaMaintenanceSettings";

  /// <summary>When true, Salmonella module requests are redirected to the module's maintenance page.</summary>
  public bool Enabled { get; set; }

  /// <summary>Headline shown on the maintenance page.</summary>
  public string Heading { get; set; } = "Salmonella Module Temporarily Unavailable";

  /// <summary>Body text shown on the maintenance page.</summary>
  public string Message { get; set; } = "The Salmonella reporting module is currently unavailable. Please check back later.";

  /// <summary>Access code that grants a bypass cookie. Leave blank to hide the bypass form.</summary>
  public string BypassToken { get; set; } = string.Empty;

  /// <summary>Name of the cookie that remembers a successful bypass.</summary>
  public string BypassCookieName { get; set; } = "afma_salmonella_bypass";

  /// <summary>How long the bypass cookie lasts. Set to 0 for a session cookie.</summary>
  public int BypassCookieDays { get; set; } = 7;
}

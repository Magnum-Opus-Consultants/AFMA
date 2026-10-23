namespace AFMA.Infrastructure.Settings
{
  /// <summary>
  /// The WimsApplicationSettings class.
  /// </summary>
  public class WimsApplicationSettings
  {
    /// <summary>
    /// Gets or sets the base Url for the application.
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the debug mode.
    /// </summary>
    public bool Debug { get; set; }

    /// <summary>
    /// Gets or sets the test site indicator.
    /// </summary>
    public bool TestSite { get; set; }

    /// <summary>
    /// Gets or sets the default cache minutes.
    /// </summary>
    public int CacheMinutes { get; set; }

    /// <summary>
    /// Gets or sets the version number.
    /// </summary>
    public string Version { get; set; } = "9.0.1";

    /// <summary>
    /// Gets or sets the email API key.
    /// </summary>
    public string EmailApiKey { get; set; } = string.Empty;
  }
}

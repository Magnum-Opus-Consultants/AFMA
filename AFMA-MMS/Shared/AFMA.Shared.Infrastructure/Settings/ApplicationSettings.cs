namespace AFMA.Infrastructure.Settings
{
  /// <summary>
  /// The ApplicationSettings class.
  /// </summary>
  public class ApplicationSettings
  {
    /// <summary>
    /// Gets or sets the deployment status flag.
    /// </summary>
    public string? Deployment { get; set; }

    /// <summary>
    /// Gets or sets the base URL for the application.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the base URL for the client application.
    /// </summary>
    public string? ClientUrl { get; set; }

    /// <summary>
    /// Gets or sets the default page size.
    /// </summary>
    public int PageSize { get; set; }

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
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// Gets or sets the CDN Url
    /// </summary>
    public string CdnUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email API key.
    /// </summary>
    public string EmailApiKey { get; set; } = string.Empty;
  }
}

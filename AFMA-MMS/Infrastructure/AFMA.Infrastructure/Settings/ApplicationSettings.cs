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
  }
}

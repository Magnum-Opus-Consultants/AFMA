namespace AFMA.ApplicationLayer.Templating.Models
{
  /// <summary>
  /// The GenericEmailModel class.
  /// </summary>
  public class GenericEmailModel
  {
    /// <summary>
    /// Gets or sets the base URL.
    /// </summary>
    public string? BaseUrl { get; set; }

    public string? Body { get; set; } = "This is a test";
  }
}

namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The MycotoxinAnalysisModel class.
  /// </summary>
  public class MycotoxinAnalysisModel
  {
    /// <summary>
    /// Gets or sets the Id of the analysis method
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the analysis method
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets of this analysis requires a comment
    /// </summary>
    public bool Other { get; set; }
  }
}

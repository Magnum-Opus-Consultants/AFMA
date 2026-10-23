namespace AFMA.Data.Entities
{
  /// <summary>
  /// The MycotoxinAnalysis class.
  /// </summary>
  public class MycotoxinAnalysis
  {
    /// <summary>
    /// Gets or sets the Id of the analysis method
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the analysis method
    /// </summary>
    public virtual string? Name { get; set; }

    /// <summary>
    /// Gets or sets of this analysis requires a comment
    /// </summary>
    public virtual bool Other { get; set; }
  }
}

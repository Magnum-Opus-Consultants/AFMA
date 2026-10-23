namespace AFMA.Data.Entities
{
  /// <summary>
  /// The MycotoxinSampleType class.
  /// </summary>
  public class MycotoxinSampleType
  {
    /// <summary>
    /// Gets or sets the Id of the sample type.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the sample type.
    /// </summary>
    public virtual string Name { get; set; }
  }
}

namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The MycotoxinSampleTypeModel class.
  /// </summary>
  [MapTo(typeof(MycotoxinSampleType))]
  public class MycotoxinSampleTypeModel
  {
    /// <summary>
    /// Gets or sets the Id of the sample type.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the sample type.
    /// </summary>
    public string Name { get; set; }
  }
}

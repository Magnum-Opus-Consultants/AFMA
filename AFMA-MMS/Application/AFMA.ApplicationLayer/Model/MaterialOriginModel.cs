namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The MaterialOriginModel class.
  /// </summary>
  [MapTo(typeof(MaterialOrigin))]
  public class MaterialOriginModel
  {
    /// <summary>
    /// Gets or sets the Id of the material origin.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the material origin.
    /// </summary>
    public string Name { get; set; }
  }
}

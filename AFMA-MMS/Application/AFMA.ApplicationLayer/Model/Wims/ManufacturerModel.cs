namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The ManufacturerModel class represents a Manufacturer in the WIMS application.
  /// </summary>
  [MapTo(typeof(Manufacturer))]
  public class ManufacturerModel
  {
    /// <summary>
    /// The unique identifier for the Manufacturer.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// The name of the Manufacturer.
    /// </summary>
    public virtual string? Description { get; set; }
  }
}

namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The SupplierModel class represents a Supplier in the WIMS application.
  /// </summary>
  [MapTo(typeof(Supplier))]
  public class SupplierModel
  {
    /// <summary>
    /// The unique identifier for the Supplier.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// The name of the Supplier.
    /// </summary>
    public virtual string? Description { get; set; }
  }
}

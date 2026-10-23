namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The Supplier class represents a supplier in the WIMS system.
  /// </summary>
  public class Supplier
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

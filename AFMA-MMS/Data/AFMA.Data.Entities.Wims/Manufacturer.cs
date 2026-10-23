namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The Manufacturer class represents a manufacturer in the WIMS system.
  /// </summary>
  public class Manufacturer
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

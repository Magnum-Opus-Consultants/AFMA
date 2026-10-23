namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The Province class.
  /// </summary>
  public class Province
  {
    /// <summary>
    /// The unique identifier for the Province.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// The name of the Province.
    /// </summary>
    public virtual string? Description { get; set; }
  }
}

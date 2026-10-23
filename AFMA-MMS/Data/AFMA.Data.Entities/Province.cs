namespace AFMA.Data.Entities
{
  /// <summary>
  /// The Province entity class.
  /// </summary>
  public class Province
  {
    /// <summary>
    /// Gets or sets the Id of the province.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the province.
    /// </summary>
    public virtual string? Description { get; set; }
  }
}

namespace AFMA.Data.Entities
{
  /// <summary>
  /// The Module entity class.
  /// </summary>
  public class Module
  {
    /// <summary>
    /// Gets or sets the Id of the member.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the member name.
    /// </summary>
    public virtual string? Description { get; set; }
  }
}

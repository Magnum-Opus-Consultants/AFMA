namespace AFMA.Data.Entities
{
  /// <summary>
  /// The MycotoxinClass class.
  /// </summary>
  public class MycotoxinClass
  {
    /// <summary>
    /// Gets or sets the Id of the mycotoxin class..
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the mycotoxin class.
    /// </summary>
    public virtual string? Name { get; set; }

    /// <summary>
    /// Gets or sets the status of the mycotoxin class.
    /// </summary>
    public virtual bool Active { get; set; }
  }
}

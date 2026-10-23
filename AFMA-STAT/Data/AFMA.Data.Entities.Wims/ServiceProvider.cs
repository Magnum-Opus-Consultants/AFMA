namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The ServiceProvider class represents a service provider in the WIMS system.
  /// </summary>
  public class ServiceProvider
  {
    /// <summary>
    /// The unique identifier for the ServiceProvider.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// The name of the ServiceProvider.
    /// </summary>
    public virtual string? Description { get; set; }
  }
}

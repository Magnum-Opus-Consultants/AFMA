namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The Service class represents a service in the WIMS system.
  /// </summary>
  public class Service
  {
    /// <summary>
    /// The unique identifier for the Service.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// The name of the Service.
    /// </summary>
    public virtual string? Description { get; set; }
  }
}

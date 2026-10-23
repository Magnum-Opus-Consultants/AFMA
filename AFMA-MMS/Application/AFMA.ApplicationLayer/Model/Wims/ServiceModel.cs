namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The ServiceModel class represents a service in the WIMS application.
  /// </summary>
  [MapTo(typeof(Service))]
  public class ServiceModel
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

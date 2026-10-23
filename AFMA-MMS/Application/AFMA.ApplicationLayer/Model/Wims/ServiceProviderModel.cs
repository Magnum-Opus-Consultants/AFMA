namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The ServiceProviderModel class represents a service provider in the WIMS application.
  /// </summary>
  [MapTo(typeof(ServiceProvider))]
  public class ServiceProviderModel
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

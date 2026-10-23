namespace AFMA.Data.Entities
{
  /// <summary>
  /// The Salmonella Sample Type class.
  /// </summary>
  public class SalmonellaSampleType
  {
    /// <summary>
    /// Gets or sets the Id of the Salmonella Sample Type
    /// </summary>
    public virtual int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the short code.
    /// </summary>
    public virtual string? ShortCode { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the Salmonella Sample Type
    /// </summary>
    public virtual string? Name { get; set; }
    
    /// <summary>
    /// Gets or sets the status of the Salmonella Sample Type
    /// </summary>
    public virtual bool Active { get; set; }
  }
}
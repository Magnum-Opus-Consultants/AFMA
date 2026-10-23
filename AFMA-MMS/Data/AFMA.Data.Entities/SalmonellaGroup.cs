namespace AFMA.Data.Entities
{
  /// <summary>
  /// The Salmonella Group Class.
  /// </summary>
  public class SalmonellaGroup
  {
    /// <summary>
    /// Gets or sets the Id of the Salmonella Group.
    /// </summary>
    public virtual int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the Salmonella Group.
    /// </summary>
    public virtual string? Name { get; set; }
    
    /// <summary>
    /// Gets or sets the display order of the Salmonella Group.
    /// </summary>
    public virtual int DisplayOrder { get; set; }
    
    /// <summary>
    /// Gets or sets the status of the Salmonella Group.
    /// </summary>
    public virtual bool Active { get; set; }
  }
}

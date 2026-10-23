namespace AFMA.Data.Entities
{
  /// <summary>
  /// The Salmonella Type Class.
  /// </summary>
  public class SalmonellaType
  {
    /// <summary>
    /// Gets or sets the Id of the Salmonella Type.
    /// </summary>
    public virtual int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the Salmonella Type.
    /// </summary>
    public virtual string? Name { get; set; }
    
    /// <summary>
    /// Gets or sets the Salmonella Group.
    /// </summary>
    public virtual SalmonellaGroup SalmonellaGroup { get; set; }
    
    /// <summary>
    /// Gets or sets the Display Order.
    /// </summary>
    public virtual int DisplayOrder { get; set; }
    
    /// <summary>
    /// Gets or sets the status of the Salmonella Type.
    /// </summary>
    public virtual bool Active { get; set; }
  }
}

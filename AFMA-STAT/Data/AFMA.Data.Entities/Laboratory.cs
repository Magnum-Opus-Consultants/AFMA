namespace AFMA.Data.Entities
{
  /// <summary>
  /// The Laboratory Class.
  /// </summary>
  public class Laboratory
  {
    /// <summary>
    /// Gets or sets the Id of the laboratory.
    /// </summary>
    public virtual int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the laboratory.
    /// </summary>
    public virtual string? Name { get; set; }

    /// <summary>
    /// Gets or sets the address of the laboratory.
    /// </summary>
    public virtual string? Address { get; set; }
    
    /// <summary>
    /// Gets or sets the status of the laboratory.
    /// </summary>
    public virtual bool Active { get; set; }
  }
}

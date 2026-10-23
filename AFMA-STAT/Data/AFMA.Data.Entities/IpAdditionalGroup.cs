namespace AFMA.Data.Entities
{
  /// <summary>
  /// The IpAdditionalGroup class.
  /// </summary>
  public class IpAdditionalGroup
  {
    /// <summary>
    /// Gets or sets the Id of the group.
    /// </summary>
    public virtual int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    public virtual string? GroupName { get; set; }
    
    /// <summary>
    /// Gets or sets the total for the group.
    /// </summary>
    public virtual double GroupTotal { get; set; }
    
    /// <summary>
    /// Gets or sets the group measurements.
    /// </summary>
    public virtual string? GroupMeasure { get; set; }
    
    /// <summary>
    /// Gets or sets the code of the group.
    /// </summary>
    public virtual string? GroupCode { get; set; }
    
    /// <summary>
    /// Gets or sets the group display order.
    /// </summary>
    public virtual int DisplayOrder { get; set; }
    
    /// <summary>
    /// Gets or sets the status for the group.
    /// </summary>
    public virtual bool Active { get; set; }
  }
}

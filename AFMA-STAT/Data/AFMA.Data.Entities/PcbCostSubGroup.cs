namespace AFMA.Data.Entities
{
  /// <summary>
  /// The PcbCostSubGroup class.
  /// </summary>
  public class PcbCostSubGroup
  {
    /// <summary>
    /// Gets or sets the Id of the group.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the main group.
    /// </summary>
    public virtual PcbCostMainGroup PcbCostMainGroup { get; set; }

    /// <summary>
    /// Gets or sets the sub group header name.
    /// </summary>
    public virtual string? HeaderName { get; set; }

    /// <summary>
    /// Gets or sets the sub group code.
    /// </summary>
    public virtual string? SubGroupCode { get; set; }

    /// <summary>
    /// Gets or sets the sub group name.
    /// </summary>
    public virtual string? SubGroupName { get; set; }

    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    public virtual int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public virtual bool Active { get; set; }

    /// <summary>
    /// Gets or sets the list of Cost Item Groups
    /// </summary>
    public virtual ICollection<PcbCostItemGroup> PcbCostItemGroups { get; set; }
  }
}

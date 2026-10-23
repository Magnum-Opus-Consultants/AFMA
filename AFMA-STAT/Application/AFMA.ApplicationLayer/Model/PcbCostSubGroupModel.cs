namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The PcbCostSubGroupModel class.
  /// </summary>
  public class PcbCostSubGroupModel
  {
    /// <summary>
    /// Gets or sets the Id of the group.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the main group.
    /// </summary>
    public int PcbCostMainGroupId { get; set; }

    /// <summary>
    /// Gets or sets the main group.
    /// </summary>
    public string PcbCostMainGroup { get; set; }

    /// <summary>
    /// Gets or sets the sub group header name.
    /// </summary>
    public string? HeaderName { get; set; }

    /// <summary>
    /// Gets or sets the sub group code.
    /// </summary>
    public string? SubGroupCode { get; set; }

    /// <summary>
    /// Gets or sets the sub group name.
    /// </summary>
    public string? SubGroupName { get; set; }

    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Gets or sets the list of Cost Item Groups
    /// </summary>
    public ICollection<PcbCostItemGroupModel> PcbCostItemGroups { get; set; }
  }
}

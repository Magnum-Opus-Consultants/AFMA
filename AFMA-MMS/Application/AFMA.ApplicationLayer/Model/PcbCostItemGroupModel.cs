namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The PcbCostItemGroupModel class.
  /// </summary>
  public class PcbCostItemGroupModel
  {
    /// <summary>
    /// Gets or sets the Id of item group.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the sub group.
    /// </summary>
    public int PcbCostSubGroupId { get; set; }

    /// <summary>
    /// Gets or sets the sub group.
    /// </summary>
    public string? PcbCostSubGroup { get; set; }

    /// <summary>
    /// Gets or sets the header name.
    /// </summary>
    public string? HeaderName { get; set; }

    /// <summary>
    /// Gets or sets the group name.
    /// </summary>
    public string? ItemGroupName { get; set; }

    /// <summary>
    /// Gets or sets the group code/
    /// </summary>
    public string? ItemGroupCode { get; set; }

    /// <summary>
    /// Gets or sets the group help text.
    /// </summary>
    public string? ItemGroupHelpText { get; set; }

    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Gets or sets the list of Cost Items
    /// </summary>
    public ICollection<PcbCostItemModel> PcbCostItems { get; set; }
  }
}

namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The PcbCostMainGroupModel class.
  /// </summary>
  public class PcbCostMainGroupModel
  {
    public PcbCostMainGroupModel()
    {
      SubGroups = new List<PcbCostSubGroupModel>();
    }

    /// <summary>
    /// Gets or sets the Id of the group.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the group code.
    /// </summary>
    public string? GroupCode { get; set; }

    /// <summary>
    /// Gets or sets the group name.
    /// </summary>
    public string? GroupName { get; set; }

    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public bool Active { get; set; }

    public ICollection<PcbCostSubGroupModel> SubGroups { get; set; }
  }
}

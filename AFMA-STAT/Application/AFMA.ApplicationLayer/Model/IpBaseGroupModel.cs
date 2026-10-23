namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The IpBaseGroupModel class.
  /// </summary>
  public class IpBaseGroupModel
  {
    /// <summary>
    /// Gets or sets the Id of the group.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    [Display(Name = "Group Name")]
    public string? GroupName { get; set; }

    /// <summary>
    /// Gets or sets the total for the group.
    /// </summary>
    [Display(Name = "Group Total")]
    public double GroupTotal { get; set; }

    /// <summary>
    /// Gets or sets the group measurements.
    /// </summary>
    [Display(Name = "Group Measure")]
    public string? GroupMeasure { get; set; }

    /// <summary>
    /// Gets or sets the code of the group.
    /// </summary>
    [Display(Name = "Group Code")]
    public string? GroupCode { get; set; }

    /// <summary>
    /// Gets or sets the group display order.
    /// </summary>
    [Display(Name = "Display Order")]
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the status for the group.
    /// </summary>
    [Display(Name = "Status")]
    public bool Active { get; set; }
  }
}

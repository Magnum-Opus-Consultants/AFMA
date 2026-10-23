namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The IndustryCostGroupModel class.
  /// </summary>
  [MapTo(typeof(IndustryCostGroup))]
  public class IndustryCostGroupModel
  {
    public IndustryCostGroupModel()
    {
      Items = new List<IndustryGroupItemModel>();
    }

    /// <summary>
    /// Gets or sets the Id of the industry cost group.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Id of the industry profile report.
    /// </summary>
    public int IndustryProfileReportId { get; set; }

    /// <summary>
    /// Gets or sets the name of the industry cost group.
    /// </summary>
    public string? GroupName { get; set; }

    /// <summary>
    /// Gets or sets the financial year.
    /// </summary>
    public int FinancialYear { get; set; }
    
    /// <summary>
    /// Gets or sets the linked table.
    /// </summary>
    public string? LinkedTable { get; set; }

    /// <summary>
    /// Gets or sets the linked Id.
    /// </summary>
    public int LinkedId { get; set; }
    
    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public bool Active { get; set; }

    public ICollection<IndustryGroupItemModel> Items { get; set; }
  }
}

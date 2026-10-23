namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The RunningCostExpense class.
  /// </summary>
  public class RunningCostModel
  {
    /// <summary>
    /// Gets or sets the Id of the running cost.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the running cost group.
    /// </summary>
    [Display(Name = "Running Cost Group")]
    public int RunningCostGroupId { get; set; }

    /// <summary>
    /// Gets or sets the running cost group.
    /// </summary>
    [Display(Name = "Running Cost Group")]
    public string? RunningCostGroup { get; set; }

    /// <summary>
    /// Gets or sets the item code.
    /// </summary>
    [Display(Name = "Item Code")]
    public string? ItemCode { get; set; }

    /// <summary>
    /// Gets or sets the item name.
    /// </summary>
    [Display(Name = "Item Name")]
    public string? ItemName { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    [Display(Name = "Description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the total cost.
    /// </summary>
    [Display(Name = "Total Cose")]
    public decimal TotalCost { get; set; }

    /// <summary>
    /// Gets or sets production enabled flag.
    /// </summary>
    [Display(Name = "Production Enabled")]
    public bool ProductionEnable { get; set; }

    /// <summary>
    /// Gets or sets the sales enabled flag.
    /// </summary>
    [Display(Name = "Sales Enabled")]
    public bool SalesEnable { get; set; }

    /// <summary>
    /// Gets or sets the administration enabled flag.
    /// </summary>
    [Display(Name = "Administration Enabled")]
    public bool AdministrationEnable { get; set; }

    /// <summary>
    /// Gets or sets the road transport enabled flag.
    /// </summary>
    [Display(Name = "Road Transport Enabled")]
    public bool RoadTransportEnable { get; set; }

    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    [Display(Name = "Display Order")]
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the excel row.
    /// </summary>
    [Display(Name = "Excel Row")]
    public int ExcelRow { get; set; }

    /// <summary>
    /// Gets or sets the excel sheet name.
    /// </summary>
    [Display(Name = "Excel Sheet Name")]
    public string? ExcelSheetName { get; set; }

    /// <summary>
    /// Gets or sets the excel sheet Id.
    /// </summary>
    [Display(Name = "Excel Sjeet Id")]
    public int? ExcelSheetId { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    [Display(Name = "Status")]
    public bool Active { get; set; }

    /// <summary>
    /// Gets or sets the excel column for the production value.
    /// </summary>
    [Display(Name = "Production Excel Column")]
    public int ExcelColumnProduction { get; set; }

    /// <summary>
    /// Gets or sets the excel column for the administration value.
    /// </summary>
    [Display(Name = "Administration Excel Column")]
    public int ExcelColumnAdministration { get; set; }

    /// <summary>
    /// Gets or sets the excel column for the sales value.
    /// </summary>
    [Display(Name = "Sales Excel Column")]
    public int ExcelColumnSales { get; set; }

    /// <summary>
    /// Gets or sets the excel column for the road transport value.
    /// </summary>
    [Display(Name = "Road Transport Excel Column")]
    public int ExcelColumnRoadTransport { get; set; }
  }
}

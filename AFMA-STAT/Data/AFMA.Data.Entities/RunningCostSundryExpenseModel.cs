namespace AFMA.Data.Entities
{
  using System.ComponentModel.DataAnnotations;

  /// <summary>
  /// The RunningCostSundryExpenseModel class.
  /// </summary>
  public class RunningCostSundryExpenseModel
  {
    /// <summary>
    /// Gets or sets the Id of the sundry expense.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the item name.
    /// </summary>
    [Display(Name = "Item Name")]
    public string? ItemName { get; set; }

    /// <summary>
    /// Gets or sets the production enabled flag.
    /// </summary>
    [Display(Name = "Production Enabled")]
    public bool ProductionEnabled { get; set; }

    /// <summary>
    /// Gets or sets the marketing enabled flag.
    /// </summary>
    [Display(Name = "Marketing Enabled")]
    public bool MarketingEnabled { get; set; }

    /// <summary>
    /// Gets or sets the administration enabled flag.
    /// </summary>
    [Display(Name = "Administration Enabled")]
    public bool AdministrationEnabled { get; set; }

    /// <summary>
    /// Gets or sets the road transport enabled flag.
    /// </summary>
    [Display(Name = "Road Transport Enabled")]
    public bool RoadTransportEnabled { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    [Display(Name = "Active")]
    public bool Active { get; set; }

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
    /// Gets or sets the excel column for production value.
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

    /// <summary>
    /// Gets or sets the excel sheet name.
    /// </summary>
    [Display(Name = "Excel Sheet Name")]
    public string? ExcelSheetName { get; set; }

    /// <summary>
    /// Gets or sets the excel sheet Id.
    /// </summary>
    [Display(Name = "Excel Sheet Id")]
    public int? ExcelSheetId { get; set; }
  }
}

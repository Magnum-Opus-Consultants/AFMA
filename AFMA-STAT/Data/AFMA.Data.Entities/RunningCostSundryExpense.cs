namespace AFMA.Data.Entities
{
  /// <summary>
  /// The RunningCostSundryExpense class.
  /// </summary>
  public class RunningCostSundryExpense
  {
    /// <summary>
    /// Gets or sets the Id of the sundry expense.
    /// </summary>
    public virtual int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the item name.
    /// </summary>
    public virtual string? ItemName { get; set; }
    
    /// <summary>
    /// Gets or sets the production enabled flag.
    /// </summary>
    public virtual bool ProductionEnabled { get; set; }
    
    /// <summary>
    /// Gets or sets the marketing enabled flag.
    /// </summary>
    public virtual bool MarketingEnabled { get; set; }
    
    /// <summary>
    /// Gets or sets the administration enabled flag.
    /// </summary>
    public virtual bool AdministrationEnabled { get; set; }
    
    /// <summary>
    /// Gets or sets the road transport enabled flag.
    /// </summary>
    public virtual bool RoadTransportEnabled { get; set; }
    
    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public virtual bool Active { get; set; }
    
    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    public virtual int DisplayOrder { get; set; }
    
    /// <summary>
    /// Gets or sets the excel row.
    /// </summary>
    public virtual int ExcelRow { get; set; }
    
    /// <summary>
    /// Gets or sets the excel column for production value.
    /// </summary>
    public virtual int ExcelColumnProduction { get; set; }
    
    /// <summary>
    /// Gets or sets the excel column for the administration value.
    /// </summary>
    public virtual int ExcelColumnAdministration { get; set; }
    
    /// <summary>
    /// Gets or sets the excel column for the sales value.
    /// </summary>
    public virtual int ExcelColumnSales { get; set; }
    
    /// <summary>
    /// Gets or sets the excel column for the road transport value.
    /// </summary>
    public virtual int ExcelColumnRoadTransport { get; set; }
    
    /// <summary>
    /// Gets or sets the excel sheet name.
    /// </summary>
    public virtual string? ExcelSheetName { get; set; }
    
    /// <summary>
    /// Gets or sets the excel sheet Id.
    /// </summary>
    public virtual int? ExcelSheetId { get; set; }
  }
}

namespace AFMA.Data.Entities
{
  /// <summary>
  /// The RunningCost class.
  /// </summary>
  public class RunningCost
  {
    /// <summary>
    /// Gets or sets the Id of the running cost.
    /// </summary>
    public virtual int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the running cost group.
    /// </summary>
    public virtual RunningCostGroup RunningCostGroup { get; set; }
    
    /// <summary>
    /// Gets or sets the item code.
    /// </summary>
    public virtual string? ItemCode { get; set; }
    
    /// <summary>
    /// Gets or sets the item name.
    /// </summary>
    public virtual string? ItemName { get; set; }
    
    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public virtual string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the total cost.
    /// </summary>
    public virtual decimal TotalCost { get; set; }
    
    /// <summary>
    /// Gets or sets production enabled flag.
    /// </summary>
    public virtual bool ProductionEnable { get; set; }
    
    /// <summary>
    /// Gets or sets the sales enabled flag.
    /// </summary>
    public virtual bool SalesEnable { get; set; } 
    
    /// <summary>
    /// Gets or sets the administration enabled flag.
    /// </summary>
    public virtual bool AdministrationEnable { get; set; }
    
    /// <summary>
    /// Gets or sets the road transport enabled flag.
    /// </summary>
    public virtual bool RoadTransportEnable { get; set; }
    
    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    public virtual int DisplayOrder { get; set; }
    
    /// <summary>
    /// Gets or sets the excel row.
    /// </summary>
    public virtual int ExcelRow { get; set; }
    
    /// <summary>
    /// Gets or sets the excel sheet name.
    /// </summary>
    public virtual string? ExcelSheetName { get; set; }
    
    /// <summary>
    /// Gets or sets the excel sheet Id.
    /// </summary>
    public virtual int? ExcelSheetId { get; set; }
    
    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public virtual bool Active { get; set; }
    
    /// <summary>
    /// Gets or sets the excel column for the production value.
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
  }
}

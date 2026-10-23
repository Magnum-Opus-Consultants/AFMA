namespace AFMA.Data.Entities
{
  /// <summary>
  /// The RunningCostGroup class.
  /// </summary>
  public class RunningCostGroup
  {
    /// <summary>
    /// Gets or sets the Id of the group.
    /// </summary>
    public virtual int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the group code.
    /// </summary>
    public virtual string? GroupCode { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    public virtual string? GroupName { get; set; }
    
    /// <summary>
    /// Gets or sets the group total.
    /// </summary>
    public virtual double GroupTotal { get; set; }
    
    /// <summary>
    /// Gets or sets the production total.
    /// </summary>
    public virtual double TotalProduction { get; set; }
    
    /// <summary>
    /// Gets or sets the sales total.
    /// </summary>
    public virtual double TotalSales { get; set; }
    
    /// <summary>
    /// Gets or sets the administration total.
    /// </summary>
    public virtual double TotalAdministration { get; set; }
    
    /// <summary>
    /// Gets or sets the road transport total.
    /// </summary>
    public virtual double TotalRoadTransport { get; set; }
    
    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    public virtual int DisplayOrder { get; set; }
    
    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public virtual bool Active { get; set; }
    
    /// <summary>
    /// Gets or sets the note.
    /// </summary>
    public virtual string? Note { get; set; }
    
    /// <summary>
    /// Gets or sets the production enabled flag.
    /// </summary>
    public virtual bool ProductionEnabled { get; set; }
    
    /// <summary>
    /// Gets or sets the sales enabled flag.
    /// </summary>
    public virtual bool SalesEnabled { get; set; }
    
    /// <summary>
    /// Gets or sets the administration enabled flag.
    /// </summary>
    public virtual bool AdministrationEnabled { get; set; }
    
    /// <summary>
    /// Gets or sets the road transport enabled flag.
    /// </summary>
    public virtual bool RoadTransportEnabled { get; set; }
    
    /// <summary>
    /// Gets or sets the group sub items flag. 
    /// </summary>
    public virtual bool GroupHasSubItems { get; set; }
    
    /// <summary>
    /// Gets or sets production excel column.
    /// </summary>
    public virtual int ExcelColumnProduction { get; set; }
    
    /// <summary>
    /// Gets or sets the administration excel column.
    /// </summary>
    public virtual int ExcelColumnAdministration { get; set; }
    
    /// <summary>
    /// Gets or sets the sales excel column.
    /// </summary>
    public virtual int ExcelColumnSales { get; set; }
    
    /// <summary>
    /// Gets or sets the road transport excel column.
    /// </summary>
    public virtual int ExcelColumnRoadTransport { get; set; }
    
    /// <summary>
    /// Gets or sets excel sheet name.
    /// </summary>
    public virtual string? ExcelSheetName { get; set; }
    
    /// <summary>
    /// Gets or sets the excel sheet Id.
    /// </summary>
    public virtual int ExcelSheetId { get; set; }
    
    /// <summary>
    /// Gets or sets the excel row.
    /// </summary>
    public virtual int ExcelRow { get; set; }
    
    /// <summary>
    /// Gets or sets the calculation type.
    /// </summary>
    public virtual int CalculationType { get; set; }

    public virtual ICollection<RunningCost> RunningCosts { get; set; }
  }
}

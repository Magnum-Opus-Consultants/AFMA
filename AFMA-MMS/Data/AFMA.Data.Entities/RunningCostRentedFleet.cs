namespace AFMA.Data.Entities
{
  /// <summary>
  /// The RunningCostRentedFleet class.
  /// </summary>
  public class RunningCostRentedFleet
  {
    /// <summary>
    /// Gets or sets the Id of the rented fleet.
    /// </summary>
    public virtual int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the item code.
    /// </summary>
    public virtual string? ItemCode { get; set; }
    
    /// <summary>
    /// Gets or sets the item name.
    /// </summary>
    public virtual string? ItemName { get; set; }
    
    /// <summary>
    /// Gets or sets the note.
    /// </summary>
    public virtual string? Note { get; set; }
    
    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public virtual bool Active { get; set; }
    
    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    public virtual int DisplayOrder { get; set; }
    
    /// <summary>
    /// Gets or sets the excel sheet name.
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
    /// Gets or sets the excel column.
    /// </summary>
    public virtual int ExcelColumn { get; set; }
  }
}

namespace AFMA.Data.Entities
{
  /// <summary>
  /// The PcbCostItem class.
  /// </summary>
  public class PcbCostItem
  {
    /// <summary>
    /// Gets or sets the Id of the item.
    /// </summary>
    public virtual int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the item group.
    /// </summary>
    public virtual PcbCostItemGroup PcbCostItemGroup { get; set; }
    
    /// <summary>
    /// Gets or sets the item code.
    /// </summary>
    public virtual string? ItemCode { get; set; }
    
    /// <summary>
    /// Gets or sets the item description.
    /// </summary>
    public virtual string? ItemDescription { get; set; }
    
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public virtual decimal Value { get; set; }
    
    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    public virtual int DisplayOrder { get; set; }
    
    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public virtual bool Active { get; set; }
    
    /// <summary>
    /// Gets or sets the editable flag.
    /// </summary>
    public virtual bool Editable { get; set; }
    
    /// <summary>
    /// Gets or sets the excel sheet name.
    /// </summary>
    public virtual string? ExcelSheetName { get; set; }
    
    /// <summary>
    /// Gets or sets the excel sheet Id.
    /// </summary>
    public virtual string? ExcelSheetId { get; set; }
    
    /// <summary>
    /// Gets or sets the excel row.
    /// </summary>
    public virtual int ExcelRow { get; set; }
    
    /// <summary>
    /// Gets or sets the excel column.
    /// </summary>
    public virtual int ExcelColumn { get; set; }
    
    /// <summary>
    /// Gets or sets the add to total flag.
    /// </summary>
    public virtual bool AddToTotal { get; set; }
    
    /// <summary>
    /// Gets or sets the readonly flag.
    /// </summary>
    public virtual bool ReadOnly { get; set; }
  }
}

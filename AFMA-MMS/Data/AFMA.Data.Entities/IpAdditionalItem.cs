namespace AFMA.Data.Entities
{
  /// <summary>
  /// The IpAdditionalItem class.
  /// </summary>
  public class IpAdditionalItem
  {
    /// <summary>
    /// Gets or sets the Id of the item.
    /// </summary>
    public virtual int Id { get; set; }
    public virtual IpAdditionalGroup IpAdditionalGroup { get; set; }
    public virtual decimal ReplacementValue { get; set; }
    public virtual double Quantity { get; set; }
    public virtual string? ItemName { get; set; }
    public virtual string? Description { get; set; }
    public virtual int DisplayOrder { get; set; }
    public virtual int ExcelRow { get; set; }
    public virtual int ExcelColumn { get; set; }
    public virtual string? ExcelSheetName { get; set; }
    public virtual int? ExcelSheetId { get; set; }
    public virtual bool Active { get; set; }
  }
}

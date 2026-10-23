namespace AFMA.Data.Entities
{
  public class IpBaseItem
  {
    public virtual int Id { get; set; }
    public virtual double Tons { get; set; }
    public virtual IpBaseGroup IpBaseGroup { get; set; }
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

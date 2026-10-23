namespace AFMA.Data.Entities
{
  public class IpAdminItem
  {
    public virtual int Id { get; set; }
    public virtual decimal CostInRand { get; set; }
    public virtual double QuantityOrUnits { get; set; }
    public virtual IpAdminGroup IpAdminGroup { get; set; }
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

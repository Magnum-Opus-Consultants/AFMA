namespace AFMA.ApplicationLayer.Model
{
  public class IpAdminItemModel
  {
    public int Id { get; set; }

    [Display(Name = "Cost In Rand")]
    public decimal CostInRand { get; set; }

    [Display(Name = "Quantity Or Units")]
    public double QuantityOrUnits { get; set; }

    [Display(Name = "Admin Group")]
    public int IpAdminGroupId { get; set; }

    [Display(Name = "Admin Group")]
    public string? IpAdminGroup { get; set; }

    [Display(Name = "Item Name")]
    public string? ItemName { get; set; }

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Display Order")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Excel Row")]
    public int ExcelRow { get; set; }

    [Display(Name = "Excel Column")]
    public int ExcelColumn { get; set; }

    [Display(Name = "Excel Sheet Name")]
    public string? ExcelSheetName { get; set; }

    [Display(Name = "Excel Sheet Id")]
    public int? ExcelSheetId { get; set; }

    [Display(Name = "Status")]
    public bool Active { get; set; }
  }
}

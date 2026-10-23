namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The IpAdditionalItemModel class.
  /// </summary>
  public class IpAdditionalItemModel
  {
    public int Id { get; set; }

    [Display(Name = "Additional Group")]
    public int IpAdditionalGroupId { get; set; }

    [Display(Name = "Additional Group")]
    public string? IpAdditionalGroup { get; set; }

    [Display(Name = "Replacement Value")]
    public decimal ReplacementValue { get; set; }

    [Display(Name = "Quantity")]
    public double Quantity { get; set; }

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

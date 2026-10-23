namespace AFMA.ApplicationLayer.Model
{
  [MapTo(typeof(Product))]
  public class ProductModel
  {
    public int Id { get; set; }

    [Display(Name = "Product Name")]
    public string? Name { get; set; }

    [Display(Name = "Product Group")]
    public int? ProductGroupId { get; set; }

    [Display(Name = "Product Name")]
    public string? ProductGroup { get; set; }

    [Display(Name = "Code")]
    public string? Code { get; set; }

    [Display(Name = "Display Order")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Active")]
    public bool Active { get; set; }

    [Display(Name = "Visible To Members")]
    public bool VisibleToMembers { get; set; }

    [Display(Name = "Requires Other Description")]
    public bool Other { get; set; }

    [Display(Name = "Allow Duplicated")]
    public bool AllowDuplicates { get; set; }
  }
}

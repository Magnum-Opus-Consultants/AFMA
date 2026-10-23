namespace AFMA.ApplicationLayer.Model
{
  using System.ComponentModel.DataAnnotations;

  [MapTo(typeof(ProductGroup))]
  public class ProductGroupModel
  {
    public int Id { get; set; }

    [Display(Name = "Name")]
    public string? Name { get; set; }

    [Display(Name = "Code")]
    public string? Code { get; set; }

    [Display(Name = "Display Order")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Active")]
    public bool Active { get; set; }

    //TODO: add this
    //public virtual SalmonellaSampleTypeId SalmonellaSampleType { get; set; }

    [Display(Name = "Hidden In Feedsales")]
    public bool HideOnFeedsales { get; set; }

    [Display(Name = "Hidden In Salmonella")]
    public bool HideOnSalmonella { get; set; }

    [Display(Name = "Visible To Members")]
    public bool VisibleToMembers { get; set; }

    [Display(Name = "Salmonella Product Group Category")]
    public int? SalmonellaProductGroupCategoryId { get; set; }

    [Display(Name = "Salmonella Product Group Category")]
    public string? SalmonellaProductGroupCategory { get; set; }

    [Display(Name = "Products")]
    public IEnumerable<ProductModel> Products { get; set; } = new List<ProductModel>();
  }
}

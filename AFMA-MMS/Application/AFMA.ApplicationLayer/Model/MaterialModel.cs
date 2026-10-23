namespace AFMA.ApplicationLayer.Model
{
  using System.ComponentModel.DataAnnotations;

  [MapTo(typeof(Material))]
  public class MaterialModel
  {
    public int Id { get; set; }
    
    [Display(Name = "Name")]
    public string? Name { get; set; }

    [Display(Name = "Category")]
    public int? MaterialCategoryId { get; set; }

    [Display(Name = "Category")]
    public string? MaterialCategory { get; set; }

    [Display(Name = "Code")]
    public string? Code { get; set; }

    [Display(Name = "Active")]
    public bool Active { get; set; }

    [Display(Name = "Salmonella Sample Type")]
    public int? SalmonellaSampleTypeId { get; set; }

    [Display(Name = "Salmonella Sample Type")]
    public string? SalmonellaSampleType { get; set; }

    [Display(Name = "Salmonella Report Group")]
    public int? SalmonellaReportGroupId { get; set; }

    [Display(Name = "Salmonella Report Group")]
    public string? SalmonellaReportGroup { get; set; }

    [Display(Name = "Feedsales Report Group")]
    public int? FeedsalesReportGroupId { get; set; }

    [Display(Name = "Feedsales Report Group")]
    public string? FeedsalesReportGroup { get; set; }

    [Display(Name = "Hidden In Feedsales")]
    public bool HideOnFeedsales { get; set; }

    [Display(Name = "Hidden In Salmonella")]
    public bool HideOnSalmonella { get; set; }

    [Display(Name = "Visible To Members")]
    public bool VisibleToMembers { get; set; }

    [Display(Name = "Requires Other")]
    public bool Other { get; set; }

    [Display(Name = "Allow Duplicates")]
    public bool AllowDuplicates { get; set; }

    [Display(Name = "Show Alert Message")]
    public bool ShowAlert { get; set; }

    [Display(Name = "Alert Message")]
    public string? AlertMessage { get; set; }
  }
}

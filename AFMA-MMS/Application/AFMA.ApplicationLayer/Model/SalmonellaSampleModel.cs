namespace AFMA.ApplicationLayer.Model
{
  using System.ComponentModel.DataAnnotations;
  using Alpha.Services.Models.Attributes;

  /// <summary>
  /// The SalmonellaSampleModel class
  /// </summary>
  [MapTo(typeof(SalmonellaSample))]
  public class SalmonellaSampleModel
  {
    /// <summary>
    /// Gets or sets the Id of the sample
    /// </summary>
    [Display(Name = "Sample Id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Id of the salmonella report
    /// </summary>
    [Display(Name = "Report No")]
    public int SalmonellaReportId { get; set; }

    /// <summary>
    /// Gets or sets the salmonella sample type
    /// </summary>
    [Display(Name = "Sample Type")]
    //[Required(ErrorMessage = "Please select a sample type.")]
    //[Range(1, int.MaxValue, ErrorMessage = "Please select a sample type.")]
    public int SalmonellaSampleTypeId { get; set; }

    /// <summary>
    /// Gets or sets the salmonella sample type
    /// </summary>
    [Display(Name = "Sample Type")]
    public string SalmonellaSampleType { get; set; }

    /// <summary>
    /// Gets or sets the salmonella sample type Short code.
    /// </summary>
    [Display(Name = "Sample Type Short Code")]
    public string SalmonellaSampleTypeShortCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets ior sets the salmonella detected indicator
    /// </summary>
    [Display(Name = "Salmonella Detected")]
    public bool SalmonellaDetected { get; set; }

    /// <summary>
    /// Gets or sets the product
    /// </summary>
    [Display(Name = "Finished Product")]
    public int? ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product
    /// </summary>
    [Display(Name = "Finished Product")]
    public string? Product { get; set; }

    /// <summary>
    /// Gets or sets the material
    /// </summary>
    [Display(Name = "Raw Material")]
    //[RequiredIf(nameof(SalmonellaSampleTypeShortCode), "RAW", ErrorMessage = "Please select a raw material.")]
    //[Range(1, int.MaxValue, ErrorMessage = "Please select a raw material.")]
    public virtual int? MaterialId { get; set; }

    /// <summary>
    /// Gets or sets the material
    /// </summary>
    [Display(Name = "Raw Material")]
    public string? Material { get; set; }

    /// <summary>
    /// Gets or sets the additional info
    /// </summary>
    [Display(Name = "Additional Info")]
    public string? AdditionalInfo { get; set; }

    /// <summary>
    /// Gets or sets the note
    /// </summary>
    [Display(Name = "Note")]
    public string? Note { get; set; }

    /// <summary>
    /// Gets or sets the main location
    /// </summary>
    [Display(Name = "Main Location")]
    //[RequiredIf(nameof(SalmonellaSampleTypeShortCode), "DSB", ErrorMessage = "Please select a sample location.")]
    //[Range(1, int.MaxValue, ErrorMessage = "Please select a sample location.")]
    public int? SalmonellaSampleTypeItemId { get; set; }

    /// <summary>
    /// Gets or sets the main location
    /// </summary>
    [Display(Name = "Main Location")]
    public string? SalmonellaSampleTypeItem { get; set; }

    /// <summary>
    /// Gets or sets the sub location
    /// </summary>
    [Display(Name = "Sub Location")]
    //[RequiredIf(nameof(SalmonellaSampleTypeShortCode), "DSB", ErrorMessage = "Please select a sample sub type.")]
    //[Range(1, int.MaxValue, ErrorMessage = "Please select a sample sub type.")]
    public int? SalmonellaSampleTypeSubItemId { get; set; }

    /// <summary>
    /// Gets or sets the sub location
    /// </summary>
    [Display(Name = "Sub Location")]
    public string? SalmonellaSampleTypeSubItem { get; set; }

    /// <summary>
    /// Gets or sets the salmonella group
    /// </summary>
    [Display(Name = "Salmonlla Group")]
    public int? SalmonellaGroupId { get; set; }

    /// <summary>
    /// Gets or sets the salmonella group
    /// </summary>
    [Display(Name = "Salmonella Group")]
    public virtual string? SalmonellaGroup { get; set; }

    /// <summary>
    /// Gets or sets the salmonella type
    /// </summary>
    [Display(Name = "Salmonella Type")]
    //[RequiredIf(nameof(SalmonellaDetected), true, ErrorMessage = "Please specify a salmonella type.")]
    //[Range(1, int.MaxValue, ErrorMessage = "Please specify a salmonella type.")]
    public int? SalmonellaTypeId { get; set; }

    /// <summary>
    /// Gets or sets the salmonella type
    /// </summary>
    [Display(Name = "Salmonella Type")]
    public string? SalmonellaType { get; set; }

    /// <summary>
    /// Gets or sets the product group
    /// </summary>
    [Display(Name = "Product Group")]
    //[RequiredIf(nameof(SalmonellaSampleTypeShortCode), "FPD", ErrorMessage = "Please select a product.")]
    //[Range(1, int.MaxValue, ErrorMessage = "Please select a product.")]
    public int? ProductGroupId { get; set; }

    /// <summary>
    /// Gets or sets the submitted indicator.
    /// </summary>
    [Display(Name = "Submitted")]
    public bool IsSubmitted { get; set; }

    /// <summary>
    /// Gets or sets the member name.
    /// </summary>
    [Display(Name = "Member")]
    public string? Member { get; set; }

    /// <summary>
    /// Gets or sets the member branch name.
    /// </summary>
    [Display(Name = "Branch")]
    public string? MemberBranch { get; set; }

    /// <summary>
    /// Gets or sets the product group
    /// </summary>
    [Display(Name = "Product Group")]
    public string? ProductGroup { get; set; }

    public void ClearSelectedSalmonellaSampleData()
    {
      this.AdditionalInfo = null;
      this.SalmonellaSampleTypeItemId = null;
      this.SalmonellaSampleTypeSubItemId = null;
      this.MaterialId = null;
      this.ProductId = null;
    }

    public void ResetSalmonallaType()
    {
      this.SalmonellaTypeId = null;
    }
  }
}

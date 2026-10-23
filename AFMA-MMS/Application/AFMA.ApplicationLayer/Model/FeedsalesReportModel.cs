namespace AFMA.ApplicationLayer.Model
{
  using System;
  using System.ComponentModel.DataAnnotations;

  /// <summary>
  /// The FeedsalesReportModel class
  /// </summary>
  [MapTo(typeof(FeedsalesReport))]
  public class FeedsalesReportModel
  {
    /// <summary>
    /// Gets or sets the Id of the Report
    /// </summary>
    [Display(Name = "Report No")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the submission state
    /// </summary>
    [Display(Name = "Submitted")]
    public bool IsSubmitted { get; set; }

    /// <summary>
    /// Gets or sets the date submitted
    /// </summary>
    [Display(Name = "Submitted ")]
    public DateTime? SubmittedDate { get; set; }

    /// <summary>
    /// Gets or sets the Member submitting the report
    /// </summary>
    [Display(Name = "Member")]
    public int MemberId { get; set; }

    /// <summary>
    /// Gets or sets the Member submitting the report
    /// </summary>
    [Display(Name = "Member")]
    public string Member { get; set; }

    /// <summary>
    /// Gets or sets the Member Branch
    /// </summary>
    [Display(Name = "Branch")]
    public int MemberBranchId { get; set; }

    /// <summary>
    /// Gets or sets the Member Branch
    /// </summary>
    [Display(Name = "Branch")]
    public string MemberBranch { get; set; }

    /// <summary>
    /// Gets or sets the Member Account
    /// </summary>
    [Display(Name = "Submitted By")]
    public int? MemberAccountId { get; set; }

    /// <summary>
    /// Gets or sets the Member Account
    /// </summary>
    [Display(Name = "Submitted By")]
    public string? MemberAccount { get; set; }

    /// <summary>
    /// Gets or sets the Member Account Email address
    /// </summary>
    [Display(Name = "Member Account Email")]
    public string MemberAccountEmail { get; set; }

    /// <summary>
    /// Gets or sets the user who created the report
    /// </summary>
    [Display(Name = "Created By")]
    public string CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the created date
    /// </summary>
    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the due date for the report
    /// </summary>
    [Display(Name = "Due Date")]
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Gets or sets the info for date
    /// </summary>
    [Display(Name = "Info For")]
    public DateTime InfoFor { get; set; }

    /// <summary>
    /// Gets or sets the report year
    /// </summary>
    [Display(Name = "Report Year")]
    public int ReportYear { get; set; }

    /// <summary>
    /// Gets or sets the report month
    /// </summary>
    [Display(Name = "Report Month")]
    public int ReportMonth { get; set; }

    /// <summary>
    /// Gets or sets the reason for difference
    /// </summary>
    [Display(Name = "Reason For Difference")]
    public virtual string DifferenceReason { get; set; }

    /// <summary>
    /// Gets or sets the report comment
    /// </summary>
    [Display(Name = "Comment")]
    public virtual string Comment { get; set; }

    public int TProductId { get; set; }
    public int TMaterialId { get; set; }
    public decimal TAmount { get; set; }
    public string? TPOther { get; set; }
    public string? TMOther { get; set; }

    public decimal RawMaterialTotal => ReportItems.Where(x => x.MaterialId != null).Sum(s => s.Value);

    public decimal FinishedProductTotal => ReportItems.Where(x => x.ProductId != null).Sum(s => s.Value);

    public decimal Variance => (FinishedProductTotal - RawMaterialTotal) / 100;

    /// <summary>
    /// Gets or sets the report items
    /// </summary>
    public ICollection<FeedsalesReportItemModel> ReportItems { get; set; } = new List<FeedsalesReportItemModel>();

    public decimal ProductsCurrentMonth => ReportItems.Where(x => x.ProductId.HasValue).Sum(s => s.Value);
    public decimal MaterialsCurrentMonth => ReportItems.Where(x => x.MaterialId.HasValue).Sum(s => s.Value);

    /// <summary>
    /// Gets or sets the report items for the previous month
    /// </summary>
    public ICollection<FeedsalesReportItemModel> ReportItemsPreviousMonth { get; set; } = new List<FeedsalesReportItemModel>();

    public decimal ProductsPreviousMonth =>
    ReportItemsPreviousMonth.Any()
    ? ReportItemsPreviousMonth.Where(x => x.ProductId.HasValue).Sum(s => s.Value)
    : 0;

    public decimal MaterialsPreviousMonth =>
    ReportItemsPreviousMonth.Any()
    ? ReportItemsPreviousMonth.Where(x => x.MaterialId.HasValue).Sum(s => s.Value)
    : 0;
  }
}

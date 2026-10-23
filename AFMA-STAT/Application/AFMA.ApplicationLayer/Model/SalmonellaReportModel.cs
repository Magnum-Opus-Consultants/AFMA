namespace AFMA.ApplicationLayer.Model
{
  using System;
  using System.ComponentModel.DataAnnotations;

  [MapTo(typeof(SalmonellaReport))]
  public class SalmonellaReportModel
  {
    public SalmonellaReportModel()
    {
      ReportItems = new List<SalmonellaSampleModel>();
    }

    /// <summary>
    /// Gets or sets the Id of the report
    /// </summary>
    [Display(Name = "Report No")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Id of the laboratory
    /// </summary>
    [Display(Name = "Laboratory")]
    [Required(ErrorMessage = "Please select a laboratory.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a laboratory.")]
    public int? LaboratoryId { get; set; }

    /// <summary>
    /// Gets or sets the laboratory
    /// </summary>
    [Display(Name = "Laboratory")]
    public string? Laboratory { get; set; }

    /// <summary>
    /// Gets or sets the member
    /// </summary>
    [Display(Name = "Member")]
    public int MemberId { get; set; }

    /// <summary>
    /// Gets or sets the member
    /// </summary>
    [Display(Name = "Member")]
    public string Member { get; set; }

    /// <summary>
    /// Gets or sets the member branch
    /// </summary>
    [Display(Name = "Branch")]
    public int MemberBranchId { get; set; }

    /// <summary>
    /// Gets or sets the member branch
    /// </summary>
    [Display(Name = "Branch")]
    public string MemberBranch { get; set; }

    /// <summary>
    /// Gets or sets the member account
    /// </summary>
    [Display(Name = "Submitted By")]
    public int? MemberAccountId { get; set; }

    /// <summary>
    /// Gets or sets the member account
    /// </summary>
    [Display(Name = "Submitted By")]
    public string? MemberAccount { get; set; }

    /// <summary>
    /// Gets or sets the submitted date
    /// </summary>
    [Display(Name = "Submitted Date")]
    public DateTime? SubmittedDate { get; set; }

    /// <summary>
    /// Gets or sets the submission status
    /// </summary>
    [Display(Name = "Submitted")]
    public bool IsSubmitted { get; set; }

    /// <summary>
    /// Gets or sets the order number
    /// </summary>
    [Display(Name = "Order Number")]
    public string? OrderNumber { get; set; }

    /// <summary>
    /// Gets or sets the sampler
    /// </summary>
    [Display(Name = "Sampled Taken By")]
    public string? Sampler { get; set; }

    /// <summary>
    /// Gets or sets the laboratory report number
    /// </summary>
    [Display(Name = "Laboratory Report No")]
    public string? LaboratoryReportNo { get; set; }

    /// <summary>
    /// Gets or sets the site code
    /// </summary>
    [Display(Name = "Site Code")]
    public string? SiteCode { get; set; }

    /// <summary>
    /// Gets or sets the due date
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
    public int? ReportYear { get; set; }

    /// <summary>
    /// Gets or sets the report month
    /// </summary>
    [Display(Name = "Report Month")]
    public int? ReportMonth { get; set; }

    /// <summary>
    /// Gets or sets a comment for this report
    /// </summary>
    [Display(Name = "Comment")]
    public string? Comment { get; set; }

    /// <summary>
    /// Gets or sets the report items
    /// </summary>
    public ICollection<SalmonellaSampleModel> ReportItems { get; set; }
  }
}

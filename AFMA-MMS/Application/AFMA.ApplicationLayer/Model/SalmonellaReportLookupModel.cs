namespace AFMA.ApplicationLayer.Model
{
  using System;

  public class SalmonellaReportLookupModel
  {
    /// <summary>
    /// Gets or sets the Id of the report
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Id of the laboratory
    /// </summary>
    public int? LaboratoryId { get; set; }

    /// <summary>
    /// Gets or sets the laboratory
    /// </summary>
    public string? Laboratory { get; set; }

    /// <summary>
    /// Gets or sets the member
    /// </summary>
    public int MemberId { get; set; }

    /// <summary>
    /// Gets or sets the member
    /// </summary>
    public string Member { get; set; }

    /// <summary>
    /// Gets or sets the member branch
    /// </summary>
    public int MemberBranchId { get; set; }

    /// <summary>
    /// Gets or sets the member branch
    /// </summary>
    public string MemberBranch { get; set; }

    /// <summary>
    /// Gets or sets the member account
    /// </summary>
    public int MemberAccountId { get; set; }

    /// <summary>
    /// Gets or sets the member account
    /// </summary>
    public string MemberAccount { get; set; }

    /// <summary>
    /// Gets or sets the member account email address
    /// </summary>
    public string MemberAccountEmail { get; set; }

    /// <summary>
    /// Gets or sets the submitted date
    /// </summary>
    public DateTime? SubmittedDate { get; set; }

    /// <summary>
    /// Gets or sets the submission status
    /// </summary>
    public bool IsSubmitted { get; set; }

    public string Status =>
      IsSubmitted
      ? "Submitted"
      : "Outstanding";

    /// <summary>
    /// Gets or sets the order number
    /// </summary>
    public string? OrderNumber { get; set; }

    /// <summary>
    /// Gets or sets the sampler
    /// </summary>
    public string? Sampler { get; set; }

    /// <summary>
    /// Gets or sets the laboratory report number
    /// </summary>
    public string? LaboratoryReportNo { get; set; }

    /// <summary>
    /// Gets or sets the site code
    /// </summary>
    public string? SiteCode { get; set; }

    /// <summary>
    /// Gets or sets the due date
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Gets or sets the info for date
    /// </summary>
    public DateTime InfoFor { get; set; }

    /// <summary>
    /// Gets or sets the report year
    /// </summary>
    public int? ReportYear { get; set; }

    /// <summary>
    /// Gets or sets the report month
    /// </summary>
    public int? ReportMonth { get; set; }

    /// <summary>
    /// Gets or sets a comment for this report
    /// </summary>
    [Display(Name = "Comment")]
    public string? Comment { get; set; }
  }
}

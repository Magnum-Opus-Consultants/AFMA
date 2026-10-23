namespace AFMA.ApplicationLayer.Model
{
  using System;

  /// <summary>
  /// The FeedsalesReportModel class
  /// </summary>
  public class FeedsalesReportLookupModel
  {

    /// <summary>
    /// Gets or sets the Id of the Report
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the submission state
    /// </summary>
    public bool IsSubmitted { get; set; }

    public string Status =>
      IsSubmitted
      ? "Submitted"
      : "Outstanding";

    /// <summary>
    /// Gets or sets the date submitted
    /// </summary>
    public DateTime? SubmittedDate { get; set; }

    /// <summary>
    /// Gets or sets the Member submitting the report
    /// </summary>
    public int MemberId { get; set; }

    /// <summary>
    /// Gets or sets the Member submitting the report
    /// </summary>
    public string Member { get; set; }

    /// <summary>
    /// Gets or sets the Member Branch
    /// </summary>
    public int MemberBranchId { get; set; }

    /// <summary>
    /// Gets or sets the Member Branch
    /// </summary>
    public string MemberBranch { get; set; }

    /// <summary>
    /// Gets or sets the Member Account
    /// </summary>
    public int MemberAccountId { get; set; }

    /// <summary>
    /// Gets or sets the Member Account
    /// </summary>
    public string MemberAccount { get; set; }

    /// <summary>
    /// Gets or sets the Member Account Email
    /// </summary>
    public string MemberAccountEmail { get; set; }

    /// <summary>
    /// Gets or sets the user who created the report
    /// </summary>
    public string CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the created date
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the due date for the report
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Gets or sets the info for date
    /// </summary>
    public DateTime InfoFor { get; set; }

    /// <summary>
    /// Gets or sets the report year
    /// </summary>
    public int ReportYear { get; set; }

    /// <summary>
    /// Gets or sets the report month
    /// </summary>
    public int ReportMonth { get; set; }
  }

  public class FeedsalesReportWithItemsLookupModel
  {

    /// <summary>
    /// Gets or sets the Id of the Report
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the submission state
    /// </summary>
    public bool IsSubmitted { get; set; }

    public string Status =>
      IsSubmitted
      ? "Submitted"
      : "Outstanding";

    /// <summary>
    /// Gets or sets the date submitted
    /// </summary>
    public DateTime? SubmittedDate { get; set; }

    /// <summary>
    /// Gets or sets the Member submitting the report
    /// </summary>
    public int MemberId { get; set; }

    /// <summary>
    /// Gets or sets the Member submitting the report
    /// </summary>
    public string Member { get; set; }

    /// <summary>
    /// Gets or sets the Member Branch
    /// </summary>
    public int MemberBranchId { get; set; }

    /// <summary>
    /// Gets or sets the Member Branch
    /// </summary>
    public string MemberBranch { get; set; }

    /// <summary>
    /// Gets or sets the Member Account
    /// </summary>
    public int MemberAccountId { get; set; }

    /// <summary>
    /// Gets or sets the Member Account
    /// </summary>
    public string MemberAccount { get; set; }

    /// <summary>
    /// Gets or sets the user who created the report
    /// </summary>
    public string CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the created date
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the due date for the report
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Gets or sets the info for date
    /// </summary>
    public DateTime InfoFor { get; set; }

    /// <summary>
    /// Gets or sets the report year
    /// </summary>
    public int ReportYear { get; set; }

    /// <summary>
    /// Gets or sets the report month
    /// </summary>
    public int ReportMonth { get; set; }

    public ICollection<FeedsalesReportItemLookupModel> ReportItems { get; set; }

    public decimal RawMaterialTotal => ReportItems.Where(x => x.MaterialId != null).Sum(s => s.Value);

    public decimal FinishedProductTotal => ReportItems.Where(x => x.ProductId != null).Sum(s => s.Value);
  }
}

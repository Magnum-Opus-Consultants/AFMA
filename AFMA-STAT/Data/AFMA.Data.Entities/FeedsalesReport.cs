namespace AFMA.Data.Entities
{
  /// <summary>
  /// The FeedsalesReport class
  /// </summary>
  public class FeedsalesReport
  {
    /// <summary>
    /// Gets or sets the Id of the Report
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the submission state
    /// </summary>
    public virtual bool IsSubmitted { get; set; }

    /// <summary>
    /// Gets or sets the date submitted
    /// </summary>
    public virtual DateTime? SubmittedDate { get; set; }

    /// <summary>
    /// Gets or sets the Member submitting the report
    /// </summary>
    public virtual Member Member { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Member Branch
    /// </summary>
    public virtual MemberBranch MemberBranch { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Member Account
    /// </summary>
    public virtual MemberAccount? MemberAccount { get; set; }

    /// <summary>
    /// Gets or sets the user who created the report
    /// </summary>
    public virtual string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the created date
    /// </summary>
    public virtual DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the due date for the report
    /// </summary>
    public virtual DateTime DueDate { get; set; }

    /// <summary>
    /// Gets or sets the info for date
    /// </summary>
    public virtual DateTime InfoFor { get; set; }

    /// <summary>
    /// Gets or sets the report year
    /// </summary>
    public virtual int ReportYear { get; set; }

    /// <summary>
    /// Gets or sets the report month
    /// </summary>
    public virtual int ReportMonth { get; set; }

    /// <summary>
    /// Gets or sets the reason for difference
    /// </summary>
    public virtual string DifferenceReason { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the report comment
    /// </summary>
    public virtual string Comment { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the report items
    /// </summary>
    public virtual ICollection<FeedsalesReportItem> ReportItems { get; set; } = new List<FeedsalesReportItem>();
  }

  public class FeedsalesReportLookup
  {
    /// <summary>
    /// Gets or sets the Id of the Report
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the submission state
    /// </summary>
    public virtual bool IsSubmitted { get; set; }

    /// <summary>
    /// Gets or sets the date submitted
    /// </summary>
    public virtual DateTime? SubmittedDate { get; set; }

    /// <summary>
    /// Gets or sets the Member submitting the report
    /// </summary>
    public virtual Member Member { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Member Branch
    /// </summary>
    public virtual MemberBranch MemberBranch { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Member Account
    /// </summary>
    public virtual MemberAccount? MemberAccount { get; set; }

    /// <summary>
    /// Gets or sets the user who created the report
    /// </summary>
    public virtual string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the created date
    /// </summary>
    public virtual DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the due date for the report
    /// </summary>
    public virtual DateTime DueDate { get; set; }

    /// <summary>
    /// Gets or sets the info for date
    /// </summary>
    public virtual DateTime InfoFor { get; set; }

    /// <summary>
    /// Gets or sets the report year
    /// </summary>
    public virtual int ReportYear { get; set; }

    /// <summary>
    /// Gets or sets the report month
    /// </summary>
    public virtual int ReportMonth { get; set; }

    /// <summary>
    /// Gets or sets the report comment
    /// </summary>
    public virtual string Comment { get; set; } = string.Empty;
  }

  public class FeedsalesReportWithItemsLookup
  {
    /// <summary>
    /// Gets or sets the Id of the Report
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the submission state
    /// </summary>
    public virtual bool IsSubmitted { get; set; }

    /// <summary>
    /// Gets or sets the date submitted
    /// </summary>
    public virtual DateTime? SubmittedDate { get; set; }

    /// <summary>
    /// Gets or sets the Member submitting the report
    /// </summary>
    public virtual Member Member { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Member Branch
    /// </summary>
    public virtual MemberBranch MemberBranch { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Member Account
    /// </summary>
    public virtual MemberAccount? MemberAccount { get; set; }

    /// <summary>
    /// Gets or sets the user who created the report
    /// </summary>
    public virtual string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the created date
    /// </summary>
    public virtual DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the due date for the report
    /// </summary>
    public virtual DateTime DueDate { get; set; }

    /// <summary>
    /// Gets or sets the info for date
    /// </summary>
    public virtual DateTime InfoFor { get; set; }

    /// <summary>
    /// Gets or sets the report year
    /// </summary>
    public virtual int ReportYear { get; set; }

    /// <summary>
    /// Gets or sets the report month
    /// </summary>
    public virtual int ReportMonth { get; set; }

    /// <summary>
    /// Gets or sets the report comment
    /// </summary>
    public virtual string Comment { get; set; } = string.Empty;

    public virtual ICollection<FeedsalesReportItemLookup> ReportItems { get; set; } = new List<FeedsalesReportItemLookup>();
  }
}

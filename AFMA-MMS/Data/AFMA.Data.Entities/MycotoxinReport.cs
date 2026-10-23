namespace AFMA.Data.Entities
{
  /// <summary>
  /// The MycotoxinReport class.
  /// </summary>
  public class MycotoxinReport
  {
    /// <summary>
    /// Gets or sets the Id of the report
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the Member submitting the report.
    /// </summary>
    public virtual Member Member { get; set; }

    /// <summary>
    /// Gets or sets the Member Branch.
    /// </summary>
    public virtual MemberBranch MemberBranch { get; set; }

    /// <summary>
    /// Gets or sets the Member Account
    /// </summary>
    public virtual MemberAccount? MemberAccount { get; set; }

    /// <summary>
    /// Gets or sets the info for date.
    /// </summary>
    public virtual DateTime InfoFor { get; set; }

    /// <summary>
    /// Gets or sets the due date.
    /// </summary>
    public virtual DateTime DueDate { get; set; }

    /// <summary>
    /// Gets or sets the laboratory.
    /// </summary>
    public virtual Laboratory Laboratory { get; set; }

    /// <summary>
    /// Gets or sets the laboratory report number.
    /// </summary>
    public virtual string? LaboratoryReportNo { get; set; }

    /// <summary>
    /// Gets or sets the report year
    /// </summary>
    public virtual int ReportYear { get; set; }

    /// <summary>
    /// Gets or sets the report month
    /// </summary>
    public virtual int ReportMonth { get; set; }

    /// <summary>
    /// Gets or sets the submission state
    /// </summary>
    public virtual bool IsSubmitted { get; set; }

    /// <summary>
    /// Gets or sets the submitted date.
    /// </summary>
    public virtual DateTime? SubmittedDate { get; set; }

    /// <summary>
    /// Gets or sets the submission comment.
    /// </summary>
    public virtual string? Comment { get; set; }

    /// <summary>
    /// Gets or sets the report samples.
    /// </summary>
    public virtual ICollection<MycotoxinSample> Samples { get; set; }
  }

  public class MycotoxinReportLookup
  {
    /// <summary>
    /// Gets or sets the Id of the report
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the Member submitting the report
    /// </summary>
    public virtual Member Member { get; set; }

    /// <summary>
    /// Gets or sets the Member Branch
    /// </summary>
    public virtual MemberBranch MemberBranch { get; set; }

    /// <summary>
    /// Gets or sets the Member Account
    /// </summary>
    public virtual MemberAccount MemberAccount { get; set; }

    /// <summary>
    /// Gets or sets the info for date.
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
    /// Gets or sets the submission state
    /// </summary>
    public virtual bool IsSubmitted { get; set; }

    /// <summary>
    /// Gets or sets the submission comment.
    /// </summary>
    public virtual string? Comment { get; set; }
  }
}

namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The MycotoxinReportModel class.
  /// </summary>
  [MapTo(typeof(MycotoxinReport))]
  public class MycotoxinReportModel
  {
    /// <summary>
    /// Gets or sets the Id of the report
    /// </summary>
    [Display(Name = "Report No")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Id of the member.
    /// </summary>
    [Display(Name = "Member")]
    public int MemberId { get; set; }

    /// <summary>
    /// Gets or sets the name of the member.
    /// </summary>
    [Display(Name = "Member")]
    public string Member { get; set; }

    /// <summary>
    /// Gets or sets the Id of the member branch.
    /// </summary>
    [Display(Name = "Member Branch")]
    public int MemberBranchId { get; set; }

    /// <summary>
    /// Gets or sets the name of the member branch.
    /// </summary>
    [Display(Name = "Member Branch")]
    public string MemberBranch { get; set; }

    /// <summary>
    /// Gets or sets the Id of the member account.
    /// </summary>
    [Display(Name = "Member Account")]
    public int? MemberAccountId { get; set; }

    /// <summary>
    /// Gets or sets the name of the member account.
    /// </summary>
    [Display(Name = "Member Account")]
    public string? MemberAccount { get; set; }

    /// <summary>
    /// Gets or sets the info for date.
    /// </summary>
    [Display(Name = "Info For")]
    public DateTime InfoFor { get; set; }

    /// <summary>
    /// Gets or sets the due date.
    /// </summary>
    [Display(Name = "Due Date")]
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Gets or sets the Id of the laboratory.
    /// </summary>
    [Display(Name = "Laboratory")]
    public int? LaboratoryId { get; set; }

    /// <summary>
    /// Gets or sets the name of the laboratory.
    /// </summary>
    [Display(Name = "Laboratory")]
    public string? Laboratory { get; set; }

    /// <summary>
    /// Gets or sets the laboratory report number.
    /// </summary>
    [Display(Name = "Laboratory Report No")]
    public string? LaboratoryReportNo { get; set; }

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
    /// Gets or sets the submission state
    /// </summary>
    [Display(Name = "Submitted")]
    public bool IsSubmitted { get; set; }

    /// <summary>
    /// Gets or sets the submitted date.
    /// </summary>
    [Display(Name = "Submitted Date")]
    public DateTime? SubmittedDate { get; set; }

    /// <summary>
    /// Gets or sets the submission comment.
    /// </summary>
    [Display(Name = "Comment")]
    public string? Comment { get; set; }

    /// <summary>
    /// Gets or sets the report samples.
    /// </summary>
    public ICollection<MycotoxinSampleModel> Samples { get; set; }
  }
}

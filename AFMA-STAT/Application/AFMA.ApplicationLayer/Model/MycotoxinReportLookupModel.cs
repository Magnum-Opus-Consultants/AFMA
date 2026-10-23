namespace AFMA.ApplicationLayer.Model
{
  using System;

  /// <summary>
  /// The MycotoxinReportLookupModel class.
  /// </summary>
  [MapTo(typeof(MycotoxinReportLookup))]

  public class MycotoxinReportLookupModel
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
    public int MemberAccountId { get; set; }

    /// <summary>
    /// Gets or sets the name of the member account.
    /// </summary>
    [Display(Name = "Member Account")]
    public string MemberAccount { get; set; }

    /// <summary>
    /// Gets or sets the info for date.
    /// </summary>
    [Display(Name = "Info For")]
    public DateTime InfoFor { get; set; }

    /// <summary>
    /// Gets or sets the submission state
    /// </summary>
    [Display(Name = "Submitted")]
    public bool IsSubmitted { get; set; }

    /// <summary>
    /// Gets or sets the submission comment.
    /// </summary>
    [Display(Name = "Comment")]
    public string? Comment { get; set; }
  }
}

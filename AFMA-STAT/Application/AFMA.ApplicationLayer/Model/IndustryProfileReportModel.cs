namespace AFMA.ApplicationLayer.Model
{
  using Alpha.Services.NHibernate.Repository.Attributes;
  using Data.Entities;

  [MapTo(typeof(IndustryProfileReport))]
  public class IndustryProfileReportModel
  {
    /// <summary>
    /// Gets or sets the Id of the report.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Id of the member.
    /// </summary>
    public int MemberId { get; set; }

    /// <summary>
    /// Gets or sets the member name.
    /// </summary>
    public string? Member { get; set; }

    /// <summary>
    /// Gets or sets the Id of the member branch.
    /// </summary>
    public int MemberBranchId { get; set; }

    /// <summary>
    /// Gets or sets the member branch name.
    /// </summary>
    public string? MemberBranch { get; set; }

    /// <summary>
    /// Gets or sets the Id of the member account.
    /// </summary>
    public int MemberAccountId { get; set; }

    /// <summary>
    /// Gets or sets the member account name.
    /// </summary>
    public string? MemberAccount { get; set; }

    /// <summary>
    /// Gets or sets the report year.
    /// </summary>
    public int ReportYear { get; set; }

    /// <summary>
    /// Gets or sets the submitted status.
    /// </summary>
    public bool Submitted { get; set; }

    /// <summary>
    /// Gets or sets the submitted by.
    /// </summary>
    public string? SubmittedBy { get; set; }

    /// <summary>
    /// Gets or sets the annual production average.
    /// </summary>
    public long? AnnualProductionAverage { get; set; }

    /// <summary>
    /// Gets or sets the Id of the member type.
    /// </summary>
    public int MemberTypeId { get; set; }

    /// <summary>
    /// Gets or sets the member type description.
    /// </summary>
    public string? MemberType { get; set; }

    /// <summary>
    /// Gets or sets the created by.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the active indicator.
    /// </summary>
    public bool Active { get; set; }
  }
}

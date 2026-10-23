namespace AFMA.Data.Entities
{
  /// <summary>
  /// The IndustryProfileReport class.
  /// </summary>
  public class IndustryProfileReport
  {
    /// <summary>
    /// Gets or sets the Id of the report.
    /// </summary>
    public virtual int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the member.
    /// </summary>
    public virtual Member? Member { get; set; }
    
    /// <summary>
    /// Gets or sets the member branch.
    /// </summary>
    public virtual MemberBranch? MemberBranch { get; set; }
    
    /// <summary>
    /// Gets or sets the member account.
    /// </summary>
    public virtual MemberAccount? MemberAccount { get; set; }
    
    /// <summary>
    /// Gets or sets the report year.
    /// </summary>
    public virtual int ReportYear { get; set; }
    
    /// <summary>
    /// Gets or sets the submitted status.
    /// </summary>
    public virtual bool Submitted { get; set; }
    
    /// <summary>
    /// Gets or sets the submitted by.
    /// </summary>
    public virtual string? SubmittedBy { get; set; }
    
    /// <summary>
    /// Gets or sets the annual production average.
    /// </summary>
    public virtual long? AnnualProductionAverage { get; set; }
    
    /// <summary>
    /// Gets or sets the member type.
    /// </summary>
    public virtual MemberType? MemberType { get; set; }
    
    /// <summary>
    /// Gets or sets the member type description.
    /// </summary>
    public virtual string? MemberTypeDescription { get; set; }
    
    /// <summary>
    /// Gets or sets the created by.
    /// </summary>
    public virtual string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the active indicator.
    /// </summary>
    public virtual bool Active { get; set; }
  }
}
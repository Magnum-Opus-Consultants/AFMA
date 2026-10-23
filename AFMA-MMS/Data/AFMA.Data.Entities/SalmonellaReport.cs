namespace AFMA.Data.Entities
{
  /// <summary>
  /// The SalmonellaReport class
  /// </summary>
  public class SalmonellaReport
  {
    /// <summary>
    /// Gets or sets the Id of the report
    /// </summary>
    public virtual int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the laboratory
    /// </summary>
    public virtual Laboratory? Laboratory { get; set; }
    
    /// <summary>
    /// Gets or sets the member
    /// </summary>
    public virtual Member Member { get; set; }
    
    /// <summary>
    /// Gets or sets the member branch
    /// </summary>
    public virtual MemberBranch MemberBranch { get; set; }
    
    /// <summary>
    /// Gets or sets the member account
    /// </summary>
    public virtual MemberAccount? MemberAccount { get; set; }
    
    /// <summary>
    /// Gets or sets the submitted date
    /// </summary>
    public virtual DateTime? SubmittedDate { get; set; }
    
    /// <summary>
    /// Gets or sets the submission status
    /// </summary>
    public virtual bool IsSubmitted { get; set; }
    
    /// <summary>
    /// Gets or sets the order number
    /// </summary>
    public virtual string? OrderNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the sampler
    /// </summary>
    public virtual string? Sampler { get; set; }
    
    /// <summary>
    /// Gets or sets the laboratory report number
    /// </summary>
    public virtual string? LaboratoryReportNo { get; set; }
    
    /// <summary>
    /// Gets or sets the site code
    /// </summary>
    public virtual string? SiteCode { get; set; }
    
    /// <summary>
    /// Gets or sets the due date
    /// </summary>
    public virtual DateTime DueDate { get; set; }
    
    /// <summary>
    /// Gets or sets the info for date
    /// </summary>
    public virtual DateTime InfoFor { get; set; }
    
    /// <summary>
    /// Gets or sets the report year
    /// </summary>
    public virtual int? ReportYear { get; set; }
    
    /// <summary>
    /// Gets or sets the report month
    /// </summary>
    public virtual int? ReportMonth { get; set; }

    /// <summary>
    /// Gets or sets a comment for this report
    /// </summary>
    public virtual string? Comment { get; set; }

    /// <summary>
    /// Gets or sets the report items
    /// </summary>
    public virtual ICollection<SalmonellaSample> ReportItems { get; set; }
  }

  /// <summary>
  /// The SalmonellaReportLookup class
  /// </summary>
  public class SalmonellaReportLookup
  {
    /// <summary>
    /// Gets or sets the Id of the report
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the laboratory
    /// </summary>
    public virtual Laboratory? Laboratory { get; set; }

    /// <summary>
    /// Gets or sets the member
    /// </summary>
    public virtual Member Member { get; set; }

    /// <summary>
    /// Gets or sets the member branch
    /// </summary>
    public virtual MemberBranch MemberBranch { get; set; }

    /// <summary>
    /// Gets or sets the member account
    /// </summary>
    public virtual MemberAccount? MemberAccount { get; set; }

    /// <summary>
    /// Gets or sets the submitted date
    /// </summary>
    public virtual DateTime? SubmittedDate { get; set; }

    /// <summary>
    /// Gets or sets the submission status
    /// </summary>
    public virtual bool IsSubmitted { get; set; }

    /// <summary>
    /// Gets or sets the order number
    /// </summary>
    public virtual string? OrderNumber { get; set; }

    /// <summary>
    /// Gets or sets the sampler
    /// </summary>
    public virtual string? Sampler { get; set; }

    /// <summary>
    /// Gets or sets the laboratory report number
    /// </summary>
    public virtual string? LaboratoryReportNo { get; set; }

    /// <summary>
    /// Gets or sets the site code
    /// </summary>
    public virtual string? SiteCode { get; set; }

    /// <summary>
    /// Gets or sets the due date
    /// </summary>
    public virtual DateTime DueDate { get; set; }

    /// <summary>
    /// Gets or sets the info for date
    /// </summary>
    public virtual DateTime InfoFor { get; set; }

    /// <summary>
    /// Gets or sets the report year
    /// </summary>
    public virtual int? ReportYear { get; set; }

    /// <summary>
    /// Gets or sets the report month
    /// </summary>
    public virtual int? ReportMonth { get; set; }

    /// <summary>
    /// Gets or sets a comment for this report
    /// </summary>
    public virtual string? Comment { get; set; }
  }
}

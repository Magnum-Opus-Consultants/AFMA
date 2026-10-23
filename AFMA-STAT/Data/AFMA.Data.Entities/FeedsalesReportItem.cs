namespace AFMA.Data.Entities
{
  /// <summary>
  /// The FeedsalesReportItem class
  /// </summary>
  public class FeedsalesReportItem
  {
    /// <summary>
    /// Gets or sets the Id of the report item
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the feedsales report
    /// </summary>
    public virtual FeedsalesReport FeedsalesReport { get; set; }

    /// <summary>
    /// Gets or sets the finished product
    /// </summary>
    public virtual Product? Product { get; set; }

    /// <summary>
    /// Gets or sets the raw material
    /// </summary>
    public virtual Material? Material { get; set; }

    /// <summary>
    /// Gets or sets the submitted value
    /// </summary>
    public virtual decimal Value { get; set; }

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
    /// Gets or sets the other description
    /// </summary>
    public virtual string? Other { get; set; }
  }

  public class FeedsalesReportItemLookup
  {
    /// <summary>
    /// Gets or sets the Id of the report item
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the feedsales report
    /// </summary>
    public virtual FeedsalesReportLookup FeedsalesReport { get; set; }

    /// <summary>
    /// Gets or sets the finished product
    /// </summary>
    public virtual Product? Product { get; set; }

    /// <summary>
    /// Gets or sets the raw material
    /// </summary>
    public virtual Material? Material { get; set; }

    /// <summary>
    /// Gets or sets the submitted value
    /// </summary>
    public virtual decimal Value { get; set; }

    /// <summary>
    /// Gets or sets the other description
    /// </summary>
    public virtual string? Other { get; set; }

    ///// <summary>
    ///// Gets or sets the report year
    ///// </summary>
    //public virtual int ReportYear { get; set; }

    ///// <summary>
    ///// Gets or sets the report month
    ///// </summary>
    //public virtual int ReportMonth { get; set; }

    ///// <summary>
    ///// Gets or sets the submitted
    ///// </summary>
    //public virtual bool IsSubmitted { get; set; }
  }
}
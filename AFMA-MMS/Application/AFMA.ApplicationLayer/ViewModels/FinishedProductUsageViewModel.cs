namespace AFMA.ApplicationLayer.ViewModels
{
  /// <summary>
  /// The RawMaterialUsageByMemberByMonthViewModel class.
  /// </summary>
  public class FinishedProductUsageModel
  {
    /// <summary>
    /// Gets or sets the Feedsales Report Item Id
    /// </summary>
    public int FeedsalesReportItemId { get; set; }

    /// <summary>
    /// Gets or sets the Feedsales Report Id
    /// </summary>
    public int FeedsalesReportId { get; set; }

    /// <summary>
    /// Gets or sets the Member Id
    /// </summary>
    public int MemberId { get; set; }

    /// <summary>
    /// Gets or sets the Member Branch Id
    /// </summary>
    public int MemberBranchId { get; set; }

    /// <summary>
    /// Gets or sets the product Id
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product group Id
    /// </summary>
    public int ProductGroupId { get; set; }

    /// <summary>
    /// Gets or sets the product name
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the submitted calue
    /// </summary>
    public decimal SubmittedValue { get; set; }

    /// <summary>
    /// Gets or sets the report date
    /// </summary>
    public DateTime InfoFor { get; set; }

    /// <summary>
    /// Gets the report year
    /// </summary>
    public int Year => InfoFor.Year;

    /// <summary>
    /// Gets the report month
    /// </summary>
    public int Month => InfoFor.Month;
  }
}

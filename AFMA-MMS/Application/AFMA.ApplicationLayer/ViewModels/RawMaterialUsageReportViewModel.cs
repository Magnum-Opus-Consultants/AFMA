namespace AFMA.ApplicationLayer.ViewModels
{
  using System;

  /// <summary>
  /// The Raw Material Usage Report View Model
  /// </summary>
  public class RawMaterialUsageReportViewModel
  {
    /// <summary>
    /// Gets or sets the Feedsales Report Id
    /// </summary>
    public int ReportId { get; set; }
    
    /// <summary>
    /// Gets or sets the Member ID
    /// </summary>
    public int MemberId { get; set; }
    
    /// <summary>
    /// Gets or sets the Member Name
    /// </summary>
    public string? MemberName { get; set; }
    
    /// <summary>
    /// Gets or sets the Member Branch ID
    /// </summary>
    public int MemberBranchId { get; set; }
    
    /// <summary>
    /// Gets or sets the Member Branch Name
    /// </summary>
    public string? MemberBranchName { get; set;}
    
    /// <summary>
    /// Gets or sets the Info For date
    /// </summary>
    public DateTime InfoFor { get; set; }
    
    /// <summary>
    /// Gets the Info For date in specific format
    /// </summary>
    public string InfoForText => string.Format(this.InfoFor.ToString("MMMM yyyy"));

    /// <summary>
    /// Gets or sets the Raw Material ID
    /// </summary>
    public int MaterialId { get; set; }

    /// <summary>
    /// Gets or sets the Raw Material Name
    /// </summary>
    public string? MaterialName { get; set; }
    
    /// <summary>
    /// Gets or sets the submitted value
    /// </summary>
    public decimal Value { get; set; }
  }
}

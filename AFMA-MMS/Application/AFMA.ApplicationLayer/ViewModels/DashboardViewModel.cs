namespace AFMA.ApplicationLayer.ViewModels
{
  using Helpers;

  /// <summary>
  /// The DashboardViewModel class.
  /// </summary>
  public class DashboardViewModel
  {
    /// <summary>
    /// Gets or sets the name of the dashboard
    /// </summary>
    public string DashboardName { get; set; }

    /// <summary>
    /// Gets or sets the code of the dashboard
    /// </summary>
    public string DashboardCode { get; set; }

    /// <summary>
    /// Gets or sets the display order of the dashboard
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the module the dashboard is for
    /// </summary>
    public ModuleEnum Module { get; set; }
    public int ReportCount { get; set; }
    public int Submitted { get; set; }
    public int NotSubmitted { get; set; }
    public int SamplesPositive { get; set; }
    public int TotalProducts { get; set; }
    public int TotalMaterials { get; set; }
  }
}

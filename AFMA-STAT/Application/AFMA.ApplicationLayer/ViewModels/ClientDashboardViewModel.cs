namespace AFMA.ApplicationLayer.ViewModels
{
  using AFMA.ApplicationLayer.Helpers;
  using System;
  using System.Collections.Generic;

  public class ClientDashboardViewModel
  {
    public ClientDashboardViewModel()
    {
      ReportSummary = new List<ClientDashboardItemViewModel>();
    }

    /// <summary>
    /// Gets or sets the member name
    /// </summary>
    public string? Member { get; set; }
    
    /// <summary>
    /// Gets or sets the member branch
    /// </summary>
    public string? MemberBranch { get; set; }
    
    /// <summary>
    /// Gets or sets the member account
    /// </summary>
    public string? MemberAccount { get; set; }
    
    /// <summary>
    /// Gets or sets the member account email address
    /// </summary>
    public string? MemberAccountEmail { get; set; }
    
    /// <summary>
    /// Gets or sets the member account username
    /// </summary>
    public string? MemberUAccountUserName { get; set; }

    /// <summary>
    /// Gets or sets the last login date
    /// </summary>
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// Gets or sets the report summary per module
    /// </summary>
    public ICollection<ClientDashboardItemViewModel> ReportSummary { get; set; }
  }

  public class ClientDashboardItemViewModel
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
    /// Gets or sets the dashboard image
    /// </summary>
    public string DashboardImage { get; set; }

    /// <summary>
    /// Gets or sets the display order of the dashboard
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the module the dashboard is for
    /// </summary>
    public ModuleEnum Module { get; set; }
    
    /// <summary>
    /// Gets or sets the total report count
    /// </summary>
    public int ReportCount { get; set; }
    
    /// <summary>
    /// Gets or sets the total submitted report count
    /// </summary>
    public int Submitted { get; set; }
    
    /// <summary>
    /// Gets or sets the total unsubmitted report count
    /// </summary>
    public int NotSubmitted { get; set; }

    /// <summary>
    /// Gets or sets the total due reports
    /// </summary>
    public int CurrentDue { get; set; }

    /// <summary>
    /// Gets or sets the total overdue reports
    /// </summary>
    public int Overdue { get; set; }
  }
}

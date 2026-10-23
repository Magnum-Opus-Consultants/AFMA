namespace AFMA.ApplicationLayer.ViewModels
{
  using System;
  using System.ComponentModel.DataAnnotations;

  public class SubmissionStatsViewModel
  {
    [Display(Name = "Report No")]
    public int? ReportId { get; set; }

    [Display(Name = "Member")]
    public string MemberName { get; set; }

    [Display(Name = "Branch Name")]
    public string BranchName { get; set; }

    [Display(Name = "Branch Code")]
    public string BranchCode { get; set; }

    [Display(Name = "Member Email")]
    public string MemberAccountEmail { get; set; }

    [Display(Name = "Info For")]
    public string InfoFor { get; set; }

    [Display(Name = "Due Date")]
    public DateTime? DueDate { get; set; }

    [Display(Name = "Status")]
    public string Status { get; set; }

    [Display(Name = "Submitted")]
    public bool Submitted { get; set; }

    [Display(Name = "Date Submitted")]
    public DateTime? DateSubmitted { get; set; }

    public DateTime Period { get; set; } = new DateTime(2022, 1, 1);

    [Display(Name = "Comment")]
    public string? Comment { get; set; }
  }
}

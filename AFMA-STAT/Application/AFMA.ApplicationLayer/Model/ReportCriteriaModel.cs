namespace AFMA.ApplicationLayer.Model
{
  using System;
  using System.ComponentModel.DataAnnotations;

  public class ReportCriteriaModel
  {
    [Display(Name = "Start Date")]
    public DateTime StartDate { get; set; }

    [Display(Name = "End Date")]
    public DateTime EndDate { get; set; }

    [Display(Name = "Member")]
    public int? MemberId { get; set; }

    [Display(Name = "Member Branch")]
    public int? MemberBranchId { get; set; }

    [Display(Name = "Sample Type")]
    public int SampleTypeId { get; set; }

    [Display(Name = "Finished Product")]
    public int? ProductId { get; set; }

    [Display(Name = "Finished Product Group")]
    public int? ProductGroupId { get; set; }

    [Display(Name = "Raw Material")]
    public int? MaterialId { get; set; }

    [Display(Name = "Member Type")]
    public int? MemberTypeId { get; set; }

    [Display(Name = "Start Year")]
    public int StartYear { get; set; }

    [Display(Name = "End Year")]
    public int EndYear { get; set; }
  }
}

namespace AFMA.ApplicationLayer.ViewModels.Wims
{
  using AFMA.ApplicationLayer.Model.Wims;
  using System.Collections.Generic;

  public class MemberUserBranchViewModel
  {
    public int MemberUserId { get; set; }

    public int MemberId { get; set; }

    public ICollection<MemberUserBranchModel> MemberBranches { get; set; } = new List<MemberUserBranchModel>();
  }
}

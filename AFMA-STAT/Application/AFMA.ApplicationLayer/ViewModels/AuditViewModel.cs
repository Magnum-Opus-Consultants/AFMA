namespace AFMA.ApplicationLayer.ViewModels
{
  using Helpers;

  public class MemberModuleAccessViewModel
  {
    public MemberModuleAccessViewModel()
    {
      MemberModules = new List<MemberModuleViewModel>();
    }

    public int MemberId { get; set; }
    public string Member { get; set; }
    public string MemberType { get; set; }
    public ICollection<MemberModuleViewModel> MemberModules { get; set; }
  }

  public class MemberModuleViewModel
  {
    public ModuleEnum ModuleId { get; set; }
    public string Module { get; set; }
    public bool HasAccess { get; set; }
  }

  public sealed class AccountModuleAccessViewModel : MemberModuleAccessViewModel
  {
    public int MemberBranchId { get; set; }
    public string MemberBranch { get; set; }
    public int MemberAccountId { get; set; }
    public string MemberAccountFullName { get; set; }
    public string MemberAccountEmail { get; set; }
    public string UserName { get; set; }
  }

  public sealed class MemberAccountAuditViewModel
  {
    public int MemberAccountId { get; set; }
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public int MemberBranchId { get; set; }
    public string MemberBranchName { get; set; } = string.Empty;
    public int MemberAccountBranchId { get; set; }
    public string MemberAccountBranchName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string AccountEmailAddress { get; set; } = string.Empty;
    public bool MemberAccountValid { get; set; }
    public string ValidationMessage { get; set; } = string.Empty;
    public bool MemberAccountActive { get; set; }
  }
}

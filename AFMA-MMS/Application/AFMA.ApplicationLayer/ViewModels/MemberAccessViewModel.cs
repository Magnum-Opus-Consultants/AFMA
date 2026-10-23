namespace AFMA.ApplicationLayer.ViewModels
{
  public class MemberAccessViewModel
  {
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName => $"{FirstName} {LastName}";
    public int MemberId { get; set; }
    public string? Member { get; set; }
    public int MemberBranchId { get; set; }
    public string? MemberBranch { get; set; }
    public int MemberAccountId { get; set; }
    public bool HasAccess { get; set; } = false;
    public bool HasFeedsalesAccess { get; set; }
    public bool HasSalmonellaAccess { get; set; }
    public bool HasIndustryProfileAccess { get; set; }
    public bool HasMycotoxinAccess { get; set; }
    public int? MemberTypeId { get; set; }
    public DateTime? LastLogin { get; set; }
  }
}

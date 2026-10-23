namespace AFMA.ApplicationLayer.Model
{
  using System.ComponentModel.DataAnnotations;

  /// <summary>
  /// The MemberAccountModel class.
  /// </summary>
  [MapTo(typeof(MemberAccount))]
  public class MemberAccountModel
  {
    public MemberAccountModel()
    {
      Modules = new List<MemberAccountModuleModel>();
    }

    /// <summary>
    /// Gets or sets the Id of the member account.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Id of the member.
    /// </summary>
    [Display(Name = "Member")]
    public int MemberId { get; set; }

    /// <summary>
    /// Gets or sets the name of the member.
    /// </summary>
    [Display(Name = "Member")]
    public string? Member { get; set; }

    /// <summary>
    /// Gets or sets the Id of the member branch.
    /// </summary>
    [Display(Name = "Branch")]
    public int MemberBranchId { get; set; }

    /// <summary>
    /// Gets or sets the name of the member branch.
    /// </summary>
    [Display(Name = "Branch")]
    public string? MemberBranch { get; set; }

    /// <summary>
    /// Gets or sets the email address for the member account.
    /// </summary>
    [Display(Name = "Email Address")]
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the Id of the user.
    /// </summary>
    [Display(Name = "User Id")]
    public string UserId { get; set; }

    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    [Display(Name = "Username")]
    public string? UserName { get; set; }

    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    [Display(Name = "Password")]
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    [Display(Name = "First Name")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the full name of the user.
    /// </summary>
    [Display(Name = "Full Name")]
    public string? FullName { get; set; }

    /// <summary>
    /// Gets or sets the registered date.
    /// </summary>
    [Display(Name = "Date Registered")]
    public DateTime Registered { get; set; }

    /// <summary>
    /// Gets or sets the last login date.
    /// </summary>
    [Display(Name = "Last Login")]
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// Gets or sets the status of this member account.
    /// </summary>
    [Display(Name = "Active")]
    public bool Active { get; set; }

    /// <summary>
    /// Gets or sets the administrator status of this member account.
    /// </summary>
    [Display(Name = "Is Administrator")]
    public bool IsAdministrator { get; set; }

    /// <summary>
    /// Gets or sets the salmonella access of this member account.
    /// </summary>
    [Display(Name = "Salmonella Access")]
    public bool SalmonellaAccess { get; set; }

    /// <summary>
    /// Gets or sets the feedsales access of this member account.
    /// </summary>
    [Display(Name = "Feedsales Access")]
    public bool FeedsalesAccess { get; set; }

    /// <summary>
    /// Gets or sets the industry profile access of this member account.
    /// </summary>
    [Display(Name = "Industry Profile Access")]
    public bool IndustryProfileAccess { get; set; }

    /// <summary>
    /// Gets or sets the mycotoxins access of this member account.
    /// </summary>
    [Display(Name = "Mycotoxins Access")]
    public bool MycotoxinsAccess { get; set; }

    public ICollection<MemberAccountModuleModel> Modules { get; set; }
  }
}

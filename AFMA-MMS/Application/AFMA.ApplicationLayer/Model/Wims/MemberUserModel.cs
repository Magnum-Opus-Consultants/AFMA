namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.ApplicationLayer.ViewModels.Wims;
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The MemberUserModel class represents a member user in the WIMS application.
  /// </summary>
  [MapTo(typeof(MemberUser))]
  public class MemberUserModel
  {
    /// <summary>
    /// Gets or sets the unique identifier for the MemberUser.
    /// </summary>  
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Member Id foreign key.
    /// </summary>
    [Display(Name = "Member Id")]
    public int MemberId { get; set; }

    /// <summary>
    /// Gets or sets the name of the member associated with this member user.
    /// </summary>
    [Display(Name = "Member Name")]
    public string Member { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the User Id foreign key.
    /// </summary>
    [Display(Name = "User Id")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the user associated with this member.
    /// </summary>
    [Display(Name = "First Name")]
    [Required(ErrorMessage = "First Name is required")]
    [StringLength(255)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name of the user associated with this member.
    /// </summary>
    [Display(Name = "Last Name")]
    [Required(ErrorMessage = "Last Name is required")]
    [StringLength(255)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password of the user associated with this member.
    /// </summary>
    [Display(Name = "Password")]
    [Required(ErrorMessage = "Password is required")]
    [RegularExpression(@"(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}", ErrorMessage = "Password should be minimum eight characters, at least one letter, one number and one special character")]
    [StringLength(255)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the user can edit this member's information.
    /// </summary>
    [Display(Name = "Can Edit Member Info")]
    public bool CanEdit { get; set; }

    /// <summary>
    /// Gets or sets the email of the user associated with this member.
    /// </summary>
    [Display(Name = "Email/Username")]
    [Required(ErrorMessage = "Email Address is required")]
    [RegularExpression("[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}", ErrorMessage = "Please enter a valid email address")]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last login date and time of the user associated with this member.
    /// </summary>
    [Display(Name = "Last Login")]
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user associated with this member is active.
    /// </summary>
    [Display(Name = "Status")]
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets the full name of the user associated with this member by combining the first and last names.
    /// </summary>
    [Display(Name = "Full Name")]
    public string? FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Gets or sets a collection of MemberUserBranchModel instances representing the branches associated with this member user.
    /// </summary>
    public ICollection<MemberUserBranchModel>? Branches { get; set; } = new List<MemberUserBranchModel>();

    public MemberUserBranchViewModel MemberBranches { get; set; } = new MemberUserBranchViewModel();

  }
}

namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// Rhe MemberBranchContactModel class represents a contact person for a member branch in the WIMS application.
  /// </summary>
  [MapTo(typeof(MemberBranchContact))]
  public class MemberBranchContactModel
  {
    /// <summary>
    /// The unique identifier for the Province.
    /// </summary>
    [Display(Name = "Id")]
    public int Id { get; set; }

    /// <summary>
    /// gets or sets the Id of the member.
    /// </summary>
    [Display(Name = "Member")]
    public int MemberId { get; set; }

    /// <summary>
    /// gets or sets the name of the member.
    /// </summary>
    [Display(Name = "Member")]
    public string Member { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Id of the member branch.
    /// </summary>
    [Display(Name = "Member Branch")]
    public int MemberBranchId { get; set; }

    /// <summary>
    /// Gets or sets the Id of the member branch.
    /// </summary>
    [Display(Name = "Member Branch")]
    public string MemberBranch { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the member number contact type.
    /// </summary>
    [Display(Name = "Member Contact Type")]
    public int MemberContactTypeId { get; set; }

    /// <summary>
    /// Gets or sets the member number contact type.
    /// </summary>
    [Display(Name = "Member Contact Type")]
    public string MemberContactType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title of the contact person.
    /// </summary>
    [Display(Name = "Title")]
    [Required(ErrorMessage = "Title is required")]
    [StringLength(50)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the first name of the contact person.
    /// </summary>
    [Display(Name = "First Name")]
    [Required(ErrorMessage = "First Name is required")]
    [StringLength(150)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the surname of the contact person.
    /// </summary>
    [Display(Name = "Surname")]
    [Required(ErrorMessage = "Surname is required")]
    [StringLength(150)]
    public string Surname { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the telephone number of the contact person.
    /// </summary>
    [Display(Name = "Telephone No")]
    [Required(ErrorMessage = "Telephone No is required")]
    [StringLength(50)]
    public string TelephoneNo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the mobile number of the contact person.
    /// </summary>
    [Display(Name = "Mobile No")]
    [StringLength(50)]
    public string MobileNo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address of the contact person.
    /// </summary>
    [Display(Name = "Email Address")]
    [RegularExpression("[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}", ErrorMessage = "Please enter a valid email address")]
    [Required(ErrorMessage = "Email address is required")]
    [StringLength(150)]
    public string EmailAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company-wide indicator
    /// </summary>
    public bool CompanyContact { get; set; }
  }
}

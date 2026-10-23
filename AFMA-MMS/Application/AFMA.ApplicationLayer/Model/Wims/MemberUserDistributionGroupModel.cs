namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The MemberUserDistributionGroupModel class represents the association between a member user and a distribution group in the WIMS application.
  /// </summary>
  [MapTo(typeof(MemberUserDistributionGroup))]
  public class MemberUserDistributionGroupModel
  {
    /// <summary>
    /// Gets or sets the unique identifier for the MemberUserDistributionGroup.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Member Distribution Group foreign key.
    /// </summary>
    [Display(Name = "Distribution Group")]
    [Required(ErrorMessage = "Distribution Group is required")]
    public int DistributionGroupId { get; set; }

    /// <summary>
    /// Gets or sets the Member Distribution Group foreign key.
    /// </summary>
    [Display(Name = "Distribution Group")]
    public string DistributionGroup { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Member User foreign key.
    /// </summary>
    [Display(Name = "Member User")]
    [Required(ErrorMessage = "Member User is required")]
    public int MemberUserId { get; set; }

    /// <summary>
    /// Gets or sets the Member User foreign key.
    /// </summary>
    [Display(Name = "Member User")]
    public string MemberUser { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the first name of the user associated with this member distribution group.
    /// </summary>
    [Display(Name = "First Name")]
    [Required(ErrorMessage = "First Name is required")]
    [StringLength(255)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name of the user associated with this member distribution group.
    /// </summary>
    [Display(Name = "Last Name")]
    [Required(ErrorMessage = "Last Name is required")]
    [StringLength(255)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address of the user associated with this member distribution group.
    /// </summary>
    [Display(Name = "Email Address")]
    [Required(ErrorMessage = "Email Address is required")]
    [RegularExpression("[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}", ErrorMessage = "Please enter a valid email address")]
    [StringLength(255)]
    public string EmailAddress { get; set; } = string.Empty;
  }
}

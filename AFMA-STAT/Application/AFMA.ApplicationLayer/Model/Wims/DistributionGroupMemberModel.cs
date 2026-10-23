namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The DistributionGroupMember class represents a member of a distribution group.
  /// </summary>
  [MapTo(typeof(DistributionGroupMember))]
  public class DistributionGroupMemberModel
  {
    /// <summary>
    /// Gets or sets the unique identifier for the distribution group member.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the distribution group to which this member belongs.
    /// </summary>
    [Display(Name = "Distribution Group")]
    [Required(ErrorMessage = "Distribution Group is required")]
    public int DistributionGroupId { get; set; }

    /// <summary>
    /// Gets or sets the distribution group to which this member belongs.
    /// </summary>
    [Display(Name = "Distribution Group")]
    [StringLength(150, ErrorMessage = "Distribution Group cannot exceed 150 characters")]

    public string DistributionGroup { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the distribution group member's branch contact.
    /// </summary>
    [Display(Name = "Member Branch Contact")]
    [Required(ErrorMessage = "Member Branch Contact is required")]
    public int? MemberBranchContactId { get; set; }

    /// <summary>
    /// Gets or sets the distribution group member's branch contact.
    /// </summary>
    [Display(Name = "Member Branch Contact")]
    [StringLength(150, ErrorMessage = "Member Branch Contact cannot exceed 150 characters")]
    public string MemberBranchContact { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the contact for this distribution group member.
    /// </summary>
    [Display(Name = "Contact Name")]
    [Required(ErrorMessage = "Contact is required")]
    [StringLength(150, ErrorMessage = "Contact Name cannot exceed 150 characters")]
    public string ContactName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address of the contact for this distribution group member.
    /// </summary>
    [Display(Name = "Email Address")]
    [Required(ErrorMessage = "Email Address is required")]
    [RegularExpression("[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}", ErrorMessage = "Please enter a valid email address")]
    [StringLength(150, ErrorMessage = "Email Address cannot exceed 150 characters")]
    public string EmailAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the contact number of the contact for this distribution group member.
    /// </summary>
    [Display(Name = "Contact Number")]
    [StringLength(150, ErrorMessage = "Contact Number cannot exceed 150 characters")]
    public string ContactNumber { get; set; } = string.Empty;
  }
}

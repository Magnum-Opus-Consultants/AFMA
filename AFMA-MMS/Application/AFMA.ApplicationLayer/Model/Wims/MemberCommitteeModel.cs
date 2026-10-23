namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The MemberCommittee class represents a member of a committee.
  /// </summary>
  [MapTo(typeof(MemberCommittee))]
  public class MemberCommitteeModel
  {
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the committee.
    /// </summary>
    [Display(Name = "Committee Name")]
    [Required(ErrorMessage = "Committee is required")]
    public int CommitteeId { get; set; }

    /// <summary>
    /// Gets or sets the committee.
    /// </summary>
    public string Committee { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the member.
    /// </summary>
    [Display(Name = "Member Name")]
    public int? MemberId { get; set; }

    /// <summary>
    /// Gets or sets the member.
    /// </summary>
    public string? Member { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the member name.
    /// </summary>
    [Display(Name = "Member Name")]
    public string? MemberName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the member branch contact details.
    /// </summary>
    [Display(Name = "Member Contact")]
    public int? MemberBranchContactId { get; set; }

    /// <summary>
    /// Gets or sets the member branch contact details.
    /// </summary>
    public string? MemberBranchContact { get; set; }

    /// <summary>
    /// Gets or sets the name of the committee member.
    /// </summary>
    [Display(Name = "Contact Name")]
    [Required(ErrorMessage = "Contact Name is required")]
    [StringLength(150)]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the name of the member branch.
    /// </summary>
    [Display(Name = "Member Branch")]
    [StringLength(150)]
    public string? MemberBranch { get; set; }

    ///// <summary>
    ///// Gets or sets the name of the member branch.
    ///// </summary>
    //[Display(Name = "Member Branch")]
    //[StringLength(150)]
    //public int MemberBranchId { get; set; }

    /// <summary>
    /// Gets or sets the name of the member contact type.
    /// </summary>
    [Display(Name = "Member Contact Type")]
    [StringLength(150)]
    public string? MemberContactType { get; set; }

    /// <summary>
    /// Gets or sets the email address of the committee member.
    /// </summary>
    [Display(Name = "Email Address")]
    [RegularExpression("[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}", ErrorMessage = "Please enter a valid email address")]
    [Required(ErrorMessage = "Email address is required")]
    [StringLength(150)]
    public string? EmailAddress { get; set; }

    /// <summary>
    /// Gets or sets the Branch Contact Match indicator
    /// </summary>
    public bool MemberBranchContactMatch { get; set; }

    public ICollection<CommitteeDocumentModel> Documents { get; set; } = new List<CommitteeDocumentModel>();
  }
}

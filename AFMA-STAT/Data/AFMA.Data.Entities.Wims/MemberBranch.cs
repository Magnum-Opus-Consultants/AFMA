namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The MemberBranch entity class represents a branch of a member in the WIMS application.
  /// </summary>
  public class MemberBranch
  {
    /// <summary>
    /// Gets or sets the unique identifier for the MemberBranch.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the member.
    /// </summary>
    public virtual Member Member { get; set; } = new();

    /// <summary>
    /// Gets or sets the member number.
    /// </summary>
    public virtual string MemberIdentificationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the branch
    /// </summary>
    public virtual string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the province.
    /// </summary>
    public virtual Province Province { get; set; } = new();

    /// <summary>
    /// Gets or sets the physical address of the member.
    /// </summary>
    public virtual string PhysicalAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the postal address of the member.
    /// </summary>
    public virtual string PostalAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the telephone number of the member.
    /// </summary>
    public virtual string TelephoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date of membership.
    /// </summary>
    public virtual DateTime? DateOfMembership { get; set; }

    /// <summary>
    /// Gets or sets the other AFMA member ID.
    /// </summary>
    public virtual string OtherMemberIdentificationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date of issue for the member branch.
    /// </summary>
    public virtual DateTime? DateOfIssue { get; set; }

    /// <summary>
    /// Gets or sets any comments related to the member branch.
    /// </summary>
    public virtual string Comments { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the GPS coordinates for the member.
    /// </summary>
    public virtual string GpsCoordinates { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of animal feed products for the member branch.
    /// </summary>
    public virtual ICollection<MemberBranchAnimalFeedProduct> AnimalFeedProducts { get; set; } = new List<MemberBranchAnimalFeedProduct>();

    /// <summary>
    /// Gets or sets the collection of service providers for the member branch.
    /// </summary>
    public virtual ICollection<MemberBranchServiceProvider> ServiceProviders { get; set; } = new List<MemberBranchServiceProvider>();

    /// <summary>
    /// Gets or sets the collection of contacts for the member branch.
    /// </summary>
    public virtual ICollection<MemberBranchContact> Contacts { get; set; } = new List<MemberBranchContact>();
  }
}

namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The Member entity class.
  /// </summary>
  public class Member
  {
    /// <summary>
    /// Gets or sets the Id.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the member number.
    /// </summary>
    public virtual string MemberIdentificationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the member name.
    /// </summary>
    public virtual string MemberTradeName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the member name.
    /// </summary>
    public virtual string MemberLegalName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch of the member.
    /// </summary>
    public virtual string Branch { get; set; } = string.Empty;

    /// <summary>
    /// Gerts or sets the province of the member.
    /// </summary>
    public virtual Province Province { get; set; } = new();

    /// <summary>
    /// Gets or sets the other name(s) of the member.
    /// </summary>
    public virtual string MemberOtherName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the registration number of the member.
    /// </summary>
    public virtual string RegistrationNo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the VAT registration number of the member.
    /// </summary>
    public virtual string VatRegistrationNo { get; set; } = string.Empty;

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
    public virtual string TelephoneNo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the website of the member.
    /// </summary>
    public virtual string Website { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the member type of the member.
    /// </summary>
    public virtual MemberType MemberType { get; set; } = new();

    /// <summary>
    /// Gets or sets the comments for the member.
    /// </summary>
    public virtual string Comments { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the GPS coordinates for the member.
    /// </summary>
    public virtual string GpsCoordinates { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status for the member.
    /// </summary>
    public virtual bool Active { get; set; }

    public virtual ICollection<MemberBranch> Branches { get; set; } = new List<MemberBranch>();

    public virtual ICollection<MemberDocument> Documents { get; set; } = new List<MemberDocument>();

    public virtual ICollection<MemberCommittee> Committees { get; set; } = new List<MemberCommittee>();

    public virtual ICollection<MemberUser> Users { get; set; } = new List<MemberUser>();
  }
}

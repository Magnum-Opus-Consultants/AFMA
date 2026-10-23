namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;
  using System.Collections.Generic;

  /// <summary>
  /// The MemberModel class represents a member in the WIMS application.
  /// </summary>
  [MapTo(typeof(Member))]
  public class MemberModel
  {
    /// <summary>
    /// Gets or sets the Id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the member number.
    /// </summary>
    [Display(Name = "Membership Number")]
    [Required(ErrorMessage = "Membership Number is required")]
    [StringLength(50)]
    public string MemberIdentificationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the member name.
    /// </summary>
    [Display(Name = "Member Trade Name")]
    [StringLength(255)]
    public string? MemberTradeName { get; set; }

    /// <summary>
    /// Gets or sets the member name.
    /// </summary>
    [Display(Name = "Member Legal Name")]
    [Required(ErrorMessage = "Member Legal Name is required")]
    [StringLength(255)]
    public string MemberLegalName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch of the member.
    /// </summary>
    [Display(Name = "Branches")]
    [StringLength(255)]
    public string? Branch { get; set; }

    /// <summary>
    /// Gets or sets the province of the member.
    /// </summary>
    [Display(Name = "Province")]
    [Required(ErrorMessage = "Province is required")]
    public int ProvinceId { get; set; }

    /// <summary>
    /// Gets or sets the province of the member.
    /// </summary>
    [Display(Name = "Province")]
    public string? Province { get; set; }

    /// <summary>
    /// Gets or sets the other name(s) of the member.
    /// </summary>
    [Display(Name = "Other Name(s)")]
    public string? MemberOtherName { get; set; }

    /// <summary>
    /// Gets or sets the registration number of the member.
    /// </summary>
    [Display(Name = "Registration Number")]
    [StringLength(50)]
    public string? RegistrationNo { get; set; }

    /// <summary>
    /// Gets or sets the VAT registration number of the member.
    /// </summary>
    [Display(Name = "VAT Registration Number")]
    [StringLength(50)]
    public string? VatRegistrationNo { get; set; }

    /// <summary>
    /// Gets or sets the physical address of the member.
    /// </summary>
    [Display(Name = "Physical Address")]
    [Required(ErrorMessage = "Physical Address is required")]
    [StringLength(1000)]
    public string PhysicalAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the postal address of the member.
    /// </summary>
    [Display(Name = "Postal Address")]
    [Required(ErrorMessage = "Postal Address is required")]
    [StringLength(1000)]
    public string PostalAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the telephone number of the member.
    /// </summary>
    [Display(Name = "Telephone Number")]
    [Required(ErrorMessage = "Telephone Number is required")]
    [StringLength(100)]
    public string TelephoneNo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the website of the member.
    /// </summary>
    [Display(Name = "Website")]
    [StringLength(255)]
    public string? Website { get; set; }

    /// <summary>
    /// Gets or sets the member type of the member.
    /// </summary>
    [Display(Name = "Member Type")]
    [Required(ErrorMessage = "Member Type is Required")]
    public int MemberTypeId { get; set; }

    /// <summary>
    /// Gets or sets the member type of the member.
    /// </summary>
    [Display(Name = "Member Type")]
    public string? MemberType { get; set; }

    /// <summary>
    /// Gets or sets the comments for the member.
    /// </summary>
    [Display(Name = "Comments")]
    public string? Comments { get; set; }

    /// <summary>
    /// Gets or sets the GPS Coordinates for the member.
    /// </summary>
    [Display(Name = "GPS Coordinates")]
    public string? GpsCoordinates { get; set; }

    /// <summary>
    /// Gets or sets the status for the member.
    /// </summary>
    [Display(Name = "Status")]
    public bool Active { get; set; }

    [Display(Name = "Branches")]
    public ICollection<MemberBranchModel> Branches { get; set; } = new List<MemberBranchModel>();

    public ICollection<MemberDocumentModel> Documents { get; set; } = new List<MemberDocumentModel>();

    public ICollection<MemberCommitteeModel> Committees { get; set; } = new List<MemberCommitteeModel>();

    public ICollection<MemberUserModel> Users { get; set; } = new List<MemberUserModel>();
  }
}

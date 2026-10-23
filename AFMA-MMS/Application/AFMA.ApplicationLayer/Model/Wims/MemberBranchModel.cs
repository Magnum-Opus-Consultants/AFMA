namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.ApplicationLayer.ViewModels.Wims;
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The MemberBranchModel class represents a member's branch information in the WIMS application.
  /// </summary>
  [MapTo(typeof(MemberBranch))]
  public class MemberBranchModel
  {
    /// <summary>
    /// Gets or sets the unique identifier for the MemberBranch.
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
    public string Member { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the member number.
    /// </summary>
    [Display(Name = "Membership Number")]
    [StringLength(50)]
    public string MemberIdentificationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the branch.
    /// </summary>
    [Display(Name = "Branch Name")]
    [Required(ErrorMessage = "Branch Name is required")]
    [StringLength(150)]
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Id of the province.
    /// </summary>
    [Display(Name = "Province")]
    [Required(ErrorMessage = "Province is required")]
    public int ProvinceId { get; set; }

    /// <summary>
    /// Gets or sets the name of the province.
    /// </summary>
    [Display(Name = "Province")]
    public string Province { get; set; } = string.Empty;

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
    [StringLength(150)]
    public string TelephoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date of membership.
    /// </summary>
    [Display(Name = "Date Of Membership")]
    public DateTime? DateOfMembership { get; set; }

    /// <summary>
    /// Gets or sets the other AFMA member ID.
    /// </summary>
    [Display(Name = "Other Member ID")]
    [StringLength(50)]
    public string? OtherMemberIdentificationId { get; set; }

    /// <summary>
    /// Gets or sets the date of issue for the member branch.
    /// </summary>
    [Display(Name = "Date Of Issue")]
    public DateTime? DateOfIssue { get; set; }

    /// <summary>
    /// Gets or sets any comments related to the member branch.
    /// </summary>
    [Display(Name = "Comments")]
    public string? Comments { get; set; }

    /// <summary>
    /// Gets or sets the GPS Coordinates for the branch.
    /// </summary>
    [Display(Name = "GPS Coordinates")]
    public string GpsCoordinates { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of animal feed products for the member branch.
    /// </summary>
    public ICollection<MemberBranchAnimalFeedProductModel> AnimalFeedProducts { get; set; } = new List<MemberBranchAnimalFeedProductModel>();

    /// <summary>
    /// Gets or sets the collection of animal feed products for the member branch.
    /// </summary>
    public ICollection<MemberBranchServiceProviderModel> ServiceProviders { get; set; } = new List<MemberBranchServiceProviderModel>();

    /// <summary>
    /// Gets or sets the collection of services for the member branch.
    /// </summary>
    public ICollection<MemberBranchServiceModel> Services { get; set; } = new List<MemberBranchServiceModel>();

    /// <summary>
    /// Gets or sets the collection of manufacturers for the member branch.
    /// </summary>
    public ICollection<MemberBranchManufacturerModel> Manufacturers { get; set; } = new List<MemberBranchManufacturerModel>();

    /// <summary>
    /// Gets or sets the collection of suppliers for the member branch.
    /// </summary>
    public ICollection<MemberBranchSupplierModel> Suppliers { get; set; } = new List<MemberBranchSupplierModel>();

    /// <summary>
    /// Gets or sets the collection of contacts for the member branch.
    /// </summary>
    public ICollection<MemberBranchContactModel> Contacts { get; set; } = new List<MemberBranchContactModel>();

    /// <summary>
    /// Gets or sets the categorized contacts for the member branch.
    /// </summary>
    public ICollection<MemberBranchContactTypeViewModel> CategorizedContacts { get; set; } = new List<MemberBranchContactTypeViewModel>();
  }
}

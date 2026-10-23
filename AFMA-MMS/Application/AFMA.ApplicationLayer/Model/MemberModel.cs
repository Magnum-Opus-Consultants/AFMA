namespace AFMA.ApplicationLayer.Model
{
  using System.ComponentModel.DataAnnotations;

  /// <summary>
  /// The MemberModel class.
  /// </summary>
  [MapTo(typeof(Member))]
  public class MemberModel
  {
    public MemberModel()
    {
      Modules = new List<MemberModuleModel>();
    }

    /// <summary>
    /// Gets or sets the internal Id of the member.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Name of the member.
    /// </summary>
    [Display(Name = "Name")]
    [Required(ErrorMessage = "Name is required."), MaxLength(200, ErrorMessage = "Name should be 200 characters or less.")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the Id for the member.
    /// </summary>
    [Display(Name = "Member ID")]
    public string? MemberId { get; set; }

    /// <summary>
    /// Gets or sets the Comments for the member.
    /// </summary>
    [Display(Name = "Comment")]
    [MaxLength(4000, ErrorMessage = "Comments should be 4000 characters or less.")]
    public string? Comment { get; set; }

    /// <summary>
    /// Gets or sets the Status for the member.
    /// </summary>
    [Display(Name = "Active")]
    [Required(ErrorMessage = "Status is required.")]
    public bool Active { get; set; }

    /// <summary>
    /// Gets or sets the Created Date for the member.
    /// </summary>
    [Display(Name = "Create Date")]
    [Required(ErrorMessage = "Created Date is required."), DataType(DataType.Date)]
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the Updated Date for the member.
    /// </summary>
    [Display(Name = "Updated Date")]
    [DataType(DataType.Date)]
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Gets or sets the last updated user.
    /// </summary>
    [Display(Name = "Updated By")]
    public UserModel? UpdatedUser { get; set; }

    /// <summary>
    /// Gets or sets the Id of the member type.
    /// </summary>
    [Required(ErrorMessage = "Member type is required.")]
    [Display(Name = "Member Type")]
    public int MemberTypeId { get; set; }

    /// <summary>
    /// Gets or sets the member type.
    /// </summary>
    [Display(Name = "Member Type")]
    public string? MemberType { get; set; }

    public ICollection<MemberBranchModel>? Branches { get; set; }

    public ICollection<MemberAccountModel>? Accounts { get; set; }
    public ICollection<MemberModuleModel>? Modules { get; set; }
  }

  /// <summary>
  /// The MemberSummaryModel class.
  /// </summary>
  public class MemberSummaryModel
  {
    /// <summary>
    /// Gets or sets the internal Id of the member.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Name of the member.
    /// </summary>
    [Display(Name = "Name")]
    [Required(ErrorMessage = "Name is required."), MaxLength(200, ErrorMessage = "Name should be 200 characters or less.")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the Id for the member.
    /// </summary>
    [Display(Name = "Member ID")]
    public string? MemberId { get; set; }

    /// <summary>
    /// Gets or sets the Status for the member.
    /// </summary>
    [Display(Name = "Active")]
    [Required(ErrorMessage = "Status is required.")]
    public bool Active { get; set; }

    /// <summary>
    /// Gets or sets the Id of the member type.
    /// </summary>
    [Required(ErrorMessage = "Member type is required.")]
    [Display(Name = "Member Type")]
    public int MemberTypeId { get; set; }

    /// <summary>
    /// Gets or sets the member type.
    /// </summary>
    [Display(Name = "Member Type")]
    public string? MemberType { get; set; }

    public ICollection<MemberBranchSummaryModel>? Branches { get; set; }
    
    //public ICollection<MemberAccountModel>? Accounts { get; set; }

    //public ICollection<MemberModuleModel>? Modules { get; set; }
  }
}

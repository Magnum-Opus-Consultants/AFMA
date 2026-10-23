namespace AFMA.ApplicationLayer.Model
{
  using System.ComponentModel.DataAnnotations;

  /// <summary>
  /// The MemberBranchModel class
  /// </summary>
  [MapTo(typeof(MemberBranch))]
  public class MemberBranchModel
  {
    /// <summary>
    /// Gets or sets the Id of the member.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Name of the member.
    /// </summary>
    [Display(Name = "Branch Name")]
    [Required(ErrorMessage = "Name is required."), MaxLength(200, ErrorMessage = "Name should be 100 characters or less.")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the Code for the member.
    /// </summary>
    [MaxLength(50, ErrorMessage = "Code should be 200 characters or less.")]
    public string? Code { get; set; }

    /// <summary>
    /// Gets or sets the Id of the member.
    /// </summary>
    [Required(ErrorMessage = "Member is required.")]
    [Display(Name = "Member")]
    public int MemberId { get; set; }

    /// <summary>
    /// Gets or sets the member.
    /// </summary>
    [Display(Name = "Member")]
    public string? Member { get; set; }

    /// <summary>
    /// Gets or sets the Id of the province.
    /// </summary>
    [Required(ErrorMessage = "Province is required.")]
    [Display(Name = "Province")]
    public int ProvinceId { get; set; }

    /// <summary>
    /// Gets or sets the province.
    /// </summary>
    [Display(Name = "Province")]
    public string? Province { get; set; }

    /// <summary>
    /// Gets or sets the created date.
    /// </summary>
    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    [Display(Name = "Status")]
    public bool Active { get; set; }
  }

  /// <summary>
  /// The MemberBranchSummaryModel class
  /// </summary>
  public class MemberBranchSummaryModel
  {
    /// <summary>
    /// Gets or sets the Id of the member.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Name of the member.
    /// </summary>
    [Required(ErrorMessage = "Name is required."), MaxLength(200, ErrorMessage = "Name should be 100 characters or less.")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the Code for the member.
    /// </summary>
    [MaxLength(50, ErrorMessage = "Code should be 200 characters or less.")]
    public string? Code { get; set; }

    /// <summary>
    /// Gets or sets the Id of the member.
    /// </summary>
    [Required(ErrorMessage = "Member is required.")]
    [Display(Name = "Member")]
    public int MemberId { get; set; }

    /// <summary>
    /// Gets or sets the member.
    /// </summary>
    [Display(Name = "Member")]
    public string? Member { get; set; }

    /// <summary>
    /// Gets or sets the Id of the province.
    /// </summary>
    [Required(ErrorMessage = "Province is required.")]
    [Display(Name = "Province")]
    public int ProvinceId { get; set; }

    /// <summary>
    /// Gets or sets the province.
    /// </summary>
    [Display(Name = "Province")]
    public string? Province { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    [Display(Name = "Status")]
    public bool Active { get; set; }
  }
}
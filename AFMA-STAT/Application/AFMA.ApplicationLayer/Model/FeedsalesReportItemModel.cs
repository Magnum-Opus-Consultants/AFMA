namespace AFMA.ApplicationLayer.Model
{
  using Alpha.Services.Models.Attributes;

  /// <summary>
  /// The FeedsalesReportItemModel class
  /// </summary>
  [MapTo(typeof(FeedsalesReportItem))]
  public class FeedsalesReportItemModel
  {
    /// <summary>
    /// Gets or sets the Id of the report item
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the feedsales report
    /// </summary>
    public int FeedsalesReportId { get; set; }

    /// <summary>
    /// Gets or sets the finished product
    /// </summary>
    [RequiredIf(nameof(IsRawMaterial), false, ErrorMessage = "Please select a product")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a product.")]
    public int? ProductId { get; set; }

    /// <summary>
    /// Gets or sets the finished product group
    /// </summary>
    public int? ProductGroupId { get; set; }

    /// <summary>
    /// Gets or sets the finished product
    /// </summary>
    public string? Product { get; set; }

    /// <summary>
    /// Gets or sets the finished product code
    /// </summary>
    public string? ProductCode { get; set; }

    /// <summary>
    /// Gets or sets the raw material
    /// </summary>
    [RequiredIf(nameof(IsRawMaterial), true, ErrorMessage = "Please select a material")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a material.")]
    public int? MaterialId { get; set; }

    /// <summary>
    /// Gets or sets the raw material
    /// </summary>
    public string? Material { get; set; }

    /// <summary>
    /// Gets or sets the raw material code
    /// </summary>
    public string? MaterialCode { get; set; }

    /// <summary>
    /// Gets or sets the submitted value
    /// </summary>
    [Required(ErrorMessage = "Please enter a value")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a valid value")]
    public decimal Value { get; set; }

    /// <summary>
    /// Gets or sets the previous submitted value
    /// </summary>
    public decimal PreviousValue { get; set; }

    /// <summary>
    /// Gets or sets the member
    /// </summary>
    public int MemberId { get; set; }

    /// <summary>
    /// Gets or sets the member
    /// </summary>
    public string Member { get; set; }

    /// <summary>
    /// Gets or sets the member branch
    /// </summary>
    public int MemberBranchId { get; set; }

    /// <summary>
    /// Gets or sets the member branch
    /// </summary>
    public string MemberBranch { get; set; }

    /// <summary>
    /// Gets or sets the member account
    /// </summary>
    public int? MemberAccountId { get; set; }

    /// <summary>
    /// Gets or sets the member account
    /// </summary>
    public string MemberAccount { get; set; }

    /// <summary>
    /// Gets or sets the other description
    /// </summary>
    [RequiredIf(nameof(IsOther), true, ErrorMessage = "Please specify another material")]
    public string? Other { get; set; }

    #region Validation Properties

    /// <summary>
    /// Gets or sets a value indicating whether or not a material or item is an other material.
    /// </summary>
    /// <remarks>
    /// This is not for database actions, but rather for validations. We need to check if the <see cref="this.Other"/> is completed when this is true.
    /// See the Front end for an example.
    /// </remarks>
    public bool IsOther { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not a material or item is raw material.
    /// </summary>
    /// <remarks>
    /// This is not for database actions, but rather for validations. We need to check if the different properties is completed when this is true or false.
    /// See the Front end for an example.
    /// </remarks>
    public bool IsRawMaterial { get; set; }

    #endregion
  }
}

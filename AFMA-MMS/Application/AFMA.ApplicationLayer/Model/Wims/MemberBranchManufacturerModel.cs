namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The MemberBranchManufacturer Model class represents a manufacturer associated with a member branch in the WIMS application.
  /// </summary>
  [MapTo(typeof(MemberBranchManufacturer))]
  public class MemberBranchManufacturerModel
  {
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the manufacturer Id.
    /// </summary>
    public int ManufacturerId { get; set; }

    /// <summary>
    /// Gets or sets the manufacturer name.
    /// </summary>
    public string Manufacturer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the member branch Id.
    /// </summary>
    public int MemberBranchId { get; set; }

    /// <summary>
    /// Gets or sets the member branch name.
    /// </summary>
    public string MemberBranch { get; set; } = string.Empty;
  }
}

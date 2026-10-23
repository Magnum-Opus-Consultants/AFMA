namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The MemberBranchSupplier Model class represents a service provided by a member branch in the WIMS application.
  /// </summary>
  [MapTo(typeof(MemberBranchSupplier))]
  public class MemberBranchSupplierModel
  {
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the supplier Id.
    /// </summary>
    public int SupplierId { get; set; }

    /// <summary>
    /// Gets or sets the supplier name.
    /// </summary>
    public string Supplier { get; set; } = string.Empty;

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

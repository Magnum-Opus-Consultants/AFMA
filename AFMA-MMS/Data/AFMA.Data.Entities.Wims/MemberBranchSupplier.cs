namespace AFMA.Data.Entities.Wims
{

  /// <summary>
  /// The MemberBranchSupplier class represents a supplier associated with a member branch in the WIMS system.
  /// </summary>
  public class MemberBranchSupplier
  {
    /// <summary>
    /// The unique identifier for the MemberBranchSupplier.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the Member Branch.
    /// </summary>
    public virtual MemberBranch MemberBranch { get; set; } = new();

    /// <summary>
    /// Gets or sets the unique identifier for the Supplier.
    /// </summary>
    public virtual Supplier Supplier { get; set; } = new();
  }
}

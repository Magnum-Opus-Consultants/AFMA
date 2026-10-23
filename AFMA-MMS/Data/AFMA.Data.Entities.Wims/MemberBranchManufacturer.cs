namespace AFMA.Data.Entities.Wims
{

  /// <summary>
  /// The MemberBranchManufacturer class represents a manufacturer associated with a member branch in the WIMS system.
  /// </summary>
  public class MemberBranchManufacturer
  {
    /// <summary>
    /// The unique identifier for the MemberBranchManufacturer.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the Member Branch.
    /// </summary>
    public virtual MemberBranch MemberBranch { get; set; } = new();

    /// <summary>
    /// Gets or sets the unique identifier for the Service.
    /// </summary>
    public virtual Manufacturer Manufacturer { get; set; } = new();
  }
}

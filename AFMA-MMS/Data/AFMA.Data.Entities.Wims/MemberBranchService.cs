namespace AFMA.Data.Entities.Wims
{

  /// <summary>
  /// The MemberBranchService class represents a service provided by a member branch in the WIMS system.
  /// </summary>
  public class MemberBranchService
  {
    /// <summary>
    /// The unique identifier for the MemberBranchService.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the Member Branch.
    /// </summary>
    public virtual MemberBranch MemberBranch { get; set; } = new();

    /// <summary>
    /// Gets or sets the unique identifier for the Service.
    /// </summary>
    public virtual Service Service { get; set; } = new();
  }
}

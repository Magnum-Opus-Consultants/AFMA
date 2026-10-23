namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The MemberBranchServiceModel class represents a service provided by a member branch in the WIMS application.
  /// </summary>
  [MapTo(typeof(MemberBranchService))]
  public class MemberBranchServiceModel
  {
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the service Id.
    /// </summary>
    public int ServiceId { get; set; }

    /// <summary>
    /// Gets or sets the service name.
    /// </summary>
    public string Service { get; set; } = string.Empty;

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

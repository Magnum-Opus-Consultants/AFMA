namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The MemberServiceProviderModel class represents a member branch animal feed product in the WIMS application.
  /// </summary>
  [MapTo(typeof(MemberBranchServiceProvider))]
  public class MemberBranchServiceProviderModel
  {
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the service provider Id.
    /// </summary>
    public int ServiceProviderId { get; set; }

    /// <summary>
    /// Gets or sets the service provider name.
    /// </summary>
    public string ServiceProvider { get; set; } = string.Empty;


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

namespace AFMA.Data.Entities.Wims
{
  public class MemberBranchServiceProvider
  {
    /// <summary>
    /// The unique identifier for the MemberBranchAnimalFeedProduct.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the MemberBranchAnimalFeedProduct.
    /// </summary>
    public virtual MemberBranch MemberBranch { get; set; } = new();

    /// <summary>
    /// Gets or sets the unique identifier for the ServiceProvider.
    /// </summary>
    public virtual ServiceProvider ServiceProvider { get; set; } = new();
  }
}

namespace AFMA.Data.Entities.Wims
{
  public class MemberBranchAnimalFeedProduct
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
    /// Gets or sets the unique identifier for the MemberBranchAnimalFeedProduct.
    /// </summary>
    public virtual AnimalFeedProduct AnimalFeedProduct { get; set; } = new();
  }
}

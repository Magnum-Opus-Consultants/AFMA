namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The MemberBranchAnimalFeedProductModel class represents a member branch animal feed product in the WIMS application.
  /// </summary>
  [MapTo(typeof(MemberBranchAnimalFeedProduct))]
  public class MemberBranchAnimalFeedProductModel
  {
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the animal feed product Id.
    /// </summary>
    public int AnimalFeedProductId { get; set; }

    /// <summary>
    /// Gets or sets the animal feed product name.
    /// </summary>
    public string AnimalFeedProduct { get; set; } = string.Empty;


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

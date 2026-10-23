namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The AnimalFeedProductModel class represents an animal feed product in the WIMS application.
  /// </summary>
  [MapTo(typeof(AnimalFeedProduct))]
  public class AnimalFeedProductModel
  {
    /// <summary>
    /// The unique identifier for the AnimalFeedProduct.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the AnimalFeedProduct.
    /// </summary>
    public string? Description { get; set; }
  }
}

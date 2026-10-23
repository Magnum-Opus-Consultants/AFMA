namespace AFMA.Data.Entities.Wims
{
  public class AnimalFeedProduct
  {
    /// <summary>
    /// The unique identifier for the AnimalFeedProduct.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// The name of the AnimalFeedProduct.
    /// </summary>
    public virtual string? Description { get; set; }
  }
}

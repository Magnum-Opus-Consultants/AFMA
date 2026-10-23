namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The ProvinceModel class represents a province in the WIMS application.
  /// </summary>
  [MapTo(typeof(Province))]
  public class ProvinceModel
  {
    /// <summary>
    /// The unique identifier for the Province.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the Province.
    /// </summary>
    public string? Description { get; set; }
  }
}

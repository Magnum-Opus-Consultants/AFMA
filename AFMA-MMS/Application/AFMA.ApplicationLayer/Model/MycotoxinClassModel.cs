namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The MycotoxinClassModel class.
  /// </summary>
  [MapTo(typeof(MycotoxinClass))]
  public class MycotoxinClassModel
  {
    /// <summary>
    /// Gets or sets the Id of the mycotoxin class..
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the mycotoxin class.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the status of the mycotoxin class.
    /// </summary>
    public bool Active { get; set; }
  }
}
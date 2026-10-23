namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The MycotoxinTypeModel class.
  /// </summary>
  [MapTo(typeof(MycotoxinType))]
  public class MycotoxinTypeModel
  {
    /// <summary>
    /// Gets or sets the Id of the mycotoxin type..
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the mycotoxin type.
    /// </summary>
    [Display(Name = "Name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the status of the mycotoxin type.
    /// </summary>
    [Display(Name = "Active")]
    public bool Active { get; set; }

    /// <summary>
    /// Gets or sets the Id of the mycotoxin type.
    /// </summary>
    [Display(Name = "Mycotoxin Class")]
    public int MycotoxinClassId { get; set; }

    /// <summary>
    /// Gets or sets the name of the mycotoxin type.
    /// </summary>
    [Display(Name = "Mycotoxin Class")]
    public string? MycotoxinClass { get; set; }
  }
}

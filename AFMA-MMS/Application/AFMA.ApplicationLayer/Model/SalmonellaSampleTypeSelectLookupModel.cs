namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The SalmonellaSampleTypeModel Class.
  /// </summary>
  public class SalmonellaSampleTypeSelectLookupModel
  {
    /// <summary>
    /// Gets or sets the Id of the Salmonella Sample Type
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the short code.
    /// </summary>
    public string? ShortCode { get; set; }

    /// <summary>
    /// Gets or sets the name of the Salmonella Sample Type
    /// </summary>
    public string? Name { get; set; }
  }
}

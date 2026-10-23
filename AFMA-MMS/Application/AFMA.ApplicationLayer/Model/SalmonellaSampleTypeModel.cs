namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The SalmonellaSampleTypeModel Class.
  /// </summary>
  [MapTo(typeof(SalmonellaSampleType))]
  public class SalmonellaSampleTypeModel
  {
    /// <summary>
    /// Gets or sets the Id of the Salmonella Sample Type
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the short code.
    /// </summary>
    [Display(Name = "Short Code")]
    public string? ShortCode { get; set; }

    /// <summary>
    /// Gets or sets the name of the Salmonella Sample Type
    /// </summary>
    [Display(Name = "Name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the status of the Salmonella Sample Type
    /// </summary>
    [Display(Name = "Active")]
    public bool Active { get; set; }
  }
}

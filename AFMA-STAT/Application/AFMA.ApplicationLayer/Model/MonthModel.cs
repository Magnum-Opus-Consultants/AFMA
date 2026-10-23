namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The MonthModel class.
  /// </summary>
  [MapTo(typeof(Month))]
  public class MonthModel
  {
    /// <summary>
    /// Gets or sets the Id of the month.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the number of the month.
    /// </summary>
    [Display(Name = "Month Number")]
    public int MonthNumber { get; set; }

    /// <summary>
    /// Gets or sets the name of the month.
    /// </summary>
    [Display(Name = "Month Name")]
    public string MonthName { get; set; }

    /// <summary>
    /// Gets or sets the season of the month.
    /// </summary>
    [Display(Name = "Season")]
    public int Season { get; set; }

    /// <summary>
    /// Gets or sets the status of the month.
    /// </summary>
    [Display(Name = "Status")]
    public bool Active { get; set; }
  }
}

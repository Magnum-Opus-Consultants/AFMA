namespace AFMA.Data.Entities
{
  /// <summary>
  /// The Month class.
  /// </summary>
  public class Month
  {
    /// <summary>
    /// Gets or sets the Id of the month.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the number of the month.
    /// </summary>
    public virtual int MonthNumber { get; set; }

    /// <summary>
    /// Gets or sets the name of the month.
    /// </summary>
    public virtual string MonthName { get; set; }

    /// <summary>
    /// Gets or sets the season of the month.
    /// </summary>
    public virtual int Season { get; set; }

    /// <summary>
    /// Gets or sets the status of the month.
    /// </summary>
    public virtual bool Active { get; set; }
  }
}

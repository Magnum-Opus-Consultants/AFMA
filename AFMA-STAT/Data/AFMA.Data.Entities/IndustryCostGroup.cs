namespace AFMA.Data.Entities
{
  /// <summary>
  /// The IndustryCostGroup class.
  /// </summary>
  public class IndustryCostGroup
  {
    public IndustryCostGroup()
    {
      Items = new List<IndustryGroupItem>();
    }

    /// <summary>
    /// Gets or sets the Id of the industry cost group.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the industry cost group.
    /// </summary>
    public virtual string? GroupName { get; set; }

    /// <summary>
    /// Gets or sets the financial year.
    /// </summary>
    public virtual int FinancialYear { get; set; }

    /// <summary>
    /// Gets or sets the linked table.
    /// </summary>
    public virtual string? LinkedTable { get; set; }

    /// <summary>
    /// Gets or sets the linked Id.
    /// </summary>
    public virtual int LinkedId { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public virtual bool Active { get; set; }

    /// <summary>
    /// Gets or sets the industry group items.
    /// </summary>
    public virtual ICollection<IndustryGroupItem> Items { get; set; }
  }
}

namespace AFMA.Data.Entities
{
  /// <summary>
  /// The SalmonellaSampleTypeSubItem Class.
  /// </summary>
  public class SalmonellaSampleTypeSubItem
  {
    /// <summary>
    /// Gets or sets the Id of the Salmonella Sample Type Item
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the Salmonella Sample Type Item
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Gets or sets the Salmonella Sample Type
    /// </summary>
    public virtual SalmonellaSampleType SalmonellaSampleType { get; set; }

    /// <summary>
    /// Gets or sets the short code of the Salmonella Sample Type Item
    /// </summary>
    public virtual string ShortCode { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public virtual int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the visible to members status
    /// </summary>
    public virtual bool VisibleToMembers { get; set; }

    /// <summary>
    /// Gets or sets the status of the Salmonella Sample Type Item
    /// </summary>
    public virtual bool Active { get; set; }
  }
}

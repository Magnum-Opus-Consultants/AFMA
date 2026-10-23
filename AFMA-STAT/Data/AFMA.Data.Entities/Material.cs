namespace AFMA.Data.Entities
{
  /// <summary>
  /// The Raw Material class.
  /// </summary>
  public class Material
  {
    /// <summary>
    /// Gets or sets the Id of the Raw Material.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the raw material name.
    /// </summary>
    public virtual string? Name { get; set; }

    /// <summary>
    /// Gets or sets the raw material category.
    /// </summary>
    public virtual MaterialCategory? MaterialCategory { get; set; }

    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    public virtual int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the raw material code.
    /// </summary>
    public virtual string? Code { get; set; }

    /// <summary>
    /// Gets or sets the raw material status.
    /// </summary>
    public virtual bool Active { get; set; }

    public virtual SalmonellaSampleType? SalmonellaSampleType { get; set; }

    /// <summary>
    /// Gets or sets the salmonella report group.
    /// </summary>
    public virtual SalmonellaReportGroup? SalmonellaReportGroup { get; set; }

    /// <summary>
    /// Gets or sets the feedsales report group.
    /// </summary>
    public virtual FeedsalesReportGroup? FeedsalesReportGroup { get; set; }

    /// <summary>
    /// Gets or sets the hidden on feedsales flag.
    /// </summary>
    public virtual bool HideOnFeedsales { get; set; }

    /// <summary>
    /// Gets or sets the hidden on salmonella flag.
    /// </summary>
    public virtual bool HideOnSalmonella { get; set; }

    /// <summary>
    /// Gets or sets the visible to members.
    /// </summary>
    public virtual bool VisibleToMembers { get; set; }

    /// <summary>
    /// Gets or sets the other details required flag.
    /// </summary>
    public virtual bool Other { get; set; }

    /// <summary>
    /// Gets or sets the allow duplicated flag.
    /// </summary>
    public virtual bool AllowDuplicates { get; set; }

    /// <summary>
    /// Gets or sets the show alert message flag.
    /// </summary>
    public virtual bool ShowAlert { get; set; }

    /// <summary>
    /// Gets or sets the alert message.
    /// </summary>
    public virtual string? AlertMessage { get; set; }
  }
}

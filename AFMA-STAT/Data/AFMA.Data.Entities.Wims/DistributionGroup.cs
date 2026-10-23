namespace AFMA.Data.Entities.Wims
{
  using System.Collections.Generic;

  /// <summary>
  /// The DistributionGroup class represents a group of members for distribution purposes.
  /// </summary>
  public class DistributionGroup
  {
    /// <summary>
    /// Gets or sets the unique identifier for the distribution group.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the distribution group.
    /// </summary>
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the distribution group.
    /// </summary>
    public virtual string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the owner of the distribution group.
    /// </summary>
    public virtual string Owner { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address from which the distribution group sends emails.
    /// </summary>
    public virtual string FromAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the privacy status of the distribution group.
    /// </summary>
    public virtual bool Private { get; set; }

    /// <summary>
    /// Gets or sets the collection of members in the distribution group.
    /// </summary>
    public virtual ICollection<DistributionGroupMember> DistributionGroupMembers { get; set; } = [];
  }
}

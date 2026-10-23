namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The DistributionGroupMember class represents a member of a distribution group.
  /// </summary>
  public class DistributionGroupMember
  {
    /// <summary>
    /// Gets or sets the unique identifier for the distribution group member.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the distribution group to which this member belongs.
    /// </summary>
    public virtual DistributionGroup DistributionGroup { get; set; } = null!;

    /// <summary>
    /// Gets or sets the distribution group member's branch contact.
    /// </summary>
    public virtual MemberBranchContact MemberBranchContact { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the contact for this distribution group member.
    /// </summary>
    public virtual string ContactName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address of the contact for this distribution group member.
    /// </summary>
    public virtual string EmailAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the contact number of the contact for this distribution group member.
    /// </summary>
    public virtual string ContactNumber { get; set; } = string.Empty;
  }
}

namespace AFMA.Data.Entities.Wims
{

  /// <summary>
  /// The MemberUserDistributionGroup class represents the association between a member user and a distribution group in the WIMS application.
  /// </summary>
  public class MemberUserDistributionGroup
  {

    /// <summary>
    /// Gets or sets the unique identifier for the MemberUserDistributionGroup.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the Member Distribution Group foreign key.
    /// </summary>
    public virtual DistributionGroup DistributionGroup { get; set; } = new();

    /// <summary>
    /// Gets or sets the Member User foreign key.
    /// </summary>
    public virtual MemberUser MemberUser { get; set; } = new();

    /// <summary>
    /// Gets or sets the first name of the user associated with this member distribution group.
    /// </summary>
    public virtual string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name of the user associated with this member distribution group.
    /// </summary>
    public virtual string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address of the user associated with this member distribution group.
    /// </summary>
    public virtual string EmailAddress { get; set; } = string.Empty;
  }
}

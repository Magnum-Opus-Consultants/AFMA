namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The committee member entity.
  /// </summary>
  public class MemberCommittee
  {
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the committee.
    /// </summary>
    public virtual Committee Committee { get; set; } = new();

    /// <summary>
    /// Gets or sets the member.
    /// </summary>
    public virtual Member? Member { get; set; } = new();

    /// <summary>
    /// Gets or sets the member branch contact details.
    /// </summary>
    public virtual MemberBranchContact? MemberBranchContact { get; set; } = new();

    /// <summary>
    /// Gets or sets the name of the committee member.
    /// </summary>
    public virtual string? Name { get; set; }

    /// <summary>
    /// Gets or sets the email address of the committee member.
    /// </summary>
    public virtual string? EmailAddress { get; set; }

    /// <summary>
    /// Gets or sets the name of the committee member.
    /// </summary>
    public virtual string? MemberName { get; set; }
  }
}

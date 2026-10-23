namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The committee entity.
  /// </summary>
  public class Committee
  {
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the committee name.
    /// </summary>
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the committee members
    /// </summary>
    public virtual ICollection<MemberCommittee> Members { get; set; } = new List<MemberCommittee>();

    /// <summary>
    /// Gets or sets the committee documents.
    /// </summary>
    public virtual ICollection<CommitteeDocument> Documents { get; set; } = new List<CommitteeDocument>();
  }
}

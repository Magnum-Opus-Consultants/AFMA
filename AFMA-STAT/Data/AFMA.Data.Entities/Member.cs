namespace AFMA.Data.Entities
{
  /// <summary>
  /// The Member entity class.
  /// </summary>
  public class Member
  {
    /// <summary>
    /// Gets or sets the Id of the member.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the member name.
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Gets or sets the Member Id.
    /// </summary>
    public virtual string MemberId { get; set; }

    /// <summary>
    /// Gets or sets the member comments.
    /// </summary>
    public virtual string Comment { get; set; }

    /// <summary>
    /// Gets or set the member status.
    /// </summary>
    public virtual bool Active { get; set; }

    /// <summary>
    /// Gets or set the member created date.
    /// </summary>
    public virtual DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or set the member updated date.
    /// </summary>
    public virtual DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Gets or set the last updated user.
    /// </summary>
    public virtual User? UpdatedUser { get; set; }

    /// <summary>
    /// Gets or sets the member type.
    /// </summary>
    public virtual MemberType? MemberType { get; set; }

    /// <summary>
    /// Gets the list of branches for this member.
    /// </summary>
    public virtual ICollection<MemberBranch>? Branches { get; set; }

    /// <summary>
    /// Gets the list of accounts for this member.
    /// </summary>
    public virtual ICollection<MemberAccount>? Accounts { get; set; }

    /// <summary>
    /// Gets the list of modules for this member.
    /// </summary>
    public virtual ICollection<MemberModule>? Modules { get; set; }
  }

  /// <summary>
  /// The MemberSummary entity class.
  /// </summary>
  public class MemberSummary
  {
    /// <summary>
    /// Gets or sets the Id of the member.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the member name.
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Gets or sets the Member Id.
    /// </summary>
    public virtual string MemberId { get; set; }

    /// <summary>
    /// Gets or set the member status.
    /// </summary>
    public virtual bool Active { get; set; }

    /// <summary>
    /// Gets or sets the member type.
    /// </summary>
    public virtual MemberType? MemberType { get; set; }

    //public virtual ICollection<MemberAccount>? Accounts { get; set; }
    public virtual ICollection<MemberModule>? Modules { get; set; }

    /// </summary>
    public virtual ICollection<MemberBranchSummary>? Branches { get; set; }
  }
}

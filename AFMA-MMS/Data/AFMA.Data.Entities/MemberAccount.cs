namespace AFMA.Data.Entities
{
  /// <summary>
  /// The MemberAccount entity class.
  /// </summary>
  public class MemberAccount
  {
    /// <summary>
    /// Gets or sets the Id of the branch.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the Member.
    /// </summary>
    public virtual Member Member { get; set; }

    /// <summary>
    /// Gets or sets the Member Branch.
    /// </summary>
    public virtual MemberBranch MemberBranch { get; set; }

    /// <summary>
    /// Gets or sets the Application User.
    /// </summary>
    public virtual ApplicationUser ApplicationUser { get; set; }

    /// <summary>
    /// Gets or set the account status.
    /// </summary>
    public virtual bool Active { get; set; }

    /// <summary>
    /// Gets or sets the Branches.
    /// </summary>
    public virtual ICollection<MemberAccountModule>? Modules { get; set; }
  }
}
namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The Member User class.
  /// </summary>
  public class MemberUser
  {
    /// <summary>
    /// The unique identifier for the Member User.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the last name of the user associated with this member.
    /// </summary>
    public virtual string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name of the user associated with this member.
    /// </summary>
    public virtual string LastName { get; set; } = string.Empty;

    /// <summary>
    /// The Member Id foreign key.
    /// </summary>
    public virtual Member Member { get; set; } = new();

    /// <summary>
    /// The User Id foreign key.
    /// </summary>
    public virtual WimsApplicationUser User { get; set; } = new();

    /// <summary>
    /// Can the user edit this member's information?
    /// </summary>
    public virtual bool CanEdit { get; set; }

    /// <summary>
    /// Gets or sets the member user branches
    /// </summary>
    public virtual IList<MemberUserBranch> Branches { get; set; } = new List<MemberUserBranch>();
  }
}

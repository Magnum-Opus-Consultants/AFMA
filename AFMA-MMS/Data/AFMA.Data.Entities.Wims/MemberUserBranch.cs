namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The Member User Branch class.
  /// </summary>
  public class MemberUserBranch
  {
    /// <summary>
    /// The unique identifier for the Member User Branch.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// The Member User Id foreign key.
    /// </summary>
    public virtual MemberUser MemberUser { get; set; } = new();

    /// <summary>
    /// The Member Branch Id foreign key.
    /// </summary>
    public virtual MemberBranch MemberBranch { get; set; } = new();

    /// <summary>
    /// Can the user edit this member branch's information?
    /// </summary>
    public virtual bool CanEdit { get; set; }
  }
}

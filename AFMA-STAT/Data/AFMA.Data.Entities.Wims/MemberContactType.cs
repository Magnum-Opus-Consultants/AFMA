namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The MemberContactType class represents a type of contact for a member in the WIMS application.
  /// </summary>
  public class MemberContactType
  {
    /// <summary>
    /// The unique identifier for the MemberContactType.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// The name of the MemberContactType.
    /// </summary>
    public virtual string? Description { get; set; }
  }
}

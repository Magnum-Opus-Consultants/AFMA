namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The MemberType class represents a type of member in the WIMS system.
  /// </summary>
  public class MemberType
  {
    /// <summary>
    /// The unique identifier for the MemberType.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// The name of the MemberType.
    /// </summary>
    public virtual string? Description { get; set; }
  }
}

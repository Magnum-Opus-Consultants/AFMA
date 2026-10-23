namespace AFMA.ApplicationLayer.Model.Wims
{
  /// <summary>
  /// The MemberTypeModel class represents a member type in the WIMS application.
  /// </summary>
  [MapTo(typeof(MemberType))]
  public class MemberTypeModel
  {
    /// <summary>
    /// The unique identifier for the MemberType.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the MemberType.
    /// </summary>
    public string? Description { get; set; }
  }
}

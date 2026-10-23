namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The MemberContactTypeModel class represents a type of contact for a member in the WIMS application.
  /// </summary>
  [MapTo(typeof(MemberContactType))]
  public class MemberContactTypeModel
  {
    /// <summary>
    /// The unique identifier for the MemberContactType.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the MemberContactType.
    /// </summary>
    public string? Description { get; set; }
  }
}

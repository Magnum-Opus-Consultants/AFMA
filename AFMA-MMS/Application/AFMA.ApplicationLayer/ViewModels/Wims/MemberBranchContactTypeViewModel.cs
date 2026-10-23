namespace AFMA.ApplicationLayer.ViewModels.Wims
{
  using AFMA.ApplicationLayer.Model.Wims;
  using System.Collections.Generic;

  /// <summary>
  /// The MemberBranchContactTypeViewModel class represents a type of contact for a member branch in the WIMS application.
  /// </summary>
  public class MemberBranchContactTypeViewModel
  {
    /// <summary>
    /// Gets or sets the unique identifier for the MemberBranchContactType.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the contact type.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of member branch contacts associated with this contact type.
    /// </summary>
    public List<MemberBranchContactModel> MemberBranchContacts { get; set; } = new();
  }
}

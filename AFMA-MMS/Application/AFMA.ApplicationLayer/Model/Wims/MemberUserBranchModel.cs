namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The MemberUserBranchModel class represents a member user branch in the WIMS application.
  /// </summary>
  [MapTo(typeof(MemberUserBranch))]
  public class MemberUserBranchModel
  {
    /// <summary>
    /// Gets or sets the unique identifier for the MemberUserBranch.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Member User Id foreign key.
    /// </summary>
    public int MemberUserId { get; set; }

    /// <summary>
    /// Gets or sets the name of the user associated with this member branch.
    /// </summary>
    public string MemberUser { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Member Branch Id foreign key.
    /// </summary>
    public int MemberBranchId { get; set; }

    /// <summary>
    /// Gets or sets the name of the member branch associated with this member user.
    /// </summary>
    public string MemberBranch { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the user can edit this member branch's information.
    /// </summary>
    public bool CanEdit { get; set; }
  }
}

namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The MemberAccountModuleModel class.
  /// </summary>
  public class MemberAccountModuleModel
  {
    /// <summary>
    /// Gets or sets the Id of the member account module.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Id of the member.
    /// </summary>
    public int MemberAccountId { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the member.
    /// </summary>
    public string? MemberAccount { get; set; }

    /// <summary>
    /// Gets or sets the Id of the module.
    /// </summary>
    public int ModuleId { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the module.
    /// </summary>
    public string? Module { get; set; }

    /// <summary>
    /// Gets or sets the if the module is selected
    /// </summary>
    public bool Selected { get; set; }
  }
}

namespace AFMA.Data.Entities
{
  /// <summary>
  /// The MemberModule class.
  /// </summary>
  public class MemberModule
  {
    /// <summary>
    /// Gets or sets the Id.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the Member.
    /// </summary>
    public virtual Member? Member { get; set; }

    /// <summary>
    /// Gets or sets the Module.
    /// </summary>
    public virtual Module? Module { get; set; }
  }
}

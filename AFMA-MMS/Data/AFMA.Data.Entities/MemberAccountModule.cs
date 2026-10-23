namespace AFMA.Data.Entities
{
  public class MemberAccountModule
  {
    /// <summary>
    /// Gets or sets the Id.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the Member Account.
    /// </summary>
    public virtual MemberAccount MemberAccount { get; set; }

    /// <summary>
    /// Gets or sets the Module.
    /// </summary>
    public virtual Module Module { get; set; }
  }
}

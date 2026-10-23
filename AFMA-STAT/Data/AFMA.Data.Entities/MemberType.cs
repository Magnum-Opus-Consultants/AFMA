namespace AFMA.Data.Entities
{
  /// <summary>
  /// The MemberType entity class.
  /// </summary>
  public class MemberType
  {
    /// <summary>
    /// Gets or sets the Id of the member type.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public virtual string Description { get; set; }

    /// <summary>
    /// Gets or sets the deletable indicator.
    /// </summary>
    public virtual bool Deletable { get; set; }
  }
}

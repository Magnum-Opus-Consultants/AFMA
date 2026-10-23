namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The Document Type entity class.
  /// </summary>
  public class DocumentType
  {
    /// <summary>
    /// Gets or sets the unique identifier for the Document Type.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the Document Type.
    /// </summary>
    public virtual string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the Document Type.
    /// </summary>
    public virtual bool RequireTemplate { get; set; }

    /// <summary>
    /// Gets or sets the name of the Document Template.
    /// </summary>
    public virtual DocumentTemplate DocumentTemplate { get; set; } = new();
  }
}

namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The committee document entity.
  /// </summary>
  public class CommitteeDocument
  {
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the document type.
    /// </summary>
    public virtual DocumentType DocumentType { get; set; } = new();

    /// <summary>
    /// Gets or sets the committee.
    /// </summary>
    public virtual Committee Committee { get; set; } = new();

    /// <summary>
    /// Gets or sets the name of the document.
    /// </summary>
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the document.
    /// </summary>
    public virtual string? Description { get; set; }

    /// <summary>
    /// Gets or sets the document content.
    /// </summary>
    public virtual byte[] DocumentContent { get; set; } = [];

    /// <summary>
    /// Gets or sets the document content type.
    /// </summary>
    public virtual string DocumentContentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the document file name.
    /// </summary>
    public virtual string DocumentFileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the document file size.
    /// </summary>
    public virtual int DocumentFileSize { get; set; }
  }
}

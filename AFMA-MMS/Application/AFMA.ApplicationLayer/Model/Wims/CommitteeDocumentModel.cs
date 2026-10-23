namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.ApplicationLayer.Attributes;
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The CommitteeDocument class represents a document associated with a committee.
  /// </summary>
  [MapTo(typeof(CommitteeDocument))]
  public class CommitteeDocumentModel
  {
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the document type.
    /// </summary>
    [Display(Name = "Document Type")]
    public int DocumentTypeId { get; set; }

    /// <summary>
    /// Gets or sets the document type.
    /// </summary>
    [Display(Name = "Document Type")]
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the committee.
    /// </summary>
    public int CommitteeId { get; set; }

    /// <summary>
    /// Gets or sets the committee.
    /// </summary>
    public string Committee { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the document.
    /// </summary>
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the document.
    /// </summary>
    [Display(Name = "Description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the document content.
    /// </summary>
    [Display(Name = "Document Content")]
    [MaxFileSize(15728640)]
    public byte[]? DocumentContent { get; set; } = [];

    /// <summary>
    /// Gets or sets the document content type.
    /// </summary>
    [Display(Name = "Document Format")]
    public string DocumentContentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the document file name.
    /// </summary>
    [Display(Name = "File Name")]
    public string DocumentFileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the document file size.
    /// </summary>
    [Display(Name = "File Size")]
    public int DocumentFileSize { get; set; }
  }
}

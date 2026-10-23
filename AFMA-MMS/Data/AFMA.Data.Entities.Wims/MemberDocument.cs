namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The Member Document entity class.
  /// </summary>
  public class MemberDocument
  {
    /// <summary>
    /// Gets or sets the unique identifier for the Member Document.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the Member Document.
    /// </summary>
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Document Type.
    /// </summary>
    public virtual DocumentType DocumentType { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Member.
    /// </summary>
    public virtual Member Member { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether the document is visible to the member.
    /// </summary>
    public virtual bool Visible { get; set; }

    /// <summary>
    /// Gets or sets the expiry date of the document that was uploaded.
    /// </summary>
    public virtual DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// Gets or sets the date when the last notification was sent to the member regarding this document.
    /// </summary>
    public virtual DateTime? LastNotificationSent { get; set; }

    /// <summary>
    /// Gets or sets the content of the document that was uploaded.
    /// </summary>
    public virtual byte[] DocumentContent { get; set; } = [];

    /// <summary>
    /// Gets or sets the content type of the document that was uploaded.
    /// </summary>
    public virtual string DocumentContentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file name of the document that was uploaded.
    /// </summary>
    public virtual string DocumentFileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file size of the document that was uploaded.
    /// </summary>
    public virtual int DocumentFileSize { get; set; }
  }
}

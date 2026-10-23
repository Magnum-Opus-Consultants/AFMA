namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The BatchRecipientAttachment class represents an attachment associated with a batch recipient, including details about the attachment file name, content type, and size.
  /// </summary>
  public class BatchAttachment
  {
    /// <summary>
    /// Gets or sets the unique identifier for the batch recipient attachment.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the batch associated with this attachment.
    /// </summary>
    public virtual Batch Batch { get; set; } = new Batch();

    /// <summary>
    /// Gets or sets the file name of the attachment.
    /// </summary>
    public virtual string AttachmentFileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content type of the attachment, indicating the type of file (e.g., image/jpeg, application/pdf).
    /// </summary>
    public virtual string AttachmentContentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the binary data of the attachment.
    /// </summary>
    public virtual byte[] Attachment { get; set; } = [];

    /// <summary>
    /// Gets or sets the size of the attachment file in bytes.
    /// </summary>
    public virtual int AttachmentFileSize { get; set; }
  }
}

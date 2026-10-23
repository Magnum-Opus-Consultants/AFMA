namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;

  [MapTo(typeof(BatchAttachment))]
  public class BatchAttachmentModel
  {
    /// <summary>
    /// Gets or sets the unique identifier for the batch recipient attachment.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the batch Id associated with this attachment.
    /// </summary>
    public int BatchId { get; set; }

    /// <summary>
    /// Gets or sets the batch name associated with this attachment.
    /// </summary>
    public string Batch { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file name of the attachment.
    /// </summary>
    [Display(Name = "File Name")]
    public string AttachmentFileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content type of the attachment, indicating the type of file (e.g., image/jpeg, application/pdf).
    /// </summary>
    [Display(Name = "File Type")]
    public string AttachmentContentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the binary data of the attachment.
    /// </summary>
    public byte[] Attachment { get; set; } = [];

    /// <summary>
    /// Gets or sets the size of the attachment file in bytes.
    /// </summary>
    [Display(Name = "File Size")]
    public int AttachmentFileSize { get; set; }
  }
}

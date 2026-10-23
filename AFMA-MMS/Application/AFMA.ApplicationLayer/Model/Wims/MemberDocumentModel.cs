namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.ApplicationLayer.Attributes;
  using AFMA.Data.Entities.Wims;
  using System;

  [MapTo(typeof(MemberDocument))]
  public class MemberDocumentModel
  {
    /// <summary>
    /// Gets or sets the unique identifier for the Member Document.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the Member Document.
    /// </summary>
    [Display(Name = "Name")]
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot be more than 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Document Type.
    /// </summary>
    [Display(Name = "Document Type")]
    [Required(ErrorMessage = "Document Type is required")]
    public int DocumentTypeId { get; set; }

    /// <summary>
    /// Gets or sets the Document Type.
    /// </summary>
    [Display(Name = "Document Type")]
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Member.
    /// </summary>
    [Display(Name = "Member")]
    [Required(ErrorMessage = "Member is required")]
    public int MemberId { get; set; }

    /// <summary>
    /// Gets or sets the Member.
    /// </summary>
    [Display(Name = "Member")]
    public string Member { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the document is visible to the member.
    /// </summary>
    [Display(Name = "Visible")]
    public bool Visible { get; set; }

    /// <summary>
    /// Gets or sets the expiry date of the document that was uploaded.
    /// </summary>
    [Display(Name = "Expiry Date")]
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// Gets or sets the date when the last notification was sent to the member regarding this document.
    /// </summary>
    [Display(Name = "Last Notification Sent")]
    public DateTime? LastNotificationSent { get; set; }

    /// <summary>
    /// Gets or sets the content of the document that was uploaded.
    /// </summary>
    [Display(Name = "Document")]
    [MaxFileSize(15728640)]
    public byte[]? DocumentContent { get; set; } = [];

    /// <summary>
    /// Gets or sets the content type of the document that was uploaded.
    /// </summary>
    [Display(Name = "Content Type")]
    public string DocumentContentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file name of the document that was uploaded.
    /// </summary>
    [Display(Name = "File Name")]
    public string DocumentFileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file size of the document that was uploaded.
    /// </summary>
    [Display(Name = "File Size")]
    public int DocumentFileSize { get; set; }
  }
}

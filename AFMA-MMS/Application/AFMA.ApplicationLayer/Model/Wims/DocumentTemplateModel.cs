namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.ApplicationLayer.Attributes;
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The Document Template model class.
  /// </summary>
  [MapTo(typeof(DocumentTemplate))]
  public class DocumentTemplateModel
  {
    /// <summary>
    /// Gets or sets the unique identifier for the Document Template.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the Document Template.
    /// </summary>
    [Display(Name = "Name")]
    [Required(ErrorMessage = "Name is required")]
    [StringLength(255, ErrorMessage = "Description cannot be longer than 255 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the Document Template.
    /// </summary>
    [Display(Name = "Template")]
    [MaxFileSize(15728640)]
    public byte[]? TemplateContent { get; set; }

    /// <summary>
    /// Gets or sets the name of the Document Content Type.
    /// </summary>
    [Display(Name = "Document Type")]
    public string TemplateContentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the Document File Name.
    /// </summary>
    [Display(Name = "File Name")]
    public string TemplateFileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the Document Template File Size.
    /// </summary>
    [Display(Name = "File Size (bytes)")]
    public int TemplateFileSize { get; set; }
  }
}

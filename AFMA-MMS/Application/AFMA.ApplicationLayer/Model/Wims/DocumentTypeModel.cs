namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The Document Type model class.
  /// </summary>
  [MapTo(typeof(DocumentType))]
  public class DocumentTypeModel
  {
    /// <summary>
    /// Gets or sets the unique identifier for the Document Type.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the Document Type.
    /// </summary>
    [Display(Name = "Description")]
    [Required(ErrorMessage = "Description is required")]
    [StringLength(255, ErrorMessage = "Description cannot be longer than 255 characters")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the Document Type.
    /// </summary>
    [Display(Name = "Requires Template")]
    public bool RequireTemplate { get; set; }

    /// <summary>
    /// Gets or sets the name of the Document Template.
    /// </summary>
    [Display(Name = "Document Template")]
    public int? DocumentTemplateId { get; set; }

    /// <summary>
    /// Gets or sets the name of the Document Template.
    /// </summary>
    [Display(Name = "Document Template")]
    public string DocumentTemplate { get; set; } = string.Empty;
  }
}

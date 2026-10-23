namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The Document Template entity class.
  /// </summary>
  public class DocumentTemplate
  {
    /// <summary>
    /// Gets or sets the unique identifier for the Document Template.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the Document Template.
    /// </summary>
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the Document Template.
    /// </summary>
    public virtual byte[] TemplateContent { get; set; } = [];

    /// <summary>
    /// Gets or sets the name of the Document Content Type.
    /// </summary>
    public virtual string TemplateContentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the Document File Name.
    /// </summary>
    public virtual string TemplateFileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the Document Template File Size.
    /// </summary>
    public virtual int TemplateFileSize { get; set; }
  }
}

namespace AFMA.ApplicationLayer.Model
{
  using System.ComponentModel.DataAnnotations;

  /// <summary>
  /// The SalmonellaSampleTypeItemModel Class.
  /// </summary>
  [MapTo(typeof(SalmonellaSampleTypeItem))]
  public class SalmonellaSampleTypeItemModel
  {
    /// <summary>
    /// Gets or sets the Id of the Salmonella Sample Type Item
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the Salmonella Sample Type Item
    /// </summary>
    [Display(Name = "Name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the Salmonella Sample Type
    /// </summary>
    [Display(Name = "Sample Type")]
    public int SalmonellaSampleTypeId { get; set; }

    /// <summary>
    /// Gets or sets the Salmonella Sample Type
    /// </summary>
    [Display(Name = "Sample Type")]
    public string SalmonellaSampleType { get; set; }

    /// <summary>
    /// Gets or sets the short code of the Salmonella Sample Type Item
    /// </summary>
    [Display(Name = "Short Code")]
    public string ShortCode { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    [Display(Name = "Display Order")]
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the visible to members status
    /// </summary>
    public bool VisibleToMembers { get; set; }

    /// <summary>
    /// Gets or sets the status of the Salmonella Sample Type Item
    /// </summary>
    [Display(Name = "Active")]
    public bool Active { get; set; }
  }
}

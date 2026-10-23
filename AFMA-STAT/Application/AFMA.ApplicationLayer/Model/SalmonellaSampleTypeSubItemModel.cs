namespace AFMA.ApplicationLayer.Model
{
  using System.ComponentModel.DataAnnotations;

  /// <summary>
  /// The SalmonellaSampleTypeSubItemModel Class.
  /// </summary>
  [MapTo(typeof(SalmonellaSampleTypeSubItem))]
  public class SalmonellaSampleTypeSubItemModel
  {
    /// <summary>
    /// Gets or sets the Id of the Salmonella Sample Type Sub Item
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the Salmonella Sample Type Sub Item
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
    /// Gets or sets the short code of the Salmonella Sample Type Sub Item
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
    [Display(Name = "Visible To Members")]
    public bool VisibleToMembers { get; set; }

    /// <summary>
    /// Gets or sets the status of the Salmonella Sample Type Sub Item
    /// </summary>
    [Display(Name = "Active")]
    public bool Active { get; set; }
  }
}

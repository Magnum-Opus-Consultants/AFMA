namespace AFMA.ApplicationLayer.Model
{
  using System.ComponentModel.DataAnnotations;

  /// <summary>
  /// The SalmonellaGroupModel class.
  /// </summary>
  [MapTo(typeof(SalmonellaGroup))]
  public class SalmonellaGroupModel
  {
    /// <summary>
    /// Gets or sets the Id of the Salmonella Group.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the Salmonella Group.
    /// </summary>
    [Display(Name = "Name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the name of the Salmonella Group.
    /// </summary>
    [Display(Name = "Display Order")]
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the status of the Salmonella Group.
    /// </summary>
    [Display(Name = "Active")]
    public bool Active { get; set; }
  }
}

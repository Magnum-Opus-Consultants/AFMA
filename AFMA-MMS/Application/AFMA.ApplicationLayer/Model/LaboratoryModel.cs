namespace AFMA.ApplicationLayer.Model
{
  using System.ComponentModel.DataAnnotations;

  /// <summary>
  /// The LaboratoryModel class.
  /// </summary>
  [MapTo(typeof(Laboratory))]
  public class LaboratoryModel
  {
    /// <summary>
    /// Gets or sets the Id of the Laboratory.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the Laboratory.
    /// </summary>
    [Display(Name = "Name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the address of the Laboratory.
    /// </summary>
    [Display(Name = "Address")]
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the status of the Laboratory.
    /// </summary>
    [Display(Name = "Active")]
    public bool Active { get; set; }
  }
}

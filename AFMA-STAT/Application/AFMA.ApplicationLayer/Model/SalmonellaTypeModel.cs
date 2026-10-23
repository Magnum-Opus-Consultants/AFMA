namespace AFMA.ApplicationLayer.Model
{
  using System.ComponentModel.DataAnnotations;

  /// <summary>
  /// The SalmonellaTypeModel class.
  /// </summary>
  [MapTo(typeof(SalmonellaType))]
  public class SalmonellaTypeModel
  {
    /// <summary>
    /// Gets or sets the Id of the Salmonella Type.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the Salmonella Type.
    /// </summary>
    [Display(Name = "Name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the Salmonella Group Id of the Salmonella Type.
    /// </summary>
    [Display(Name = "Salmonella Group")]
    public int? SalmonellaGroupId { get; set; }

    /// <summary>
    /// Gets or sets the Salmonella Group of the Salmonella Type.
    /// </summary>
    [Display(Name = "Salmonella Group")]
    public string? SalmonellaGroup { get; set; }

    /// <summary>
    /// Gets or sets the name of the Salmonella Type.
    /// </summary>
    [Display(Name = "Display Order")]
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the status of the Salmonella Type.
    /// </summary>
    [Display(Name = "Active")]
    public bool Active { get; set; }
  }
}

namespace AFMA.ApplicationLayer.Model
{
  using System.ComponentModel.DataAnnotations;

  [MapTo(typeof(Province))]
  public class ProvinceModel
  {
    /// <summary>
    /// Gets or sets the Id for the member type.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the description for this member type.
    /// </summary>
    [Required(ErrorMessage = "Description is required"), MaxLength(100, ErrorMessage = "Description should be 100 characters or less.")]
    public string? Description { get; set; }
  }
}

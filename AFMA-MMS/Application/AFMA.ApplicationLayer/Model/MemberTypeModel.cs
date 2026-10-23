namespace AFMA.ApplicationLayer.Model
{
  using System.ComponentModel.DataAnnotations;

  /// <summary>
  /// The MemberType class.
  /// </summary>
  [MapTo(typeof(MemberType))]
  public class MemberTypeModel
  {
    /// <summary>
    /// Gets or sets the Id for the member type.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the description for this member type.
    /// </summary>
    [Required(ErrorMessage = "Description is required"), MaxLength(50, ErrorMessage = "Description should be 50 characters or less.")]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the deletable indicator.
    /// </summary>
    public bool Deletable { get; set; }
  }
}
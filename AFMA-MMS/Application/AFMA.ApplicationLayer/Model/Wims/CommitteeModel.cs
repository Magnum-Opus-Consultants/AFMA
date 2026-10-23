namespace AFMA.ApplicationLayer.Model.Wims
{
  using System.Collections.Generic;
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The Committee class represents a committee within the system.
  /// </summary>
  [MapTo(typeof(Committee))]
  public class CommitteeModel
  {
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the committee name.
    /// </summary>
    [Display(Name = "Committee Name")]
    [Required(ErrorMessage = "Committee Name is required")]
    [StringLength(100, ErrorMessage = "Committee Name exceed 255 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the committee members
    /// </summary>
    public ICollection<MemberCommitteeModel> Members { get; set; } = new List<MemberCommitteeModel>();

    /// <summary>
    /// Gets or sets the committee documents.
    /// </summary>
    public ICollection<CommitteeDocumentModel> Documents { get; set; } = new List<CommitteeDocumentModel>();
  }
}

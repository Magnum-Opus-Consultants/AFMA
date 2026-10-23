namespace AFMA.ApplicationLayer.Model.Wims
{
  using System.Collections.Generic;
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The DistributionGroup class represents a group of members for distribution purposes.
  /// </summary>
  [MapTo(typeof(DistributionGroup))]
  public class DistributionGroupModel
  {
    /// <summary>
    /// Gets or sets the unique identifier for the distribution group.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the distribution group.
    /// </summary>
    [Display(Name = "Group Name")]
    [Required(ErrorMessage = "Group Name is required")]
    [StringLength(150, ErrorMessage = "Group Name cannot exceed 150 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the distribution group.
    /// </summary>
    [Display(Name = "Group Description")]
    [Required(ErrorMessage = "Group Description is required")]
    [StringLength(255, ErrorMessage = "Group Description cannot exceed 255 characters")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the owner of the distribution group.
    /// </summary>
    [Display(Name = "Owner")]
    [Required(ErrorMessage = "Owner is required")]
    [StringLength(150, ErrorMessage = "Owner cannot exceed 150 characters")]
    public string Owner { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address from which the distribution group sends emails.
    /// </summary>
    [Display(Name = "From Address")]
    [Required(ErrorMessage = "From Address is required")]
    [RegularExpression("[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}", ErrorMessage = "Please enter a valid email address")]
    [StringLength(150, ErrorMessage = "From Address cannot exceed 150 characters")]
    public string FromAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the privacy status of the distribution group.
    /// </summary>
    [Display(Name = "Private")]
    public bool Private { get; set; }

    /// <summary>
    /// Gets or sets the collection of members in the distribution group.
    /// </summary>
    public ICollection<DistributionGroupMemberModel> DistributionGroupMembers { get; set; } = [];
  }
}

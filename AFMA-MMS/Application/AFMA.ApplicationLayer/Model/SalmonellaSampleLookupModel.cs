namespace AFMA.ApplicationLayer.Model
{
  using System.ComponentModel.DataAnnotations;

  /// <summary>
  /// The SalmonellaSampleModel class
  /// </summary>
  public class SalmonellaSampleLookupModel
  {
    /// <summary>
    /// Gets or sets the Id of the sample
    /// </summary>
    [Display(Name = "Sample Id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the salmonella sample type
    /// </summary>
    [Display(Name = "Sample Type")]
    public string SalmonellaSampleType { get; set; }

    /// <summary>
    /// Gets or sets the additional info
    /// </summary>
    [Display(Name = "Additional Info")]
    public string? AdditionalInfo { get; set; }

    /// <summary>
    /// Gets or sets the note
    /// </summary>
    [Display(Name = "Note")]
    public string? Note { get; set; }

    /// <summary>
    /// Gets or sets the main location
    /// </summary>
    [Display(Name = "Main Location")]
    public string? SalmonellaSampleTypeItem { get; set; }

    /// <summary>
    /// Gets or sets the sub location
    /// </summary>
    [Display(Name = "Sub Location")]
    public string? SalmonellaSampleTypeSubItem { get; set; }

    /// <summary>
    /// Gets or sets the salmonella group
    /// </summary>
    [Display(Name = "Salmonella Group")]
    public virtual string? SalmonellaGroup { get; set; }

    /// <summary>
    /// Gets or sets the salmonella type
    /// </summary>
    [Display(Name = "Salmonella Type")]
    public string? SalmonellaType { get; set; }
  }
}

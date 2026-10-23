namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The MycotoxinSampleModel class.
  /// </summary>
  [MapTo(typeof(MycotoxinSample))]
  public class MycotoxinSampleModel
  {
    /// <summary>
    /// Gets or sets the Id of the sample.
    /// </summary>
    [Display(Name = "Sample Id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Myctoxin report.
    /// </summary>
    [Display(Name = "Report No")]
    public int MycotoxinReportId { get; set; }

    /// <summary>
    /// Gets or sets the Mycotoxin sample type.
    /// </summary>
    [Display(Name = "Sample Type")] 
    public int MycotoxinSampleTypeId { get; set; }

    /// <summary>
    /// Gets or sets the Mycotoxin sample type.
    /// </summary>
    [Display(Name = "Sample Type")]
    public string MycotoxinSampleType { get; set; }

    /// <summary>
    /// Gets or sets the finished product.
    /// </summary>
    [Display(Name = "Finished Product")]
    public int? ProductId { get; set; }

    /// <summary>
    /// Gets or sets the finished product.
    /// </summary>
    [Display(Name = "Finished Product")]
    public string? Product { get; set; }

    /// <summary>
    /// Gets or sets the raw material.
    /// </summary>
    [Display(Name = "Raw Material")]
    public int? MaterialId { get; set; }

    /// <summary>
    /// Gets or sets the raw material.
    /// </summary>
    [Display(Name = "Raw Material")]
    public string? Material { get; set; }

    /// <summary>
    /// Gets or sets the raw material origin.
    /// </summary>
    [Display(Name = "Raw Material Origin")]
    public int? MaterialOriginId { get; set; }

    /// <summary>
    /// Gets or sets the raw material origin.
    /// </summary>
    [Display(Name = "Raw Material Origin")]
    public string? MaterialOrigin { get; set; }

    /// <summary>
    /// Gets or sets the origin province of the raw material.
    /// </summary>
    [Display(Name = "Local Raw Material Orogin")]
    public int? ProvinceId { get; set; }

    /// <summary>
    /// Gets or sets the origin province of the raw material.
    /// </summary>
    [Display(Name = "Local Raw Material Origin")]
    public string? Province { get; set; }

    /// <summary>
    /// Gets or sets the raw material country of origin.
    /// </summary>
    [Display(Name = "Imported Raw Material Origin")]
    public string? MaterialCountryOfOrigin { get; set; }

    /// <summary>
    /// Gets or sets the notes.
    /// </summary>
    [Display(Name = "Notes")]
    public string? Note { get; set; }

    /// <summary>
    /// Gets or sets the analysis method.
    /// </summary>
    [Display(Name = "Analysis method")]
    public int MycotoxinAnalysisId { get; set; }

    /// <summary>
    /// Gets or sets the analysis method.
    /// </summary>
    [Display(Name = "Analysis Method")]
    public string MycotoxinAnalysis { get; set; }

    /// <summary>
    /// Gets or sets the analysis if not specified.
    /// </summary>
    [Display(Name = "Other Analysis Method")]
    public string? MycotoxinAnalysisOther { get; set; }

    /// <summary>
    /// Gets or sets the analysis if not specified.
    /// </summary>
    [Display(Name = "Mycotoxins Detected")]
    public bool MycotoxinDetected { get; set; }

    /// <summary>
    /// Gets or sets the member name.
    /// </summary>
    [Display(Name = "Member")]
    public string? Member { get; set; }

    /// <summary>
    /// Gets or sets the member branch.
    /// </summary>
    [Display(Name = "Branch")]
    public string? MemberBranch { get; set; }

    /// <summary>
    /// Gets or sets the submitted status.
    /// </summary>
    [Display(Name = "Submitted")]
    public bool IsSubmitted { get; set; }

    [Display(Name = "Detections")]
    public ICollection<MycotoxinSampleDetectionModel> Detections { get; set; }
  }
}

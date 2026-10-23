namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The MycotoxinSampleDetectionModel class.
  /// </summary>
  [MapTo(typeof(MycotoxinSampleDetection))]
  public class MycotoxinSampleDetectionModel
  {
    /// <summary>
    /// Gets or sets the Id of the sample.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Mycotoxin sample.
    /// </summary>
    [Display(Name = "Sample")]
    public int MycotoxinSampleId { get; set; }

    /// <summary>
    /// Gets or sets the Mycotoxin report no.
    /// </summary>
    [Display(Name = "Report No")]
    public int MycotoxinReportId { get; set; }

    /// <summary>
    /// Gets or sets the Mycotoxin class.
    /// </summary>
    [Display(Name = "Mycotoxin Class")]
    public int MycotoxinClassId { get; set; }

    /// <summary>
    /// Gets or sets the Mycotoxin class.
    /// </summary>
    [Display(Name = "Mycotoxin Class")]
    public string MycotoxinClass { get; set; }

    /// <summary>
    /// Gets or sets the Mycotoxin type.
    /// </summary>
    [Display(Name = "Mycotoxin Type")]
    public int MycotoxinTypeId { get; set; }

    /// <summary>
    /// Gets or sets the Mycotoxin type.
    /// </summary>
    [Display(Name = "Mycotoxin Type")]
    public string MycotoxinType { get; set; }

    /// <summary>
    /// Gets or sets the concentration.
    /// </summary>
    [Display(Name = "Concentration")]
    public string Concentration { get; set; }

    /// <summary>
    /// Gets or sets the LOD.
    /// </summary>
    [Display(Name = "LOD")]
    public string Lod { get; set; }
  }
}

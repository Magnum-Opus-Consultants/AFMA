namespace AFMA.Data.Entities
{
  public class MycotoxinSample
  { 
    /// <summary>
    /// Gets or sets the Id of the sample.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the Myctoxin report.
    /// </summary>
    public virtual MycotoxinReport MycotoxinReport { get; set; }

    /// <summary>
    /// Gets or sets the Mycotoxin sample type.
    /// </summary>
    public virtual MycotoxinSampleType MycotoxinSampleType { get; set; }

    /// <summary>
    /// Gets or sets the finished product.
    /// </summary>
    public virtual Product? Product { get; set; }

    /// <summary>
    /// Gets or sets the raw material.
    /// </summary>
    public virtual Material? Material { get; set; }

    /// <summary>
    /// Gets or sets the raw material origin.
    /// </summary>
    public virtual MaterialOrigin? MaterialOrigin { get; set; }

    /// <summary>
    /// Gets or sets the origin province of the raw material.
    /// </summary>
    public virtual Province? Province { get; set; }
    
    /// <summary>
    /// Gets or sets the raw material country of origin.
    /// </summary>
    public virtual string? MaterialCountryOfOrigin { get; set; }
    
    /// <summary>
    /// Gets or sets the notes.
    /// </summary>
    public virtual string? Note { get; set; }
    
    /// <summary>
    /// Gets or sets the analysis method.
    /// </summary>
    public virtual MycotoxinAnalysis MycotoxinAnalysis { get; set; }
    
    /// <summary>
    /// Gets or sets the analysis if not specified.
    /// </summary>
    public virtual string? MycotoxinAnalysisOther { get; set; }

    /// <summary>
    /// Gets or sets the detected indicator..
    /// </summary>
    public virtual bool MycotoxinDetected { get; set; }

    /// <summary>
    /// Gets or sets the detections.
    /// </summary>
    public virtual ICollection<MycotoxinSampleDetection> Detections { get; set; }
  }
}

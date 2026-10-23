namespace AFMA.Data.Entities
{
  /// <summary>
  /// The MycotoxinSampleDetection class.
  /// </summary>
  public class MycotoxinSampleDetection
  {
    /// <summary>
    /// Gets or sets the Id of the sample.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the Mycotoxin sample.
    /// </summary>
    public virtual MycotoxinSample MycotoxinSample { get; set; }

    /// <summary>
    /// Gets or sets the Mycotoxin class.
    /// </summary>
    public virtual MycotoxinClass MycotoxinClass { get; set; }
    
    /// <summary>
    /// Gets or sets the Mycotoxin type.
    /// </summary>
    public virtual MycotoxinType MycotoxinType { get; set; }

    /// <summary>
    /// Gets or sets the concentration.
    /// </summary>
    public virtual string Concentration { get; set; }
    
    /// <summary>
    /// Gers or sets the LOD.
    /// </summary>
    public virtual string Lod { get; set; }
  }
}

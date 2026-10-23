namespace AFMA.Data.Entities
{
  /// <summary>
  /// The SalmonellaSample class
  /// </summary>
  public class SalmonellaSample
  {
    /// <summary>
    /// Gets or sets the Id of the sample
    /// </summary>
    public virtual int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the salmonella report
    /// </summary>
    public virtual SalmonellaReport SalmonellaReport { get; set; }
    
    /// <summary>
    /// Gets or sets the salmonella sample type
    /// </summary>
    public virtual SalmonellaSampleType SalmonellaSampleType { get; set; }
    
    /// <summary>
    /// Gets ior sets the salmonella detected indicator
    /// </summary>
    public virtual bool SalmonellaDetected { get; set; }
    
    /// <summary>
    /// Gets or sets the product
    /// </summary>
    public virtual Product? Product { get; set; }
    
    /// <summary>
    /// Gets or sets the material
    /// </summary>
    public virtual Material? Material { get; set; }
    
    /// <summary>
    /// Gets or sets the additional info
    /// </summary>
    public virtual string? AdditionalInfo { get; set; }
    
    /// <summary>
    /// Gets or sets the note
    /// </summary>
    public virtual string? Note { get; set; }
    
    /// <summary>
    /// Gets or sets the main location
    /// </summary>
    public virtual SalmonellaSampleTypeItem? SalmonellaSampleTypeItem { get; set; }
    
    /// <summary>
    /// Gets or sets the sub location
    /// </summary>
    public virtual SalmonellaSampleTypeSubItem? SalmonellaSampleTypeSubItem { get; set; }
    
    /// <summary>
    /// Gets or sets the salmonella group
    /// </summary>
    public virtual SalmonellaGroup? SalmonellaGroup { get; set; }
    
    /// <summary>
    /// Gets or sets the salmonella type
    /// </summary>
    public virtual SalmonellaType? SalmonellaType { get; set; }
    
    /// <summary>
    /// Gets or sets the product group
    /// </summary>
    public virtual ProductGroup? ProductGroup { get; set; }
  }
}

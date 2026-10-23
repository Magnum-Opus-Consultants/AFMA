namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The Feedsales Report Item Lookup Model class
  /// </summary>
  public class FeedsalesReportItemLookupModel
  {
    /// <summary>
    /// Gets or sets the Id of the report item
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the report.
    /// </summary>
    public int? FeedsalesReportId { get; set; }

    /// <summary>
    /// Gets or sets the Id of the finished product
    /// </summary>
    public int? ProductId { get; set; }

    /// <summary>
    /// Gets or sets the finished product
    /// </summary>
    public string? Product { get; set; }

    /// <summary>
    /// Gets or sets the finished product
    /// </summary>
    public string? ProductCode { get; set; }

    /// <summary>
    /// Gets or sets the finished product
    /// </summary>
    public string? MaterialCode { get; set; }

    /// <summary>
    /// Gets or sets the raw material
    /// </summary>
    public int? ProductGroupId { get; set; }

    /// <summary>
    /// Gets or sets the product group
    /// </summary>
    public string ProductGroup { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the raw material
    /// </summary>
    public int? MaterialId { get; set; }

    /// <summary>
    /// Gets or sets the raw material
    /// </summary>
    public string? Material { get; set; }

    /// <summary>
    /// Gets or sets the submitted value
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Gets or sets the other description
    /// </summary>
    public string Other { get; set; }

    /// <summary>
    /// Gets or sets the report year
    /// </summary>
    public int ReportYear { get; set; }

    /// <summary>
    /// Gets or sets the report month
    /// </summary>
    public int ReportMonth { get; set; }

    /// <summary>
    /// Gets or sets the submitted
    /// </summary>
    public bool IsSubmitted { get; set; }
  }
}

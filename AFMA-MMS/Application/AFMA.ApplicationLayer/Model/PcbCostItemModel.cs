namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The PcbCostItemModel class.
  /// </summary>
  public class PcbCostItemModel
  {
    /// <summary>
    /// Gets or sets the Id of the item.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the item group.
    /// </summary>
    public int PcbCostItemGroupId { get; set; }

    /// <summary>
    /// Gets or sets the item group.
    /// </summary>
    public string? PcbCostItemGroup { get; set; }

    /// <summary>
    /// Gets or sets the main item group.
    /// </summary>
    public int PcbCostItemMainGroupId { get; set; }

    /// <summary>
    /// Gets or sets the item code.
    /// </summary>
    public string? ItemCode { get; set; }

    /// <summary>
    /// Gets or sets the item description.
    /// </summary>
    public string? ItemDescription { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Gets or sets the editable flag.
    /// </summary>
    public bool Editable { get; set; }

    /// <summary>
    /// Gets or sets the excel sheet name.
    /// </summary>
    public string? ExcelSheetName { get; set; }

    /// <summary>
    /// Gets or sets the excel sheet Id.
    /// </summary>
    public string? ExcelSheetId { get; set; }

    /// <summary>
    /// Gets or sets the excel row.
    /// </summary>
    public int ExcelRow { get; set; }

    /// <summary>
    /// Gets or sets the excel column.
    /// </summary>
    public int ExcelColumn { get; set; }

    /// <summary>
    /// Gets or sets the add to total flag.
    /// </summary>
    public bool AddToTotal { get; set; }

    /// <summary>
    /// Gets or sets the readonly flag.
    /// </summary>
    public bool ReadOnly { get; set; }
  }
}

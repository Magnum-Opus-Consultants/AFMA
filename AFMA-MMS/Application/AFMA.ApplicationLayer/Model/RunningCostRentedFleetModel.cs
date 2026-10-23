namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The RunningCostRentedFleetModel class.
  /// </summary>
  public class RunningCostRentedFleetModel
  {
    /// <summary>
    /// Gets or sets the Id of the rented fleet.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the item code.
    /// </summary>
    [Display(Name = "Item Code")]
    public string? ItemCode { get; set; }

    /// <summary>
    /// Gets or sets the item name.
    /// </summary>
    [Display(Name = "Item Name")]
    public string? ItemName { get; set; }

    /// <summary>
    /// Gets or sets the note.
    /// </summary>
    [Display(Name = "Note")]
    public string? Note { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    [Display(Name = "Active")]
    public bool Active { get; set; }

    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    [Display(Name = "Display Name")]
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the excel sheet name.
    /// </summary>
    [Display(Name = "Excel Sheet Name")]
    public string? ExcelSheetName { get; set; }

    /// <summary>
    /// Gets or sets the excel sheet Id.
    /// </summary>
    [Display(Name = "Excel Sheet Id")]
    public int ExcelSheetId { get; set; }

    /// <summary>
    /// Gets or sets the excel row.
    /// </summary>
    [Display(Name = "Excel Row")]
    public int ExcelRow { get; set; }

    /// <summary>
    /// Gets or sets the excel column.
    /// </summary>
    [Display(Name = "Excel Column")]
    public int ExcelColumn { get; set; }
  }
}

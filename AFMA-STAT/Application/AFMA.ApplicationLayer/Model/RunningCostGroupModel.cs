namespace AFMA.ApplicationLayer.Model
{
  public class RunningCostGroupModel
  {
    /// <summary>
    /// Gets or sets the Id of the group.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the group code.
    /// </summary>
    [Display(Name = "Group Code")]
    public string? GroupCode { get; set; }

    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    [Display(Name = "Group Name")]
    public string? GroupName { get; set; }

    /// <summary>
    /// Gets or sets the group total.
    /// </summary>
    [Display(Name = "Group Total")]
    public double GroupTotal { get; set; }

    /// <summary>
    /// Gets or sets the production total.
    /// </summary>
    [Display(Name = "Total Production")]
    public double TotalProduction { get; set; }

    /// <summary>
    /// Gets or sets the sales total.
    /// </summary>
    [Display(Name = "Total Sales")]
    public double TotalSales { get; set; }

    /// <summary>
    /// Gets or sets the administration total.
    /// </summary>
    [Display(Name = "Total Administration")]
    public double TotalAdministration { get; set; }

    /// <summary>
    /// Gets or sets the road transport total.
    /// </summary>
    [Display(Name = "Total Road Transport")]
    public double TotalRoadTransport { get; set; }

    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    [Display(Name = "Display Order")]
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    [Display(Name = "Status")]
    public bool Active { get; set; }

    /// <summary>
    /// Gets or sets the note.
    /// </summary>
    [Display(Name = "Note")]
    public string? Note { get; set; }

    /// <summary>
    /// Gets or sets the production enabled flag.
    /// </summary>
    [Display(Name = "Production Enabled")]
    public bool ProductionEnabled { get; set; }

    /// <summary>
    /// Gets or sets the sales enabled flag.
    /// </summary>
    [Display(Name = "Sales Enabled")]
    public bool SalesEnabled { get; set; }

    /// <summary>
    /// Gets or sets the administration enabled flag.
    /// </summary>
    [Display(Name = "Administration Enabled")]
    public bool AdministrationEnabled { get; set; }

    /// <summary>
    /// Gets or sets the road transport enabled flag.
    /// </summary>
    [Display(Name = "Road Transport Enabled")]
    public bool RoadTransportEnabled { get; set; }

    /// <summary>
    /// Gets or sets the group sub items flag. 
    /// </summary>
    [Display(Name = "Has Sub Items")]
    public bool GroupHasSubItems { get; set; }

    /// <summary>
    /// Gets or sets production excel column.
    /// </summary>
    [Display(Name = "Production Excel Column")]
    public int ExcelColumnProduction { get; set; }

    /// <summary>
    /// Gets or sets the administration excel column.
    /// </summary>
    [Display(Name = "Administration Excel Column")]
    public int ExcelColumnAdministration { get; set; }

    /// <summary>
    /// Gets or sets the sales excel column.
    /// </summary>
    [Display(Name = "Sales Excel Column")]
    public int ExcelColumnSales { get; set; }

    /// <summary>
    /// Gets or sets the road transport excel column.
    /// </summary>
    [Display(Name = "Road Transport Excel Column")]
    public int ExcelColumnRoadTransport { get; set; }

    /// <summary>
    /// Gets or sets excel sheet name.
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
    /// Gets or sets the calculation type.
    /// </summary>
    [Display(Name = "Calculation Type")]
    public int CalculationType { get; set; }
  }
}

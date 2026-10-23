namespace AFMA.ApplicationLayer.Model.Export
{
  public class ExcelWorkbookExportModel
  {
    public string FileName { get; set; } = string.Empty;

    public ICollection<ExcelWorksheetExportModel> Worksheets { get; set; } = [];
  }
}

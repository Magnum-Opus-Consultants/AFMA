namespace AFMA.ApplicationLayer.Model.Export
{
  public class ExcelWorksheetExportModel
  {
    public string Name { get; set; } = string.Empty;

    public object Data { get; set; } = null!;

    public ICollection<string> PropertyNames { get; set; } = [];
  }
}

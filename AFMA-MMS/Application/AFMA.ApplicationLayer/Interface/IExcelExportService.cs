namespace AFMA.ApplicationLayer.Interface
{
  using AFMA.ApplicationLayer.Model.Export;

  public interface IExcelExportService
  {
    ExportFileModel CreateWorkbook(ExcelWorkbookExportModel workbookExportModel);
  }
}

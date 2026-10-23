namespace AFMA.ApplicationLayer.Model.Export
{
  public class ExportFileModel
  {
    public byte[] Content { get; set; } = [];

    public string ContentType { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;
  }
}

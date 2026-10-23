namespace AFMA.Stats.Admin.Models
{
  public class ErrorViewModel
  {
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    public string? ErrorTitle { get; set; } 
    public string? ExceptionMessage { get; set; }
    public string? InnerExceptionMessage { get; set; }
    public string? Path { get; set; }
    public string? Referrer { get; set; }
  }
}
namespace AFMA.ApplicationLayer.Interface
{
  public interface INotificationService
  {
    Task<bool> SendResetPasswordNotification(string userId);
    Task<bool> SendChangePasswordNotification(string userId, string newPassword);
    Task<bool> SendFeedsalesSubmissionReminder(int reportId);
    Task<bool> SendSalmonellaSubmissionReminder(int reportId);
    Task<bool> SendGenericEmail(string recipientEmail, string subject, string body);
  }
}

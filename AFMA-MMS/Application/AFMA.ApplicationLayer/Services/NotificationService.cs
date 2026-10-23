namespace AFMA.ApplicationLayer.Services
{
  using AFMA.Infrastructure.Settings;
  using AFMA.Shared.Infrastructure.Settings;
  using Alpha.Services.Notifications.Smtp;
  using Alpha.Services.Notifications.Templates;
  using Helpers;
  using Interface;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.Options;
  using Razor.Templating.Core;
  using Templating.Models;

  public class NotificationService : INotificationService
  {
    private readonly IAlphaNotificationTemplateService _notificationTemplateService;
    private readonly IRazorTemplateEngine _razorTemplateEngine;
    private readonly IEmailClient _emailClient;
    private readonly ApplicationSettings _applicationSettings;
    private readonly SmtpSettings _smtpSettings;
    private readonly IIdentityLayer _identityLayer;
    private readonly IApplicationLayer _applicationLayer;
    //TODO: Need to remove salmonella from this generic service
    private readonly ISalmonellaLayer _salmonellaLayer;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
      IEmailClient emailClient,
      IAlphaNotificationTemplateService notificationTemplateService,
      IRazorTemplateEngine razorTemplateEngine,
      IOptions<ApplicationSettings> applicationSettings,
      IOptions<SmtpSettings> smtpSettings,
      IIdentityLayer identityLayer,
      IApplicationLayer applicationLayer,
      ISalmonellaLayer salmonellaLayer,
      ILogger<NotificationService> logger)
    {
      this._emailClient = emailClient;
      this._notificationTemplateService = notificationTemplateService;
      this._razorTemplateEngine = razorTemplateEngine;
      this._applicationSettings = applicationSettings.Value;
      this._smtpSettings = smtpSettings.Value;
      this._identityLayer = identityLayer;
      this._applicationLayer = applicationLayer;
      this._salmonellaLayer = salmonellaLayer;
      this._logger = logger;
    }

    public async Task<bool> SendResetPasswordNotification(string userId)
    {
      _logger.LogInformation("SendResetPasswordNotification");

      try
      {
        var user = await _identityLayer.Identity.UserManager.FindByIdAsync(userId).ConfigureAwait(false);

        if (user == null)
        {
          this._logger.LogError($"User not found: {userId}");
          throw new Exception("User not found.");
        }

        this._logger.LogInformation($"Changing password for user: {user.Id}");

        var tempPassword = Helpers.PasswordGenerator.CreatePassword(5);
        var newPassword = $"{tempPassword}Xa!6";

        this._logger.LogInformation($"Changing password to: {newPassword}");

        _identityLayer.Identity.UserManager.UserValidators.Clear();
        var rcpResult = await _identityLayer.Identity.ChangePasswordAsync(user, newPassword).ConfigureAwait(false);

        this._logger.LogInformation($"Password changed result: {rcpResult.Succeeded}");

        if (!rcpResult.Succeeded)
        {
          this._logger.LogError($"Errors: {rcpResult.Errors.First().Description}");
          return false;
        }

        this._logger.LogInformation("Sending password email...");

        var adminUsers = await _identityLayer.GetUsersByRole("Admin").ConfigureAwait(false);
        var baseUrl = this._applicationSettings.BaseUrl;

        if (adminUsers.All(x => !string.Equals(x.Id!, userId, StringComparison.CurrentCultureIgnoreCase)))
        {
          baseUrl = this._applicationSettings.ClientUrl;
        }

        var emailMessage = await this._razorTemplateEngine.RenderAsync("/Templates/GenericEmail.cshtml",
          new GenericEmailModel
          {
            BaseUrl = baseUrl,
            Body = Templating.EmailContent.Accounts.GenerateResetPasswordEmail(baseUrl, user.UserName, newPassword)
          }).ConfigureAwait(false);

        var recipient = user.Email;

        if (this._smtpSettings.Debug && !string.IsNullOrEmpty(this._smtpSettings.DebugEmail))
        {
          recipient = this._smtpSettings.DebugEmail;
        }

        await _emailClient.SendAsync(recipient,
            "AFMA STATS PORTAL: Access details",
            emailMessage, null, new List<string> { "operations@resolvetech.co.za" })
          .ConfigureAwait(false);

        this._logger.LogInformation("Email sending successful!");

        return true;
      }
      catch (Exception ex)
      {
        this._logger.LogError(ex, "Could not reset password");
        return false;
      }
    }

    public async Task<bool> SendChangePasswordNotification(string userId, string newPassword)
    {
      try
      {
        var user = await _identityLayer.Identity.GetUserByIdAsync(userId).ConfigureAwait(false);
        _identityLayer.Identity.UserManager.UserValidators.Clear();
        var rcpResult = await _identityLayer.Identity.ChangePasswordAsync(user, newPassword).ConfigureAwait(false);

        if (!rcpResult.Succeeded)
        {
          return false;
        }

        var adminUsers = await _identityLayer.GetUsersByRole("Admin").ConfigureAwait(false);
        var baseUrl = this._applicationSettings.BaseUrl;

        if (adminUsers.All(x => !string.Equals(x.Id!, userId, StringComparison.CurrentCultureIgnoreCase)))
        {
          baseUrl = this._applicationSettings.ClientUrl;
        }

        var emailMessage = await this._razorTemplateEngine.RenderAsync("/Templates/GenericEmail.cshtml",
          new GenericEmailModel
          {
            BaseUrl = baseUrl,
            Body = Templating.EmailContent.Accounts.GenerateResetPasswordEmail(baseUrl, user.UserName, newPassword)
          }).ConfigureAwait(false);

        var recipient = user.Email;

        if (this._smtpSettings.Debug && !string.IsNullOrEmpty(this._smtpSettings.DebugEmail))
        {
          recipient = this._smtpSettings.DebugEmail;
        }

        await _emailClient.SendAsync(recipient,
            "AFMA STATS PORTAL: Access details",
            emailMessage, null, new List<string> { "operations@resolvetech.co.za" })
          .ConfigureAwait(false);

        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }


    public async Task<bool> SendFeedsalesSubmissionReminder(int reportId)
    {
      try
      {
        var report = await _applicationLayer.GetFeedsalesReportsLookup().ConfigureAwait(false);
        var selectedReport = report.Single(x => x.Id == reportId);
        var activeMemberAccounts = await _applicationLayer.GetActiveMemberBranchAccounts(selectedReport.MemberBranchId, ModuleEnum.Feedsales).ConfigureAwait(false);

        var ccRecipients = new List<string>();
        var bccRecipients = new List<string> { "operations@resolvetech.co.za" };

        foreach (var memberAccount in activeMemberAccounts)
        {
          var memberWithBranch = $"{selectedReport.Member} {selectedReport.MemberBranch}";
          this._logger.LogWarning($"Sending Salmonella reminder for: {memberAccount.Email} from {memberWithBranch}");
          var submissionEmail = await this._razorTemplateEngine.RenderAsync("/Templates/GenericEmail.cshtml",
            new GenericEmailModel
            {
              BaseUrl = this._applicationSettings.BaseUrl,
              Body = Templating.EmailContent.Submissions.GenerateFeedsalesEmail(selectedReport.ReportMonth,
                selectedReport.ReportYear, memberAccount.Email, memberWithBranch),
            }).ConfigureAwait(false);

          var recipient = memberAccount.Email;

          if (this._smtpSettings.Debug && !string.IsNullOrEmpty(this._smtpSettings.DebugEmail))
          {
            recipient = this._smtpSettings.DebugEmail;
          }

          if (!string.IsNullOrEmpty(this._smtpSettings.SubmissionStatsEmail))
          {
            ccRecipients.Add(this._smtpSettings.SubmissionStatsEmail);
          }

          await _emailClient.SendAsync(recipient,
              $"AFMA FEEDSALES: Outstanding Report for {selectedReport.Member} - {selectedReport.MemberBranch}",
              submissionEmail, ccRecipients, bccRecipients)
            .ConfigureAwait(false);
        }
        return true;
      }
      catch (Exception ex)
      {
        this._logger.LogError(ex, "Could not send Feedsales Submission Reminder");
        throw new Exception(ex.Message, ex);
      }
    }

    public async Task<bool> SendSalmonellaSubmissionReminder(int reportId)
    {
      try
      {
        var selectedReport = await _salmonellaLayer.GetSalmonellaReport(reportId).ConfigureAwait(false);
        var activeMemberAccounts = await _applicationLayer.GetActiveMemberBranchAccounts(selectedReport.MemberBranchId, ModuleEnum.Salmonella).ConfigureAwait(false);

        var ccRecipients = new List<string>();
        var bccRecipients = new List<string> { "operations@resolvetech.co.za" };

        foreach (var memberAccount in activeMemberAccounts)
        {
          var memberWithBranch = $"{selectedReport.Member} {selectedReport.MemberBranch}";
          this._logger.LogWarning($"Sending Salmonella reminder for: {memberAccount.Email} from {memberWithBranch}");
          var submissionEmail = await this._razorTemplateEngine.RenderAsync("/Templates/GenericEmail.cshtml",
            new GenericEmailModel
            {
              BaseUrl = this._applicationSettings.BaseUrl,
              Body = Templating.EmailContent.Submissions.GenerateSalmonellaEmail(selectedReport.ReportMonth!.Value,
                selectedReport.ReportYear!.Value, memberAccount.Email, memberWithBranch),
            }).ConfigureAwait(false);


          var recipient = memberAccount.Email;

          if (this._smtpSettings.Debug && !string.IsNullOrEmpty(this._smtpSettings.DebugEmail))
          {
            recipient = this._smtpSettings.DebugEmail;
          }

          if (!string.IsNullOrEmpty(this._smtpSettings.SubmissionStatsEmail))
          {
            ccRecipients.Add(this._smtpSettings.SubmissionStatsEmail);
          }

          await _emailClient.SendAsync(recipient,
              $"AFMA SALMONELLA: Outstanding Report for {selectedReport.Member} - {selectedReport.MemberBranch}",
              submissionEmail, ccRecipients, bccRecipients)
            .ConfigureAwait(false);
        }

        return true;
      }
      catch (Exception ex)
      {
        this._logger.LogError(ex, "Could not send Salmonella Submission Reminder");
        throw new Exception(ex.Message, ex);
      }
    }

    public async Task<bool> SendGenericEmail(string recipientEmail, string subject, string body)
    {
      try
      {
        var emailMessage = await this._razorTemplateEngine.RenderAsync("/Templates/GenericEmail.cshtml",
          new GenericEmailModel
          {
            BaseUrl = this._applicationSettings.BaseUrl,
            Body = body
          }).ConfigureAwait(false);

        var recipient = recipientEmail;

        if (this._smtpSettings.Debug && !string.IsNullOrEmpty(this._smtpSettings.DebugEmail))
        {
          recipient = this._smtpSettings.DebugEmail;
        }

        await _emailClient.SendAsync(recipient,
            subject,
            emailMessage, null, new List<string> { "operations@resolvetech.co.za" })
            .ConfigureAwait(false);

        return true;
      }
      catch (Exception ex)
      {
        this._logger.LogError(ex, "Could not send Feedsales Submission Reminder");
        throw new Exception(ex.Message, ex);
      }
    }
  }
}

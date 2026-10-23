namespace AFMA.ApplicationLayer
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Templating.Models;
  using AFMA.Data.Entities;
  using AFMA.Data.Entities.Wims;
  using AFMA.Infrastructure.Options;
  using AFMA.Infrastructure.Settings;
  using AFMA.Shared.Infrastructure.Settings;
  using Alpha.Services.NHibernate.Repository.Services;
  using Alpha.Services.NHibernate.Repository.Services.Identity;
  using Alpha.Services.Notifications.Smtp;
  using Alpha.Services.Notifications.Templates;
  using Microsoft.AspNetCore.Identity;
  using Microsoft.Extensions.Options;
  using Razor.Templating.Core;

  /// <summary>
  /// The Identity Layer
  /// </summary>
  public class WimsIdentityLayer : IWimsIdentityLayer
  {
    private readonly IIdentityService<WimsApplicationUser> _identityService;
    private readonly IEmailClient _emailClient;
    private readonly IAlphaNotificationTemplateService _notificationTemplateService;
    private readonly IRazorTemplateEngine _razorTemplateEngine;
    private readonly ApplicationSettings _applicationSettings;
    private readonly SmtpSettings _smtpSettings;
    private readonly IApplicationRepository<WimsSessionFactory> _applicationRepository;


    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityLayer" /> class.
    /// </summary>
    /// <param name="identityService">The Identity service.</param>
    /// <param name="emailClient">The email client service.</param>
    /// <param name="alphaNotificationTemplateService">The notification template service.</param>
    /// <param name="razorTemplateEngine">The razor template engine.</param>
    /// <param name="smtpSettings">The SMTP settings.</param>
    /// <param name="applicationSettings">The Application settings.</param>
    public WimsIdentityLayer(IIdentityService<WimsApplicationUser> identityService,
      IEmailClient emailClient,
      IAlphaNotificationTemplateService alphaNotificationTemplateService,
      IRazorTemplateEngine razorTemplateEngine,
      IOptions<SmtpSettings> smtpSettings,
      IOptions<ApplicationSettings> applicationSettings,
      IApplicationRepository<WimsSessionFactory> applicationRepository)
    {
      this._identityService = identityService;
      this._emailClient = emailClient;
      this._notificationTemplateService = alphaNotificationTemplateService;
      this._razorTemplateEngine = razorTemplateEngine;
      this._smtpSettings = smtpSettings.Value;
      this._applicationSettings = applicationSettings.Value;
      this._applicationRepository = applicationRepository;
    }

    /// <inheritdoc/>
    public IIdentityService<WimsApplicationUser> Identity => this._identityService;

    /// <inheritdoc/>
    public async Task<ICollection<UserModel>> GetUsersByRole(string roleName)
    {
      var userList = new List<UserModel>();

      var users = (List<WimsApplicationUser>)await this.Identity.GetUsersInRoleAsync(roleName).ConfigureAwait(false);

      foreach (var user in users)
      {
        var roles = await this.Identity.UserManager.GetRolesAsync(user).ConfigureAwait(false);
        var roleNames = string.Join(',', roles);
        userList.Add(new UserModel
        {
          Email = user.Email,
          FirstName = user.FirstName,
          LastName = user.LastName,
          Id = user.Id,
          LastLogin = user.LastLoginDate,
          Username = user.UserName,
          RoleNames = roleNames,
        });
      }

      return userList;
    }

    public async Task SendTestEmail()
    {
      var submissionEmail = await this._razorTemplateEngine.RenderAsync("/Templates/GenericEmail.cshtml",
        new GenericEmailModel
        {
          BaseUrl = this._applicationSettings.BaseUrl,
          Body = Templating.EmailContent.Submissions.GenerateFeedsalesEmail(1, 2022, "jacques@di.co.za", "Digital Interactive"),
        }).ConfigureAwait(false);

      if (this._smtpSettings.Debug && !string.IsNullOrEmpty(this._smtpSettings.DebugEmail))
      {
        await _emailClient.SendAsync(this._smtpSettings.DebugEmail, "Test Email", submissionEmail)
          .ConfigureAwait(false);
      }
    }

    public async Task<IdentityResult> DeleteUser(string userId)
    {
      var user = await this.Identity.GetUserByIdAsync(userId).ConfigureAwait(false);
      return await this.Identity.UserManager.DeleteAsync(user).ConfigureAwait(false);
    }

    #region Login History

    public async Task LogLogin(string userId, string userName, string ipAddress, string hostName, string member,
      string memberBranch, string userType)
    {
      var loginHistoryModel = new LoginHistory
      {
        UserId = userId,
        UserName = userName,
        LoginDate = DateTime.Now,
        IpAddress = ipAddress,
        HostName = "Not available",
        Member = member,
        Memberbranch = memberBranch,
        UserType = userType,
      };

      var result =
        await _applicationRepository.SaveOrUpdateAndCommitAsync(loginHistoryModel);
    }

    #endregion
  }
}

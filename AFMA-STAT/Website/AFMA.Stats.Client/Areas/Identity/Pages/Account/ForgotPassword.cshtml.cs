namespace AFMA.Stats.Client.Areas.Identity.Pages.Account
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.Infrastructure.Settings;
  using Alpha.Services.Notifications.Smtp;
  using ApplicationLayer.Templating.Models;
  using DNTCaptcha.Core;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.RazorPages;
  using Microsoft.AspNetCore.WebUtilities;
  using Microsoft.Extensions.Options;
  using Razor.Templating.Core;
  using System.ComponentModel.DataAnnotations;
  using System.Text;
  using System.Threading.Tasks;

  [AllowAnonymous]
  public class ForgotPasswordModel : PageModel
  {
    private readonly IIdentityLayer identityLayer;
    private readonly IEmailClient emailClient;
    private readonly INotificationService notificattionService;
    private readonly IOptions<ApplicationSettings> applicationSettings;
    private readonly IRazorTemplateEngine razorTemplateEngine;
    private readonly IDNTCaptchaValidatorService validatorService;

    public ForgotPasswordModel(
      IIdentityLayer identityLayer,
      IEmailClient emailClient,
      INotificationService notificattionService,
      IRazorTemplateEngine razorTemplateEngine,
      IOptions<ApplicationSettings> applicationSettings,
      IDNTCaptchaValidatorService validatorService)
    {
      this.identityLayer = identityLayer;
      this.emailClient = emailClient;
      this.applicationSettings = applicationSettings;
      this.razorTemplateEngine = razorTemplateEngine;
      this.notificattionService = notificattionService;
      this.validatorService = validatorService;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
      [Required]
      public string Username { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {

      if (ModelState.IsValid)
      {
        if (!validatorService.HasRequestValidCaptchaEntry())
        {
          ModelState.AddModelError( "", "Please Enter Valid Security Code.");
          return Page();
        }

        var user = await identityLayer.Identity.GetUserByUsernameAsync(Input.Username);
        if (user == null || !await identityLayer.Identity.UserEmailConfirmedInd(user.Id))
        {
          return RedirectToPage("./ForgotPasswordConfirmation");
        }

        var code = await identityLayer.Identity.GenerateUserPasswordResetTokenAsync(user.Id);

        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        
        var callbackUrl = Url.Page(
            "/Account/ResetPassword",
            pageHandler: null,
            values: new { area = "Identity", code },
            protocol: Request.Scheme);

        var emailContent = await razorTemplateEngine.RenderAsync("/Templates/GenericEmail.cshtml",
          new GenericEmailModel
          {
            BaseUrl = applicationSettings.Value.BaseUrl,
            Body = ApplicationLayer.Templating.EmailContent.Accounts.GenerateResetPasswordRequestEmail(callbackUrl!),
          }).ConfigureAwait(false);

        await emailClient.SendAsync(user.Email,
            "AFMA STATS: Password reset request",
            emailContent, null, new List<string> { "operations@resolvetech.co.za" })
          .ConfigureAwait(false);

        return RedirectToPage("./ForgotPasswordConfirmation");
      }

      return Page();
    }
  }
}

namespace AFMA.Web.Admin.Areas.Identity.Pages.Account
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.Data.Entities.Wims;
  using Microsoft.AspNetCore.Authentication;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.RazorPages;
  using Microsoft.AspNetCore.WebUtilities;
  using Microsoft.Extensions.Logging;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;
  using System.Text;
  using System.Threading.Tasks;

  [AllowAnonymous]
  public class RegisterModel(
      IWimsIdentityLayer wimsIdentityLayer,
      IWimsApplicationLayer wimsApplicationLayer,
      IHttpContextAccessor httpContextAccessor,
      ILogger<RegisterModel> logger)
      : PageModel
  {
    private readonly IWimsApplicationLayer _wimsApplicationLayer = wimsApplicationLayer;
    private readonly ILogger _logger = logger;

    [BindProperty]
    public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public class InputModel
    {
      [Required]
      [EmailAddress]
      [Display(Name = "Email")]
      public string Email { get; set; }

      [Required]
      [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
      [DataType(DataType.Password)]
      [Display(Name = "Password")]
      public string Password { get; set; }

      [DataType(DataType.Password)]
      [Display(Name = "Confirm password")]
      [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
      public string ConfirmPassword { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(string returnUrl = null)
    {
      return LocalRedirect("/");
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
      returnUrl ??= Url.Content("~/");
      if (ModelState.IsValid)
      {
        var user = new WimsApplicationUser { UserName = Input.Email, Email = Input.Email };
        var result = await wimsIdentityLayer.Identity.CreateUserAsync(user, Input.Password);
        if (result.Succeeded)
        {
          _logger.LogInformation("User created a new account with password.");

          var code = await wimsIdentityLayer.Identity.GenerateEmailConfirmationTokenAsync(user.Id);
          code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
          var callbackUrl = Url.Page(
              "/Account/ConfirmEmail",
              pageHandler: null,
              values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
              protocol: Request.Scheme);

          await wimsIdentityLayer.Identity.SignInAsync(user, isPersistent: false);
          return LocalRedirect(returnUrl);
        }
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError(string.Empty, error.Description);
        }
      }

      // If we got this far, something failed, redisplay form
      return Page();
    }
  }
}

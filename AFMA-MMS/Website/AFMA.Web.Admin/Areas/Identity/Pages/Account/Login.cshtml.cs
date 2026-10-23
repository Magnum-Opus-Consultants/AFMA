namespace AFMA.Web.Admin.Areas.Identity.Pages.Account
{
  using AFMA.ApplicationLayer.Interface;
  using Microsoft.AspNetCore.Authentication;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Identity;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.RazorPages;
  using System.ComponentModel.DataAnnotations;
  using System.Security.Claims;

  [AllowAnonymous]
  public class LoginModel(
    IWimsIdentityLayer wimsIdentityLayer,
    IWimsApplicationLayer wimsApplicationLayer,
    IHttpContextAccessor httpContextAccessor,
    ILogger<LoginModel> logger)
    : PageModel
  {
    private readonly IWimsApplicationLayer _wimsApplicationLayer = wimsApplicationLayer;
    private readonly ILogger _logger = logger;

    [BindProperty] public InputModel Input { get; set; } = new InputModel();

    public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

    public string ReturnUrl { get; set; } = string.Empty;

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;

    public class InputModel
    {
      [Required(ErrorMessage = "The Email field is required")]
      [RegularExpression("[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}", ErrorMessage = "Please enter a valid email address")]
      public string Email { get; set; } = string.Empty;

      [Required(ErrorMessage = "The Password field is required")]
      [DataType(DataType.Password)]
      public string Password { get; set; } = string.Empty;

      [Display(Name = "Remember me?")]
      public bool RememberMe { get; set; }
    }

    public async Task OnGetAsync(string returnUrl = null)
    {
      if (!string.IsNullOrEmpty(ErrorMessage))
      {
        ModelState.AddModelError(string.Empty, ErrorMessage);
      }

      returnUrl ??= Url.Content("~/");

      // Clear the existing external cookie to ensure a clean login process
      await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

      ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
      returnUrl ??= Url.Content("~/");

      if (ModelState.IsValid)
      {
        // This doesn't count login failures towards account lockout
        // To enable password failures to trigger account lockout, set lockoutOnFailure: true
        var result = await wimsIdentityLayer.Identity.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
          _logger.LogWarning("Login success!");
          var userClaims = httpContextAccessor.HttpContext!.User.Claims.ToList();

          if (userClaims.Any())
          {
            var userId = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            _logger.LogWarning($"{userId} logged in");

            if (userId != null)
            {
              //Add spesific content here

              HttpContext.Session.SetString("UserId", userId);

              //await applicationLayer.AddToLoginHistory(userId, user.UserName,
              //  Request.HttpContext.Connection.RemoteIpAddress!.ToString(), "", memberAccount!.Member, memberAccount.MemberBranch!, "ADMIN",
              //  Request.Headers.UserAgent).ConfigureAwait(false);
            }
          }

          return LocalRedirect(returnUrl);
        }
        if (result.RequiresTwoFactor)
        {
          return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });
        }
        if (result.IsLockedOut)
        {
          return RedirectToPage("./Lockout");
        }
        else
        {
          ModelState.AddModelError(string.Empty, "Invalid login attempt.");
          return Page();
        }
      }

      // If we got this far, something failed, redisplay form
      return Page();
    }
  }
}

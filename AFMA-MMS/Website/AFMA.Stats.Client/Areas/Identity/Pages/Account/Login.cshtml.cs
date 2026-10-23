// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
namespace AFMA.Stats.Client.Areas.Identity.Pages.Account
{
#nullable disable

  using AFMA.ApplicationLayer.Interface;
  using AFMA.Infrastructure.Settings;
  using Microsoft.AspNetCore.Authentication;
  using Microsoft.AspNetCore.Identity;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.RazorPages;
  using Microsoft.Extensions.Options;
  using System.ComponentModel.DataAnnotations;
  using System.Security.Claims;

  public class LoginModel : PageModel
  {
    private readonly IIdentityLayer identityLayer;
    private readonly IOptions<ApplicationSettings> applicationSettings;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IApplicationLayer applicationLayer;

    public LoginModel(IIdentityLayer identityLayer, IOptions<ApplicationSettings> applicationSettings, IHttpContextAccessor httpContextAccessor, IApplicationLayer applicationLayer)
    {
      this.identityLayer = identityLayer;
      this.applicationSettings = applicationSettings;
      this.httpContextAccessor = httpContextAccessor;
      this.applicationLayer = applicationLayer;
    }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public string ReturnUrl { get; set; }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [TempData]
    public string ErrorMessage { get; set; }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class InputModel
    {
      /// <summary>
      ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
      ///     directly from your code. This API may change or be removed in future releases.
      /// </summary>
      [Required]
      [Display(Name = "Username/Email")]
      public string Email { get; set; }

      /// <summary>
      ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
      ///     directly from your code. This API may change or be removed in future releases.
      /// </summary>
      [Required]
      [DataType(DataType.Password)]
      public string Password { get; set; }

      /// <summary>
      ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
      ///     directly from your code. This API may change or be removed in future releases.
      /// </summary>
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

        //if (this.applicationSettings.Value.Debug)
        //{
        //  this.Input.Email = "afmatest1";
        //  this.Input.Password = "P@ssword12345";
        //}

        var result = await identityLayer.Identity.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
          var userClaims = httpContextAccessor.HttpContext!.User.Claims.ToList();

          if (userClaims.Any())
          {
            var userId = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
              var user = await identityLayer.Identity.GetUserByIdAsync(userId).ConfigureAwait(false);

              if (user != null)
              {
                var memberAccount = await applicationLayer.GetMemberAccount(user.Id).ConfigureAwait(false);

                try
                {
                  await applicationLayer.AddToLoginHistory(userId, user.UserName,
                    Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "", "",
                    memberAccount?.Member ?? "", memberAccount?.MemberBranch ?? "", "CLIENT",
                    Request.Headers.UserAgent).ConfigureAwait(false);
                }
                catch
                {
                  // Login history is non-critical; never block sign-in if it fails.
                }
              }
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

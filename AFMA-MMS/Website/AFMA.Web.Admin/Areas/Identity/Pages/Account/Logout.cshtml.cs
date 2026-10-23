namespace AFMA.Web.Admin.Areas.Identity.Pages.Account
{
  using AFMA.Data.Entities.Wims;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Identity;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.RazorPages;

  [AllowAnonymous]
  public class LogoutModel : PageModel
  {
    private readonly SignInManager<WimsApplicationUser> _signInManager;
    private readonly ILogger<LogoutModel> _logger;

    public LogoutModel(SignInManager<WimsApplicationUser> signInManager, ILogger<LogoutModel> logger)
    {
      _signInManager = signInManager;
      _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(string returnUrl = null)
    {
      await _signInManager.SignOutAsync();
      return LocalRedirect("/");
    }

    public async Task<IActionResult> OnPost(string returnUrl = null)
    {
      await _signInManager.SignOutAsync();
      _logger.LogInformation("User logged out.");
      if (returnUrl != null)
      {
        return LocalRedirect(returnUrl);
      }
      else
      {
        return RedirectToPage();
      }
    }
  }
}

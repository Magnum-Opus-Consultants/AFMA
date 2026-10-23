namespace AFMA.Stats.Client.Controllers
{
  using AFMA.ApplicationLayer.Interface;
  using Microsoft.AspNetCore.Authentication;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Identity;
  using Microsoft.AspNetCore.Mvc;

  [Authorize(Roles = "User")]
  public class AccountController : Controller
  {
    private readonly IIdentityLayer _identityLayer;

    public AccountController(IIdentityLayer identityLayer)
    {
      _identityLayer = identityLayer;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Logout()
    {
      await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
      await _identityLayer.Identity.SignOutAsync().ConfigureAwait(false);
      return LocalRedirect(Url.Content("~/"));
    }

    public async Task<IActionResult> Profile()
    {
      return View();
    }

    public IActionResult ReportNotFound()
    {
      return View();
    }
  }
}

namespace AFMA.Web.Admin.Controllers
{
  using AFMA.ApplicationLayer.Interface;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;

  [Authorize(Roles = "Member")]
  public class ProfileController(
    IWimsApplicationLayer wimsApplicationLayer,
    ILogger<MemberController> logger)
    : Controller
  {
    private readonly IWimsApplicationLayer _wimsApplicationLayer = wimsApplicationLayer;
    private readonly ILogger<MemberController> _logger = logger;

    public async Task<IActionResult> Index()
    {
      var userId = HttpContext.Session.GetString("UserId");

      if (string.IsNullOrEmpty(userId))
      {
        throw new Exception("UserId not found!");
      }

      var memberUser = await _wimsApplicationLayer.GetMemberUser(userId).ConfigureAwait(false);

      if (memberUser != null)
      {
        return RedirectToAction("details", new { id = memberUser.MemberId });
      }

      return View();
    }
  }
}

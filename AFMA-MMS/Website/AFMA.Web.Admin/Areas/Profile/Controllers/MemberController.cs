namespace AFMA.Web.Admin.Areas.Profile.Controllers
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Model.Wims;
  using AFMA.Infrastructure.Extensions;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;

  [Area("Profile")]
  [Authorize(Roles = "Member")]
  public class MemberController
  (
    IWimsApplicationLayer wimsApplicationLayer,
    ILogger<MemberController> logger
  ) : BaseController
  {
    private readonly IWimsApplicationLayer _wimsApplicationLayer = wimsApplicationLayer;
    private readonly ILogger<MemberController> _logger = logger;

    public async Task<IActionResult> Index()
    {
      var userId = HttpContext.Session.GetString("UserId");

      if (string.IsNullOrEmpty(userId))
      {
        return LocalRedirect("/Identity/Account/Logout");
      }

      var memberUser = await _wimsApplicationLayer.GetMemberUser(userId).ConfigureAwait(false);

      if (memberUser != null)
      {
        return RedirectToAction("details", new { id = memberUser.MemberId });
      }

      return View();
    }

    public async Task<IActionResult> Details(int id)
    {
      var permission = await wimsApplicationLayer.VerifyMemberUserPermission(id).ConfigureAwait(false);

      if (permission)
      {
        var result = await wimsApplicationLayer.GetMember(id).ConfigureAwait(false);
        var memberUserId = await _wimsApplicationLayer.GetMemberUserIdFromUserId().ConfigureAwait(false);
        var memberUserBranches = (await _wimsApplicationLayer.GetMemberUserBranches(memberUserId)).Select(a => a.MemberBranchId).ToHashSet();

        result.Branches = result.Branches.Where(a => memberUserBranches.Contains(a.Id)).ToList();
        return View(result);
      }

      throw new UnauthorizedAccessException("Permission denied!!");
    }

    public async Task<IActionResult> DetailsBranch(int id)
    {
      var permission = await wimsApplicationLayer.VerifyMemberUserBranchPermission(id).ConfigureAwait(false);

      if (permission)
      {
        var result = await wimsApplicationLayer.GetMemberBranch(id).ConfigureAwait(false);
        return View(result);
      }

      throw new UnauthorizedAccessException("Permission denied!!");
    }

    public async Task<IActionResult> EditBranch(int id)
    {
      var permission = await wimsApplicationLayer.VerifyMemberUserBranchPermission(id).ConfigureAwait(false);

      if (permission)
      {
        var result = await wimsApplicationLayer.GetMemberBranch(id).ConfigureAwait(false);
        return View(result);
      }

      throw new UnauthorizedAccessException("Permission denied!!");
    }

    public async Task<IActionResult> CreateCommittee()
    {
      var memberUserId = await wimsApplicationLayer.GetMemberUserIdFromUserId().ConfigureAwait(false);
      var memberUser = await wimsApplicationLayer.GetMemberUser(memberUserId).ConfigureAwait(false);

      var permission = await wimsApplicationLayer.VerifyMemberUserPermission(memberUser.MemberId).ConfigureAwait(false);

      if (permission)
      {
        return View(new MemberCommitteeModel { MemberId = memberUser.MemberId });
      }

      throw new UnauthorizedAccessException("Permission denied!!");
    }
  }
}

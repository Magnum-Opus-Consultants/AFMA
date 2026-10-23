namespace AFMA.Web.Admin.Areas.Profile.Controllers
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Model.Wims;
  using AFMA.Infrastructure.Extensions;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using System.Threading.Tasks;

  [Area("Profile")]
  [Authorize(Roles = "Member")]
  public class DistributionController
  (
    IWimsApplicationLayer wimsApplicationLayer,
    ILogger<MemberController> logger
  ) : BaseController
  {
    private readonly IWimsApplicationLayer _wimsApplicationLayer = wimsApplicationLayer;
    private readonly ILogger<MemberController> _logger = logger;

    public async Task<IActionResult> Index()
    {
      var memberUserDistributionGroups = await _wimsApplicationLayer.GetMemberUserDistributionGroups();
      return View(memberUserDistributionGroups);
    }

    [HttpPost]
    public async Task<IActionResult> Index(IFormCollection collection)
    {
      await _wimsApplicationLayer.DeleteAllMemberUserDistributionGroups().ConfigureAwait(false);
      var userId = await _wimsApplicationLayer.GetMemberUserIdFromUserId();
      var memberUser = await _wimsApplicationLayer.GetMemberUser(userId).ConfigureAwait(false);

      var publicDistributionGroups = await _wimsApplicationLayer.GetPublicDistributionGroups().ConfigureAwait(false);

      foreach (var dg in publicDistributionGroups)
      {
        if (Request.Form[$"dg_{dg.Id}"] == "on")
        {
          var r = await wimsApplicationLayer.SaveOrUpdate(new MemberUserDistributionGroupModel
          {
            MemberUserId = memberUser.Id,
            DistributionGroupId = dg.Id,
            FirstName = memberUser.FirstName,
            LastName = memberUser.LastName,
            EmailAddress = memberUser.Email,
          }).ConfigureAwait(false);
        }
      }

      return RedirectToAction("Index", "Distribution");


    }
  }
}

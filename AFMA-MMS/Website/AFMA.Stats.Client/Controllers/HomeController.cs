namespace AFMA.Stats.Client.Controllers
{
  using System.Diagnostics;
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.ViewModels;
  using AFMA.Stats.Client.Models;
  using ApplicationLayer.Helpers;
  using AspNetCoreHero.ToastNotification.Abstractions;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;

  [Authorize(Roles = "User")]
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly INotyfService _notyf;
    private readonly MemberAccessViewModel memberAccessViewModel;

    public HomeController(ILogger<HomeController> logger, IApplicationLayer applicationLayer, INotyfService notyf)
    {
      _logger = logger;
      _applicationLayer = applicationLayer;
      _notyf = notyf;
      memberAccessViewModel = _applicationLayer.GetCurrentMemberAccess().Result;
    }

    public async Task<IActionResult> Index()
    {
      var dashboard = new ClientDashboardViewModel
      {
        LastLogin = memberAccessViewModel.LastLogin,
        Member = memberAccessViewModel.Member,
        MemberAccount = memberAccessViewModel.FullName,
        MemberBranch = memberAccessViewModel.MemberBranch,
        MemberUAccountUserName = memberAccessViewModel.UserName,
      };

      if (memberAccessViewModel.HasFeedsalesAccess)
      {
        dashboard.ReportSummary.Add(await _applicationLayer.GenerateClientItemDashboard(memberAccessViewModel.MemberId,
          memberAccessViewModel.MemberBranchId, ModuleEnum.Feedsales).ConfigureAwait(false));
      }

      if (memberAccessViewModel.HasSalmonellaAccess)
      {
        dashboard.ReportSummary.Add(await _applicationLayer.GenerateClientItemDashboard(memberAccessViewModel.MemberId,
          memberAccessViewModel.MemberBranchId, ModuleEnum.Salmonella).ConfigureAwait(false));
      }

      if (memberAccessViewModel.HasIndustryProfileAccess)
      {
        dashboard.ReportSummary.Add(await _applicationLayer.GenerateClientItemDashboard(memberAccessViewModel.MemberId,
          memberAccessViewModel.MemberBranchId, ModuleEnum.IndustryProfile).ConfigureAwait(false));
      }

      if (memberAccessViewModel.HasMycotoxinAccess)
      {
        dashboard.ReportSummary.Add(await _applicationLayer.GenerateClientItemDashboard(memberAccessViewModel.MemberId,
          memberAccessViewModel.MemberBranchId, ModuleEnum.Mycotoxin).ConfigureAwait(false));
      }

      return View(dashboard);
    }

    public IActionResult About()
    {
      return View();
    }

    public IActionResult Contact()
    {
      return View();
    }

    public IActionResult ComingSoon()
    {
      return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> TestSuccessToast()
    {
      _notyf.Success("This is a successful test notification");
      return RedirectToAction("Index");
    }

    public async Task<IActionResult> TestErrorToast()
    {
      _notyf.Error("This is an error test notification");
      return RedirectToAction("Index");
    }
  }
}
namespace AFMA.Stats.Admin.Areas.General.Controllers
{
  using AFMA.ApplicationLayer.Interface;
  using ApplicationLayer.Model;
  using AspNetCoreHero.ToastNotification.Abstractions;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;

  [Area("General")]
  [Authorize(Roles = "Admin")]
  public class MemberTypeController : Controller
  {
    private readonly ILogger<ModuleController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly INotyfService _notyf;

    public MemberTypeController(ILogger<ModuleController> logger, IApplicationLayer applicationLayer, INotyfService notyf)
    {
      _logger = logger;
      _applicationLayer = applicationLayer;
      _notyf = notyf;
    }

    public async Task<IActionResult> Index()
    {
      var results = await _applicationLayer.GetMemberTypes().ConfigureAwait(false);
      return View(results);
    }

    public async Task<IActionResult> Details(int id)
    {
      var result = await _applicationLayer.GetMemberType(id).ConfigureAwait(false);
      return View(result);
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await _applicationLayer.GetMemberType(id).ConfigureAwait(false);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(MemberTypeModel memberTypeModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(memberTypeModel);

      if (result.Success)
      {
        _notyf.Success("Member Type updated successfully");
      }
      else
      {
        _notyf.Error($"Member Type not updated: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", "MemberType", new { area = "General", id = result.SavedObjectId });
    }

    public async Task<IActionResult> Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(MemberTypeModel memberTypeModel)
    {
      memberTypeModel.Deletable = true;
      
      var result = await _applicationLayer.SaveOrUpdate(memberTypeModel);

      if (result.Success)
      {
        _notyf.Success("Member Type created successfully");
      }
      else
      {
        _notyf.Error($"Member Type not create: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", "MemberType", new { area = "General", id = result.SavedObjectId });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int memberTypeId)
    {
      var result = await _applicationLayer.DeleteMemberType(memberTypeId).ConfigureAwait(false);

      if (result.Succeeded)
      {
        _notyf.Success("Member Type deleted successfully");
      }
      else
      {
        _notyf.Error($"Member Type could not be deleted: {result.ExceptionMessage}");
      }

      return RedirectToAction("Index", "MemberType", new { area = "General" });
    }
  }
}

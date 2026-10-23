namespace AFMA.Stats.Admin.Areas.Mycotoxin.Controllers
{
  using AFMA.ApplicationLayer.Interface;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;

  [Area("Mycotoxin")]
  [Authorize(Roles = "Admin")]
  public class AnalysisController : Controller
  {
    private readonly ILogger<AnalysisController> _logger;
    private readonly IMycotoxinLayer _mycotoxinLayer;

    public AnalysisController(ILogger<AnalysisController> logger, IMycotoxinLayer mycotoxinLayer)
    {
      _logger = logger;
      _mycotoxinLayer = mycotoxinLayer;
    }

    public async Task<IActionResult> GetMycotoxinAnalysisMethod(int id)
    {
      var analysisMethod = await _mycotoxinLayer.GetMycotoxinAnalysisMethod(id).ConfigureAwait(false);
      return Json(new
      {
        analysisMethod.Id,
        analysisMethod.Other,
        analysisMethod.Name,
      });
    }
  }
}

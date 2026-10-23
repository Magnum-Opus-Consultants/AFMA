namespace AFMA.Web.Admin.Controllers
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Model.Wims;
  using AFMA.Infrastructure.Extensions;
  using Alpha.Services.Models.Data.PageData;
  using Microsoft.AspNetCore.Mvc;

  public class DocumentTypeController(IWimsApplicationLayer wimsApplicationLayer, ILogger<DocumentTypeController> logger) : BaseController
  {
    private readonly IWimsApplicationLayer _wimsApplicationLayer = wimsApplicationLayer;
    private readonly ILogger<DocumentTypeController> _logger = logger;

    public async Task<IActionResult> Index(int? page, string? filterSearchText)
    {
      var pageNo = page ?? 1;
      var notificationTemplates = await _wimsApplicationLayer.GetDocumentTypes(new AlphaPageRequest(pageNo, 25, "Description"), filterSearchText)
        .ConfigureAwait(false);
      return View(notificationTemplates);
    }

    public async Task<IActionResult> Details(int id)
    {
      var result = await wimsApplicationLayer.GetDocumentType(id).ConfigureAwait(false);
      return View(result);
    }

    public IActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(DocumentTypeModel documentTypeModel)
    {
      var result = await wimsApplicationLayer.SaveOrUpdate(documentTypeModel).ConfigureAwait(false);
      return RedirectToAction("Details", new { id = result.SavedObjectId });
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await wimsApplicationLayer.GetDocumentType(id).ConfigureAwait(false);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(DocumentTypeModel documentTypeModel)
    {
      var result = await wimsApplicationLayer.SaveOrUpdate(documentTypeModel).ConfigureAwait(false);
      return RedirectToAction("Details", new { id = documentTypeModel.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
      var result = await _wimsApplicationLayer.DeleteDocumentType(id).ConfigureAwait(false);
      return RedirectToAction("Index", "DocumentType", new { area = "" });
    }
  }
}

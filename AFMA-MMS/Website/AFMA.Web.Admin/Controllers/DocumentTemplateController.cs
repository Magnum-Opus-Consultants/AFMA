namespace AFMA.Web.Admin.Controllers
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Model.Wims;
  using AFMA.Infrastructure.Extensions;
  using Alpha.Services.Models.Data.PageData;
  using Microsoft.AspNetCore.Mvc;

  public class DocumentTemplateController(IWimsApplicationLayer wimsApplicationLayer, ILogger<DocumentTemplateController> logger) : BaseController
  {
    private readonly IWimsApplicationLayer _wimsApplicationLayer = wimsApplicationLayer;
    private readonly ILogger<DocumentTemplateController> _logger = logger;

    public async Task<IActionResult> Index(int? page, string? filterSearchText)
    {
      var pageNo = page ?? 1;
      var notificationTemplates = await _wimsApplicationLayer.GetDocumentTemplates(new AlphaPageRequest(pageNo, 25, "Name"), filterSearchText)
        .ConfigureAwait(false);
      return View(notificationTemplates);
    }

    public async Task<IActionResult> Details(int id)
    {
      var result = await wimsApplicationLayer.GetDocumentTemplate(id).ConfigureAwait(false);
      return View(result);
    }

    public IActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(string name, IFormFile templateContent)
    {
      using var memoryStream = new MemoryStream();
      await templateContent.CopyToAsync(memoryStream);

      var documentTemplateModel = new DocumentTemplateModel
      {
        Name = name,
        TemplateContentType = templateContent.ContentType,
        TemplateFileName = Path.GetFileName(templateContent.FileName),
        TemplateFileSize = (int)templateContent.Length,
        TemplateContent = memoryStream.ToArray(),
      };

      var result = await _wimsApplicationLayer.SaveOrUpdate(documentTemplateModel).ConfigureAwait(false);

      return RedirectToAction("Details", "DocumentTemplate", new { id = result.SavedObjectId });
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await wimsApplicationLayer.GetDocumentTemplate(id).ConfigureAwait(false);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(IFormFile templateContent, DocumentTemplateModel documentTemplateModel)
    {
      var documentTemplate = await wimsApplicationLayer.GetDocumentTemplate(documentTemplateModel.Id).ConfigureAwait(false);
      documentTemplate.Name = documentTemplateModel.Name;

      if (templateContent != null)
      {
        using var memoryStream = new MemoryStream();
        await templateContent.CopyToAsync(memoryStream);
        documentTemplate.TemplateContent = memoryStream.ToArray();
        documentTemplate.TemplateFileName = Path.GetFileName(templateContent.FileName);
        documentTemplate.TemplateContentType = templateContent.ContentType;
        documentTemplate.TemplateFileSize = (int)templateContent.Length;
      }

      var result = await wimsApplicationLayer.Merge(documentTemplate).ConfigureAwait(false);

      return RedirectToAction("Details", new { id = documentTemplate.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
      var result = await _wimsApplicationLayer.DeleteDocumentTemplate(id).ConfigureAwait(false);
      return RedirectToAction("Index", "DocumentTemplate", new { area = "" });
    }

    public async Task<IActionResult> DownloadDocument(int id)
    {
      var result = await _wimsApplicationLayer.GetDocumentTemplate(id).ConfigureAwait(false);
      return File(result.TemplateContent, result.TemplateContentType, result.TemplateFileName);
    }
  }
}

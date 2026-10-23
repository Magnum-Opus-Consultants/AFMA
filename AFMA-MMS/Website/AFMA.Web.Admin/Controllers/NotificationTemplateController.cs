namespace AFMA.Web.Admin.Controllers
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Model.Wims;
  using AFMA.Infrastructure.Extensions;
  using Alpha.Services.Models.Data.PageData;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;

  [Authorize(Roles = "Admin")]
  public class NotificationTemplateController(IWimsApplicationLayer wimsApplicationLayer, ILogger<NotificationTemplateController> logger) : BaseController
  {
    private readonly IWimsApplicationLayer _wimsApplicationLayer = wimsApplicationLayer;
    private readonly ILogger<NotificationTemplateController> _logger = logger;

    public async Task<IActionResult> Index(int? page, string? filterSearchText)
    {
      var pageNo = page ?? 1;
      var notificationTemplates = await _wimsApplicationLayer.GetNotificationTemplates(new AlphaPageRequest(pageNo, 25, "Description"), filterSearchText)
        .ConfigureAwait(false);
      return View(notificationTemplates);
    }

    public async Task<IActionResult> Details(int id)
    {
      var result = await wimsApplicationLayer.GetNotificationTemplate(id).ConfigureAwait(false);
      return View(result);
    }

    public IActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(NotificationTemplateModel notificationTemplateModel)
    {
      var result = await wimsApplicationLayer.SaveOrUpdate(notificationTemplateModel).ConfigureAwait(false);
      return RedirectToAction("Details", new { id = result.SavedObjectId });
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await wimsApplicationLayer.GetNotificationTemplate(id).ConfigureAwait(false);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(NotificationTemplateModel notificationTemplateModel)
    {
      var result = await wimsApplicationLayer.SaveOrUpdate(notificationTemplateModel).ConfigureAwait(false);
      return RedirectToAction("Details", new { id = notificationTemplateModel.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
      var result = await wimsApplicationLayer.DeleteNotificationTemplate(id).ConfigureAwait(false);
      return RedirectToAction("Index", "NotificationTemplate", new { area = "" });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteContact(int deleteContactId, int parentId)
    {
      var result = await wimsApplicationLayer.DeleteNotificationContact(deleteContactId).ConfigureAwait(false);
      return RedirectToAction("Details", "NotificationTemplate", new { id = parentId, area = "" });
    }


    public IActionResult CreateContact(int notificationTemplateId)
    {
      return View(new NotificationContactModel { NotificationTemplateId = notificationTemplateId });
    }

    [HttpPost]
    public async Task<IActionResult> CreateContact(NotificationContactModel notificationContactModel)
    {
      var result = await wimsApplicationLayer.SaveOrUpdate(notificationContactModel).ConfigureAwait(false);
      return RedirectToAction("Details", new { id = notificationContactModel.NotificationTemplateId });
    }

    public async Task<IActionResult> EditContact(int id)
    {
      var result = await wimsApplicationLayer.GetNotificationContact(id).ConfigureAwait(false);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> EditContact(NotificationContactModel notificationContactModel)
    {
      var result = await wimsApplicationLayer.SaveOrUpdate(notificationContactModel).ConfigureAwait(false);
      return RedirectToAction("Details", new { id = notificationContactModel.NotificationTemplateId });
    }

  }
}

namespace AFMA.Web.Admin.Controllers
{
  using AFMA.ApplicationLayer.Helpers;
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Model.Export;
  using AFMA.ApplicationLayer.Model.Wims;
  using AFMA.Infrastructure.Settings;
  using Alpha.Services.Models.Data.PageData;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.Options;

  [Authorize(Roles = "Admin")]
  public class BatchController : Controller
  {

    private readonly IWimsApplicationLayer _wimsApplicationLayer;
    private readonly IExcelExportService _excelExportService;
    private readonly IOptions<WimsApplicationSettings> _applicationSettings;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BatchController> _logger;

    public BatchController(
      IWimsApplicationLayer wimsApplicationLayer,
      IExcelExportService excelExportService,
      IOptions<WimsApplicationSettings> applicationSettings,
      IConfiguration configuration,
      ILogger<BatchController> logger)
    {
      _applicationSettings = applicationSettings;
      _wimsApplicationLayer = wimsApplicationLayer;
      _excelExportService = excelExportService;
      _configuration = configuration;
      _logger = logger;
    }

    public async Task<IActionResult> Index(int? page, string? filterSearchText)
    {
      var pageNo = page ?? 1;
      var result = await _wimsApplicationLayer.GetBatches(new AlphaPageRequest(pageNo, 25, "CreatedDate"), filterSearchText)
        .ConfigureAwait(false);
      return View(result);
    }

    public IActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(BatchModel batchModel)
    {
      batchModel.BatchType = 1;
      batchModel.Status = BatchStatus.Pending;
      batchModel.CreatedDate = DateTime.Now;
      batchModel.User = User.Identity?.Name ?? "Unknown User";

      var result = await _wimsApplicationLayer.SaveOrUpdate(batchModel).ConfigureAwait(false);

      if (result.Success)
      {
        if (batchModel.RecipientSelector == 1)
        {
          var recipients = await _wimsApplicationLayer.GetBatchRecipients((int)result.SavedObjectId!, batchModel.DistributionGroupId, null);

          foreach (var recipient in recipients)
          {
            await _wimsApplicationLayer.AddBatchRecipient((int)result.SavedObjectId, recipient.ToEmail, recipient.ToName).ConfigureAwait(false);
          }
        }

        if (batchModel.RecipientSelector == 2)
        {
          var recipients = await _wimsApplicationLayer.GetBatchRecipients((int)result.SavedObjectId!, null, batchModel.CommitteeId);

          foreach (var recipient in recipients)
          {
            await _wimsApplicationLayer.AddBatchRecipient((int)result.SavedObjectId, recipient.ToEmail, recipient.ToName).ConfigureAwait(false);
          }
        }
      }

      return RedirectToAction("Details", new { id = result.SavedObjectId });
    }

    public async Task<IActionResult> Details(int id)
    {
      var result = await _wimsApplicationLayer.GetBatch(id).ConfigureAwait(false);
      return View(result);
    }

    public async Task<IActionResult> Export(int id)
    {
      var batch = await _wimsApplicationLayer.GetBatch(id).ConfigureAwait(false);

      var workbook = _excelExportService.CreateWorkbook(new ExcelWorkbookExportModel
      {
        FileName = $"Email Batch - {batch.Description}",
        Worksheets =
        [
          new ExcelWorksheetExportModel
          {
            Name = "Batch",
            Data = batch,
            PropertyNames =
            [
              nameof(BatchModel.Description),
              nameof(BatchModel.Status),
              nameof(BatchModel.FromName),
              nameof(BatchModel.FromEmail),
              nameof(BatchModel.Subject),
              nameof(BatchModel.Message),
              nameof(BatchModel.CreatedDate),
              nameof(BatchModel.SubmissionDate),
              nameof(BatchModel.DistributionGroup),
              nameof(BatchModel.RecipientCount),
            ],
          },
          new ExcelWorksheetExportModel
          {
            Name = "Recipients",
            Data = batch.Recipients,
            PropertyNames =
            [
              nameof(BatchRecipientModel.ToName),
              nameof(BatchRecipientModel.ToEmail),
            ],
          },
          new ExcelWorksheetExportModel
          {
            Name = "Attachments",
            Data = batch.Attachments,
            PropertyNames =
            [
              nameof(BatchAttachmentModel.AttachmentFileName),
              nameof(BatchAttachmentModel.AttachmentContentType),
              nameof(BatchAttachmentModel.AttachmentFileSize),
            ],
          },
        ],
      });

      return File(workbook.Content, workbook.ContentType, workbook.FileName);
    }

    public async Task<IActionResult> Submit(int id)
    {
      var batch = await _wimsApplicationLayer.GetBatch(id).ConfigureAwait(false);

      try
      {
        AFMA.Service.Communication.EmailService.SendBatchViaSmtp(
          batch,
          _configuration["SMTPSettings:Host"] ?? string.Empty,
          int.TryParse(_configuration["SMTPSettings:Port"], out var port) ? port : 587,
          _configuration["SMTPSettings:Username"] ?? string.Empty,
          _configuration["SMTPSettings:Password"] ?? string.Empty,
          _configuration["SMTPSettings:FromName"] ?? "AFMA");

        await _wimsApplicationLayer.BatchSubmitted(id).ConfigureAwait(false);
        TempData["BatchMessage"] = "Batch sent successfully.";
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Failed to send email batch {BatchId}", id);
        TempData["BatchError"] = "Batch failed to send: " + ex.Message;
      }

      return RedirectToAction("Details", new { id });
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await _wimsApplicationLayer.GetBatch(id).ConfigureAwait(false);

      if (result.Status == BatchStatus.Pending)
      {
        return View(result);
      }

      return RedirectToAction("Details", new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(BatchModel batchModel)
    {
      var batch = await _wimsApplicationLayer.GetBatch(batchModel.Id).ConfigureAwait(false);
      batch.Description = batchModel.Description;
      batch.FromName = batchModel.FromName;
      batch.FromEmail = batchModel.FromEmail;
      batch.Subject = batchModel.Subject;

      var result = await _wimsApplicationLayer.Merge(batch).ConfigureAwait(false);

      return RedirectToAction("Details", new { id = result.SavedObjectId });
    }

    [HttpPost]
    public async Task<IActionResult> AddAttachment(int batchId, IFormFile attachment)
    {
      using var memoryStream = new MemoryStream();
      await attachment.CopyToAsync(memoryStream);

      var batchAttachmentModel = new BatchAttachmentModel
      {
        BatchId = batchId,
        Attachment = memoryStream.ToArray(),
        AttachmentContentType = attachment.ContentType,
        AttachmentFileName = Path.GetFileName(attachment.FileName),
        AttachmentFileSize = (int)attachment.Length,
      };

      var result = await _wimsApplicationLayer.SaveOrUpdate(batchAttachmentModel).ConfigureAwait(false);

      return RedirectToAction("Details", "Batch", new { id = batchId });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveAttachment(int id, int batchId)
    {
      await _wimsApplicationLayer.DeleteBatchAttachment(id).ConfigureAwait(false);
      return RedirectToAction("Details", "Batch", new { id = batchId });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveRecipient(int id, int batchId2)
    {
      await _wimsApplicationLayer.DeleteBatchRecipient(id).ConfigureAwait(false);
      return RedirectToAction("Details", "Batch", new { id = batchId2 });
    }

    public IActionResult Queue()
    {
      var result =
        new AFMA.Service.Communication.EmailService(_applicationSettings.Value.EmailApiKey, _wimsApplicationLayer).GetQueue();
      return View(result);
    }

    public IActionResult Sent()
    {
      var result =
        new AFMA.Service.Communication.EmailService(_applicationSettings.Value.EmailApiKey, _wimsApplicationLayer).GetSent();

      return View(result);
    }

    public IActionResult Errors()
    {
      var result =
        new AFMA.Service.Communication.EmailService(_applicationSettings.Value.EmailApiKey, _wimsApplicationLayer).GetError();

      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
      var result = await _wimsApplicationLayer.DeleteBatch(id).ConfigureAwait(false);
      return RedirectToAction("Index", "Batch", new { area = "" });
    }
  }
}

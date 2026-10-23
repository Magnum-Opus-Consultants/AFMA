namespace AFMA.Web.Admin.Controllers
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Model.Export;
  using AFMA.ApplicationLayer.Model.Wims;
  using AFMA.Infrastructure.Extensions;
  using Alpha.Services.Models.Data.PageData;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;

  [Authorize(Roles = "Admin")]
  public class CommitteeController(
    IWimsApplicationLayer wimsApplicationLayer,
    IExcelExportService excelExportService,
    ILogger<CommitteeController> logger) : BaseController
  {
    private readonly IWimsApplicationLayer _wimsApplicationLayer = wimsApplicationLayer;
    private readonly IExcelExportService _excelExportService = excelExportService;
    private readonly ILogger<CommitteeController> _logger = logger;

    public async Task<IActionResult> Index(int? page, string? filterSearchText)
    {
      var pageNo = page ?? 1;
      var committees = await _wimsApplicationLayer.GetCommittees(new AlphaPageRequest(pageNo, 25, "Name"), filterSearchText)
        .ConfigureAwait(false);
      return View(committees);
    }

    public async Task<IActionResult> Details(int id)
    {
      var result = await _wimsApplicationLayer.GetCommittee(id).ConfigureAwait(false);

      foreach (var member in result.Members.Where(x => x.MemberBranchContactId.HasValue))
      {
        var memberBranchContact = await _wimsApplicationLayer.GetMemberBranchContact(member.MemberBranchContactId.Value).ConfigureAwait(false);
        var contactName = $"{memberBranchContact.FirstName} {memberBranchContact.Surname}";
        member.MemberBranchContactMatch = memberBranchContact.EmailAddress == member.EmailAddress && contactName == member.Name;
      }

      return View(result);
    }

    public async Task<IActionResult> Export(int id)
    {
      var committee = await _wimsApplicationLayer.GetCommittee(id).ConfigureAwait(false);

      var workbook = _excelExportService.CreateWorkbook(new ExcelWorkbookExportModel
      {
        FileName = $"Committee - {committee.Name}",
        Worksheets =
        [
          new ExcelWorksheetExportModel
          {
            Name = "Committee",
            Data = committee,
            PropertyNames = [nameof(CommitteeModel.Name)],
          },
          new ExcelWorksheetExportModel
          {
            Name = "Members",
            Data = committee.Members,
            PropertyNames =
            [
              nameof(MemberCommitteeModel.Name),
              nameof(MemberCommitteeModel.MemberName),
              nameof(MemberCommitteeModel.EmailAddress),
            ],
          },
          new ExcelWorksheetExportModel
          {
            Name = "Documents",
            Data = committee.Documents,
            PropertyNames =
            [
              nameof(CommitteeDocumentModel.Name),
              nameof(CommitteeDocumentModel.DocumentType),
              nameof(CommitteeDocumentModel.DocumentContentType),
              nameof(CommitteeDocumentModel.DocumentFileName),
              nameof(CommitteeDocumentModel.DocumentFileSize),
            ],
          },
        ],
      });

      return File(workbook.Content, workbook.ContentType, workbook.FileName);
    }

    public IActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CommitteeModel committeeModel)
    {
      var result = await _wimsApplicationLayer.SaveOrUpdate(committeeModel).ConfigureAwait(false);
      return RedirectToAction("Details", new { id = result.SavedObjectId });
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await _wimsApplicationLayer.GetCommittee(id).ConfigureAwait(false);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CommitteeModel committeeModel)
    {
      var result = await _wimsApplicationLayer.SaveOrUpdate(committeeModel).ConfigureAwait(false);
      return RedirectToAction("Details", new { id = committeeModel.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
      var result = await _wimsApplicationLayer.DeleteCommittee(id).ConfigureAwait(false);
      return RedirectToAction("Index", "Committee", new { area = "" });
    }

    public IActionResult CreateMember(int committeeId)
    {
      var memberCommitteeModel = new MemberCommitteeModel
      {
        CommitteeId = committeeId
      };

      return View(memberCommitteeModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMember(MemberCommitteeModel memberCommitteeModel)
    {
      var result = await _wimsApplicationLayer.SaveOrUpdate(memberCommitteeModel).ConfigureAwait(false);
      return RedirectToAction("Details", new { id = memberCommitteeModel.CommitteeId });
    }

    public async Task<IActionResult> EditMember(int id, bool match)
    {
      var result = await _wimsApplicationLayer.GetCommitteeMember(id).ConfigureAwait(false);
      result.MemberBranchContactMatch = match;
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> EditMember(MemberCommitteeModel memberCommitteeModel)
    {
      var result = await _wimsApplicationLayer.SaveOrUpdate(memberCommitteeModel).ConfigureAwait(false);
      return RedirectToAction("Details", new { id = memberCommitteeModel.CommitteeId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMember(int deleteMemberId, int parentId)
    {
      var result = await _wimsApplicationLayer.DeleteCommitteeMember(deleteMemberId).ConfigureAwait(false);
      return RedirectToAction("Details", "Committee", new { id = parentId, area = "" });
    }

    public IActionResult CreateDocument(int committeeId)
    {
      var committeeDocumentModel = new CommitteeDocumentModel
      {
        CommitteeId = committeeId
      };

      return View(committeeDocumentModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDocument(int committeeId, string name, int documentTypeId, string description, IFormFile documentContent)
    {
      using var memoryStream = new MemoryStream();
      await documentContent.CopyToAsync(memoryStream);

      var committeeDocumentModel = new CommitteeDocumentModel
      {
        Name = name,
        CommitteeId = committeeId,
        Description = description,
        DocumentTypeId = documentTypeId,
        DocumentContentType = documentContent.ContentType,
        DocumentFileName = Path.GetFileName(documentContent.FileName),
        DocumentFileSize = (int)documentContent.Length,
        DocumentContent = memoryStream.ToArray()
      };

      var result = await _wimsApplicationLayer.SaveOrUpdate(committeeDocumentModel).ConfigureAwait(false);

      return RedirectToAction("Details", "Committee", new { id = committeeDocumentModel.CommitteeId });
    }

    public async Task<IActionResult> EditDocument(int id)
    {
      var result = await _wimsApplicationLayer.GetCommitteeDocument(id).ConfigureAwait(false);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> EditDocument(IFormFile documentContent, CommitteeDocumentModel committeeDocumentModel)
    {
      var document = await _wimsApplicationLayer.GetCommitteeDocument(committeeDocumentModel.Id).ConfigureAwait(false);

      document.Name = committeeDocumentModel.Name;
      document.Description = committeeDocumentModel.Description;
      document.DocumentTypeId = committeeDocumentModel.DocumentTypeId;

      if (documentContent != null)
      {
        using var memoryStream = new MemoryStream();
        await documentContent.CopyToAsync(memoryStream);
        document.DocumentContent = memoryStream.ToArray();
        document.DocumentFileName = Path.GetFileName(documentContent.FileName);
        document.DocumentContentType = documentContent.ContentType;
        document.DocumentFileSize = (int)documentContent.Length;
      }

      var result = await _wimsApplicationLayer.Merge(document).ConfigureAwait(false);

      return RedirectToAction("Details", new { id = document.CommitteeId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteDocument(int deleteDocumentId, int parentId)
    {
      var result = await _wimsApplicationLayer.DeleteCommitteeDocument(deleteDocumentId).ConfigureAwait(false);
      return RedirectToAction("Details", "Committee", new { id = parentId, area = "" });
    }

    public async Task<IActionResult> DownloadDocument(int id)
    {
      var result = await _wimsApplicationLayer.GetCommitteeDocument(id).ConfigureAwait(false);
      return File(result.DocumentContent, result.DocumentContentType, result.DocumentFileName);
    }

    public async Task<IActionResult> GetMemberContacts(int memberId)
    {
      var result = await _wimsApplicationLayer.GetMemberContacts(memberId).ConfigureAwait(false);
      return Json(result);
    }

    public async Task<IActionResult> GetMemberBranchContact(int memberBranchContactId)
    {
      var result = await _wimsApplicationLayer.GetMemberBranchContact(memberBranchContactId).ConfigureAwait(false);
      return Json(result);
    }
  }
}

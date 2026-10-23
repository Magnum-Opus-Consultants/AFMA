namespace AFMA.Web.Admin.Controllers
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Model.Export;
  using AFMA.ApplicationLayer.Model.Wims;
  using Alpha.Services.Models.Data.PageData;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;

  [Authorize(Roles = "Admin")]
  public class DistributionController(
    IWimsApplicationLayer wimsApplicationLayer,
    IExcelExportService excelExportService,
    ILogger<DistributionController> logger) : Controller
  {
    private readonly IWimsApplicationLayer _wimsApplicationLayer = wimsApplicationLayer;
    private readonly IExcelExportService _excelExportService = excelExportService;
    private readonly ILogger<DistributionController> _logger = logger;

    public async Task<IActionResult> Index(int? page, int? filterMemberTypeId, string? filterSearchText)
    {
      var pageNo = page ?? 1;
      var distributionGroups = await _wimsApplicationLayer.GetDistributionGroups(new AlphaPageRequest(pageNo, 25, "Name"), filterSearchText)
        .ConfigureAwait(false);
      return View(distributionGroups);
    }

    public async Task<IActionResult> Details(int id)
    {
      var result = await wimsApplicationLayer.GetDistributionGroup(id).ConfigureAwait(false);

      foreach (var member in result.DistributionGroupMembers.Where(x => x.MemberBranchContactId.HasValue))
      {
        var memberBranchContact = await _wimsApplicationLayer.GetMemberBranchContact(member.MemberBranchContactId.Value).ConfigureAwait(false);
        var contactName = $"{memberBranchContact.FirstName} {memberBranchContact.Surname}";
        member.MemberBranchContactMatch = memberBranchContact.EmailAddress == member.EmailAddress && contactName == member.ContactName && memberBranchContact.TelephoneNo == member.ContactNumber;
      }

      return View(result);
    }

    public async Task<IActionResult> Export(int id)
    {
      var distributionGroup = await _wimsApplicationLayer.GetDistributionGroup(id).ConfigureAwait(false);

      var workbook = _excelExportService.CreateWorkbook(new ExcelWorkbookExportModel
      {
        FileName = $"Distribution Group - {distributionGroup.Name}",
        Worksheets =
        [
          new ExcelWorksheetExportModel
          {
            Name = "Group",
            Data = distributionGroup,
            PropertyNames = [nameof(DistributionGroupModel.Name), nameof(DistributionGroupModel.Description), nameof(DistributionGroupModel.Owner), nameof(DistributionGroupModel.FromAddress), nameof(DistributionGroupModel.Private)],
          },
          new ExcelWorksheetExportModel
          {
            Name = "Members",
            Data = distributionGroup.DistributionGroupMembers,
            PropertyNames = ["ContactName", "EmailAddress", "ContactNumber"],
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
    public async Task<IActionResult> Edit(DistributionGroupModel distributionGroupModel)
    {
      var result = await wimsApplicationLayer.SaveOrUpdate(distributionGroupModel).ConfigureAwait(false);
      return RedirectToAction("Details", new { id = distributionGroupModel.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await wimsApplicationLayer.GetDistributionGroup(id).ConfigureAwait(false);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(DistributionGroupModel distributionGroupModel)
    {
      var result = await wimsApplicationLayer.SaveOrUpdate(distributionGroupModel).ConfigureAwait(false);
      return RedirectToAction("Details", new { id = result.SavedObjectId });
    }

    public IActionResult CreateMember(int memberId)
    {
      return View(new DistributionGroupMemberModel { DistributionGroupId = memberId });
    }

    [HttpPost]
    public async Task<IActionResult> CreateMember(DistributionGroupMemberModel distributionGroupMemberModel)
    {
      var result = await wimsApplicationLayer.SaveOrUpdate(distributionGroupMemberModel).ConfigureAwait(false);
      return RedirectToAction("Details", new { id = distributionGroupMemberModel.DistributionGroupId });
    }

    public async Task<IActionResult> EditMember(int id, bool match)
    {
      var result = await wimsApplicationLayer.GetDistributionGroupMember(id).ConfigureAwait(false);
      result.MemberBranchContactMatch = match;
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> EditMember(DistributionGroupMemberModel distributionGroupMemberModel)
    {
      var result = await wimsApplicationLayer.SaveOrUpdate(distributionGroupMemberModel).ConfigureAwait(false);
      return RedirectToAction("Details", new { id = distributionGroupMemberModel.DistributionGroupId });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
      var result = await _wimsApplicationLayer.DeleteDistributionGroup(id).ConfigureAwait(false);
      return RedirectToAction("Index", "Distribution", new { area = "" });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMember(int deleteMemberId, int parentId)
    {
      var result = await _wimsApplicationLayer.DeleteDistributionGroupMember(deleteMemberId).ConfigureAwait(false);
      return RedirectToAction("Details", "Distribution", new { id = parentId, area = "" });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMemberUser(int deleteMemberUserId, int parentId)
    {
      var result = await _wimsApplicationLayer.DeleteDistributionGroupMemberUser(deleteMemberUserId).ConfigureAwait(false);
      return RedirectToAction("Details", "Distribution", new { id = parentId, area = "" });
    }
  }
}

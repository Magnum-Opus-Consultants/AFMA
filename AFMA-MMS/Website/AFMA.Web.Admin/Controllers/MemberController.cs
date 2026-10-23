namespace AFMA.Web.Admin.Controllers
{
  using System.Threading.Tasks;
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Model.Export;
  using AFMA.ApplicationLayer.Model.Wims;
  using AFMA.ApplicationLayer.ViewModels.Wims;
  using Alpha.Services.Models.Data.PageData;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;

  [Authorize(Roles = "Admin")]
  public class MemberController(
    IWimsApplicationLayer wimsApplicationLayer,
    IExcelExportService excelExportService,
    ILogger<MemberController> logger) : Controller
  {
    private readonly IWimsApplicationLayer _wimsApplicationLayer = wimsApplicationLayer;
    private readonly IExcelExportService _excelExportService = excelExportService;
    private readonly ILogger<MemberController> _logger = logger;

    public async Task<IActionResult> Index(int? page, int? filterMemberTypeId, int? filterProvinceId, string? filterMemberStatusId, string? filterSearchText)
    {
      var pageNo = page ?? 1;
      var members = await _wimsApplicationLayer.GetMembers(new AlphaPageRequest(pageNo, 25, "MemberTradeName"), filterMemberTypeId, filterProvinceId, filterMemberStatusId, filterSearchText)
        .ConfigureAwait(false);
      _logger.LogWarning($"{members.Count()} members found.");

      if (members.Count() == 1)
      {
        return RedirectToAction("details", new { id = members.First().Id });
      }

      return View(members);
    }

    public async Task<IActionResult> Details(int id)
    {
      var result = await wimsApplicationLayer.GetMember(id).ConfigureAwait(false);
      return View(result);
    }

    public async Task<IActionResult> Export(int id)
    {
      var member = await _wimsApplicationLayer.GetMember(id).ConfigureAwait(false);
      var branchWorksheet = CreateBranchWorksheet(member.Branches);

      var workbook = _excelExportService.CreateWorkbook(new ExcelWorkbookExportModel
      {
        FileName = $"Member - {member.MemberLegalName}",
        Worksheets =
        [
          new ExcelWorksheetExportModel
          {
            Name = "Member",
            Data = member,
            PropertyNames =
            [
              nameof(MemberModel.Id),
              nameof(MemberModel.MemberType),
              nameof(MemberModel.MemberIdentificationId),
              nameof(MemberModel.MemberTradeName),
              nameof(MemberModel.MemberLegalName),
              nameof(MemberModel.RegistrationNo),
              nameof(MemberModel.VatRegistrationNo),
              nameof(MemberModel.MemberOtherName),
              nameof(MemberModel.Province),
              nameof(MemberModel.PhysicalAddress),
              nameof(MemberModel.PostalAddress),
              nameof(MemberModel.TelephoneNo),
              nameof(MemberModel.Website),
              nameof(MemberModel.GpsCoordinates),
              nameof(MemberModel.Active),
            ],
          },
          new ExcelWorksheetExportModel
          {
            Name = branchWorksheet.Name,
            Data = branchWorksheet.Data,
            PropertyNames = branchWorksheet.PropertyNames,
          },
          new ExcelWorksheetExportModel
          {
            Name = "Documents",
            Data = member.Documents,
            PropertyNames =
            [
              nameof(MemberDocumentModel.Id),
              nameof(MemberDocumentModel.MemberId),
              nameof(MemberDocumentModel.Name),
              nameof(MemberDocumentModel.DocumentType),
              nameof(MemberDocumentModel.DocumentFileName),
              nameof(MemberDocumentModel.ExpiryDate),
              nameof(MemberDocumentModel.LastNotificationSent),
              nameof(MemberDocumentModel.Visible),
              nameof(MemberDocumentModel.DocumentContentType),
              nameof(MemberDocumentModel.DocumentFileSize),
            ],
          },
          new ExcelWorksheetExportModel
          {
            Name = "Branch Contacts",
            Data = member.Branches.SelectMany(x => x.Contacts),
            PropertyNames =
            [
              nameof(MemberBranchContactModel.Id),
              nameof(MemberBranchContactModel.MemberId),
              nameof(MemberBranchContactModel.MemberBranchId),
              nameof(MemberBranchContactModel.Member),
              nameof(MemberBranchContactModel.MemberBranch),
              nameof(MemberBranchContactModel.MemberContactTypeId),
              nameof(MemberBranchContactModel.MemberContactType),
              nameof(MemberBranchContactModel.Title),
              nameof(MemberBranchContactModel.FirstName),
              nameof(MemberBranchContactModel.Surname),
              nameof(MemberBranchContactModel.TelephoneNo),
              nameof(MemberBranchContactModel.MobileNo),
              nameof(MemberBranchContactModel.EmailAddress),
              nameof(MemberBranchContactModel.CompanyContact),
              nameof(MemberBranchContactModel.Designation),
            ],
          },
          new ExcelWorksheetExportModel
          {
            Name = "Committees",
            Data = member.Committees,
            PropertyNames =
            [
              nameof(MemberCommitteeModel.Id),
              nameof(MemberCommitteeModel.MemberId),
              nameof(MemberCommitteeModel.Committee),
              nameof(MemberCommitteeModel.Member),
              nameof(MemberCommitteeModel.Name),
              nameof(MemberCommitteeModel.MemberBranch),
              nameof(MemberCommitteeModel.MemberContactType),
              nameof(MemberCommitteeModel.EmailAddress),
            ],
          },
          new ExcelWorksheetExportModel
          {
            Name = "Users",
            Data = member.Users,
            PropertyNames =
            [
              nameof(MemberUserModel.Id),
              nameof(MemberUserModel.MemberId),
              nameof(MemberUserModel.FirstName),
              nameof(MemberUserModel.LastName),
              nameof(MemberUserModel.Email),
              nameof(MemberUserModel.CanEdit),
              nameof(MemberUserModel.IsActive),
              nameof(MemberUserModel.LastLogin),
            ],
          },
        ],
      });

      return File(workbook.Content, workbook.ContentType, workbook.FileName);
    }

    public async Task<IActionResult> ExportList(int? filterMemberTypeId, int? filterProvinceId, string? filterMemberStatusId, string? filterSearchText)
    {
      var members = (await _wimsApplicationLayer.GetMembers(filterMemberTypeId, filterProvinceId, filterMemberStatusId, filterSearchText)
        .ConfigureAwait(false)).ToArray();

      var memberDetails = await Task.WhenAll(members.Select(x => _wimsApplicationLayer.GetMember(x.Id))).ConfigureAwait(false);
      var branchWorksheet = CreateBranchWorksheet(memberDetails.SelectMany(x => x.Branches));

      var workbook = _excelExportService.CreateWorkbook(new ExcelWorkbookExportModel
      {
        FileName = "Members",
        Worksheets =
        [
          new ExcelWorksheetExportModel
          {
            Name = "Members",
            Data = memberDetails,
            PropertyNames =
            [
              nameof(MemberModel.Id),
              nameof(MemberModel.MemberType),
              nameof(MemberModel.MemberIdentificationId),
              nameof(MemberModel.MemberTradeName),
              nameof(MemberModel.MemberLegalName),
              nameof(MemberModel.RegistrationNo),
              nameof(MemberModel.VatRegistrationNo),
              nameof(MemberModel.MemberOtherName),
              nameof(MemberModel.Province),
              nameof(MemberModel.PhysicalAddress),
              nameof(MemberModel.PostalAddress),
              nameof(MemberModel.TelephoneNo),
              nameof(MemberModel.Website),
              nameof(MemberModel.GpsCoordinates),
              nameof(MemberModel.Active),
            ],
          },
          new ExcelWorksheetExportModel
          {
            Name = branchWorksheet.Name,
            Data = branchWorksheet.Data,
            PropertyNames = branchWorksheet.PropertyNames,
          },
          new ExcelWorksheetExportModel
          {
            Name = "Documents",
            Data = memberDetails.SelectMany(x => x.Documents),
            PropertyNames =
            [
              nameof(MemberDocumentModel.Id),
              nameof(MemberDocumentModel.MemberId),
              nameof(MemberDocumentModel.Member),
              nameof(MemberDocumentModel.Name),
              nameof(MemberDocumentModel.DocumentType),
              nameof(MemberDocumentModel.DocumentFileName),
              nameof(MemberDocumentModel.ExpiryDate),
              nameof(MemberDocumentModel.LastNotificationSent),
              nameof(MemberDocumentModel.Visible),
              nameof(MemberDocumentModel.DocumentContentType),
              nameof(MemberDocumentModel.DocumentFileSize),
            ],
          },
          new ExcelWorksheetExportModel
          {
            Name = "Branch Contacts",
            Data = memberDetails.SelectMany(x => x.Branches).SelectMany(x => x.Contacts),
            PropertyNames =
            [
              nameof(MemberBranchContactModel.Id),
              nameof(MemberBranchContactModel.MemberId),
              nameof(MemberBranchContactModel.MemberBranchId),
              nameof(MemberBranchContactModel.Member),
              nameof(MemberBranchContactModel.MemberBranch),
              nameof(MemberBranchContactModel.MemberContactTypeId),
              nameof(MemberBranchContactModel.MemberContactType),
              nameof(MemberBranchContactModel.Title),
              nameof(MemberBranchContactModel.FirstName),
              nameof(MemberBranchContactModel.Surname),
              nameof(MemberBranchContactModel.TelephoneNo),
              nameof(MemberBranchContactModel.MobileNo),
              nameof(MemberBranchContactModel.EmailAddress),
              nameof(MemberBranchContactModel.CompanyContact),
              nameof(MemberBranchContactModel.Designation),
            ],
          },
          new ExcelWorksheetExportModel
          {
            Name = "Committees",
            Data = memberDetails.SelectMany(x => x.Committees),
            PropertyNames =
            [
              nameof(MemberCommitteeModel.Id),
              nameof(MemberCommitteeModel.MemberId),
              nameof(MemberCommitteeModel.Member),
              nameof(MemberCommitteeModel.Committee),
              nameof(MemberCommitteeModel.MemberName),
              nameof(MemberCommitteeModel.Name),
              nameof(MemberCommitteeModel.MemberBranch),
              nameof(MemberCommitteeModel.MemberContactType),
              nameof(MemberCommitteeModel.EmailAddress),
            ],
          },
          new ExcelWorksheetExportModel
          {
            Name = "Users",
            Data = memberDetails.SelectMany(x => x.Users),
            PropertyNames =
            [
              nameof(MemberUserModel.Id),
              nameof(MemberUserModel.MemberId),
              nameof(MemberUserModel.Member),
              nameof(MemberUserModel.FirstName),
              nameof(MemberUserModel.LastName),
              nameof(MemberUserModel.Email),
              nameof(MemberUserModel.CanEdit),
              nameof(MemberUserModel.IsActive),
              nameof(MemberUserModel.LastLogin),
            ],
          },
        ],
      });

      return File(workbook.Content, workbook.ContentType, workbook.FileName);
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await wimsApplicationLayer.GetMember(id).ConfigureAwait(false);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(MemberModel memberModel)
    {
      var result = await wimsApplicationLayer.SaveOrUpdate(memberModel).ConfigureAwait(false);
      return RedirectToAction("Details", new { id = memberModel.Id });
    }

    public IActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(MemberModel memberModel)
    {
      var result = await wimsApplicationLayer.SaveOrUpdate(memberModel).ConfigureAwait(false);
      return RedirectToAction("Details", new { id = result.SavedObjectId });
    }

    public async Task<IActionResult> DetailsBranch(int id)
    {
      var result = await wimsApplicationLayer.GetMemberBranch(id).ConfigureAwait(false);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
      var result = await wimsApplicationLayer.DeleteMember(id).ConfigureAwait(false);
      return RedirectToAction("Index");
    }

    public IActionResult CreateBranch(int memberId)
    {
      var branch = new MemberBranchModel { MemberId = memberId };
      return View(branch);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBranch(MemberBranchModel memberBranchModel)
    {

      var result = await wimsApplicationLayer.SaveOrUpdate(memberBranchModel).ConfigureAwait(false);

      var afps = await _wimsApplicationLayer.GetAnimalFeedProducts().ConfigureAwait(false);

      foreach (var afp in afps)
      {
        if (Request.Form[$"afp_{afp.Id}"] == "on")
        {
          await wimsApplicationLayer.SaveOrUpdate(new MemberBranchAnimalFeedProductModel
          {
            AnimalFeedProductId = afp.Id,
            MemberBranchId = result.SavedObjectId!.Value
          }).ConfigureAwait(false);
        }
      }

      var sps = await _wimsApplicationLayer.GetServiceProviders().ConfigureAwait(false);

      foreach (var sp in sps)
      {
        if (Request.Form[$"sp_{sp.Id}"] == "on")
        {
          await wimsApplicationLayer.SaveOrUpdate(new MemberBranchServiceProviderModel
          {
            ServiceProviderId = sp.Id,
            MemberBranchId = result.SavedObjectId!.Value
          }).ConfigureAwait(false);
        }
      }

      var services = await _wimsApplicationLayer.GetServices().ConfigureAwait(false);

      foreach (var ss in services)
      {
        if (Request.Form[$"ss_{ss.Id}"] == "on")
        {
          var r = await wimsApplicationLayer.SaveOrUpdate(new MemberBranchServiceModel
          {
            ServiceId = ss.Id,
            MemberBranchId = result.SavedObjectId!.Value
          }).ConfigureAwait(false);
        }
      }

      var manufacturers = await _wimsApplicationLayer.GetManufacturers().ConfigureAwait(false);

      foreach (var mf in manufacturers)
      {
        if (Request.Form[$"mf_{mf.Id}"] == "on")
        {
          var r = await wimsApplicationLayer.SaveOrUpdate(new MemberBranchManufacturerModel
          {
            ManufacturerId = mf.Id,
            MemberBranchId = result.SavedObjectId!.Value
          }).ConfigureAwait(false);
        }
      }

      var suppliers = await _wimsApplicationLayer.GetSuppliers().ConfigureAwait(false);

      foreach (var sr in suppliers)
      {
        if (Request.Form[$"sr_{sr.Id}"] == "on")
        {
          var r = await wimsApplicationLayer.SaveOrUpdate(new MemberBranchSupplierModel
          {
            SupplierId = sr.Id,
            MemberBranchId = result.SavedObjectId!.Value
          }).ConfigureAwait(false);
        }
      }

      return RedirectToAction("DetailsBranch", new { id = result.SavedObjectId!.Value });
    }

    public async Task<IActionResult> EditBranch(int id)
    {
      var result = await wimsApplicationLayer.GetMemberBranch(id).ConfigureAwait(false);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> EditBranch(MemberBranchModel memberBranchModel)
    {
      var result = await wimsApplicationLayer.SaveOrUpdate(memberBranchModel).ConfigureAwait(false);

      if (!result.Success)
      {
        return RedirectToAction("DetailsBranch", new { id = memberBranchModel.Id });
      }


      #region Animal Feed Products

      await _wimsApplicationLayer.DeleteAllMemberAnimalFeedProducts(memberBranchModel.Id).ConfigureAwait(false);

      var animalFeedProducts = await _wimsApplicationLayer.GetAnimalFeedProducts().ConfigureAwait(false);

      foreach (var afp in animalFeedProducts)
      {
        if (Request.Form[$"afp_{afp.Id}"] == "on")
        {
          await wimsApplicationLayer.SaveOrUpdate(new MemberBranchAnimalFeedProductModel
          {
            AnimalFeedProductId = afp.Id,
            MemberBranchId = result.SavedObjectId!.Value
          }).ConfigureAwait(false);
        }
      }

      #endregion

      #region Service Providers

      await _wimsApplicationLayer.DeleteAllServiceProviders(memberBranchModel.Id).ConfigureAwait(false);

      var serviceProviders = await _wimsApplicationLayer.GetServiceProviders().ConfigureAwait(false);

      foreach (var sp in serviceProviders)
      {
        if (Request.Form[$"sp_{sp.Id}"] == "on")
        {
          var r = await wimsApplicationLayer.SaveOrUpdate(new MemberBranchServiceProviderModel
          {
            ServiceProviderId = sp.Id,
            MemberBranchId = result.SavedObjectId!.Value
          }).ConfigureAwait(false);
        }
      }

      #endregion

      #region Services

      await _wimsApplicationLayer.DeleteAllServices(memberBranchModel.Id).ConfigureAwait(false);

      var services = await _wimsApplicationLayer.GetServices().ConfigureAwait(false);

      foreach (var ss in services)
      {
        if (Request.Form[$"ss_{ss.Id}"] == "on")
        {
          var r = await wimsApplicationLayer.SaveOrUpdate(new MemberBranchServiceModel
          {
            ServiceId = ss.Id,
            MemberBranchId = result.SavedObjectId!.Value
          }).ConfigureAwait(false);
        }
      }

      #endregion

      #region Manufacturers

      await _wimsApplicationLayer.DeleteAllManufacturers(memberBranchModel.Id).ConfigureAwait(false);

      var manufacturers = await _wimsApplicationLayer.GetManufacturers().ConfigureAwait(false);

      foreach (var mf in manufacturers)
      {
        if (Request.Form[$"mf_{mf.Id}"] == "on")
        {
          var r = await wimsApplicationLayer.SaveOrUpdate(new MemberBranchManufacturerModel
          {
            ManufacturerId = mf.Id,
            MemberBranchId = result.SavedObjectId!.Value
          }).ConfigureAwait(false);
        }
      }

      #endregion

      #region Suppliers

      await _wimsApplicationLayer.DeleteAllSuppliers(memberBranchModel.Id).ConfigureAwait(false);

      var suppliers = await _wimsApplicationLayer.GetSuppliers().ConfigureAwait(false);

      foreach (var sr in suppliers)
      {
        if (Request.Form[$"sr_{sr.Id}"] == "on")
        {
          var r = await wimsApplicationLayer.SaveOrUpdate(new MemberBranchSupplierModel
          {
            SupplierId = sr.Id,
            MemberBranchId = result.SavedObjectId!.Value
          }).ConfigureAwait(false);
        }
      }

      #endregion

      return RedirectToAction("DetailsBranch", new { id = memberBranchModel.Id });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteBranch(DeleteViewModel deleteViewModel)
    {
      var result = await wimsApplicationLayer.DeleteBranch(deleteViewModel.Id).ConfigureAwait(false);
      return RedirectToAction("Details", new { id = deleteViewModel.ParentId });
    }

    public IActionResult CreateBranchContact(int memberId, int branchId)
    {
      var branch = new MemberBranchContactModel { MemberBranchId = branchId, MemberId = memberId };
      return View(branch);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBranchContact(MemberBranchContactModel memberBranchContactModel)
    {
      var result = await wimsApplicationLayer.SaveOrUpdate(memberBranchContactModel).ConfigureAwait(false);
      return RedirectToAction("DetailsBranch", new { id = memberBranchContactModel.MemberBranchId });
    }

    public async Task<IActionResult> EditBranchContact(int id)
    {
      var result = await wimsApplicationLayer.GetMemberBranchContact(id).ConfigureAwait(false);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> EditBranchContact(MemberBranchContactModel memberBranchContactModel)
    {
      var result = await wimsApplicationLayer.SaveOrUpdate(memberBranchContactModel).ConfigureAwait(false);
      return RedirectToAction("DetailsBranch", new { id = memberBranchContactModel.MemberBranchId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteBranchContact(int deleteBranchContactId, int parentId)
    {
      var result = await wimsApplicationLayer.DeleteBranchContact(deleteBranchContactId).ConfigureAwait(false);
      return RedirectToAction("DetailsBranch", new { id = parentId });
    }

    public async Task<IActionResult> GetMemberBranches(int memberId)
    {
      var result = await wimsApplicationLayer.GetMemberBranches(memberId).ConfigureAwait(false);
      return Json(result.OrderBy(o => o.BranchName));
    }

    public async Task<IActionResult> GetMemberBranchContacts(int memberBranchId)
    {
      var result = await wimsApplicationLayer.GetMemberBranchContacts(memberBranchId).ConfigureAwait(false);
      return Json(result.OrderBy(o => o.EmailAddress));
    }

    public async Task<IActionResult> GetMemberBranchContact(int memberBranchContactId)
    {
      var result = await wimsApplicationLayer.GetMemberBranchContact(memberBranchContactId).ConfigureAwait(false);
      return Json(result);
    }

    public async Task<IActionResult> GetMemberAccounts(int memberId)
    {
      var result = await wimsApplicationLayer.GetMemberContacts(memberId).ConfigureAwait(false);
      return Json(result);
    }

    public async Task<IActionResult> GetMemberAccountContacts(int memberId)
    {
      var result = await wimsApplicationLayer.GetMemberContacts(memberId).ConfigureAwait(false);
      return Json(result);
    }

    public async Task<IActionResult> CreateBranchFromMember(int id)
    {
      var result = await wimsApplicationLayer.CreateBranchFromMember(id).ConfigureAwait(false);
      return RedirectToAction("DetailsBranch", new { id = result });
    }

    public async Task<IActionResult> CreateAllBranchFromMember()
    {
      var result = await wimsApplicationLayer.CreateBranchFromMember().ConfigureAwait(false);
      return RedirectToAction("DetailsBranch", new { id = result });
    }

    public IActionResult CreateDocument(int memberId)
    {
      return View(new MemberDocumentModel { MemberId = memberId });
    }

    [HttpPost]
    public async Task<IActionResult> CreateDocument(IFormFile documentContent, MemberDocumentModel memberDocumentModel)
    {
      const long maxFileSize = 15 * 1024 * 1024; // 15 MB

      if (documentContent.Length > maxFileSize)
      {
        ModelState.AddModelError("", "The document must not exceed 15 MB.");
        return View(memberDocumentModel);
      }

      using var memoryStream = new MemoryStream();
      await documentContent.CopyToAsync(memoryStream);

      memberDocumentModel.DocumentContent = memoryStream.ToArray();
      memberDocumentModel.DocumentFileName = Path.GetFileName(documentContent.FileName);
      memberDocumentModel.DocumentContentType = documentContent.ContentType;
      memberDocumentModel.DocumentFileSize = (int)documentContent.Length;

      var result = await _wimsApplicationLayer.SaveOrUpdate(memberDocumentModel).ConfigureAwait(false);

      return RedirectToAction("Details", new { id = memberDocumentModel.MemberId });
    }

    public async Task<IActionResult> EditDocument(int id)
    {
      var result = await wimsApplicationLayer.GetMemberDocument(id).ConfigureAwait(false);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> EditDocument(IFormFile documentContent, MemberDocumentModel memberDocumentModel)
    {
      var result = await wimsApplicationLayer.GetMemberDocument(memberDocumentModel.Id).ConfigureAwait(false);
      result.Name = memberDocumentModel.Name;
      result.DocumentTypeId = memberDocumentModel.DocumentTypeId;
      result.ExpiryDate = memberDocumentModel.ExpiryDate;

      if (documentContent != null)
      {
        using var memoryStream = new MemoryStream();
        await documentContent.CopyToAsync(memoryStream);
        result.DocumentContent = memoryStream.ToArray();
        result.DocumentFileName = Path.GetFileName(documentContent.FileName);
        result.DocumentContentType = documentContent.ContentType;
        result.DocumentFileSize = (int)documentContent.Length;
      }

      var update = await _wimsApplicationLayer.Merge(result).ConfigureAwait(false);

      return RedirectToAction("Details", new { id = memberDocumentModel.MemberId });
    }


    [HttpPost]
    public async Task<IActionResult> DeleteDocument(int deleteDocumentId, int parentId)
    {
      var result = await wimsApplicationLayer.DeleteMemberDocument(deleteDocumentId).ConfigureAwait(false);
      return RedirectToAction("Details", new { id = parentId });
    }

    public async Task<IActionResult> DownloadDocument(int id)
    {
      var result = await _wimsApplicationLayer.GetMemberDocument(id).ConfigureAwait(false);
      return File(result.DocumentContent, result.DocumentContentType, result.DocumentFileName);
    }

    public async Task<FileContentResult> OpenDocument(int id)
    {
      var result = await _wimsApplicationLayer.GetMemberDocument(id).ConfigureAwait(false);
      return new FileContentResult(result.DocumentContent, result.DocumentContentType);
    }

    public IActionResult CreateCommittee(int memberId)
    {
      return View(new MemberCommitteeModel { MemberId = memberId });
    }

    [HttpPost]
    public async Task<IActionResult> CreateCommittee(MemberCommitteeModel memberCommitteeModel)
    {
      var result = await wimsApplicationLayer.SaveOrUpdate(memberCommitteeModel).ConfigureAwait(false);
      return RedirectToAction("Details", new { id = memberCommitteeModel.MemberId });
    }

    public async Task<IActionResult> EditCommittee(int id)
    {
      var result = await wimsApplicationLayer.GetCommitteeMember(id).ConfigureAwait(false);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> EditCommittee(MemberCommitteeModel memberCommitteeModel)
    {
      var result = await wimsApplicationLayer.SaveOrUpdate(memberCommitteeModel).ConfigureAwait(false);
      return RedirectToAction("Details", new { id = memberCommitteeModel.MemberId });
    }

    public async Task<IActionResult> DetailsCommittee(int id)
    {
      var result = await _wimsApplicationLayer.GetCommitteeMember(id).ConfigureAwait(false);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCommittee(DeleteViewModel deleteViewModel)
    {
      var result = await wimsApplicationLayer.DeleteMemberCommittee(deleteViewModel.Id).ConfigureAwait(false);
      return RedirectToAction("Details", new { id = deleteViewModel.ParentId });
    }

    public IActionResult CreateUser(int memberId)
    {
      return View(new MemberUserModel { MemberId = memberId });
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(MemberUserModel memberUserModel)
    {
      var userResult = await wimsApplicationLayer.SaveOrUpdateMemberUser(memberUserModel).ConfigureAwait(false);

      memberUserModel.UserId = userResult.SavedObjectId?.ToString();

      if (userResult.Success)
      {
        if (!userResult.Success)
        {
          ModelState.AddModelError(string.Empty, $"User creation failed: {userResult.ErrorMessage}");
          return View(memberUserModel);
        }
      }
      else
      {
        ModelState.AddModelError(string.Empty, $"Member user creation failed: {userResult.ErrorMessage}");
        return View(memberUserModel);
      }

      return RedirectToAction("Details", new { id = memberUserModel.MemberId });
    }

    public async Task<IActionResult> DetailsUser(int id)
    {
      var result = await _wimsApplicationLayer.GetMemberUser(id).ConfigureAwait(false);
      return View(result);
    }

    public async Task<IActionResult> EditUser(int id)
    {
      var result = await _wimsApplicationLayer.GetMemberUser(id).ConfigureAwait(false);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> EditUser(MemberUserModel memberUserModel)
    {
      var memberUser = await wimsApplicationLayer.GetMemberUser(memberUserModel.Id).ConfigureAwait(false);
      memberUser.FirstName = memberUserModel.FirstName;
      memberUser.LastName = memberUserModel.LastName;
      memberUser.Email = memberUserModel.Email;
      memberUser.CanEdit = memberUserModel.CanEdit;
      memberUser.IsActive = memberUserModel.IsActive;

      var userResult = await wimsApplicationLayer.SaveOrUpdateMemberUser(memberUser).ConfigureAwait(false);

      await _wimsApplicationLayer.DeleteAllMemberBranchUsers(memberUserModel.Id).ConfigureAwait(false);

      var memberBranches = await _wimsApplicationLayer.GetMemberBranches(memberUser.MemberId).ConfigureAwait(false);

      foreach (var mb in memberBranches)
      {
        if (Request.Form[$"mub_{mb.Id}"] == "on")
        {
          await wimsApplicationLayer.SaveOrUpdate(new MemberUserBranchModel
          {
            MemberUserId = memberUserModel.Id,
            CanEdit = true,
            MemberBranchId = mb.Id,
          }).ConfigureAwait(false);
        }
      }

      return RedirectToAction("DetailsUser", new { id = memberUserModel.Id });
    }

    [HttpPost]
    public async Task<IActionResult> ResetMemberUserPassword(int id)
    {
      var result = await _wimsApplicationLayer.ResetMemberUserPassword(id).ConfigureAwait(false);

      if (result.Success)
      {
        TempData["SuccessMessage"] = "Password reset successfully.";
      }
      else
      {
        TempData["ErrorMessage"] = $"Password reset failed: {result.ErrorMessage}";
      }

      return RedirectToAction("DetailsUser", new { id });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMemberUser(DeleteViewModel deleteViewModel)
    {
      var result = await wimsApplicationLayer.DeleteMemberUser(deleteViewModel.Id).ConfigureAwait(false);
      return RedirectToAction("Details", new { id = deleteViewModel.ParentId });
    }

    private static ExcelWorksheetExportModel CreateBranchWorksheet(IEnumerable<MemberBranchModel> branches)
    {
      var branchRows = CreateBranchExportRows(branches, out var branchColumns);

      return new ExcelWorksheetExportModel
      {
        Name = "Branches",
        Data = branchRows,
        PropertyNames = branchColumns,
      };
    }

    private static List<Dictionary<string, object>> CreateBranchExportRows(IEnumerable<MemberBranchModel> branches, out List<string> branchColumns)
    {
      var branchList = branches.ToList();
      branchColumns =
      [
        "Id",
        "Member Id",
        "Member",
        "Membership Number",
        "Branch Name",
        "Province",
        "Physical Address",
        "Postal Address",
        "Telephone Number",
        "Date Of Membership",
        "Other Member ID",
        "Date Of Issue",
        "Comments",
        "GPS Coordinates",
      ];

      var usedColumns = new HashSet<string>(branchColumns, StringComparer.OrdinalIgnoreCase);
      var animalFeedColumns = CreateBranchOptionColumns(branchList.SelectMany(x => x.AnimalFeedProducts).Select(x => x.AnimalFeedProduct), "Animal Feed Product", usedColumns);
      var serviceProviderColumns = CreateBranchOptionColumns(branchList.SelectMany(x => x.ServiceProviders).Select(x => x.ServiceProvider), "Affiliate Member Types", usedColumns);
      var serviceColumns = CreateBranchOptionColumns(branchList.SelectMany(x => x.Services).Select(x => x.Service), "Service", usedColumns);
      var manufacturerColumns = CreateBranchOptionColumns(branchList.SelectMany(x => x.Manufacturers).Select(x => x.Manufacturer), "Manufacturer", usedColumns);
      var supplierColumns = CreateBranchOptionColumns(branchList.SelectMany(x => x.Suppliers).Select(x => x.Supplier), "Supplier", usedColumns);

      branchColumns.AddRange(animalFeedColumns.Select(x => x.ColumnName));
      branchColumns.AddRange(serviceProviderColumns.Select(x => x.ColumnName));
      branchColumns.AddRange(serviceColumns.Select(x => x.ColumnName));
      branchColumns.AddRange(manufacturerColumns.Select(x => x.ColumnName));
      branchColumns.AddRange(supplierColumns.Select(x => x.ColumnName));

      return branchList.Select(branch =>
      {
        var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
          ["Id"] = branch.Id,
          ["Member Id"] = branch.MemberId,
          ["Member"] = branch.Member,
          ["Membership Number"] = branch.MemberIdentificationId,
          ["Branch Name"] = branch.BranchName,
          ["Province"] = branch.Province,
          ["Physical Address"] = branch.PhysicalAddress,
          ["Postal Address"] = branch.PostalAddress,
          ["Telephone Number"] = branch.TelephoneNumber,
          ["Date Of Membership"] = branch.DateOfMembership,
          ["Other Member ID"] = branch.OtherMemberIdentificationId,
          ["Date Of Issue"] = branch.DateOfIssue,
          ["Comments"] = branch.Comments,
          ["GPS Coordinates"] = branch.GpsCoordinates,
        };

        if (branch.AnimalFeedProducts.Any())
        {
          foreach (var animalFeedColumn in animalFeedColumns)
          {
            row[animalFeedColumn.ColumnName] = branch.AnimalFeedProducts.Any(x => string.Equals(x.AnimalFeedProduct, animalFeedColumn.MatchValue, StringComparison.OrdinalIgnoreCase))
              ? "X"
              : string.Empty;
          }
        }

        if (branch.ServiceProviders.Any())
        {
          foreach (var serviceProviderColumn in serviceProviderColumns)
          {
            row[serviceProviderColumn.ColumnName] = branch.ServiceProviders.Any(x => string.Equals(x.ServiceProvider, serviceProviderColumn.MatchValue, StringComparison.OrdinalIgnoreCase))
              ? "X"
              : string.Empty;
          }
        }

        if (branch.Services.Any())
        {
          foreach (var serviceColumn in serviceColumns)
          {
            row[serviceColumn.ColumnName] = branch.Services.Any(x => string.Equals(x.Service, serviceColumn.MatchValue, StringComparison.OrdinalIgnoreCase))
              ? "X"
              : string.Empty;
          }
        }

        if (branch.Manufacturers.Any())
        {
          foreach (var manufacturerColumn in manufacturerColumns)
          {
            row[manufacturerColumn.ColumnName] = branch.Manufacturers.Any(x => string.Equals(x.Manufacturer, manufacturerColumn.MatchValue, StringComparison.OrdinalIgnoreCase))
              ? "X"
              : string.Empty;
          }
        }

        if (branch.Suppliers.Any())
        {
          foreach (var supplierColumn in supplierColumns)
          {
            row[supplierColumn.ColumnName] = branch.Suppliers.Any(x => string.Equals(x.Supplier, supplierColumn.MatchValue, StringComparison.OrdinalIgnoreCase))
              ? "X"
              : string.Empty;
          }
        }

        return row;

      }).ToList();
    }

    private static List<BranchOptionColumn> CreateBranchOptionColumns(IEnumerable<string> values, string prefix, HashSet<string> usedColumns)
    {
      var columns = new List<BranchOptionColumn>();

      foreach (var value in values
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .OrderBy(x => x))
      {
        var baseColumnName = $"{prefix} - {value}";
        var columnName = baseColumnName;
        var suffix = 2;

        while (!usedColumns.Add(columnName))
        {
          columnName = $"{baseColumnName} ({suffix})";
          suffix++;
        }

        columns.Add(new BranchOptionColumn(value, columnName));
      }

      return columns;
    }

    private sealed record BranchOptionColumn(string MatchValue, string ColumnName);
  }
}

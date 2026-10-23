namespace AFMA.Stats.Admin.Controllers
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.Stats.Admin.ControllerExtensions;
  using Alpha.Services.Models.Data.PageData;
  using ApplicationLayer.Helpers;
  using ApplicationLayer.Model;
  using ApplicationLayer.ViewModels;
  using AspNetCoreHero.ToastNotification.Abstractions;
  using ControllerExtensions;
  using Data.Entities;
  using Infrastructure.Settings;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Options;
  using X.PagedList;

  /// <summary>
  /// The member controller class.
  /// </summary>
  [Authorize(Roles = "Admin")]
  public class MemberController : BaseController
  {
    private readonly ILogger<MemberController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly ISalmonellaLayer _salmonellaLayer;
    private readonly IIdentityLayer _identityLayer;
    private readonly INotyfService _notyf;
    private readonly IOptions<ApplicationSettings> _applicationSettings;
    private readonly INotificationService _notificationService;

    public MemberController(ILogger<MemberController> logger, IOptions<ApplicationSettings> applicationSettings, IApplicationLayer applicationLayer, ISalmonellaLayer salmonellaLayer, IIdentityLayer identityLayer, INotyfService notyf, INotificationService notificationService)
    {
      _logger = logger;
      _applicationSettings = applicationSettings;
      _applicationLayer = applicationLayer;
      _salmonellaLayer = salmonellaLayer;
      _identityLayer = identityLayer;
      _notyf = notyf;
      _notificationService = notificationService;
    }

    /// <summary>
    /// Gets a list of members.
    /// </summary>
    /// <returns>The members.</returns>
    public async Task<IActionResult> Index(int? page, int? filterMemberTypeId, string? filterSearchText)
    {
      var pageNo = page ?? 1;
      var members = await _applicationLayer
        .GetMembersSummary(new AlphaPageRequest(pageNo, _applicationSettings.Value.PageSize, "Name"), filterMemberTypeId, filterSearchText)
        .ConfigureAwait(false);
      _logger.LogWarning($"{members.Count()} members found.");
      return View(members);
    }

    public async Task<IActionResult> Details(int id)
    {
      var result = await _applicationLayer.GetMember(id);
      return View(result);
    }

    public async Task<IActionResult> Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(MemberModel memberModel)
    {
      memberModel.CreatedDate = DateTime.Now;
      var result = await _applicationLayer.SaveOrUpdate(memberModel);

      if (result.Success)
      {
        _notyf.Success("Member created successfully");
      }
      else
      {
        _notyf.Error($"Member not created: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", "Member", new { id = result.SavedObjectId });
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await _applicationLayer.GetMember(id);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(MemberModel memberModel, IFormCollection collection)
    {
      memberModel.UpdatedDate = DateTime.Now;

      var result = await _applicationLayer.SaveOrUpdate(memberModel);

      var memberModules = _applicationLayer.GetModules().Result.Select(m => new MemberModuleModel
      {
        ModuleId = m.Id,
        MemberId = memberModel.Id,
        Selected = collection.Keys.Any(k => k.Contains($"ModuleId_{m.Id}")),
      });

      _applicationLayer.UpdateMemberModules(memberModel.Id, memberModules);

      if (result.Success)
      {
        _notyf.Success("Member updated successfully");
      }
      else
      {
        _notyf.Error($"Member not updated: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", "Member", new { id = result.SavedObjectId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMember(int memberId)
    {
      var result = await _applicationLayer.DeleteMember(memberId).ConfigureAwait(false);
      
      if (result.Succeeded)
      {
        _notyf.Success("Member deleted successfully");
      }
      else
      {
        _notyf.Error($"Member could not be deleted: {result.ExceptionMessage}");
      }

      return RedirectToAction("Index","Member");
    }

    public async Task<IActionResult> BranchDetails(int id)
    {
      var result = await _applicationLayer.GetMemberBranch(id);
      return View(result);
    }

    public async Task<IActionResult> CreateBranch(int memberId)
    {
      return View(new MemberBranchModel { MemberId = memberId });
    }

    [HttpPost]
    public async Task<IActionResult> CreateBranch(MemberBranchModel memberBranchModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(memberBranchModel);

      if (result.Success)
      {
        _notyf.Success("Member Branch updated successfully");
      }
      else
      {
        _notyf.Error($"Member Branch not updated: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", "Member", new { id = memberBranchModel.MemberId });
    }

    public async Task<IActionResult> EditBranch(int id)
    {
      var result = await _applicationLayer.GetMemberBranch(id);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> EditBranch(MemberBranchModel memberBranchModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(memberBranchModel);

      if (result.Success)
      {
        _notyf.Success("Member Branch updated successfully");
      }
      else
      {
        _notyf.Error($"Member Branch not updated: {result.ErrorMessage}");
      }

      return RedirectToAction("BranchDetails", "Member", new { id = memberBranchModel.Id });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMemberBranch(int memberBranchId, int memberId)
    {
      var result = await _applicationLayer.DeleteMemberBranch(memberBranchId,memberId).ConfigureAwait(false);

      if (result.Succeeded)
      {
        _notyf.Success("Branch deleted successfully");
      }
      else
      {
        _notyf.Error($"Branch could not be deleted: {result.ExceptionMessage}");
      }

      return RedirectToAction("Details", "Member", new { id = memberId });
    }

    [Route("member/accountdetails/{id}")]
    public async Task<IActionResult> AccountDetails(int id)
    {
      var result = await _applicationLayer.GetMemberAccount(id);
      return View(result);
    }

    [Route("member/accountdetails")]
    public async Task<IActionResult> AccountDetails(string userId)
    {
      var result = await _applicationLayer.GetMemberAccount(userId);
      return View(result);
    }

    public async Task<IActionResult> CreateAccount(int memberId)
    {
      var newPassword = PasswordGenerator.GeneratePassword(true, true, true, true, false, 8);
      
      var memberAccountModel = new MemberAccountModel
      {
        MemberId = memberId,
        Password = $"{newPassword}dA$",
      };
      
      return View(memberAccountModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount(MemberAccountModel memberAccountModel)
    {
      try
      {
        var userCreateResult = await _identityLayer.Identity.CreateUserAsync(new ApplicationUser
        {
          CreatedDate = DateTime.Now,
          Email = memberAccountModel.Email,
          EmailConfirmed = true,
          FirstName = memberAccountModel.FirstName,
          IsActive = true,
          LastName = memberAccountModel.LastName,
          ReceivedEmail = true,
          UserName = memberAccountModel.UserName
        }, memberAccountModel.Password);

        if (!userCreateResult.Succeeded)
        {
          _notyf.Error($"User could not be created: {userCreateResult.Errors.First().Description}");
          return RedirectToAction("Details", "Member", new { id = memberAccountModel.MemberId });
        }

        var user = await _identityLayer.Identity.GetUserByUsernameAsync(memberAccountModel.UserName);
        memberAccountModel.UserId = user.Id;
        var result = await _applicationLayer.SaveOrUpdate(memberAccountModel);

        if (memberAccountModel.IsAdministrator)
        {
          await _identityLayer.Identity.AddUserToRoleAsync(user.Id, "Admin");
        }
        else
        {
          await _identityLayer.Identity.AddUserToRoleAsync(user.Id, "User");
        }

        if (memberAccountModel.SalmonellaAccess && result.Success)
        {
          await _applicationLayer.AddMemberAccountToModule((int)result.SavedObjectId!, 2);
        }

        if (memberAccountModel.FeedsalesAccess && result.Success)
        {
          await _applicationLayer.AddMemberAccountToModule((int)result.SavedObjectId!, 1);
        }

        if (memberAccountModel.IndustryProfileAccess && result.Success)
        {
          await _applicationLayer.AddMemberAccountToModule((int)result.SavedObjectId!, 3);
        }

        if (memberAccountModel.MycotoxinsAccess && result.Success)
        {
          await _applicationLayer.AddMemberAccountToModule((int)result.SavedObjectId!, 1002);
        }

        if (result.Success)
        {
          _notyf.Success("Member Account created successfully");
        }
        else
        {
          _notyf.Error($"Member Account not created: {result.ErrorMessage}");
        }

        return RedirectToAction("Details", "Member", new { id = memberAccountModel.MemberId });
      }
      catch(Exception ex)
      {
        _notyf.Error($"Member Account not created: {ex.Message}");
        return RedirectToAction("Details", "Member", new { id = memberAccountModel.MemberId, error = ex.Message });
      }
    }

    public async Task<IActionResult> EditAccount(int id)
    {
      var result = await _applicationLayer.GetMemberAccount(id);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> EditAccount(MemberAccountModel memberAccountModel)
    {
      var user = await _identityLayer.Identity.GetUserByUsernameAsync(memberAccountModel.UserName);
      user.Email = memberAccountModel.Email;
      user.FirstName = memberAccountModel.FirstName;
      user.LastName = memberAccountModel.LastName;
      user.IsActive = memberAccountModel.Active;
      var userUpdateResult = await _identityLayer.Identity.UpdateUserAsync(user);

      if (!userUpdateResult.Succeeded)
      {
        _notyf.Error($"User could not be updated: {userUpdateResult.Errors.First().Description}");
        return RedirectToAction("Details", "Member", new { id = memberAccountModel.MemberId });
      }

      var result = await _applicationLayer.SaveOrUpdate(memberAccountModel);

      if (memberAccountModel.IsAdministrator)
      {
        await _identityLayer.Identity.AddUserToRoleAsync(user.Id, "Admin");
        await _identityLayer.Identity.RemoveUserFromRoleAsync(user.Id, "User");
      }
      else
      {
        await _identityLayer.Identity.AddUserToRoleAsync(user.Id, "User");
        await _identityLayer.Identity.RemoveUserFromRoleAsync(user.Id, "Admin");
      }

      if (memberAccountModel.SalmonellaAccess)
      {
        await _applicationLayer.AddMemberAccountToModule((int)result.SavedObjectId!, 2);
      }
      else
      {
        await _applicationLayer.RemoveMemberAccountFromModule((int)result.SavedObjectId!, 2);
      }

      if (memberAccountModel.FeedsalesAccess)
      {
        await _applicationLayer.AddMemberAccountToModule((int)result.SavedObjectId!, 1);
      }
      else
      {
        await _applicationLayer.RemoveMemberAccountFromModule((int)result.SavedObjectId!, 1);
      }

      if (memberAccountModel.IndustryProfileAccess)
      {
        await _applicationLayer.AddMemberAccountToModule((int)result.SavedObjectId!, 3);
      }
      else
      {
        await _applicationLayer.RemoveMemberAccountFromModule((int)result.SavedObjectId!, 3);
      }

      if (memberAccountModel.MycotoxinsAccess)
      {
        await _applicationLayer.AddMemberAccountToModule((int)result.SavedObjectId!, 1002);
      }
      else
      {
        await _applicationLayer.RemoveMemberAccountFromModule((int)result.SavedObjectId!, 1002);
      }

      if (result.Success)
      {
        _notyf.Success("Member Account updated successfully");
      }
      else
      {
        _notyf.Error($"Member Account not updated: {result.ErrorMessage}");
      }

      return RedirectToAction("AccountDetails", "Member", new { id = memberAccountModel.Id });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAccount(int memberAccountId, int memberId)
    {
      var result = await _applicationLayer.DeleteMemberAccount(memberAccountId, memberId).ConfigureAwait(false);

      if (result.Succeeded)
      {
        _notyf.Success("Account deleted successfully");
      }
      else
      {
        _notyf.Error($"Account could not be deleted: {result.ExceptionMessage}");
      }

      return RedirectToAction("Details", "Member", new { id = memberId });
    }

    public async Task<IActionResult> GetMemberBranches(int memberId)
    {
      var result = await _applicationLayer.GetMemberBranchesSummary(memberId).ConfigureAwait(false);
      return Json(result.OrderBy(o => o.Name));
    }

    public async Task<IActionResult> GetMemberBranchAccounts(int branchId, ModuleEnum module)
    {
      var result = await _applicationLayer.GetMemberBranchAccounts(branchId, module);
      return Json(result);
    }

    public async Task<IActionResult> GetFeedsalesReportSummary(int? memberId, int? branchId, int? accountId, int? currentMonth, int? currentYear)
    {
      var reports = await _applicationLayer
        .GetFeedsalesReportsWithItems(memberId, branchId, accountId, null, null, DateTime.Now.Year)
        .ConfigureAwait(false);

      var result = new FeedsalesReportSummaryViewModel
      {
        TotalReports = reports?.Count() ?? 0,
        TotalOutstanding = reports?.Count(x => !x.IsSubmitted) ?? 0,
        TotalSubmitted = reports?.Count(x => x.IsSubmitted) ?? 0,
        TotalFinishedProduct = Convert.ToDecimal(reports?.Sum(fp => fp.FinishedProductTotal)),
        TotalRawMaterial = Convert.ToDecimal(reports?.Sum(fp => fp.RawMaterialTotal)),
        TotalOverdue = reports?.Count(x => x.DueDate < DateTime.Now && !x.IsSubmitted && x.ReportYear == DateTime.Now.Year) ?? 0,
      };

      return Json(result);
    }

    public async Task<IActionResult> GetSalmonellaReportSummary(int? memberId, int? branchId, int? accountId, int? currentMonth, int? currentYear)
    {
      currentYear ??= DateTime.Now.Year;
      
      var reports =
        await _salmonellaLayer.GetSalmonellaReportsLookup(memberId, branchId, null, null, null,
          DateTime.Now.Year);

      var reportItemsSummary = await _salmonellaLayer.GetSalmonellaReportSamples((int)currentYear, memberId, branchId);

      var result = new SalmonellaSummaryViewModel
      {
        TotalReports = reports?.Count() ?? 0,
        TotalOutstanding = reports?.Count(x => !x.IsSubmitted) ?? 0,
        TotalSubmitted = reports?.Count(x => x.IsSubmitted) ?? 0,
        TotalPositives = reportItemsSummary.Count(x => x.SalmonellaDetected),
        TotalOverdue = reports?.Count(x => x.DueDate < DateTime.Now && !x.IsSubmitted && x.ReportYear == currentYear) ?? 0,
      };

      return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> ResetAccountPassword(string userId, int memberAccountId)
    {
      var result = await _notificationService.SendResetPasswordNotification(userId).ConfigureAwait(false);
      
      if (result)
      {
        _notyf.Success("Password reset successful");
      }
      else
      {
        _notyf.Error("Password could not be reset");
      }

      return RedirectToAction("AccountDetails", "Member", new { id = memberAccountId, result });
    }

    [HttpPost]
    public async Task<IActionResult> ChangeAccountPassword(string userId, int memberAccountId, string password)
    {
      var result = await _notificationService.SendChangePasswordNotification(userId, password).ConfigureAwait(false);

      if (result)
      {
        _notyf.Success("Password change successful");
      }
      else
      {
        _notyf.Error("Password could not be changed");
      }

      return RedirectToAction("AccountDetails", "Member", new { id = memberAccountId, result });
    }

  }
}

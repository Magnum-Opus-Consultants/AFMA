namespace AFMA.ApplicationLayer
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.Data.Entities;
  using AFMA.Infrastructure.Options;
  using Alpha.Services.Extensions.Attributes;
  using Alpha.Services.Models.Data.PageData;
  using Alpha.Services.Models.Repository;
  using Alpha.Services.NHibernate.Repository.Services;
  using Alpha.Services.Notifications.Smtp;
  using Alpha.Services.Notifications.Templates;
  using AutoMapper;
  using Data.Entities.Interface;
  using Expressions;
  using Helpers;
  using Infrastructure.Settings;
  using Microsoft.AspNetCore.Mvc.Rendering;
  using Microsoft.Extensions.Caching.Memory;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.Options;
  using Razor.Templating.Core;
  using Serilog;
  using Shared.Infrastructure.Settings;
  using System.Reflection;
  using ViewModels;
  using X.PagedList;
  using Member = Data.Entities.Member;
  using Module = Data.Entities.Module;

  /// <summary>
  /// The ApplicationLayer class.
  /// </summary>
  public class ApplicationLayer : IApplicationLayer
  {
    private const string BranchReportSummaryCacheKey = "BranchReportSummary";

    private readonly IApplicationRepository<SessionFactory> _applicationRepository;
    private readonly ApplicationSettings _applicationSettings;
    private readonly SmtpSettings _smtpSettings;
    private readonly IMapper _mapper;
    private readonly IIdentityLayer _identityLayer;
    private readonly IEmailClient _emailClient;
    private readonly IAlphaNotificationTemplateService _notificationTemplateService;
    private readonly IRazorTemplateEngine _razorTemplateEngine;
    private readonly ILogger<ApplicationLayer> _logger;

    private IMemoryCache _memoryCache { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationLayer"/> class.
    /// </summary>
    /// <param name="applicationRepository">The application repository.</param>
    /// <param name="mapper">The Automapper interface..</param>
    /// <param name="logger">The logger.</param>
    /// <param name="emailClient">The Email Client</param>
    public ApplicationLayer(
      IApplicationRepository<SessionFactory> applicationRepository,
      IOptions<ApplicationSettings> applicationSettings,
      IOptions<SmtpSettings> smtpSettings,
      IMapper mapper,
      IIdentityLayer identityLayer,
      IEmailClient emailClient,
      IAlphaNotificationTemplateService notificationTemplateService,
      IRazorTemplateEngine razorTemplateEngine,
      IMemoryCache memoryCache,
      ILogger<ApplicationLayer> logger)
    {
      _applicationRepository = applicationRepository;
      _applicationSettings = applicationSettings.Value;
      _smtpSettings = smtpSettings.Value;
      _mapper = mapper;
      _identityLayer = identityLayer;
      _emailClient = emailClient;
      _notificationTemplateService = notificationTemplateService;
      _razorTemplateEngine = razorTemplateEngine;
      _memoryCache = memoryCache;
      _logger = logger;
    }

    #region Select Lists

    public async Task<IEnumerable<SelectListItem>> GenerateMaterialCategorySelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<MaterialCategory>();
      return results.OrderBy(o => o.Name).ToList().ToSelectList(x => x.Id, x => x.Name, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateSalmonellaReportGroupSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<SalmonellaReportGroup>();
      return results.OrderBy(o => o.Name).ToList().ToSelectList(x => x.Id, x => x.Name, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateFeedsalesReportGroupSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<FeedsalesReportGroup>();
      return results.OrderBy(o => o.Name).ToList().ToSelectList(x => x.Id, x => x.Name, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateProductGroupSelectList(int? selectedValue)
    {
      return await _applicationRepository.QuerySelectListAsync<ProductGroup>(
        q => q.Where(q => q.VisibleToMembers).OrderBy(d => d.DisplayOrder).ThenBy(n => n.Name), x => x.Id, x => $"{x.Code} - {x.Name}", selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateSalmonellaSampleTypeSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<SalmonellaSampleType>();
      return results.Where(x => x.Active).OrderBy(n => n.Name).ToList().ToSelectList(x => x.Id, x => x.Name, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateSalmonellaSampleTypeItemSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<SalmonellaSampleTypeItem>();
      return results.Where(x => x.Active).OrderBy(n => n.Name).ToList().ToSelectList(x => x.Id, x => x.Name, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateSalmonellaSampleTypeSubItemSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<SalmonellaSampleTypeSubItem>();
      return results.Where(x => x.Active).OrderBy(n => n.Name).ToList().ToSelectList(x => x.Id, x => x.Name, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateLaboratorySelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<Laboratory>();
      return results.Where(x => x.Active).OrderBy(n => n.Name).ToList().ToSelectList(x => x.Id, x => x.Name, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateProductSelectList(int productGroupId, int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<Product>();
      return results.Where(x => x.ProductGroup.Id == productGroupId).OrderBy(o => o.Name).ToList().ToSelectList(x => x.Id, x => $"{x.Code} - {x.Name}", selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateProductSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<Product>();
      return results.OrderBy(o => o.Name).ToList().ToSelectList(x => x.Id, x => $"{x.Code} - {x.Name}", selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateMaterialSelectList(int? selectedValue)
    {
      return await _applicationRepository.QuerySelectListAsync<Material>(
        q => q.Where(q => q.VisibleToMembers).OrderBy(d => d.DisplayOrder).ThenBy(n => n.Name), x => x.Id,
        x => $"{x.Code} - {x.Name}", selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateMaterialOriginSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<MaterialOrigin>();
      return results.OrderBy(n => n.Name).ToList().ToSelectList(x => x.Id, x => x.Name, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateProvinceSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<Province>();
      return results.OrderBy(o => o.Description).ToList().ToSelectList(x => x.Id, x => x.Description, selectedValue);
    }

    public IEnumerable<SelectListItem> GenerateReportStatusSelectList(bool? selectedValue)
    {
      var r = new List<SelectListItem>
      {
        new SelectListItem { Value = "true", Text = "Submitted", Selected = selectedValue == true },
        new SelectListItem { Value = "false", Text = "Outstanding", Selected = selectedValue == false }
      };
      return r;
    }

    public async Task<IEnumerable<SelectListItem>> GenerateMemberSelectList(bool showActiveOnly, ModuleEnum? module, int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<MemberSummary>();

      if (module != null)
      {
        results = results.Where(x => x.Modules!.Any(m => m.Module!.Id == (int)module));
      }

      if (showActiveOnly)
      {
        results = results.Where(x => x.Active);
      }

      return results.OrderBy(o => o.Name).ToList().ToSelectList(x => x.Id, x => x.Name, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateMemberBranchSelectList(bool showActiveOnly, int? memberId, int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<MemberBranchSummary>(x => x.Member.Id == memberId);

      results = memberId == null ? results.Where(x => x.Member!.Id == 0) : results.Where(x => x.Member!.Id == memberId);

      if (showActiveOnly)
      {
        results = results.Where(x => x.Active);
      }

      return results.OrderBy(o => o.Name).ToList().ToSelectList(x => x.Id, x => x.Name, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateMemberAccountSelectList(bool showActiveOnly, int? memberBranchId, int? selectedValue)
    {
      //TODO: Add summary model
      var results = await _applicationRepository.QueryAsync<MemberAccount>();

      if (memberBranchId.HasValue)
      {
        results = results.Where(x => x.MemberBranch.Id == memberBranchId);
      }

      if (showActiveOnly)
      {
        results = results.Where(x => x.Active);
      }

      return results.OrderBy(o => o.ApplicationUser.FirstName).ToList().ToSelectList(x => x.Id, x => x.ApplicationUser.FirstName, selectedValue);
    }

    [Obsolete("Not used until optimized", true)]
    public async Task<IEnumerable<SelectListItem>> GenerateGenericSelectList<T>(int? selectedValue) where T : class
    {
      return await _applicationRepository.QuerySelectListAsync<T>();
    }

    public IEnumerable<SelectListItem> GenerateSampleTypeSelectList(int? selectedValue)
    {
      var r = new List<SelectListItem>
      {
        new SelectListItem { Value = "1", Text = "Finished Product", Selected = selectedValue == 1 },
        new SelectListItem { Value = "2", Text = "Raw Material", Selected = selectedValue == 2 }
      };
      return r;
    }

    public async Task<IEnumerable<SelectListItem>> GenerateMemberTypeSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<MemberType>();
      return results.OrderBy(o => o.Description).ToList().ToSelectList(x => x.Id, x => x.Description, selectedValue);
    }

    #endregion

    #region Lookups

    public async Task<string> GetMemberNameFromId(int memberId)
    {
      var result = await _applicationRepository.QuerySingleAsync<Member>(x => x.Id == memberId).ConfigureAwait(false);
      return result.Name;
    }

    public async Task<string> GetMemberBranchNameFromId(int memberBranchId)
    {
      var result = await _applicationRepository.QuerySingleAsync<MemberBranch>(x => x.Id == memberBranchId).ConfigureAwait(false);
      return string.IsNullOrEmpty(result.Name) ? "Not found" : result.Name;
    }

    #endregion

    #region Users

    /// <inheritdoc cref="GetUsers"/>
    public IEnumerable<UserModel> GetUsers()
    {
      var result = _applicationRepository.Query<User>().Select(a => new UserModel
      {
        Id = a.UserId,

      }).ToList();

      return result;
    }

    public async Task<IEnumerable<UserModel>> GetUsersWithRoles(AlphaPageRequest pageRequest, string? searchText = null)
    {
      var userList = new List<UserModel>();
      var usersFiltered = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterUsers(searchText ?? ""));

      foreach (var user in usersFiltered.PageData)
      {
        var roles = await _identityLayer.Identity.UserManager.GetRolesAsync(user).ConfigureAwait(false);
        var memberAccount = await GetMemberAccount(user.Id).ConfigureAwait(false);
        var roleNames = string.Join(',', roles);
        userList.Add(new UserModel
        {
          Email = user.Email,
          FirstName = user.FirstName,
          LastName = user.LastName,
          Id = user.Id,
          ReceiveEmail = user.ReceivedEmail,
          LastLogin = user.LastLoginDate,
          Username = user.UserName,
          RoleNames = roles.Count != 1 ? "Role error" : roleNames,
          MemberName = memberAccount == null ? "" : memberAccount.Member,
          MemberId = memberAccount?.Id,
        });
      }
      return new StaticPagedList<UserModel>(userList, pageRequest.PageNumber, (int)pageRequest.PageLength!, usersFiltered.TotalResults);
    }

    #endregion

    #region Members

    /// <inheritdoc cref="GetMembers"/>`
    public async Task<IEnumerable<MemberModel>> GetMembers()
    {
      var result = await _applicationRepository.QueryAsync<Member>();
      return _mapper.Map<IEnumerable<Member>, List<MemberModel>>(result);
    }

    public async Task<IEnumerable<MemberSummaryModel>> GetMembersSummary()
    {
      var result = await _applicationRepository.QueryAsync<MemberSummary>();
      return _mapper.Map<IEnumerable<MemberSummary>, List<MemberSummaryModel>>(result);
    }

    public async Task<IEnumerable<MemberSummaryModel>> GetMembersSummary(AlphaPageRequest pageRequest, int? memberTypeId = null, string? searchText = null)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterMembers(memberTypeId, searchText ?? "")).ConfigureAwait(false);
      var resultMapped = _mapper.Map<IEnumerable<MemberSummary>, List<MemberSummaryModel>>(result.PageData);
      return new StaticPagedList<MemberSummaryModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    /// <inheritdoc cref="GetMember"/>
    public async Task<MemberModel> GetMember(int memberId)
    {
      var result = await _applicationRepository.QuerySingleAsync<Member>(x => x.Id == memberId);
      return _mapper.Map<Member, MemberModel>(result);
    }

    public void UpdateMemberModules(int memberId, IEnumerable<MemberModuleModel> memberModules)
    {
      foreach (var module in memberModules)
      {
        if (module.Selected)
        {
          var isModuleAlreadySelected = _applicationRepository.Query<MemberModule>()
            .Where(x => x.Member.Id == memberId && x.Module.Id == module.ModuleId);

          if (!isModuleAlreadySelected.Any())
          {
            _applicationRepository.SaveOrUpdateAndCommit(new MemberModule
            {
              Member = new Member { Id = memberId },
              Module = new Module { Id = module.ModuleId }
            });
          }
        }
        else
        {
          var isModuleAlreadySelected = _applicationRepository.Query<MemberModule>()
            .Where(x => x.Member.Id == memberId && x.Module.Id == module.ModuleId);

          if (isModuleAlreadySelected.Any())
          {
            _applicationRepository.DeleteAndCommit(isModuleAlreadySelected.First());
          }
        }
      }
    }

    public async Task<RepositoryResult> DeleteMember(int id)
    {
      var member = await _applicationRepository.QuerySingleAsync<Member>(x => x.Id == id);
      return _applicationRepository.DeleteAndCommit(member);
    }

    public async Task<MemberTypeModel> GetMemberMemberType(int memberId)
    {
      var result =
        await _applicationRepository.QuerySingleAsync<MemberSummary>(x => x.Id == memberId);

      return _mapper.Map<MemberType, MemberTypeModel>(result.MemberType!);
    }

    #endregion

    #region Member Branches

    /// <inheritdoc cref="GetMemberBranches"/>
    public async Task<IEnumerable<MemberBranchModel>> GetMemberBranches()
    {
      var result = await _applicationRepository.QueryAsync<MemberBranch>();
      return _mapper.Map<IEnumerable<MemberBranch>, List<MemberBranchModel>>(result);
    }

    /// <inheritdoc cref="GetMemberBranches"/>
    public async Task<IEnumerable<MemberBranchSummaryModel>> GetMemberBranchesSummary()
    {
      var result = await _applicationRepository.QueryAsync<MemberBranchSummary>();
      return _mapper.Map<IEnumerable<MemberBranchSummary>, List<MemberBranchSummaryModel>>(result);
    }

    /// <inheritdoc cref="GetMemberBranches"/>
    public async Task<IEnumerable<MemberBranchSummaryModel>> GetMemberBranchesSummaryByMemberType(int memberTypeId)
    {
      var result =
        await _applicationRepository.QueryAsync<MemberBranchSummary>(x => x.Member.Active && x.Member.MemberType.Id == memberTypeId && x.Active);
      return _mapper.Map<IEnumerable<MemberBranchSummary>, List<MemberBranchSummaryModel>>(result);
    }

    public async Task<IEnumerable<MemberBranchModel>> GetMemberBranchesSummaryByModule(int moduleId)
    {
      var accounts = await GetMemberBranchesByModule(moduleId);
      return accounts;
    }

    public async Task<IEnumerable<MemberBranchModel>> GetActiveMemberBranchesSummaryByModule(int moduleId)
    {
      var result = await _applicationRepository.QueryAsync<MemberModule>(x => x.Module!.Id == moduleId && x.Member!.Active);
      var memberBranches = new List<MemberBranchModel>();
      foreach (var member in result.Select(m => m.Member))
      {
        if (member != null && member.Branches != null && member.Branches.Any())
        {
          memberBranches.AddRange(_mapper.Map<IEnumerable<MemberBranch>, List<MemberBranchModel>>(member.Branches.Where(x => x.Active)));
        }
      }

      return memberBranches.OrderBy(x => x.Member).ThenBy(x => x.Name);
    }

    public async Task<IEnumerable<MemberSummaryModel>> GetMembersSummaryByModule(int moduleId)
    {
      var result =
        await _applicationRepository.QueryAsync<MemberSummary>(x =>
          x.Active && x.Modules!.Any(m => m.Module.Id == moduleId));

      return _mapper.Map<IEnumerable<MemberSummary>, List<MemberSummaryModel>>(result);
    }

    /// <inheritdoc cref="GetMemberBranches"/>
    public async Task<IEnumerable<MemberBranchSummaryModel>> GetMemberBranchesSummary(int memberId)
    {
      var result = await _applicationRepository.QueryAsync<MemberBranchSummary>();
      return _mapper.Map<IEnumerable<MemberBranchSummary>, List<MemberBranchSummaryModel>>(result.Where(x => x.Member.Id == memberId));
    }

    public async Task<IEnumerable<MemberBranchSummaryModel>> GetActiveMemberBranchesSummary(int memberId)
    {
      var result = await _applicationRepository.QueryAsync<MemberBranchSummary>();
      return _mapper.Map<IEnumerable<MemberBranchSummary>, List<MemberBranchSummaryModel>>(result.Where(x => x.Member.Id == memberId && x.Active));
    }

    /// <inheritdoc cref="GetRawMaterial"/>
    public async Task<MemberBranchModel> GetMemberBranch(int memberBranchId)
    {
      var result = await _applicationRepository.QueryFirstOrDefaultAsync<MemberBranch>(x => x.Id == memberBranchId);
      return _mapper.Map<MemberBranch, MemberBranchModel>(result);
    }

    public async Task<RepositoryResult> DeleteMemberBranch(int id, int memberId)
    {
      var memberBranch = await _applicationRepository.QuerySingleAsync<MemberBranch>(x => x.Id == id && x.Member!.Id == memberId);
      return _applicationRepository.DeleteAndCommit(memberBranch);
    }

    #endregion

    #region Member Accounts

    public async Task<MemberAccountModel> GetMemberAccount(int memberAccountId)
    {
      var result = await _applicationRepository.QueryFirstOrDefaultAsync<MemberAccount>(x => x.Id == memberAccountId);
      return _mapper.Map<MemberAccount, MemberAccountModel>(result);
    }

    public async Task<MemberAccountModel?> GetMemberAccount(string userId)
    {
      try
      {
        var result =
          await _applicationRepository.QueryFirstOrDefaultAsync<MemberAccount>(x => x.ApplicationUser.Id == userId);
        return _mapper.Map<MemberAccount, MemberAccountModel>(result);
      }
      catch
      {
        return null;
      }
    }

    public async Task<ICollection<MemberAccountModel>> GetMemberAccountsByModule(int moduleId, bool showActive = false)
    {
      var result = await _applicationRepository.QueryAsync<MemberAccountModule>(x => x.Module.Id == moduleId && (!showActive || x.MemberAccount.Active));
      return _mapper.Map<IEnumerable<MemberAccount>, List<MemberAccountModel>>(result.Select(a => a.MemberAccount));
    }

    public async Task<ICollection<MemberAccountModel>> GetMemberAccounts()
    {
      var result = await _applicationRepository.QueryAsync<MemberAccount>();
      return _mapper.Map<IEnumerable<MemberAccount>, List<MemberAccountModel>>(result);
    }

    public async Task<ICollection<MemberBranchModel>> GetMemberBranchesByModule(int moduleId)
    {
      var result = await _applicationRepository.QueryAsync<MemberAccountModule>(x => x.Module.Id == moduleId);
      return _mapper.Map<List<MemberBranchModel>>(
        result
          .Where(x => x.MemberAccount != null) // Avoid null references
          .Select(x => x.MemberAccount.MemberBranch) // Extract MemberBranch
          .Distinct() // Ensure uniqueness
      );
    }

    public async Task<bool> AddMemberAccountToModule(int memberAccountId, int moduleId)
    {
      var exists = await _applicationRepository.QueryAnyAsync<MemberAccountModule>(x =>
        x.Module.Id == moduleId && x.MemberAccount.Id == memberAccountId);

      if (!exists)
      {
        var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(new MemberAccountModule
        {
          MemberAccount = new MemberAccount { Id = memberAccountId },
          Module = new Module { Id = moduleId },
        });
        return result.Succeeded;
      }

      return true;
    }

    public async Task<bool> CheckMemberAccountModule(int memberAccountId, ModuleEnum module)
    {
      try
      {
        return await _applicationRepository.QueryAnyAsync<MemberAccountModule>(x =>
          x.Module.Id == (int)module && x.MemberAccount.Id == memberAccountId);
      }
      catch
      {
        return false;
      }
    }

    /// <inheritdoc/>
    public async Task<bool> CheckMemberAccountModule(ModuleEnum module)
    {
      var currentMemberId = await GetCurrentMemberAccountId().ConfigureAwait(false);

      return await CheckMemberAccountModule(currentMemberId, module);
    }

    public async Task<bool> CheckMemberModule(int memberId, ModuleEnum module)
    {
      try
      {
        return await _applicationRepository.QueryAnyAsync<MemberModule>(x =>
          x.Module!.Id == (int)module && x.Member.Id == memberId);
      }
      catch
      {
        return false;
      }
    }

    public async Task<bool> RemoveMemberAccountFromModule(int memberAccountId, int moduleId)
    {
      try
      {
        var result = await _applicationRepository.QueryFirstOrDefaultAsync<MemberAccountModule>(x =>
          x.Module.Id == moduleId && x.MemberAccount.Id == memberAccountId);
        _applicationRepository.DeleteAndCommit(result);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public async Task<IEnumerable<MemberAccountModel>> GetMemberBranchAccounts(int memberBranchId)
    {
      var result = await _applicationRepository.QueryAsync<MemberAccount>(x => x.MemberBranch.Id == memberBranchId).ConfigureAwait(false);
      return _mapper.Map<IEnumerable<MemberAccount>, List<MemberAccountModel>>(result);
    }

    public async Task<IEnumerable<MemberAccountModel>> GetMemberBranchAccounts(int memberBranchId, ModuleEnum module)
    {
      var result = await _applicationRepository
        .QueryAsync<MemberAccount>(x =>
          x.MemberBranch.Id == memberBranchId && x.Modules.Any(m => m.Module.Id == (int)module)).ConfigureAwait(false);

      return _mapper.Map<IEnumerable<MemberAccount>, List<MemberAccountModel>>(result);
    }

    public async Task<IEnumerable<MemberAccountModel>> GetActiveMemberBranchAccounts(int memberBranchId, ModuleEnum module)
    {
      var result = await _applicationRepository
        .QueryAsync<MemberAccount>(x =>
          x.MemberBranch.Id == memberBranchId && x.Active && x.Modules.Any(m => m.Module.Id == (int)module)).ConfigureAwait(false);
      return _mapper.Map<IEnumerable<MemberAccount>, List<MemberAccountModel>>(result);
    }

    public async Task<RepositoryResult> DeleteMemberAccount(int id, int memberId)
    {
      var memberAccount = await _applicationRepository.QuerySingleAsync<MemberAccount>(x => x.Id == id && x.Member!.Id == memberId);
      var userDeleteResult = await _identityLayer.DeleteUser(memberAccount.ApplicationUser.Id).ConfigureAwait(false);

      if (!userDeleteResult.Succeeded)
      {
        throw new Exception("Cannot delete account/user.");
      }

      var repositoryResult = await _applicationRepository.DeleteAndCommitAsync(memberAccount).ConfigureAwait(false);
      return repositoryResult;
    }

    #endregion

    #region Member Types

    public async Task<IEnumerable<MemberTypeModel>> GetMemberTypes()
    {
      var result = await _applicationRepository.QueryAsync<MemberType>();
      return _mapper.Map<IEnumerable<MemberType>, List<MemberTypeModel>>(result);
    }

    public async Task<IEnumerable<MemberTypeModel>> GetMemberTypes(AlphaPageRequest pageRequest, string? searchText = null)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterMemberTypes(searchText ?? ""));
      var resultMapped = _mapper.Map<IEnumerable<MemberType>, List<MemberTypeModel>>(result.PageData);
      return new StaticPagedList<MemberTypeModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    /// <inheritdoc cref="GetMemberType"/>
    public async Task<MemberTypeModel> GetMemberType(int memberTypeId)
    {
      var result = await _applicationRepository.QueryFirstOrDefaultAsync<MemberType>(x => x.Id == memberTypeId);
      return _mapper.Map<MemberType, MemberTypeModel>(result);
    }

    /// <inheritdoc cref="DeleteMemberType"/>
    public async Task<RepositoryResult> DeleteMemberType(int memberTypeId)
    {
      var result = await _applicationRepository.QuerySingleAsync<MemberType>(x => x.Id == memberTypeId);
      return _applicationRepository.DeleteAndCommit(result);
    }

    #endregion

    #region Raw Materials

    /// <inheritdoc cref="GetRawMaterials"/>
    public async Task<IEnumerable<MaterialModel>> GetRawMaterials()
    {
      var result = await _applicationRepository.QueryAsync<Material>();
      return _mapper.Map<IEnumerable<Material>, List<MaterialModel>>(result);
    }

    /// <inheritdoc cref="GetRawMaterials"/>
    public async Task<IEnumerable<MaterialModel>> GetRawMaterials(int materialCategoryId)
    {
      var result = await _applicationRepository.QueryAsync<Material>();
      return _mapper.Map<IEnumerable<Material>, List<MaterialModel>>(result.Where(x => x.MaterialCategory.Id == materialCategoryId));
    }

    public async Task<IEnumerable<MaterialModel>> GetRawMaterials(AlphaPageRequest pageRequest, int? materialCategoryId = null, string? searchText = null)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterRawMaterials(materialCategoryId, searchText ?? ""));
      var resultMapped = _mapper.Map<IEnumerable<Material>, List<MaterialModel>>(result.PageData);
      return new StaticPagedList<MaterialModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    /// <inheritdoc cref="GetRawMaterial"/>
    public async Task<MaterialModel> GetRawMaterial(int materialId)
    {
      var result = await _applicationRepository.QueryFirstOrDefaultAsync<Material>(x => x.Id == materialId);
      return _mapper.Map<Material, MaterialModel>(result);
    }

    #endregion

    #region Raw Material Categories

    //TODO

    #endregion

    #region Finished Products

    /// <inheritdoc cref="GetFinishedProducts"/>
    public async Task<IEnumerable<ProductModel>> GetFinishedProducts()
    {
      var result = await _applicationRepository.QueryAsync<Product>();
      return _mapper.Map<IEnumerable<Product>, List<ProductModel>>(result);
    }

    /// <inheritdoc cref="GetFinishedProducts"/>
    public async Task<IEnumerable<ProductModel>> GetFinishedProducts(int productGroupId)
    {
      var result = await _applicationRepository.QueryAsync<Product>();
      return _mapper.Map<IEnumerable<Product>, List<ProductModel>>(result.Where(x => x.ProductGroup.Id == productGroupId));
    }

    public async Task<IEnumerable<ProductModel>> GetFinishedProducts(AlphaPageRequest pageRequest, int? productGroupId = null, string? searchText = null)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterFinishedProducts(productGroupId, searchText ?? ""));
      var resultMapped = _mapper.Map<IEnumerable<Product>, List<ProductModel>>(result.PageData);
      return new StaticPagedList<ProductModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    /// <inheritdoc cref="GetFinishedProduct"/>
    public async Task<ProductModel> GetFinishedProduct(int productId)
    {
      var result = await _applicationRepository.QueryFirstOrDefaultAsync<Product>(x => x.Id == productId);
      return _mapper.Map<Product, ProductModel>(result);
    }

    public async Task<IEnumerable<AvailableFinishedProductsViewModel>> AvailableFinishedProducts(int reportId, int productGroupId)
    {
      var report = await GetFeedsalesReport(reportId);
      var existingProducts =
        report.ReportItems.Where(x => x.ProductId != null).Select(a => a.ProductId).ToArray();

      var r = GetFinishedProducts().Result.Where(x =>
        x.ProductGroupId == productGroupId && existingProducts.All(p1 => p1 != x.Id || x.AllowDuplicates)).Select(a =>
        new AvailableFinishedProductsViewModel
        {
          Code = a.Code,
          Id = a.Id,
          Product = a.Name,
          MemberBranchId = report.MemberBranchId,
          MemberAccountId = report.MemberAccountId,
          MemberId = report.MemberId,
          FeedsalesReportId = reportId
        }).OrderBy(o => o.Product);

      return r;
    }

    #endregion

    #region Finished Product Groups

    public async Task<IEnumerable<ProductGroupModel>> GetFinishedProductGroups()
    {
      var result = await _applicationRepository.QueryAsync<ProductGroup>();
      return _mapper.Map<IEnumerable<ProductGroup>, List<ProductGroupModel>>(result);
    }

    public async Task<IEnumerable<ProductGroupModel>> GetFinishedProductGroups(AlphaPageRequest pageRequest, string? searchText = null)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterFinishedProductGroups(searchText ?? ""));
      var resultMapped = _mapper.Map<IEnumerable<ProductGroup>, List<ProductGroupModel>>(result.PageData);
      return new StaticPagedList<ProductGroupModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    public async Task<ProductGroupModel> GetFinishedProductGroup(int productGroupId)
    {
      var result = await _applicationRepository.QueryFirstOrDefaultAsync<ProductGroup>(x => x.Id == productGroupId);
      return _mapper.Map<ProductGroup, ProductGroupModel>(result);
    }

    public async Task<RepositoryResult> DeleteFinishedProductGroup(int productGroupId)
    {
      var result = await _applicationRepository.QuerySingleAsync<ProductGroup>(x => x.Id == productGroupId);
      return await _applicationRepository.DeleteAndCommitAsync(result).ConfigureAwait(false);
    }

    #endregion

    #region Modules

    /// <inheritdoc cref="GetModules"/>
    public async Task<IEnumerable<ModuleModel>> GetModules()
    {
      var result = await _applicationRepository.QueryAsync<Module>();
      return _mapper.Map<IEnumerable<Module>, List<ModuleModel>>(result);
    }

    /// <inheritdoc cref="GetModule"/>
    public async Task<ModuleModel> GetModule(int moduleId)
    {
      var result = await _applicationRepository.QueryFirstOrDefaultAsync<Module>(x => x.Id == moduleId);
      return _mapper.Map<Module, ModuleModel>(result);
    }

    #endregion

    #region Provinces

    /// <inheritdoc cref="GetProvinces"/>
    public async Task<IEnumerable<ProvinceModel>> GetProvinces()
    {
      var result = await _applicationRepository.QueryAsync<Province>();
      return _mapper.Map<IEnumerable<Province>, List<ProvinceModel>>(result);
    }

    /// <inheritdoc cref="GetProvince"/>
    public async Task<ProvinceModel> GetProvince(int provinceId)
    {
      var result = await _applicationRepository.QueryFirstOrDefaultAsync<Province>(x => x.Id == provinceId);
      return _mapper.Map<Province, ProvinceModel>(result);
    }

    #endregion

    #region Laboratories

    public async Task<IEnumerable<LaboratoryModel>> GetLaboratories()
    {
      var result = await _applicationRepository.QueryAsync<Laboratory>();
      return _mapper.Map<IEnumerable<Laboratory>, List<LaboratoryModel>>(result);
    }

    public async Task<IEnumerable<LaboratoryModel>> GetLaboratories(AlphaPageRequest pageRequest, string? searchText = null)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterLaboratories(searchText ?? ""));
      var resultMapped = _mapper.Map<IEnumerable<Laboratory>, List<LaboratoryModel>>(result.PageData);
      return new StaticPagedList<LaboratoryModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    /// <inheritdoc cref="GetLaboratory"/>
    public async Task<LaboratoryModel> GetLaboratory(int laboratoryId)
    {
      var result = await _applicationRepository.QueryFirstOrDefaultAsync<Laboratory>(x => x.Id == laboratoryId);
      return _mapper.Map<Laboratory, LaboratoryModel>(result);
    }

    /// <inheritdoc cref="DeleteLaboratory"/>
    public async Task<RepositoryResult> DeleteLaboratory(int laboratoryId)
    {
      var result = await _applicationRepository.QuerySingleAsync<Laboratory>(x => x.Id == laboratoryId);
      return _applicationRepository.DeleteAndCommit(result);
    }

    #endregion

    #region Main Locations

    /// <inheritdoc cref="GetMainLocations"/>
    public async Task<IEnumerable<SalmonellaSampleTypeItemModel>> GetMainLocations()
    {
      var result = await _applicationRepository.QueryAsync<SalmonellaSampleTypeItem>().ConfigureAwait(false);
      return _mapper.Map<IEnumerable<SalmonellaSampleTypeItem>, List<SalmonellaSampleTypeItemModel>>(result.OrderBy(s => s.SalmonellaSampleType.Name).ThenBy(n => n.Name));
    }

    /// <inheritdoc cref="GetMainLocation"/>
    public async Task<IEnumerable<SalmonellaSampleTypeItemModel>> GetMainLocations(AlphaPageRequest pageRequest, int? salmonellaSampleTypeId = null, string? searchText = null)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterMainLocations(salmonellaSampleTypeId, searchText ?? ""));
      var resultMapped = _mapper.Map<IEnumerable<SalmonellaSampleTypeItem>, List<SalmonellaSampleTypeItemModel>>(result.PageData);
      return new StaticPagedList<SalmonellaSampleTypeItemModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    /// <inheritdoc cref="GetMainLocation"/>
    public async Task<SalmonellaSampleTypeItemModel> GetMainLocation(int mainLocationId)
    {
      var result = await _applicationRepository.QueryFirstOrDefaultAsync<SalmonellaSampleTypeItem>(x => x.Id == mainLocationId);
      return _mapper.Map<SalmonellaSampleTypeItem, SalmonellaSampleTypeItemModel>(result);
    }

    /// <inheritdoc cref="DeleteMainLocation"/>
    public async Task<RepositoryResult> DeleteMainLocation(int mainLocationId)
    {
      var result = await _applicationRepository.QuerySingleAsync<SalmonellaSampleTypeItem>(x => x.Id == mainLocationId).ConfigureAwait(false);
      return _applicationRepository.DeleteAndCommit(result);
    }

    #endregion

    #region Sub Locations

    /// <inheritdoc cref="GetSubLocations"/>
    public async Task<IEnumerable<SalmonellaSampleTypeSubItemModel>> GetSubLocations()
    {
      var result = await _applicationRepository.QueryAsync<SalmonellaSampleTypeSubItem>().ConfigureAwait(false);
      return _mapper.Map<IEnumerable<SalmonellaSampleTypeSubItem>, List<SalmonellaSampleTypeSubItemModel>>(result.OrderBy(n => n.Name).ThenBy(s => s.SalmonellaSampleType.Id));
    }

    /// <inheritdoc cref="GetSubLocations"/>
    public async Task<IEnumerable<SalmonellaSampleTypeSubItemModel>> GetSubLocations(AlphaPageRequest pageRequest, int? salmonellaSampleTypeId = null, string? searchText = null)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterSubLocations(salmonellaSampleTypeId, searchText ?? ""));
      var resultMapped = _mapper.Map<IEnumerable<SalmonellaSampleTypeSubItem>, List<SalmonellaSampleTypeSubItemModel>>(result.PageData);
      return new StaticPagedList<SalmonellaSampleTypeSubItemModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    /// <inheritdoc cref="GetSubLocation"/>
    public async Task<SalmonellaSampleTypeSubItemModel> GetSubLocation(int subLocationId)
    {
      var result = await _applicationRepository.QueryFirstOrDefaultAsync<SalmonellaSampleTypeSubItem>(x => x.Id == subLocationId);
      return _mapper.Map<SalmonellaSampleTypeSubItem, SalmonellaSampleTypeSubItemModel>(result);
    }

    /// <inheritdoc cref="DeleteSubLocation"/>
    public async Task<RepositoryResult> DeleteSubLocation(int subLocationId)
    {
      var result = await _applicationRepository.QuerySingleAsync<SalmonellaSampleTypeSubItem>(x => x.Id == subLocationId).ConfigureAwait(false);
      return _applicationRepository.DeleteAndCommit(result);
    }

    #endregion

    #region Salmonella Sample Types

    /// <inheritdoc cref="GetSampleTypes"/>
    public async Task<IEnumerable<SalmonellaSampleTypeModel>> GetSampleTypes()
    {
      var result = await _applicationRepository.QueryAsync<SalmonellaSampleType>();
      return _mapper.Map<IEnumerable<SalmonellaSampleType>, List<SalmonellaSampleTypeModel>>(result.OrderBy(n => n.Name));
    }

    /// <inheritdoc cref="GetSampleType"/>
    public async Task<SalmonellaSampleTypeModel> GetSampleType(int sampleTypeId)
    {
      var result = await _applicationRepository.QueryFirstOrDefaultAsync<SalmonellaSampleType>(x => x.Id == sampleTypeId);
      return _mapper.Map<SalmonellaSampleType, SalmonellaSampleTypeModel>(result);
    }

    #endregion

    #region Feedsales Admin Reports

    /// <inheritdoc cref="GetFeedsalesReports"/>
    public async Task<IEnumerable<FeedsalesReportModel>> GetFeedsalesReports()
    {
      var result = await _applicationRepository.QueryAsync<FeedsalesReport>();
      return _mapper.Map<IEnumerable<FeedsalesReport>, List<FeedsalesReportModel>>(result);
    }

    /// <inheritdoc cref="GetFeedsalesReports"/>
    public async Task<IEnumerable<FeedsalesReportLookupModel>> GetFeedsalesReports(int? memberId, int? memberBranchId, bool? isSubmitted, int? month, int? year)
    {
      var result = await _applicationRepository.QueryAsync<FeedsalesReportLookup>();

      if (memberId != null)
      {
        result = result.Where(x => x.Member.Id == memberId);
      }

      if (memberBranchId != null)
      {
        result = result.Where(x => x.MemberBranch.Id == memberBranchId);
      }

      if (month != null)
      {
        result = result.Where(x => x.ReportMonth == month);
      }

      if (year != null)
      {
        result = result.Where(x => x.ReportYear == year);
      }

      if (isSubmitted != null)
      {
        result = result.Where(x => x.IsSubmitted == isSubmitted);
      }

      return _mapper.Map<IEnumerable<FeedsalesReportLookup>, List<FeedsalesReportLookupModel>>(result);
    }

    public async Task<IEnumerable<FeedsalesReportWithItemsLookupModel>> GetFeedsalesReportsWithItems(int? memberId, int? memberBranchId, int? memberAccountId, bool? isSubmitted, int? month, int? year)
    {
      var result = await _applicationRepository.QueryAsync<FeedsalesReportWithItemsLookup>();

      if (memberId != null)
      {
        result = result.Where(x => x.Member.Id == memberId);
      }

      if (memberBranchId != null)
      {
        result = result.Where(x => x.MemberBranch.Id == memberBranchId);
      }

      if (memberAccountId != null)
      {
        result = result.Where(x => x.MemberAccount.Id == memberAccountId);
      }

      if (month != null)
      {
        result = result.Where(x => x.ReportMonth == month);
      }

      if (year != null)
      {
        result = result.Where(x => x.ReportYear == year);
      }

      if (isSubmitted != null)
      {
        result = result.Where(x => x.IsSubmitted == isSubmitted);
      }

      return _mapper.Map<IEnumerable<FeedsalesReportWithItemsLookup>, List<FeedsalesReportWithItemsLookupModel>>(result);
    }

    /// <inheritdoc cref="GetFeedsalesReport"/>
    public async Task<FeedsalesReportModel> GetFeedsalesReport(int id)
    {
      var result = await _applicationRepository.QuerySingleAsync<FeedsalesReport>(x => x.Id == id);
      return _mapper.Map<FeedsalesReport, FeedsalesReportModel>(result);
    }

    /// <inheritdoc cref="GetFeedsalesReport"/>
    public async Task<FeedsalesReportModel> GetPreviousFeedsalesReport(int memberBranchId, DateTime infoFor, int month, int year)
    {
      var result = await _applicationRepository.QueryFirstOrDefaultAsync<FeedsalesReport>(x =>
        x.MemberBranch.Id == memberBranchId && x.InfoFor == infoFor.AddMonths(-1));

      return _mapper.Map<FeedsalesReport, FeedsalesReportModel>(result);
    }

    public async Task<FeedsalesReportItemModel> GetFeedsalesReportItem(int id)
    {
      var result = await _applicationRepository.QuerySingleAsync<FeedsalesReportItem>(x => x.Id == id);
      return _mapper.Map<FeedsalesReportItem, FeedsalesReportItemModel>(result);
    }

    public async Task<ICollection<FeedsalesReportItemModel>> GetFeedsalesReportItems(int feedsalesReportId)
    {
      var result = await _applicationRepository.QueryAsync<FeedsalesReportItem>(x => x.FeedsalesReport.Id == feedsalesReportId);
      return _mapper.Map<IEnumerable<FeedsalesReportItem>, List<FeedsalesReportItemModel>>(result);
    }

    public async Task<ICollection<FeedsalesReportItemLookupModel>> GetFeedsalesReportItems()
    {
      var result = await _applicationRepository.QueryAsync<FeedsalesReportItemLookup>();
      return _mapper.Map<IEnumerable<FeedsalesReportItemLookup>, List<FeedsalesReportItemLookupModel>>(result);
    }

    public async Task<bool> DeleteFeedsalesReportItem(int id)
    {
      var reportItem = await _applicationRepository.QuerySingleAsync<FeedsalesReportItem>(x => x.Id == id);
      return _applicationRepository.DeleteAndCommit(reportItem).Succeeded;
    }

    public async Task<RepositoryResult> DeleteFeedsalesReport(int id)
    {
      var report = await _applicationRepository.QuerySingleAsync<FeedsalesReport>(x => x.Id == id);
      return _applicationRepository.DeleteAndCommit(report);
    }

    public async Task<SaveOrUpdateResult> UpdateFeedsalesReportStatus(int id, bool submitStatus)
    {
      var report = await _applicationRepository.QuerySingleAsync<FeedsalesReport>(x => x.Id == id);

      if (submitStatus)
      {
        report.IsSubmitted = true;
        report.SubmittedDate = DateTime.Now;
        report.MemberAccount ??= new MemberAccount { Id = await GetCurrentMemberAccountId().ConfigureAwait(false) };
      }
      else
      {
        report.IsSubmitted = false;
        report.SubmittedDate = null;
        report.MemberAccount = null;
      }

      var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(report);

      return new SaveOrUpdateResult
      {
        Success = result.Succeeded,
        ErrorMessage = result.ExceptionMessage,
        SavedObjectId = result.SavedObjectId,
      };
    }

    public async Task<SaveOrUpdateResult> EditFeedsalesReport(int id, string differenceReason, string comment)
    {
      var report = await _applicationRepository.QuerySingleAsync<FeedsalesReport>(x => x.Id == id);
      report.DifferenceReason = differenceReason;
      report.Comment = comment;

      var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(report);

      return new SaveOrUpdateResult
      {
        Success = result.Succeeded,
        ErrorMessage = result.ExceptionMessage,
        SavedObjectId = result.SavedObjectId,
      };
    }

    private async Task<bool> UpdateReportSubmitter(int reportId, int memberAccountId)
    {
      var report = await _applicationRepository.QuerySingleAsync<FeedsalesReport>(x => x.Id == reportId)
        .ConfigureAwait(false);

      report.MemberAccount ??= new MemberAccount { Id = memberAccountId };

      var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(report);
      return result.Succeeded;

    }

    /// <inheritdoc cref="GetFeedsalesReportsLookup"/> 
    public async Task<IEnumerable<FeedsalesReportLookupModel>> GetFeedsalesReportsLookup()
    {
      var result = await _applicationRepository.QueryAsync<FeedsalesReportLookup>();
      return _mapper.Map<IEnumerable<FeedsalesReportLookup>, List<FeedsalesReportLookupModel>>(result);
    }

    /// <summary>
    /// Returns a filtered list of feedsales reports.
    /// </summary>
    /// <param name="pageRequest">The pages request.</param>
    /// <param name="memberId">The member Id.</param>
    /// <param name="memberBranchId">The member branch Id.</param>
    /// <param name="isSubmitted">The submitted indicator.</param>
    /// <returns>A paged results with feedsales reports.</returns>
    public async Task<IEnumerable<FeedsalesReportLookupModel>> GetFeedsalesReportsLookup(AlphaPageRequest pageRequest, int? memberId, int? memberBranchId, bool? isSubmitted)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest,
        SearchExpression.FilterFeedsalesReports(memberId, memberBranchId, isSubmitted));
      var resultMapped = _mapper.Map<IEnumerable<FeedsalesReportLookup>, List<FeedsalesReportLookupModel>>(result.PageData);
      return new StaticPagedList<FeedsalesReportLookupModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    /// <summary>
    /// Returns a filtered list of feedsales report for a specific member account.
    /// </summary>
    /// <param name="pageRequest">The pages request.</param>
    /// <param name="memberId">The member Id.</param>
    /// <param name="memberBranchId">The member branch Id.</param>
    /// <param name="memberAccountId">The member accunt Id.</param>
    /// <param name="isSubmitted">The submitted indicator.</param>
    /// <returns>A paged results with feedsales reports.</returns>
    public async Task<IEnumerable<FeedsalesReportLookupModel>> GetFeedsalesReportsLookup(AlphaPageRequest pageRequest, int memberId, int memberBranchId, int memberAccountId, bool? isSubmitted)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest,
        SearchExpression.FilterFeedsalesReports(memberId, memberBranchId, memberAccountId, isSubmitted));
      var resultMapped = _mapper.Map<IEnumerable<FeedsalesReportLookup>, List<FeedsalesReportLookupModel>>(result.PageData);
      return new StaticPagedList<FeedsalesReportLookupModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    public async Task<IEnumerable<FeedsalesReportLookupModel>> GetFeedsalesReportsLookup(int? memberId, int? memberBranchId, bool? isSubmitted, int? month, int? year)
    {
      var result = await _applicationRepository.QueryAsync<FeedsalesReportLookup>();

      if (memberId != null)
      {
        result = result.Where(x => x.Member.Id == memberId);
      }

      if (memberBranchId != null)
      {
        result = result.Where(x => x.MemberBranch.Id == memberBranchId);
      }

      if (month != null)
      {
        result = result.Where(x => x.ReportMonth == month);
      }

      if (year != null)
      {
        result = result.Where(x => x.ReportYear == year);
      }

      if (isSubmitted != null)
      {
        result = result.Where(x => x.IsSubmitted == isSubmitted);
      }

      return _mapper.Map<IEnumerable<FeedsalesReportLookup>, List<FeedsalesReportLookupModel>>(result);
    }

    public async Task<ICollection<FeedsalesReportGroupModel>> GetFeedsalesReportGroups()
    {
      var results = await _applicationRepository.QueryAsync<FeedsalesReportGroup>().ConfigureAwait(false);
      return _mapper.Map<IEnumerable<FeedsalesReportGroup>, List<FeedsalesReportGroupModel>>(results.OrderBy(n => n.Name));
    }

    //[Obsolete("Not used", true)]
    //public async Task<bool> SendFeedsalesSubmissionReminder(int feedsalesReportId)
    //{
    //  //TODO: Remove this!
    //  try
    //  {
    //    var report = await _applicationRepository
    //      .QuerySingleAsync<FeedsalesReportLookup>(x => x.Id == feedsalesReportId)
    //      .ConfigureAwait(false);

    //    var submissionEmail = await _razorTemplateEngine.RenderAsync("/Templates/GenericEmail.cshtml",
    //      new GenericEmailModel
    //      {
    //        BaseUrl = _applicationSettings.BaseUrl,
    //        Body = Templating.EmailContent.Submissions.GenerateFeedsalesEmail(report.ReportMonth, report.ReportYear,"",""),
    //      }).ConfigureAwait(false);

    //    var recipient = report.MemberAccount.ApplicationUser.Email;

    //    if (_smtpSettings.Debug && !string.IsNullOrEmpty(_smtpSettings.DebugEmail))
    //    {
    //      recipient = _smtpSettings.DebugEmail;
    //    }

    //    await _emailClient.SendAsync(recipient,
    //        $"AFMA FEEDSALES: Outstanding Report for {report.Member.Name} - {report.MemberBranch.Name}",
    //        submissionEmail, null, new List<string> { "operations@resolvetech.co.za" })
    //      .ConfigureAwait(false);

    //    return true;
    //  }
    //  catch
    //  {
    //    return false;
    //  }
    //}

    public decimal GetTotalFinishedProductByGroup(int year, int month, int memberBranchId, int productGroupId, bool isAdmin)
    {
      try
      {
        var result = _applicationRepository.Session().CreateSQLQuery(
            "DECLARE @return_value int; EXEC @return_value = [dbo].[sp_GetTotalFinishedProductByGroup] @Year=:year, @Month=:month, @memberBranchId=:memberBranchId, @ProductGroupId=:productGroupId, @IsAdmin=:isAdmin; SELECT 'Return Value' = @return_value")
          .SetInt32("year", year)
          .SetInt32("month", month)
          .SetInt32("memberBranchId", memberBranchId)
          .SetInt32("productGroupId", productGroupId)
          .SetBoolean("isAdmin", isAdmin)
          .List<decimal>();
        return result[0];
      }
      catch
      {
        return 0;
      }
    }

    public decimal GetFeedsalesByMemberByDate(int year, int month, int memberBranchId)
    {
      try
      {
        var result = _applicationRepository.Session().CreateSQLQuery(
            "DECLARE @return_value int; EXEC @return_value = [dbo].[sp_GetFeedsalesByMemberByDate] @Year=:year, @Month=:month, @BranchId=:memberBranchId; SELECT 'Return Value' = @return_value")
          .SetInt32("year", year)
          .SetInt32("month", month)
          .SetInt32("memberBranchId", memberBranchId)
          .List<decimal>();
        return result[0];
      }
      catch
      {
        return 0;
      }
    }

    public decimal GetRawMaterialsByMemberByDate(int year, int month, int memberBranchId)
    {
      try
      {
        var result = _applicationRepository.Session().CreateSQLQuery(
            "DECLARE @return_value int;EXEC @return_value = [dbo].[sp_GetRawMaterialsByMemberByDate] @Year=:year, @Month=:month, @BranchId=:memberBranchId;SELECT 'Return Value' = @return_value")
          .SetInt32("year", year)
          .SetInt32("month", month)
          .SetInt32("memberBranchId", memberBranchId)
          .List<decimal>();
        return result[0];
      }
      catch
      {
        return 0;
      }
    }

    public decimal GetTotalFeedsalesProductByProvinceByYear(DateTime startDate, DateTime endDate, int provinceId, int productId, int memberTypeId)
    {
      try
      {
        var result = _applicationRepository.Session().CreateSQLQuery(
            "DECLARE @return_value int;EXEC @return_value = [dbo].[sp_GetTotalFeedsalesProductByProvinceByYear] @StartDate=:startDate, @EndDate=:endDate, @ProvinceId=:provinceId, @ProductId=:productId, @MemberTypeId=:memberTypeId;SELECT 'Return Value' = @return_value")
          .SetDateTime("startDate", startDate)
          .SetDateTime("endDate", endDate)
          .SetInt32("provinceId", provinceId)
          .SetInt32("productId", productId)
          .SetInt32("memberTypeId", memberTypeId)
          .List<decimal>();
        return result[0];
      }
      catch
      {
        return 0;
      }
    }

    public decimal GetTotalFeedsalesProductGroupByProvinceByYear(DateTime startDate, DateTime endDate, int provinceId, int productGroupId, int memberTypeId)
    {
      try
      {
        var result = _applicationRepository.Session().CreateSQLQuery(
            "DECLARE @return_value int;EXEC @return_value = [dbo].[sp_GetTotalFeedsalesProductGroupByProvinceByYear] @StartDate=:startDate, @EndDate=:endDate, @ProvinceId=:provinceId, @ProductGroupId=:productGroupId, @MemberTypeId=:memberTypeId;SELECT 'Return Value' = @return_value")
          .SetDateTime("startDate", startDate)
          .SetDateTime("endDate", endDate)
          .SetInt32("provinceId", provinceId)
          .SetInt32("productGroupId", productGroupId)
          .SetInt32("memberTypeId", memberTypeId)
          .List<decimal>();
        return result[0];
      }
      catch
      {
        return 0;
      }
    }

    public decimal GetTotalFeedsalesProductGroupByProvinceByYearTotal(DateTime startDate, DateTime endDate, int provinceId, int memberTypeId)
    {
      try
      {
        var result = _applicationRepository.Session().CreateSQLQuery(
            "DECLARE @return_value int;EXEC @return_value = [dbo].[sp_GetTotalFeedsalesProductGroupByProvinceByYearTotal] @StartDate=:startDate, @EndDate=:endDate, @ProvinceId=:provinceId, @MemberTypeId=:memberTypeId;SELECT 'Return Value' = @return_value")
          .SetDateTime("startDate", startDate)
          .SetDateTime("endDate", endDate)
          .SetInt32("provinceId", provinceId)
          .SetInt32("memberTypeId", memberTypeId)
          .List<decimal>();
        return result[0];
      }
      catch
      {
        return 0;
      }
    }

    /// <summary>
    /// Gets the total finished product sales for a product group
    /// </summary>
    /// <param name="startDate">The Start Date</param>
    /// <param name="endDate">The End Date</param>
    /// <param name="productGroupId">The Product Group</param>
    /// <param name="memberTypeId">The Member Type</param>
    /// <param name="memberBranchId">The Member Branch</param>
    /// <returns>The Total Sales Value</returns>
    public decimal GetFinishedProductGroupTotalSales(DateTime startDate, DateTime endDate, int productGroupId, int memberTypeId, int? memberBranchId = 0)
    {
      try
      {
        var result = _applicationRepository.Session().CreateSQLQuery(
            "DECLARE @return_value int;EXEC @return_value = [dbo].[sp_GetFinishedProductGroupTotalSales] @StartDate=:startDate, @EndDate=:endDate, @ProductGroupId=:productGroupId, @MemberBranchId=:memberBranchId, @MemberTypeId=:memberTypeId;SELECT 'Return Value' = @return_value")
          .SetDateTime("startDate", startDate)
          .SetDateTime("endDate", endDate)
          .SetInt32("productGroupId", productGroupId)
          .SetInt32("memberBranchId", memberBranchId ?? 0)
          .SetInt32("memberTypeId", memberTypeId)
          .List<decimal>();
        return result[0];
      }
      catch
      {
        return 0;
      }
    }

    /// <summary>
    /// Gets the finished product total sales for same period of the previous year
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="productId"></param>
    /// <param name="memberTypeId"></param>
    /// <param name="memberBranchId"></param>
    /// <returns></returns>
    public decimal GetFinishedProductsTotalSales(DateTime startDate, DateTime endDate, int productId, int memberTypeId, int? memberBranchId = 0)
    {
      try
      {
        var result = _applicationRepository.Session().CreateSQLQuery(
            "DECLARE @return_value int;EXEC @return_value = [dbo].[sp_GetFinishedProductsTotalSales] @StartDate=:startDate, @EndDate=:endDate, @ProductId=:productId, @MemberBranchId=:memberBranchId, @MemberTypeId=:memberTypeId;SELECT 'Return Value' = @return_value")
          .SetDateTime("startDate", startDate)
          .SetDateTime("endDate", endDate)
          .SetInt32("productId", productId)
          .SetInt32("memberBranchId", memberBranchId ?? 0)
          .SetInt32("memberTypeId", memberTypeId)
          .List<decimal>();
        return result[0];
      }
      catch
      {
        return 0;
      }
    }

    /// <summary>
    /// Gets the total feedsales by month
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="memberTypeId"></param>
    /// <param name="memberBranchId"></param>
    /// <returns></returns>
    public decimal GetTotalFeedsalesGroupByMonth(int year, int month, int memberTypeId, int? memberBranchId = 0)
    {
      try
      {
        var result = _applicationRepository.Session().CreateSQLQuery(
            "DECLARE @return_value int;EXEC @return_value = [dbo].[sp_GetTotalFeedsalesGroupByMonth] @Year=:year, @Month=:month, @MemberBranchId=:memberBranchId, @MemberTypeId=:memberTypeId;SELECT 'Return Value' = @return_value")
          .SetInt32("year", year)
          .SetInt32("month", month)
          .SetInt32("memberBranchId", memberBranchId ?? 0)
          .SetInt32("memberTypeId", memberTypeId)
          .List<decimal>();
        return result[0];
      }
      catch
      {
        return 0;
      }
    }

    /// <summary>
    /// Gets the total feedsales by month and year
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="memberTypeId"></param>
    /// <param name="memberBranchId"></param>
    /// <returns></returns>
    public decimal GetTotalFeedsalesGroupByYear(DateTime startDate, DateTime endDate, int memberTypeId, int? memberBranchId = 0)
    {
      try
      {
        var result = _applicationRepository.Session().CreateSQLQuery(
            "DECLARE @return_value int;EXEC @return_value = [dbo].[sp_GetTotalFeedsalesGroupByYear] @StartDate=:startDate, @EndDate=:endDate, @MemberBranchId=:memberBranchId, @MemberTypeId=:memberTypeId;SELECT 'Return Value' = @return_value")
          .SetDateTime("startDate", startDate)
          .SetDateTime("endDate", endDate)
          .SetInt32("memberBranchId", memberBranchId ?? 0)
          .SetInt32("memberTypeId", memberTypeId)
          .List<decimal>();
        return result[0];
      }
      catch
      {
        return 0;
      }
    }

    /// <summary>
    /// Gets the total raw material usage by category by year.
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="reportGroupCategoryId"></param>
    /// <param name="memberTypeId"></param>
    /// <param name="memberBranchId"></param>
    /// <returns></returns>
    public decimal GetTotalRawMaterialUsageByCategoryByYear(DateTime startDate, DateTime endDate, int reportGroupCategoryId, int memberTypeId, int? memberBranchId = 0)
    {
      try
      {
        var result = _applicationRepository.Session().CreateSQLQuery(
            "DECLARE @return_value int;EXEC @return_value = [dbo].[sp_GetTotalRawMaterialUsageByCategoryByYear] @StartDate=:startDate, @EndDate=:endDate, @ReportGroupCategoryId=:reportGroupCategoryId, @MemberBranchId=:memberBranchId, @MemberTypeId=:memberTypeId;SELECT 'Return Value' = @return_value")
          .SetDateTime("startDate", startDate)
          .SetDateTime("endDate", endDate)
          .SetInt32("reportGroupCategoryId", reportGroupCategoryId)
          .SetInt32("memberBranchId", memberBranchId ?? 0)
          .SetInt32("memberTypeId", memberTypeId)
          .List<decimal>();
        return result[0];
      }
      catch
      {
        return 0;
      }
    }

    /// <summary>
    /// Gets the total raw material usage by year.
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="reportGroupId"></param>
    /// <param name="reportCategoryId"></param>
    /// <param name="memberTypeId"></param>
    /// <param name="memberBranchId"></param>
    /// <returns></returns>
    public decimal GetTotalRawMaterialUsageByYear(DateTime startDate, DateTime endDate, int reportGroupId, int reportCategoryId, int memberTypeId, int? memberBranchId = 0)
    {
      try
      {
        var result = _applicationRepository.Session().CreateSQLQuery(
            "DECLARE @return_value int;EXEC @return_value = [dbo].[sp_GetTotalRawMaterialUsageByYear] @StartDate=:startDate, @EndDate=:endDate, @ReportGroupId=:reportGroupId, @ReportCategoryId=:reportCategoryId, @MemberBranchId=:memberBranchId, @MemberTypeId=:memberTypeId;SELECT 'Return Value' = @return_value")
          .SetDateTime("startDate", startDate)
          .SetDateTime("endDate", endDate)
          .SetInt32("reportGroupId", reportGroupId)
          .SetInt32("reportCategoryId", reportCategoryId)
          .SetInt32("memberBranchId", memberBranchId ?? 0)
          .SetInt32("memberTypeId", memberTypeId)
          .List<decimal>();
        return result[0];
      }
      catch
      {
        return 0;
      }
    }

    /// <summary>
    /// Gets the total finished product value
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="productId"></param>
    /// <param name="memberTypeId"></param>
    /// <param name="memberBranchId"></param>
    /// <returns></returns>
    public decimal GetTotalFinishedProduct(int year, int month, int productId, int memberTypeId, int? memberBranchId = 0)
    {
      try
      {
        var result = _applicationRepository.Session().CreateSQLQuery(
            "DECLARE @return_value int;EXEC @return_value = [dbo].[sp_GetTotalFinishedProduct] @Year=:year, @Month=:month, @ProductId=:productId, @MemberBranchId=:memberBranchId, @MemberTypeId=:memberTypeId;SELECT 'Return Value' = @return_value")
          .SetInt32("year", year)
          .SetInt32("month", month)
          .SetInt32("productId", productId)
          .SetInt32("memberBranchId", memberBranchId ?? 0)
          .SetInt32("memberTypeId", memberTypeId)
          .List<decimal>();
        return result[0];
      }
      catch
      {
        return 0;
      }
    }

    /// <summary>
    /// Gets the total raw material value
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="materialId"></param>
    /// <param name="memberTypeId"></param>
    /// <param name="memberBranchId"></param>
    /// <returns></returns>
    public decimal GetTotalRawMaterial(int year, int month, int materialId, int memberTypeId, int? memberBranchId = 0)
    {
      try
      {
        var result = _applicationRepository.Session().CreateSQLQuery(
            "DECLARE @return_value int;EXEC @return_value = [dbo].[sp_GetTotalRawMaterial] @Year=:year, @Month=:month, @MaterialId=:materialId, @MemberBranchId=:memberBranchId, @MemberTypeId=:memberTypeId;SELECT 'Return Value' = @return_value")
          .SetInt32("year", year)
          .SetInt32("month", month)
          .SetInt32("materialId", materialId)
          .SetInt32("memberBranchId", memberBranchId ?? 0)
          .SetInt32("memberTypeId", memberTypeId)
          .List<decimal>();
        return result[0];
      }
      catch
      {
        return 0;
      }
    }

    public async Task<IEnumerable<SubmissionStatsViewModel>> GetFeedsalesSubmissions(DateTime period)
    {
      var submissionStatsList = new List<SubmissionStatsViewModel>();
      var branches = await GetActiveMemberBranchesSummaryByModule((int)ModuleEnum.Feedsales).ConfigureAwait(false);

      foreach (var branch in branches.Where(x => x.Active).OrderBy(a => a.Member).ThenBy(a => a.Name))
      {
        var s = new SubmissionStatsViewModel
        {
          BranchName = string.IsNullOrEmpty(branch.Name) ? branch.Member! : branch.Name!,
          MemberName = branch.Member!,
          Submitted = false
        };

        try
        {
          var report = await GetReportForPeriod(period, branch.Id);
          s.DueDate = report.DueDate;
          s.InfoFor = $"{report.ReportYear}/{report.ReportMonth.ToString().PadLeft(2, '0')} ({report.InfoFor.ToString("MMMM yyyy")})";
          s.ReportId = report.Id;
          s.Submitted = report.IsSubmitted;
          s.DateSubmitted = report.SubmittedDate;
          s.Comment = report.Comment;
          s.MemberAccountEmail = report.MemberAccountEmail;
          s.Status = report.IsSubmitted ? "<span class='badge bg-success' title='The report for this member for this period is submitted.'>Submitted</span>" : "<span class='badge bg-warning' title='The report for this member for the selected period is outstanding.'>Outstanding</span>";
        }
        catch (Exception ex)
        {
          s.Status = "<span class='badge bg-danger' title='A report for this member for this period was not found. If this is a new member please use the Populate Report function populate the missing reports.'>REPORT NOT FOUND</span>";
        }

        submissionStatsList.Add(s);
      }

      return submissionStatsList;
    }

    public async Task<IEnumerable<SubmissionStatsViewModel>> GetSalmonellaSubmissions(DateTime period)
    {
      var submissionStatsList = new List<SubmissionStatsViewModel>();
      var branches = await GetActiveMemberBranchesSummaryByModule((int)ModuleEnum.Salmonella).ConfigureAwait(false);

      foreach (var branch in branches.OrderBy(a => a.Member).ThenBy(a => a.Name))
      {
        var s = new SubmissionStatsViewModel
        {
          BranchName = string.IsNullOrEmpty(branch.Name) ? branch.Member! : branch.Name!,
          MemberName = branch.Member!,
          Submitted = false
        };

        try
        {
          var report = await GetSalmonellaReportForPeriod(period, branch.Id);
          s.DueDate = report.DueDate;
          s.InfoFor = $"{report.ReportYear}/{report.ReportMonth.ToString().PadLeft(2, '0')} ({report.InfoFor.ToString("MMMM yyyy")})";
          s.ReportId = report.Id;
          s.Submitted = report.IsSubmitted;
          s.DateSubmitted = report.SubmittedDate;
          s.Comment = report.Comment;
          s.MemberAccountEmail = report.MemberAccountEmail;
          s.Status = report.IsSubmitted ? "<span class='badge bg-success' title='The report for this member for this period is submitted.'>Submitted</span>" : "<span class='badge bg-warning' title='The report for this member for the selected period is outstanding.'>Outstanding</span>";
        }
        catch (Exception ex)
        {
          s.Status = "<span class='badge bg-danger' title='A report for this month was not found.' title='A report for this member for this period was not found. If this is a new member please use the Populate Report function populate the missing reports.'>REPORT NOT FOUND</span>";
        }

        submissionStatsList.Add(s);
      }

      return submissionStatsList;
    }

    public async Task<FeedsalesReportModel> GetReportForPeriod(DateTime period, int memberBranchId)
    {
      try
      {
        var results = await _applicationRepository.QuerySingleAsync<FeedsalesReport>(x => x.ReportMonth == period.Month && x.ReportYear == period.Year && x.MemberBranch.Id == memberBranchId);
        return _mapper.Map<FeedsalesReport, FeedsalesReportModel>(results);
      }
      catch
      {
        return null;
      }
    }

    public async Task<SalmonellaReportLookupModel> GetSalmonellaReportForPeriod(DateTime period, int memberBranchId)
    {
      try
      {
        var results = await _applicationRepository.QuerySingleAsync<SalmonellaReportLookup>(x =>
          x.ReportMonth == period.Month && x.ReportYear == period.Year && x.MemberBranch.Id == memberBranchId);
        return _mapper.Map<SalmonellaReportLookup, SalmonellaReportLookupModel>(results);
      }
      catch
      {
        return null;
      }
    }

    #endregion

    #region Mycotoxin Admin Report

    public async Task<bool> PopulateMycotoxinReports(int memberId, int memberBranchId, int memberAccountId, DateTime startDate, DateTime endDate)
    {
      try
      {
        var reportDates = GeneralHelpers.MonthsBetween(startDate, endDate).ToList();
        IEnumerable<MemberSummaryModel> selectedMembers;
        IEnumerable<MemberBranchSummaryModel> selectedBranches;
        IEnumerable<MemberAccountModel> selectedAccounts;

        if (memberId == 0)
        {
          selectedMembers = await GetMembersSummaryByModule((int)ModuleEnum.Mycotoxin).ConfigureAwait(false);
        }
        else
        {
          selectedMembers = new List<MemberSummaryModel> { new() { Id = memberId } };
        }

        foreach (var member in selectedMembers)
        {
          if (memberBranchId == 0)
          {
            selectedBranches = await GetMemberBranchesSummary(member.Id).ConfigureAwait(false);
          }
          else
          {
            selectedBranches = new List<MemberBranchSummaryModel> { new() { Id = memberBranchId, MemberId = member.Id } };
          }

          foreach (var branch in selectedBranches)
          {
            if (memberAccountId == 0)
            {
              selectedAccounts = await GetMemberBranchAccounts(branch.Id).ConfigureAwait(false);
            }
            else
            {
              selectedAccounts = new List<MemberAccountModel>
              {
                new()
                {
                  Id = memberAccountId,
                  Modules = new List<MemberAccountModuleModel>
                  {
                    new()
                    {
                      ModuleId = (int)ModuleEnum.Mycotoxin
                    }
                  }
                }
              };
            }

            foreach (var reportDate in reportDates)
            {
              var reportExists = await CheckIfMycotoxinReportExists(branch.Id, reportDate.Month, reportDate.Year)
                .ConfigureAwait(false);

              if (!reportExists)
              {
                var dueDate = reportDate.AddMonths(1);
                var report = new MycotoxinReportModel
                {
                  DueDate = new DateTime(dueDate.Year, dueDate.Month, 20),
                  InfoFor = reportDate,
                  IsSubmitted = false,
                  MemberBranchId = branch.Id,
                  MemberId = memberId,
                  ReportMonth = reportDate.Month,
                  ReportYear = reportDate.Year,
                };

                if (memberAccountId == -1)
                {
                  report.MemberAccountId = null;
                }
                else
                {
                  report.MemberAccountId =
                    selectedAccounts.FirstOrDefault(x => x.Modules.Any(m => m.ModuleId == (int)ModuleEnum.Mycotoxin))!
                      .Id;
                }

                var result = await SaveOrUpdate<MycotoxinReportModel>(report);
              }
            }
          }
        }

        return true;
      }
      catch
      {
        return false;
      }
    }

    public async Task<bool> CheckIfMycotoxinReportExists(int memberBranchId, int month, int year)
    {
      var result1 =
        await _applicationRepository.QueryAnyAsync<MycotoxinReport>(x => x.ReportYear == year && x.ReportMonth == month && x.MemberBranch.Id == memberBranchId);

      var result2 =
        await _applicationRepository.QueryAnyAsync<MycotoxinReport>(x => x.InfoFor.Year == year && x.InfoFor.Month == month && x.MemberBranch.Id == memberBranchId);

      return result1 && result2;
    }

    #endregion

    #region CRUD Methods and Generic SQL functions

    #region Stored Procedure and SQL Queries

    public T? ExecuteSqlQuery<T>(string sqlQuery)
    {
      try
      {
        if (string.IsNullOrEmpty(sqlQuery))
        {
          return default(T?);
        }

        var result = _applicationRepository.Session().CreateSQLQuery(sqlQuery).List<T>();
        return result[0];
      }
      catch
      {
        return default(T?);
      }
    }

    #endregion

    ///<inheritdoc/>
    public async Task<SaveOrUpdateResult> SaveOrUpdate<TSource>(TSource source)
    {
      var mapped = _mapper.Map(source, source!.GetType(), source!.GetAttribute<MapToAttribute>()!.EntityType);
      var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(mapped);

      return new SaveOrUpdateResult
      {
        Success = result.Succeeded,
        ErrorMessage = result.ExceptionMessage,
        SavedObjectId = result.SavedObjectId,
      };
    }

    ///<inheritdoc/>
    public async Task<SaveOrUpdateResult> Delete<TSource>(TSource source)
    {
      var result = _applicationRepository.DeleteAndCommit(_mapper.Map(source, source!.GetType(), source!.GetAttribute<MapToAttribute>()!.EntityType));

      return await Task.FromResult(new SaveOrUpdateResult
      {
        Success = result.Succeeded,
        ErrorMessage = result.ExceptionMessage,
      });
    }

    #endregion

    public async Task<IEnumerable<ReportModel>> GetReports()
    {
      var result = await _applicationRepository.QueryAsync<Report>();

      return result.Select(a => new ReportModel
      {
        Id = a.Id
      });
    }

    public async Task<SalmonellaSampleModel> GetSalmonellaSampleModelById(int id)
    {
      var result = await _applicationRepository.QueryFirstOrDefaultAsync<SalmonellaSample>(x => x.Id == id);

      return _mapper.Map<SalmonellaSampleModel>(result);
    }

    public async Task<FeedsalesReportItemModel> GetFeedsalesReportItemModelById(int id)
    {
      var result = await _applicationRepository.QueryFirstOrDefaultAsync<FeedsalesReportItem>(x => x.Id == id);

      return _mapper.Map<FeedsalesReportItemModel>(result);
    }

    public async Task<int> GetCurrentMemberAccountId()
    {
      var user = await _identityLayer.Identity.GetCurrentUser();
      var memberAccount = await _applicationRepository.QueryFirstOrDefaultAsync<MemberAccount>(x => x.ApplicationUser.Id == user.Id);
      return memberAccount.Id;
    }

    public async Task<MemberAccessViewModel> GetCurrentMemberAccess()
    {
      try
      {
        var user = await _identityLayer.Identity.GetCurrentUser();
        var memberAccount = await _applicationRepository.QueryFirstOrDefaultAsync<MemberAccount>(x =>
          x.ApplicationUser.Id == user.Id && x.ApplicationUser.Id == user.Id);
        return new MemberAccessViewModel
        {
          MemberBranchId = memberAccount.MemberBranch.Id,
          MemberBranch = memberAccount.MemberBranch.Name,
          MemberAccountId = memberAccount.Id,
          MemberId = memberAccount.Member.Id,
          Member = memberAccount.Member.Name,
          MemberTypeId = memberAccount.Member.MemberType?.Id,
          UserId = user.Id,
          UserName = user.UserName,
          FirstName = memberAccount.ApplicationUser.FirstName,
          LastName = memberAccount.ApplicationUser.LastName,
          LastLogin = memberAccount.ApplicationUser.LastLoginDate,
          HasAccess = true,
          HasFeedsalesAccess = await CheckMemberAccountModule(memberAccount.Id, ModuleEnum.Feedsales),
          HasIndustryProfileAccess =
            await CheckMemberAccountModule(memberAccount.Id, ModuleEnum.IndustryProfile),
          HasMycotoxinAccess = await CheckMemberAccountModule(memberAccount.Id, ModuleEnum.Mycotoxin),
          HasSalmonellaAccess = await CheckMemberAccountModule(memberAccount.Id, ModuleEnum.Salmonella),
        };
      }
      catch (Exception ex)
      {
        return new MemberAccessViewModel
        {
          HasAccess = false,
          HasFeedsalesAccess = true,
          HasSalmonellaAccess = true,
          HasMycotoxinAccess = true,
          HasIndustryProfileAccess = true,
        };
      }
    }

    public async Task<string> GetCurrentUserId()
    {
      var user = await _identityLayer.Identity.GetCurrentUser();
      return user.Id;
    }

    public async Task<bool> PopulateFeedsalesReports(int memberId, int memberBranchId, int memberAccountId, DateTime startDate, DateTime endDate)
    {
      try
      {
        var reportDates = GeneralHelpers.MonthsBetween(startDate, endDate);
        IEnumerable<MemberSummaryModel> selectedMembers;
        IEnumerable<MemberBranchSummaryModel> selectedBranches;
        IEnumerable<MemberAccountModel> selectedAccounts;

        if (memberId == 0)
        {
          selectedMembers = await GetMembersSummaryByModule((int)ModuleEnum.Feedsales).ConfigureAwait(false);
        }
        else
        {
          selectedMembers = new List<MemberSummaryModel> { new() { Id = memberId } };
        }

        foreach (var member in selectedMembers)
        {
          if (memberBranchId == 0)
          {
            selectedBranches = await GetActiveMemberBranchesSummary(member.Id).ConfigureAwait(false);
          }
          else
          {
            selectedBranches = new List<MemberBranchSummaryModel> { new() { Id = memberBranchId, MemberId = member.Id } };
          }

          foreach (var branch in selectedBranches)
          {
            if (memberAccountId == 0)
            {
              selectedAccounts = await GetMemberBranchAccounts(branch.Id).ConfigureAwait(false);
            }
            else
            {
              selectedAccounts = new List<MemberAccountModel>
              {
                new() { Id = memberAccountId,Modules = new List<MemberAccountModuleModel>{new() {ModuleId = (int)ModuleEnum.Feedsales } }}
              };
            }

            foreach (var reportDate in reportDates)
            {
              if (!CheckIfFeedsalesReportExists(branch.Id, reportDate.Month, reportDate.Year).Result)
              {
                var dueDate = reportDate.AddMonths(1);
                var feedsalesReport = new FeedsalesReportModel
                {
                  CreatedBy = "System",
                  CreatedDate = DateTime.Now,
                  DueDate = new DateTime(dueDate.Year, dueDate.Month, 20),
                  InfoFor = reportDate,
                  IsSubmitted = false,
                  MemberBranchId = branch.Id,
                  MemberId = member.Id,
                  ReportMonth = reportDate.Month,
                  ReportYear = reportDate.Year,
                };

                if (memberAccountId == -1)
                {
                  feedsalesReport.MemberAccountId = null;
                }
                else
                {
                  feedsalesReport.MemberAccountId = selectedAccounts.ToList()
                    .FirstOrDefault(x => x.Modules.Any(m => m.ModuleId == (int)ModuleEnum.Feedsales))!.Id;
                }

                var result = await SaveOrUpdate<FeedsalesReportModel>(feedsalesReport);
              }
            }
          }
        }

        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public async Task<bool> PopulateSalmonellaReports(int memberId, int memberBranchId, int memberAccountId, DateTime startDate, DateTime endDate)
    {
      Log.Debug("PopulateSalmonellaReports");
      try
      {
        var reportDates = GeneralHelpers.MonthsBetween(startDate, endDate);
        IEnumerable<MemberSummaryModel> selectedMembers;
        IEnumerable<MemberBranchSummaryModel> selectedBranches;
        IEnumerable<MemberAccountModel> selectedAccounts;

        if (memberId == 0)
        {
          Log.Debug("PopulateSalmonellaReports: Generate for all members!!");
          selectedMembers = await GetMembersSummaryByModule((int)ModuleEnum.Salmonella).ConfigureAwait(false);
        }
        else
        {
          Log.Debug($"PopulateSalmonellaReports: Generate for member: {memberId}");
          selectedMembers = new List<MemberSummaryModel> { new() { Id = memberId } };
        }

        foreach (var member in selectedMembers)
        {
          if (memberBranchId == 0)
          {
            Log.Debug("PopulateSalmonellaReports: Generate for all branches");
            selectedBranches = await GetActiveMemberBranchesSummary(member.Id).ConfigureAwait(false);
          }
          else
          {
            Log.Debug($"PopulateSalmonellaReports: Generate for branch: {memberId}");
            selectedBranches = new List<MemberBranchSummaryModel> { new() { Id = memberBranchId, MemberId = member.Id } };
          }

          foreach (var branch in selectedBranches)
          {
            if (memberAccountId == 0)
            {
              Log.Debug("PopulateSalmonellaReports: Generate for all accounts");
              selectedAccounts = await GetActiveMemberBranchAccounts(branch.Id, ModuleEnum.Salmonella).ConfigureAwait(false);
            }
            else
            {
              Log.Debug($"PopulateSalmonellaReports: Generate for member account: {memberAccountId}");
              selectedAccounts = new List<MemberAccountModel>
              {
                new() { Id = memberAccountId,Modules = new List<MemberAccountModuleModel>{new() {ModuleId = (int)ModuleEnum.Salmonella } }}
              };
            }

            if (selectedAccounts.Any())
            {
              foreach (var reportDate in reportDates)
              {
                Log.Debug($"PopulateSalmonellaReports: Checking if report exists: BranchId: {branch.Id}, Month: {reportDate.Month}, Year: {reportDate.Year}");
                if (!CheckIfSalmonellaReportExists(branch.Id, reportDate.Month, reportDate.Year).Result)
                {
                  var dueDate = reportDate.AddMonths(1);
                  var report = new SalmonellaReportModel
                  {
                    DueDate = new DateTime(dueDate.Year, dueDate.Month, 20),
                    InfoFor = reportDate,
                    IsSubmitted = false,
                    MemberBranchId = branch.Id,
                    MemberId = member.Id,
                    ReportMonth = reportDate.Month,
                    ReportYear = reportDate.Year,
                  };

                  if (memberAccountId == -1)
                  {
                    report.MemberAccountId = null;
                  }
                  else
                  {
                    report.MemberAccountId = selectedAccounts.ToList()
                      .FirstOrDefault(x => x.Modules.Any(m => m.ModuleId == (int)ModuleEnum.Salmonella))!.Id;
                  }

                  var result = await SaveOrUpdate<SalmonellaReportModel>(report);
                  Log.Debug($"PopulateSalmonellaReports: Report generated with ReportId: {result.SavedObjectId}");
                }
              }
            }
            else
            {
              Log.Debug($"PopulateSalmonellaReports: No salmonella contacts found for member: {memberId}");
            }
          }
        }

        Log.Debug($"PopulateSalmonellaReports: Report generation completed for member: {memberId}");
        return true;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Report could not be generated!");
        return false;
      }
    }

    public async Task<bool> CheckIfFeedsalesReportExists(int memberBranchId, int month, int year)
    {
      var result1 =
        await _applicationRepository.QueryAnyAsync<FeedsalesReport>(x => x.ReportYear == year && x.ReportMonth == month && x.MemberBranch.Id == memberBranchId);

      var result2 =
        await _applicationRepository.QueryAnyAsync<FeedsalesReport>(x => x.InfoFor.Year == year && x.InfoFor.Month == month && x.MemberBranch.Id == memberBranchId);

      return result1 && result2;
    }

    public async Task<bool> CheckIfSalmonellaReportExists(int memberBranchId, int month, int year)
    {
      var result1 =
        await _applicationRepository.QueryAnyAsync<SalmonellaReport>(x => x.ReportYear == year && x.ReportMonth == month && x.MemberBranch.Id == memberBranchId);

      var result2 =
        await _applicationRepository.QueryAnyAsync<SalmonellaReport>(x => x.InfoFor.Year == year && x.InfoFor.Month == month && x.MemberBranch.Id == memberBranchId);

      return result1 && result2;
    }

    #region Industry Profile

    /// <summary>
    /// Returns a filtered list of industry profile reports.
    /// </summary>
    /// <param name="pageRequest">The pages request.</param>
    /// <param name="memberId">The member Id.</param>
    /// <param name="memberBranchId">The member branch Id.</param>
    /// <param name="isSubmitted">The submitted indicator.</param>
    /// <returns>A paged results with industry profile reports.</returns>
    public async Task<IEnumerable<IndustryProfileReportModel>> GetIndustryProfileReports(AlphaPageRequest pageRequest, int? memberId, int? memberBranchId, bool? isSubmitted)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest,
        SearchExpression.FilterIndustryProfileReports(memberId, memberBranchId, isSubmitted));
      var resultMapped = _mapper.Map<IEnumerable<IndustryProfileReport>, List<IndustryProfileReportModel>>(result.PageData);
      return new StaticPagedList<IndustryProfileReportModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    public async Task<IEnumerable<IndustryProfileReportModel>> GetIndustryProfileReports()
    {
      var result = await _applicationRepository.QueryAsync<IndustryProfileReport>().ConfigureAwait(false);
      return _mapper.Map<IEnumerable<IndustryProfileReport>, List<IndustryProfileReportModel>>(result);
    }

    public async Task<IEnumerable<IndustryProfileReportModel>> GetIndustryProfileReports(int memberId, int memberBranchId, bool submitted)
    {
      var result = await _applicationRepository.QueryAsync<IndustryProfileReport>(x => x.Member!.Id == memberId && x.MemberBranch!.Id == memberBranchId).ConfigureAwait(false);

      if (submitted)
      {
        result = result.Where(x => x.Submitted);
      }

      return _mapper.Map<IEnumerable<IndustryProfileReport>, List<IndustryProfileReportModel>>(result);
    }

    public async Task<IndustryProfileReportModel> GetIndustryProfileReport(int industryProfileReportId)
    {
      var result = await _applicationRepository.QuerySingleAsync<IndustryProfileReport>(r => r.Id == industryProfileReportId).ConfigureAwait(false);
      return _mapper.Map<IndustryProfileReport, IndustryProfileReportModel>(result);
    }

    public async Task<IndustryProfileReportModel> GetIndustryProfileReport(int industryProfileReportId, int memberId, int memberBranchId)
    {
      var result = await _applicationRepository.QuerySingleAsync<IndustryProfileReport>(r => r.Id == industryProfileReportId && r.Member.Id == memberId && r.MemberBranch.Id == memberBranchId).ConfigureAwait(false);
      return _mapper.Map<IndustryProfileReport, IndustryProfileReportModel>(result);
    }

    public async Task<ICollection<MemberIpBaseItemModel>> GetMemberIpBaseItems(IndustryProfileDisplayType displayType, int memberId, int memberBranchId, int industryProfileReportId, int memberTypeId, int reportYear = 0)
    {
      try
      {
        var results = await _applicationRepository
          .QueryAsync<MemberIpBaseItem>(x => x.Member.MemberType!.Id == memberTypeId).ConfigureAwait(false);

        if (displayType == IndustryProfileDisplayType.Dashboard)
        {
          results = results.Where(x => x.IndustryProfileReport.ReportYear == reportYear);

          if (memberId != 0)
          {
            results = results.Where(x => x.Member.Id == memberId);
          }

          if (memberBranchId != 0)
          {
            results = results.Where(x => x.MemberBranch.Id == memberBranchId);
          }
        }
        else
        {
          results = results.Where(x =>
            x.MemberBranch.Id == memberBranchId && x.IndustryProfileReport.Id == industryProfileReportId);
        }

        return _mapper.Map<IEnumerable<MemberIpBaseItem>, List<MemberIpBaseItemModel>>(results);
      }
      catch (Exception ex)
      {
        throw new Exception("Could not retrieve Member Base IP items for the selected Industry Profile report", ex);
      }
    }

    public async Task<ICollection<MemberIpAdminItemModel>> GetMemberIpAdminItems(IndustryProfileDisplayType displayType, int memberId, int memberBranchId, int industryProfileReportId, int memberTypeId, int reportYear = 0)
    {
      try
      {
        var results = await _applicationRepository.QueryAsync<MemberIpAdminItem>(x => x.Member.MemberType!.Id == memberTypeId).ConfigureAwait(false);

        if (displayType == IndustryProfileDisplayType.Dashboard)
        {
          results = results.Where(x => x.IndustryProfileReport.ReportYear == reportYear);

          if (memberId != 0)
          {
            results = results.Where(x => x.Member.Id == memberId);
          }

          if (memberBranchId != 0)
          {
            results = results.Where(x => x.MemberBranch.Id == memberBranchId);
          }
        }
        else
        {
          results = results.Where(x =>
            x.MemberBranch.Id == memberBranchId && x.IndustryProfileReport.Id == industryProfileReportId);
        }

        return _mapper.Map<IEnumerable<MemberIpAdminItem>, List<MemberIpAdminItemModel>>(results);
      }
      catch (Exception ex)
      {
        throw new Exception(
          "Could not retrieve Member Administration IP Items for the selected Industry Profile report", ex);
      }
    }

    public async Task<ICollection<MemberIpAdditionalItemModel>> GetMemberIpAdditionalItems(IndustryProfileDisplayType displayType, int memberId, int memberBranchId, int industryProfileReportId, int memberTypeId, int reportYear = 0)
    {
      try
      {
        var results = await _applicationRepository.QueryAsync<MemberIpAdditionalItem>(x => x.Member.MemberType!.Id == memberTypeId).ConfigureAwait(false);

        if (displayType == IndustryProfileDisplayType.Dashboard)
        {
          results = results.Where(x => x.IndustryProfileReport.ReportYear == reportYear);

          if (memberId != 0)
          {
            results = results.Where(x => x.Member.Id == memberId);
          }

          if (memberBranchId != 0)
          {
            results = results.Where(x => x.MemberBranch.Id == memberBranchId);
          }
        }
        else
        {
          results = results.Where(x =>
            x.MemberBranch.Id == memberBranchId && x.IndustryProfileReport.Id == industryProfileReportId);
        }

        return _mapper.Map<IEnumerable<MemberIpAdditionalItem>, List<MemberIpAdditionalItemModel>>(results);
      }
      catch (Exception ex)
      {
        throw new Exception("Could not retrieve Member Additional IP Items for the selected Industry Profile report",
          ex);
      }
    }

    public async Task<ICollection<MemberRunningCostViewModel>> GetMemberRunningCosts(IndustryProfileDisplayType displayType, int memberId, int memberBranchId, int industryProfileReportId, int memberTypeId, int reportYear = 0)
    {
      #region Get Company Running Cost Model

      try
      {
        //var results = await _applicationRepository.QueryAsync<MemberRunningCost>(x =>
        //  !x.IndustryProfileReport.Submitted && x.Member.MemberType!.Id == memberTypeId).ConfigureAwait(false);

        var results = await _applicationRepository
          .QueryAsync<MemberRunningCost>(x => x.Member.MemberType!.Id == memberTypeId).ConfigureAwait(false);

        if (displayType == IndustryProfileDisplayType.Dashboard)
        {
          results = results.Where(x => x.IndustryProfileReport.ReportYear == reportYear);

          if (memberId > 0 && memberBranchId > 0)
          {
            results = results.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId);
          }

          if (memberId > 0 && memberBranchId == 0)
          {
            results = results.Where(x => x.Member.Id == memberId);
          }

          if (memberId == 0 && memberBranchId == 0)
          {
            //items = items.Where(x => x.memberId == memberId);
          }
        }
        else
        {
          results = results.Where(x =>
            x.MemberBranch.Id == memberBranchId && x.IndustryProfileReport.Id == industryProfileReportId);
        }

        var memberRunningCosts = new List<MemberRunningCostViewModel>();
        var runningCostGroups = await _applicationRepository.QueryAsync<RunningCostGroup>().ConfigureAwait(false);

        foreach (var group in runningCostGroups)
        {
          var memberRunningCostViewModel = new MemberRunningCostViewModel
          {
            GroupCode = group.GroupCode,
            GroupName = group.GroupName,
            GroupId = group.Id,
            IndustryProfileReportId = industryProfileReportId,
            Id = displayType == IndustryProfileDisplayType.Other
              ? results.Single(x => x.RunningCostGroup.Id == group.Id && x.RunningCost.Id == null).Id
              : 0,
            CalculationType = (CalculationType)group.CalculationType,
            MemberId = memberId,
            MemberBranchId = memberBranchId,
            ProductionEnabled = group.ProductionEnabled,
            SalesEnabled = group.SalesEnabled,
            AdministrationEnabled = group.AdministrationEnabled,
            RoadTransportEnabled = group.RoadTransportEnabled,
          };

          if ((CalculationType)group.CalculationType == CalculationType.Value)
          {
            memberRunningCostViewModel.ProductionValue = results
              .Where(x => x.RunningCostGroup.Id == group.Id && x.RunningCost == null).Sum(sum => sum.ProductionValue);
            memberRunningCostViewModel.SalesValue = results
              .Where(x => x.RunningCostGroup.Id == group.Id && x.RunningCost == null).Sum(sum => sum.SalesValue);
            memberRunningCostViewModel.AdministrationValue = results
              .Where(x => x.RunningCostGroup.Id == group.Id && x.RunningCost == null)
              .Sum(sum => sum.AdministrationValue);
            memberRunningCostViewModel.RoadTransportValue = results
              .Where(x => x.RunningCostGroup.Id == group.Id && x.RunningCost == null)
              .Sum(sum => sum.RoadTransportValue);
          }

          if ((CalculationType)group.CalculationType == CalculationType.Formula)
          {
            switch (group.Id)
            {
              case 27:
                memberRunningCostViewModel.ProductionValue =
                  await GetValueFromTable<MemberRunningCostSundry>(industryProfileReportId, group.Id, group.Id,
                    CostingGroup.ProductionValue, memberId, memberBranchId, reportYear).ConfigureAwait(false);

                memberRunningCostViewModel.SalesValue =
                  await GetValueFromTable<MemberRunningCostSundry>(industryProfileReportId, group.Id, group.Id,
                    CostingGroup.SalesValue, memberId, memberBranchId, reportYear).ConfigureAwait(false);

                memberRunningCostViewModel.AdministrationValue =
                  await GetValueFromTable<MemberRunningCostSundry>(industryProfileReportId, group.Id, group.Id,
                    CostingGroup.AdministrationValue, memberId, memberBranchId, reportYear).ConfigureAwait(false);

                memberRunningCostViewModel.RoadTransportValue =
                  await GetValueFromTable<MemberRunningCostSundry>(industryProfileReportId, group.Id, group.Id,
                    CostingGroup.RoadTransportValue, memberId, memberBranchId, reportYear).ConfigureAwait(false);
                break;
            }
          }

          if (group.RunningCosts.Any())
          {
            foreach (var subItem in group.RunningCosts)
            {
              var memberRunningCostSubItemViewModel = new MemberRunningCostViewModel
              {
                GroupCode = string.Empty,
                GroupName = subItem.ItemName,
                GroupId = subItem.RunningCostGroup.Id,
                IndustryProfileReportId = industryProfileReportId,
                CalculationType = (CalculationType)group.CalculationType,
                ItemId = subItem.Id,
                MemberId = memberId,
                MemberBranchId = memberBranchId,
                ProductionEnabled = group.ProductionEnabled,
                SalesEnabled = group.SalesEnabled,
                AdministrationEnabled = group.AdministrationEnabled,
                RoadTransportEnabled = group.RoadTransportEnabled,
              };

              if (memberRunningCostSubItemViewModel.CalculationType == CalculationType.Value)
              {
                memberRunningCostSubItemViewModel.ProductionValue = results
                  .Where(x => x.RunningCostGroup.Id == group.Id && x.RunningCost.Id == subItem.Id)
                  .Sum(sum => sum.ProductionValue);
                memberRunningCostSubItemViewModel.SalesValue = results
                  .Where(x => x.RunningCostGroup.Id == group.Id && x.RunningCost.Id == subItem.Id)
                  .Sum(sum => sum.SalesValue);
                memberRunningCostSubItemViewModel.AdministrationValue = results
                  .Where(x => x.RunningCostGroup.Id == group.Id && x.RunningCost.Id == subItem.Id)
                  .Sum(sum => sum.AdministrationValue);
                memberRunningCostSubItemViewModel.RoadTransportValue = results
                  .Where(x => x.RunningCostGroup.Id == group.Id && x.RunningCost.Id == subItem.Id)
                  .Sum(sum => sum.RoadTransportValue);
              }

              if ((CalculationType)group.CalculationType == CalculationType.Formula)
              {
                switch (group.Id)
                {
                  case 27:
                    memberRunningCostViewModel.ProductionValue =
                      await GetValueFromTable<MemberRunningCostSundry>(industryProfileReportId, group.Id, group.Id,
                        CostingGroup.ProductionValue, memberId, memberBranchId, reportYear).ConfigureAwait(false);

                    memberRunningCostViewModel.SalesValue =
                      await GetValueFromTable<MemberRunningCostSundry>(industryProfileReportId, group.Id, group.Id,
                        CostingGroup.SalesValue, memberId, memberBranchId, reportYear).ConfigureAwait(false);

                    memberRunningCostViewModel.AdministrationValue =
                      await GetValueFromTable<MemberRunningCostSundry>(industryProfileReportId, group.Id, group.Id,
                        CostingGroup.AdministrationValue, memberId, memberBranchId, reportYear).ConfigureAwait(false);

                    memberRunningCostViewModel.RoadTransportValue =
                      await GetValueFromTable<MemberRunningCostSundry>(industryProfileReportId, group.Id, group.Id,
                        CostingGroup.RoadTransportValue, memberId, memberBranchId, reportYear).ConfigureAwait(false);
                    break;
                }
              }

              memberRunningCostViewModel.SubItems.Add(memberRunningCostSubItemViewModel);
            }
          }

          memberRunningCosts.Add(memberRunningCostViewModel);
        }

        #endregion

        return memberRunningCosts;
      }
      catch (Exception ex)
      {
        throw new Exception("Could not retrieve Member Running Costs for the selected Industry Profile report", ex);
      }
    }

    public async Task<ICollection<MemberRunningCostFleetModel>> GetMemberRunningCostsFleet(IndustryProfileDisplayType displayType, int memberId, int memberBranchId, int industryProfileReportId, int memberTypeId, int reportYear = 0)
    {
      try
      {
        var results = await _applicationRepository.QueryAsync<MemberRunningCostFleet>(x => x.Member.MemberType!.Id == memberTypeId).ConfigureAwait(false);

        if (displayType == IndustryProfileDisplayType.Dashboard)
        {
          results = results.Where(x => x.IndustryProfileReport.ReportYear == reportYear);

          if (memberId != 0)
          {
            results = results.Where(x => x.Member.Id == memberId);
          }

          if (memberBranchId != 0)
          {
            results = results.Where(x => x.MemberBranch.Id == memberBranchId);
          }
        }
        else
        {
          results = results.Where(x =>
            x.MemberBranch.Id == memberBranchId && x.IndustryProfileReport.Id == industryProfileReportId);
        }

        return _mapper.Map<IEnumerable<MemberRunningCostFleet>, List<MemberRunningCostFleetModel>>(results);
      }
      catch (Exception ex)
      {
        throw new Exception(
          "Could not retrieve Member Administration IP Items for the selected Industry Profile report", ex);
      }
    }

    public async Task<ICollection<MemberRunningCostSundryModel>> GetMemberRunningCostsSundries(IndustryProfileDisplayType displayType, int memberId, int memberBranchId, int industryProfileReportId, int memberTypeId, int reportYear = 0)
    {
      try
      {
        var results = await _applicationRepository.QueryAsync<MemberRunningCostSundry>(x => x.Member.MemberType!.Id == memberTypeId).ConfigureAwait(false);

        if (displayType == IndustryProfileDisplayType.Dashboard)
        {
          results = results.Where(x => x.IndustryProfileReport.ReportYear == reportYear);

          if (memberId != 0)
          {
            results = results.Where(x => x.Member.Id == memberId);
          }

          if (memberBranchId != 0)
          {
            results = results.Where(x => x.MemberBranch.Id == memberBranchId);
          }
        }
        else
        {
          results = results.Where(x =>
            x.MemberBranch.Id == memberBranchId && x.IndustryProfileReport.Id == industryProfileReportId);
        }

        return _mapper.Map<IEnumerable<MemberRunningCostSundry>, List<MemberRunningCostSundryModel>>(results);
      }
      catch (Exception ex)
      {
        throw new Exception(
          "Could not retrieve Member Running Costs Fleet items for the selected Industry Profile report", ex);
      }
    }

    public async Task<ICollection<MemberTotalRawMaterialCostModel>> GetMemberTotalRawMaterialCosts(IndustryProfileDisplayType displayType, int memberId, int memberBranchId, int industryProfileReportId, int memberTypeId, int reportYear = 0)
    {
      try
      {
        var results = await _applicationRepository.QueryAsync<MemberTotalRawMaterialCost>(x => x.Member.MemberType!.Id == memberTypeId).ConfigureAwait(false);

        if (displayType == IndustryProfileDisplayType.Dashboard)
        {
          results = results.Where(x => x.IndustryProfileReport.ReportYear == reportYear);

          if (memberId != 0)
          {
            results = results.Where(x => x.Member.Id == memberId);
          }

          if (memberBranchId != 0)
          {
            results = results.Where(x => x.MemberBranch.Id == memberBranchId);
          }
        }
        else
        {
          results = results.Where(x =>
            x.MemberBranch.Id == memberBranchId && x.IndustryProfileReport.Id == industryProfileReportId);
        }

        return _mapper.Map<IEnumerable<MemberTotalRawMaterialCost>, List<MemberTotalRawMaterialCostModel>>(results);
      }
      catch (Exception ex)
      {
        throw new Exception(
          "Could not retrieve Member Total Raw Material Cost items for the selected Industry Profile report", ex);
      }
    }

    public async Task<ICollection<MemberPcbCostItemModel>> GetMemberPcbCostItems(PcbItem pcbItem, IndustryProfileDisplayType displayType, int memberId, int memberBranchId, int industryProfileReportId, int memberTypeId, int reportYear = 0)
    {
      try
      {
        var results = await _applicationRepository.QueryAsync<MemberPcbCostItem>(x =>
          x.Member.MemberType!.Id == memberTypeId &&
          x.PcbCostMainGroup.Id == (int)pcbItem).ConfigureAwait(false);

        if (memberTypeId > 0)
        {
          results = results.Where(s => s.IndustryProfileReport.Member.MemberType.Id == memberTypeId);
        }

        if (displayType == IndustryProfileDisplayType.Dashboard)
        {
          results = results.Where(x => x.IndustryProfileReport.ReportYear == reportYear);

          if (memberId > 0 && memberBranchId > 0)
          {
            results = results.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId);
          }

          if (memberId > 0 && memberBranchId == 0)
          {
            results = results.Where(x => x.Member.Id == memberId);
          }

          if (memberId == 0 && memberBranchId == 0)
          {
            //items = items.Where(x => x.memberId == memberId);
          }
        }
        else
        {
          results = results.Where(x => x.MemberBranch.Id == memberBranchId && x.IndustryProfileReport.Id == industryProfileReportId);
        }

        return _mapper.Map<IEnumerable<MemberPcbCostItem>, List<MemberPcbCostItemModel>>(results);
      }
      catch (Exception ex)
      {
        throw new Exception(
          $"Could not retrieve Member {pcbItem} cost items for the selected Industry Profile report", ex);
      }
    }

    public async Task<ICollection<IndustryCostGroupModel>> GetIndustryCostGroups(bool showAll = true)
    {
      try
      {
        var results = await _applicationRepository.QueryAsync<IndustryCostGroup>().ConfigureAwait(false);

        if (!showAll)
        {
          results = results.Where(x => x.Active);
        }

        return _mapper.Map<IEnumerable<IndustryCostGroup>, List<IndustryCostGroupModel>>(results);
      }
      catch (Exception ex)
      {
        throw new Exception(
          "Could not retrieve Member Industry Cost group items for the selected Industry Profile report", ex);
      }
    }

    public async Task<decimal> GetValueFromTable<TEntity>(int industryProfileReportId, int groupId, int itemId, CostingGroup costingGroup, int memberId, int memberBranchId, int year = 0) where TEntity : class, IIndustryCostMultiple
    {
      var result = await _applicationRepository.QueryAsync<TEntity>(x => x.IndustryProfileReport.Submitted);

      switch (costingGroup)
      {
        case CostingGroup.AdministrationValue:
          if (year == 0)
          {
            return result.Where(x => x.IndustryProfileReport.Id == industryProfileReportId).Sum(sum => (decimal?)sum.AdministrationValue) ?? 0;
          }
          else
          {
            var t = result.Where(x => x.IndustryProfileReport.ReportYear == year);

            if (memberId > 0 && memberBranchId > 0)
            {
              return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.AdministrationValue) ?? 0;
            }

            if (memberId > 0 && memberBranchId == 0)
            {
              return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.AdministrationValue) ?? 0;
            }

            if (memberId == 0 && memberBranchId == 0)
            {
              return t.Sum(sum => (decimal?)sum.AdministrationValue) ?? 0;
            }

            return 0;

            //return entity.Where(x => x.IPCBReport.ReportYear == year).Sum(sum => (decimal?)sum.AdministrationValue) ?? 0;
          }

        case CostingGroup.ProductionValue:
          if (year == 0)
          {
            return result.Where(x => x.IndustryProfileReport.Id == industryProfileReportId).Select(a => (decimal?)a.ProductionValue).Sum() ?? 0;
          }
          else
          {
            var t = result.Where(x => x.IndustryProfileReport.ReportYear == year);

            if (memberId > 0 && memberBranchId > 0)
            {
              return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.ProductionValue) ?? 0;
            }

            if (memberId > 0 && memberBranchId == 0)
            {
              return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.ProductionValue) ?? 0;
            }

            if (memberId == 0 && memberBranchId == 0)
            {
              return t.Sum(sum => (decimal?)sum.ProductionValue) ?? 0;
            }

            return 0;

            //return entity.Where(x => x.IPCBReport.ReportYear == year).Select(a => (Decimal?)a.ProductionValue).Sum() ?? 0;
          }

        case CostingGroup.RoadTransportValue:
          if (year == 0)
          {
            return result.Where(x => x.IndustryProfileReport.Id == industryProfileReportId).Select(a => (decimal?)a.RoadTransportValue).Sum() ?? 0;
          }
          else
          {
            var t = result.Where(x => x.IndustryProfileReport.ReportYear == year);

            if (memberId > 0 && memberBranchId > 0)
            {
              return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.RoadTransportValue) ?? 0;
            }

            if (memberId > 0 && memberBranchId == 0)
            {
              return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.RoadTransportValue) ?? 0;
            }

            if (memberId == 0 && memberBranchId == 0)
            {
              return t.Sum(sum => (decimal?)sum.RoadTransportValue) ?? 0;
            }

            return 0;
            //return entity.Where(x => x.IPCBReport.ReportYear == year).Select(a => (Decimal?)a.RoadTransportValue).Sum() ?? 0;
          }

        case CostingGroup.SalesValue:
          if (year == 0)
          {
            return result.Where(x => x.IndustryProfileReport.Id == industryProfileReportId).Select(a => (decimal?)a.SalesValue).Sum() ?? 0;
          }
          else
          {
            var t = result.Where(x => x.IndustryProfileReport.ReportYear == year);

            if (memberId > 0 && memberBranchId > 0)
            {
              return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.SalesValue) ?? 0;
            }

            if (memberId > 0 && memberBranchId == 0)
            {
              return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.SalesValue) ?? 0;
            }

            if (memberId == 0 && memberBranchId == 0)
            {
              return t.Sum(sum => (decimal?)sum.SalesValue) ?? 0;
            }

            return 0;
          }

        default:
          return 0;
      }
    }

    public async Task UpdateValueToTable<TEntity>(int id, int industryProfileReportId, int groupId, int itemId, decimal value) where TEntity : class, IIndustryCostSingle
    {
      var item = await _applicationRepository.QuerySingleAsync<TEntity>(x => x.Id == id);
      item.Value = value;
      var result = await _applicationRepository.SaveOrUpdateAsync(item).ConfigureAwait(false);
    }

    //public async Task UpdateValueToTable<TEntity>(int id, int industryProfileReportId, int groupId, int itemId, decimal value, CostingGroup costingGroup) where TEntity : class, IIndustryCostMultiple, new()
    //{
    //  var item = await _applicationRepository.QuerySingleAsync<TEntity>(x => x.Id == id && x.GroupId).ConfigureAwait(false);

    //  if (groupId > 0 && itemId > 0 && industryProfileReportId > 0)
    //  {
    //    item = repo.GetAll()
    //        .Where(x => x.ItemId == itemId && x.GroupId == groupId && x.IPCBReportId == reportId).First();
    //  }
    //  else
    //  {
    //    item = repo.GetAll().Where(x => x.Id == Id).First();
    //  }

    //  if (costingGroup == CostingGroup.AdministrationValue)
    //  {
    //    item.AdministrationValue = value;
    //  }

    //  if (costingGroup == CostingGroup.ProductionValue)
    //  {
    //    item.ProductionValue = value;
    //  }

    //  if (costingGroup == CostingGroup.RoadTransportValue)
    //  {
    //    item.RoadTransportValue = value;
    //  }

    //  if (costingGroup == CostingGroup.SalesValue)
    //  {
    //    item.SalesValue = value;
    //  }

    //  repo.Edit(item);
    //}

    public async Task<decimal> GetCostFromTable(int linkedId1, int? linkedId2, int industryProfileReportId, string tableName, int industryCostId, RunningCostType runningCostType, int? reportYear, int? industryCostGroupId, int memberId = 0, int memberBranchId = 0)
    {
      var memberRunningCostSundries = await _applicationRepository
        .QueryAsync<MemberRunningCostSundry>(x => x.IndustryProfileReport.Submitted).ConfigureAwait(false);

      var memberRunningCosts = await _applicationRepository
        .QueryAsync<MemberRunningCost>(x => x.IndustryProfileReport.Submitted).ConfigureAwait(false);

      var memberRunningCostsFleet = await _applicationRepository
        .QueryAsync<MemberRunningCostFleet>(x => x.IndustryProfileReport.Submitted).ConfigureAwait(false);

      decimal _total = 0M;

      switch (tableName.ToUpper())
      {
        case "RUNNINGCOST":
          if (industryCostId == 1)
          {
            //total for PRODUCTION
            if (reportYear == null)
            {
              _total = 0M;
              try
              {
                if (memberId > 0 && memberBranchId == 0)
                {
                  _total = memberRunningCostSundries.Where(x => x.Member.Id == memberId && x.IndustryProfileReport.Id == industryProfileReportId).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                  _total = _total + memberRunningCosts.Where(x => x.Member.Id == memberId && x.IndustryProfileReport.Id == industryProfileReportId).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                }

                if (memberId > 0 && memberBranchId > 0)
                {
                  _total = memberRunningCostSundries.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId && x.IndustryProfileReport.Id == industryProfileReportId).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                  _total = _total + memberRunningCosts.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId && x.IndustryProfileReport.Id == industryProfileReportId).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                }

                if (memberId == 0 && memberBranchId == 0)
                {
                  _total = memberRunningCostSundries.Where(x => x.IndustryProfileReport.Id == industryProfileReportId).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                  _total = _total + memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                }

                return _total;
              }
              catch
              {
                return 0M;
              }
            }
            else
            {
              _total = 0M;

              try
              {
                if (memberId > 0 && memberBranchId == 0)
                {
                  _total = memberRunningCostSundries.Where(x => x.Member.Id == memberId && x.IndustryProfileReport.ReportYear == reportYear).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                  _total = _total + memberRunningCosts.Where(x => x.Member.Id == memberId && x.IndustryProfileReport.ReportYear == reportYear).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                }

                if (memberId > 0 && memberBranchId > 0)
                {
                  _total = memberRunningCostSundries.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId && x.IndustryProfileReport.ReportYear == reportYear).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                  _total = _total + memberRunningCosts.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId && x.IndustryProfileReport.ReportYear == reportYear).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                }

                if (memberId == 0 && memberBranchId == 0)
                {
                  _total = memberRunningCostSundries.Where(x => x.IndustryProfileReport.ReportYear == reportYear).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                  _total = _total + memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                }

                return _total;
              }
              catch
              {
                return 0M;
              }
            }
          }

          if (industryCostId == 2)
          {
            //total for SALES/MARKETING
            if (reportYear == null)
            {
              _total = 0M;
              try
              {
                if (memberId > 0 && memberBranchId == 0)
                {
                  _total = memberRunningCostSundries.Where(x => x.Member.Id == memberId && x.IndustryProfileReport.Id == industryProfileReportId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                  _total = _total + memberRunningCosts.Where(x => x.Member.Id == memberId && x.IndustryProfileReport.Id == industryProfileReportId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                }

                if (memberId > 0 && memberBranchId > 0)
                {
                  _total = memberRunningCostSundries.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId && x.IndustryProfileReport.Id == industryProfileReportId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                  _total = _total + memberRunningCosts.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId && x.IndustryProfileReport.Id == industryProfileReportId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                }

                if (memberId == 0 && memberBranchId == 0)
                {
                  _total = memberRunningCostSundries.Where(x => x.IndustryProfileReport.Id == industryProfileReportId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                  _total = _total + memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                }

                return _total;
              }
              catch
              {
                return 0M;
              }
            }
            else
            {
              _total = 0M;
              try
              {
                if (memberId > 0 && memberBranchId == 0)
                {
                  _total = memberRunningCostSundries.Where(x => x.Member.Id == memberId && x.IndustryProfileReport.ReportYear == reportYear).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                  _total = _total + memberRunningCosts.Where(x => x.Member.Id == memberId && x.IndustryProfileReport.ReportYear == reportYear).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                }

                if (memberId > 0 && memberBranchId > 0)
                {
                  _total = memberRunningCostSundries.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId && x.IndustryProfileReport.ReportYear == reportYear).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                  _total = _total + memberRunningCosts.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId && x.IndustryProfileReport.ReportYear == reportYear).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                }

                if (memberId == 0 && memberBranchId == 0)
                {
                  _total = memberRunningCostSundries.Where(x => x.IndustryProfileReport.ReportYear == reportYear).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                  _total = _total + memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                }

                return _total;
              }
              catch
              {
                return 0M;
              }
            }
          }

          if (industryCostId == 3)
          {
            //total for ADMINISTRATION
            if (reportYear == null)
            {
              _total = 0M;
              try
              {
                if (memberId > 0 && memberBranchId == 0)
                {
                  _total =
                    memberRunningCostSundries.Where(x => x.Member.Id == memberId && x.IndustryProfileReport.Id == industryProfileReportId)
                      .Sum(sum => (decimal?)sum.AdministrationValue ?? 0M) + memberRunningCosts
                      .Where(x => x.Member.Id == memberId && x.IndustryProfileReport.Id == industryProfileReportId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                }

                if (memberId > 0 && memberBranchId > 0)
                {
                  _total =
                    memberRunningCostSundries.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId && x.IndustryProfileReport.Id == industryProfileReportId)
                      .Sum(sum => (decimal?)sum.AdministrationValue ?? 0M) + memberRunningCosts
                      .Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId && x.IndustryProfileReport.Id == industryProfileReportId)
                      .Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                }

                if (memberId == 0 && memberBranchId == 0)
                {
                  _total = memberRunningCostSundries.Where(x => x.IndustryProfileReport.Id == industryProfileReportId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M) +
                           memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                }

                return _total;
              }
              catch
              {
                return 0M;
              }
            }
            else
            {
              _total = 0M;
              try
              {
                if (memberId > 0 && memberBranchId == 0)
                {
                  _total =
                    memberRunningCostSundries
                      .Where(x => x.Member.Id == memberId && x.IndustryProfileReport.ReportYear == reportYear)
                      .Sum(sum => (decimal?)sum.AdministrationValue ?? 0M) + memberRunningCosts
                      .Where(x => x.Member.Id == memberId && x.IndustryProfileReport.ReportYear == reportYear)
                      .Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                }

                if (memberId > 0 && memberBranchId > 0)
                {
                  _total = memberRunningCostSundries
                    .Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId &&
                                x.IndustryProfileReport.ReportYear == reportYear)
                    .Sum(sum => (decimal?)sum.AdministrationValue ?? 0M) + memberRunningCosts
                    .Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId &&
                                x.IndustryProfileReport.ReportYear == reportYear)
                    .Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                }

                if (memberId == 0 && memberBranchId == 0)
                {
                  _total =
                    memberRunningCostSundries.Where(x => x.IndustryProfileReport.ReportYear == reportYear)
                      .Sum(sum => (decimal?)sum.AdministrationValue ?? 0M) + memberRunningCosts
                      .Where(x => x.IndustryProfileReport.ReportYear == reportYear)
                      .Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                }

                return _total;
              }
              catch
              {
                return 0M;
              }
            }
          }

          if (industryCostId == 4)
          {
            //total for ROAD TRANSPORT - OWN
            if (reportYear == null)
            {
              _total = 0M;
              try
              {
                if (memberId > 0 && memberBranchId == 0)
                {
                  _total =
                    memberRunningCostSundries.Where(x => x.Member.Id == memberId && x.IndustryProfileReport.Id == industryProfileReportId)
                      .Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M) + memberRunningCosts
                      .Where(x => x.Member.Id == memberId && x.IndustryProfileReport.Id == industryProfileReportId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                }

                if (memberId > 0 && memberBranchId > 0)
                {
                  _total =
                    memberRunningCostSundries.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId && x.IndustryProfileReport.Id == industryProfileReportId)
                      .Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M) + memberRunningCosts
                      .Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId && x.IndustryProfileReport.Id == industryProfileReportId)
                      .Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                }

                if (memberId == 0 && memberBranchId == 0)
                {
                  _total = memberRunningCostSundries.Where(x => x.IndustryProfileReport.Id == industryProfileReportId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M) +
                           memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                }

                return _total;
              }
              catch
              {
                return 0M;
              }
            }
            else
            {
              _total = 0M;
              try
              {
                if (memberId > 0 && memberBranchId == 0)
                {
                  _total =
                    memberRunningCostSundries
                      .Where(x => x.Member.Id == memberId && x.IndustryProfileReport.ReportYear == reportYear)
                      .Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M) + memberRunningCosts
                      .Where(x => x.Member.Id == memberId && x.IndustryProfileReport.ReportYear == reportYear)
                      .Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                }

                if (memberId > 0 && memberBranchId > 0)
                {
                  _total = memberRunningCostSundries
                    .Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId &&
                                x.IndustryProfileReport.ReportYear == reportYear)
                    .Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M) + memberRunningCosts
                    .Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId &&
                                x.IndustryProfileReport.ReportYear == reportYear)
                    .Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                }

                if (memberId == 0 && memberBranchId == 0)
                {
                  _total =
                    memberRunningCostSundries.Where(x => x.IndustryProfileReport.ReportYear == reportYear)
                      .Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M) + memberRunningCosts
                      .Where(x => x.IndustryProfileReport.ReportYear == reportYear)
                      .Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                }

                return _total;
              }
              catch
              {
                return 0M;
              }
            }
          }

          if (industryCostId == 5)
          {
            //total for ROAD TRANSPORT - RENTED
            if (reportYear == null)
            {
              try
              {
                if (memberId > 0 && memberBranchId == 0)
                {
                  return memberRunningCostsFleet.Where(x => x.Member.Id == memberId && x.IndustryProfileReport.Id == industryProfileReportId).Sum(sum => (decimal?)sum.Value ?? 0M);
                }

                if (memberId > 0 && memberBranchId > 0)
                {
                  return memberRunningCostsFleet.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId && x.IndustryProfileReport.Id == industryProfileReportId).Sum(sum => (decimal?)sum.Value ?? 0M);
                }

                if (memberId == 0 && memberBranchId == 0)
                {
                  return memberRunningCostsFleet.Where(x => x.IndustryProfileReport.Id == industryProfileReportId).Sum(sum => (decimal?)sum.Value ?? 0M);
                }
              }
              catch
              {
                return 0M;
              }
            }
            else
            {
              try
              {
                if (memberId > 0 && memberBranchId == 0)
                {
                  return memberRunningCostsFleet.Where(x => x.Member.Id == memberId && x.IndustryProfileReport.ReportYear == reportYear).Sum(sum => (decimal?)sum.Value ?? 0M);
                }

                if (memberId > 0 && memberBranchId > 0)
                {
                  return memberRunningCostsFleet.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId && x.IndustryProfileReport.ReportYear == reportYear).Sum(sum => (decimal?)sum.Value ?? 0M);
                }

                if (memberId == 0 && memberBranchId == 0)
                {
                  return memberRunningCostsFleet.Where(x => x.IndustryProfileReport.ReportYear == reportYear).Sum(sum => (decimal?)sum.Value ?? 0M);
                }
              }
              catch
              {
                return 0M;
              }
            }
          }

          if (industryCostId == 8)
          {
            //total for PRODUCTION
            if (reportYear == null)
            {
              _total = 0M;
              try
              {
                var t = memberRunningCosts.Where(x => x.RunningCostGroup.Id == 12 && x.IndustryProfileReport.Id == industryProfileReportId);

                if (memberId > 0 && memberBranchId == 0)
                {
                  t = t.Where(x => x.Member.Id == memberId);
                }

                if (memberId > 0 && memberBranchId > 0)
                {
                  t = t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId);
                }

                if (memberId == 0 && memberBranchId == 0)
                {
                  //t = t.Sum(sum => (Decimal?)sum.Value ?? 0M);
                }

                foreach (var item in t)
                {
                  if (item.RunningCost != null)
                  {
                    _total += item.ProductionValue;
                  }
                }

                return _total;
              }
              catch (Exception ex)
              {
                var r = ex.Message;
                return 0M;
              }
            }
            else
            {
              _total = 0M;

              try
              {
                var t = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == 12);

                if (memberId > 0 && memberBranchId == 0)
                {
                  t = t.Where(x => x.Member.Id == memberId);
                }

                if (memberId > 0 && memberBranchId > 0)
                {
                  t = t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId);
                }

                if (memberId == 0 && memberBranchId == 0)
                {
                  //t = t.Sum(sum => (Decimal?)sum.Value ?? 0M);
                }

                foreach (var item in t)
                {
                  if (item.RunningCost != null)
                  {
                    _total += item.ProductionValue;
                  }
                }

                return _total;
              }
              catch
              {
                return 0M;
              }
            }
          }

          if (industryCostId == 44)
          {
            //total for ROAD TRANSPORT - RENTED
            if (reportYear == null)
            {
              try
              {
                var t = memberRunningCostsFleet.Where(x => x.IndustryProfileReport.Id == industryProfileReportId);

                if (memberId > 0 && memberBranchId == 0)
                {
                  t = t.Where(x => x.Member.Id == memberId);
                }

                if (memberId > 0 && memberBranchId > 0)
                {
                  t = t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId);
                }

                if (memberId == 0 && memberBranchId == 0)
                {
                  //t = t.Sum(sum => (Decimal?)sum.Value ?? 0M);
                }

                return t.Sum(sum => (decimal?)sum.Value ?? 0M);
              }
              catch
              {
                return 0M;
              }
            }
            else
            {
              try
              {
                var t = memberRunningCostsFleet.Where(x => x.IndustryProfileReport.ReportYear == reportYear);

                if (memberId > 0 && memberBranchId == 0)
                {
                  t = t.Where(x => x.Member.Id == memberId);
                }

                if (memberId > 0 && memberBranchId > 0)
                {
                  t = t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId);
                }

                if (memberId == 0 && memberBranchId == 0)
                {
                  //t = t.Sum(sum => (Decimal?)sum.Value ?? 0M);
                }

                return t.Sum(sum => (decimal?)sum.Value ?? 0M);
              }
              catch
              {
                return 0M;
              }
            }
          }

          if (industryCostId > 5 && industryCostId < 44)
          {
            switch (runningCostType)
            {
              case RunningCostType.Production:
                if (linkedId2 != 0)
                {
                  if (reportYear == null)
                  {
                    try
                    {
                      decimal val1 = 0;
                      decimal val2 = 0;

                      if (memberId > 0 && memberBranchId == 0)
                      {
                        val1 = memberRunningCosts
                          .Where(x => x.IndustryProfileReport.Id == industryProfileReportId &&
                                      x.RunningCostGroup.Id == linkedId1 && x.Member.Id == memberId)
                          .Sum(sum => (decimal?)sum.ProductionValue ?? 0M);

                        val2 = memberRunningCosts
                          .Where(x => x.IndustryProfileReport.Id == industryProfileReportId &&
                                      x.RunningCostGroup.Id == linkedId2 && x.Member.Id == memberId)
                          .Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                      }

                      if (memberId > 0 && memberBranchId > 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId1 && x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId2 && x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                      }

                      if (memberId == 0 && memberBranchId == 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId1).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId2).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                      }

                      return val1 + val2;
                    }
                    catch
                    {
                      return 0M;
                    }
                  }
                  else
                  {
                    try
                    {
                      decimal val1 = 0;
                      decimal val2 = 0;

                      if (memberId > 0 && memberBranchId > 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId1 && x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId2 && x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                      }

                      if (memberId > 0 && memberBranchId == 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId1 && x.Member.Id == memberId).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId2 && x.Member.Id == memberId).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                      }

                      if (memberId == 0 && memberBranchId == 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId1).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId2).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                      }

                      return val1 + val2;
                    }
                    catch
                    {
                      return 0M;
                    }
                  }
                }
                else
                {
                  if (reportYear == null)
                  {
                    try
                    {
                      var t = memberRunningCosts.Where(x =>
                        x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId1);

                      if (memberId > 0 && memberBranchId > 0)
                      {
                        return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                      }

                      if (memberId > 0 && memberBranchId == 0)
                      {
                        return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                      }

                      if (memberId == 0 && memberBranchId == 0)
                      {
                        return t.Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                      }
                    }
                    catch
                    {
                      return 0M;
                    }
                  }
                  else
                  {
                    try
                    {
                      var t = memberRunningCosts.Where(x =>
                        x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId1);

                      if (memberId > 0 && memberBranchId > 0)
                      {
                        return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                            .Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                      }

                      if (memberId > 0 && memberBranchId == 0)
                      {
                        return t.Where(x => x.Member.Id == memberId)
                            .Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                      }

                      if (memberId == 0 && memberBranchId == 0)
                      {
                        return t.Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                      }
                    }
                    catch
                    {
                      return 0M;
                    }
                  }
                }

                break;

              case RunningCostType.Marketing:
                if (linkedId2 != 0)
                {
                  if (reportYear == null)
                  {
                    try
                    {
                      decimal val1 = 0;
                      decimal val2 = 0;

                      if (memberId > 0 && memberBranchId == 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId1 && x.Member.Id == memberId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId2 && x.Member.Id == memberId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                      }

                      if (memberId > 0 && memberBranchId > 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId1 && x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId2 && x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                      }

                      if (memberId == 0 && memberBranchId == 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId1).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId2).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                      }

                      return val1 + val2;
                    }
                    catch
                    {
                      return 0M;
                    }
                  }
                  else
                  {
                    try
                    {
                      decimal val1 = 0;
                      decimal val2 = 0;

                      if (memberId > 0 && memberBranchId > 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId1 && x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId2 && x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                      }

                      if (memberId > 0 && memberBranchId == 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId1 && x.Member.Id == memberId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId2 && x.Member.Id == memberId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                      }

                      if (memberId == 0 && memberBranchId == 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId1).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId2).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                      }

                      return val1 + val2;
                    }
                    catch
                    {
                      return 0M;
                    }
                  }
                }
                else
                {
                  if (reportYear == null)
                  {
                    try
                    {
                      var t = memberRunningCosts.Where(x =>
                        x.IndustryProfileReport.Id == industryCostGroupId && x.RunningCostGroup.Id == linkedId1);

                      if (memberId > 0 && memberBranchId > 0)
                      {
                        return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                      }

                      if (memberId > 0 && memberBranchId == 0)
                      {
                        return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                      }

                      if (memberId == 0 && memberBranchId == 0)
                      {
                        return t.Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                      }
                    }
                    catch
                    {
                      return 0M;
                    }
                  }
                  else
                  {
                    try
                    {
                      var t = memberRunningCosts.Where(x =>
                        x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId1);

                      if (memberId > 0 && memberBranchId > 0)
                      {
                        return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                      }

                      if (memberId > 0 && memberBranchId == 0)
                      {
                        return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                      }

                      if (memberId == 0 && memberBranchId == 0)
                      {
                        return t.Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                      }
                    }
                    catch
                    {
                      return 0M;
                    }
                  }
                }

                break;

              case RunningCostType.Administration:
                if (linkedId2 != 0)
                {
                  if (reportYear == null)
                  {
                    try
                    {
                      decimal val1 = 0;
                      decimal val2 = 0;

                      if (memberId > 0 && memberBranchId == 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId1 && x.Member.Id == memberId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId2 && x.Member.Id == memberId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                      }

                      if (memberId > 0 && memberBranchId > 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId1 && x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId2 && x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                      }

                      if (memberId == 0 && memberBranchId == 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId1).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId2).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                      }

                      return val1 + val2;
                    }
                    catch
                    {
                      return 0M;
                    }
                  }
                  else
                  {
                    try
                    {
                      decimal val1 = 0;
                      decimal val2 = 0;

                      if (memberId > 0 && memberBranchId > 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId1 && x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId2 && x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                      }

                      if (memberId > 0 && memberBranchId == 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId1 && x.Member.Id == memberId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId2 && x.Member.Id == memberId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                      }

                      if (memberId == 0 && memberBranchId == 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId1).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId2).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                      }

                      return val1 + val2;
                    }
                    catch
                    {
                      return 0M;
                    }
                  }
                }
                else
                {
                  if (reportYear == null)
                  {
                    try
                    {
                      var t = memberRunningCosts.Where(x =>
                        x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId1);

                      if (memberId > 0 && memberBranchId > 0)
                      {
                        return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                      }

                      if (memberId > 0 && memberBranchId == 0)
                      {
                        return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                      }

                      if (memberId == 0 && memberBranchId == 0)
                      {
                        return t.Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                      }
                    }
                    catch
                    {
                      return 0M;
                    }
                  }
                  else
                  {
                    try
                    {
                      var t = memberRunningCosts.Where(x =>
                        x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId1);

                      if (memberId > 0 && memberBranchId > 0)
                      {
                        return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                      }

                      if (memberId > 0 && memberBranchId == 0)
                      {
                        return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                      }

                      if (memberId == 0 && memberBranchId == 0)
                      {
                        return t.Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                      }
                    }
                    catch
                    {
                      return 0M;
                    }
                  }
                }

                break;

              case RunningCostType.RoadTransport:
                if (linkedId2 != 0)
                {
                  if (reportYear == null)
                  {
                    try
                    {
                      decimal val1 = 0;
                      decimal val2 = 0;

                      if (memberId > 0 && memberBranchId == 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId1 && x.Member.Id == memberId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId2 && x.Member.Id == memberId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                      }

                      if (memberId > 0 && memberBranchId > 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId1 && x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId2 && x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                      }

                      if (memberId == 0 && memberBranchId == 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId1).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId2).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                      }

                      return val1 + val2;
                    }
                    catch
                    {
                      return 0M;
                    }
                  }
                  else
                  {
                    try
                    {
                      decimal val1 = 0;
                      decimal val2 = 0;

                      if (memberId > 0 && memberBranchId > 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId1 && x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId2 && x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                      }

                      if (memberId > 0 && memberBranchId == 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId1 && x.Member.Id == memberId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId2 && x.Member.Id == memberId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                      }

                      if (memberId == 0 && memberBranchId == 0)
                      {
                        val1 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId1).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                        val2 = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId2).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                      }

                      return val1 + val2;
                    }
                    catch
                    {
                      return 0M;
                    }
                  }
                }
                else
                {
                  if (reportYear == null)
                  {
                    try
                    {
                      var t = memberRunningCosts.Where(x =>
                        x.IndustryProfileReport.Id == industryProfileReportId && x.RunningCostGroup.Id == linkedId1);

                      if (memberId > 0 && memberBranchId > 0)
                      {
                        return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                            .Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                      }

                      if (memberId > 0 && memberBranchId == 0)
                      {
                        return t.Where(x => x.Member.Id == memberId).Sum(sum =>
                            (decimal?)sum.RoadTransportValue ?? 0M);
                      }

                      if (memberId == 0 && memberBranchId == 0)
                      {
                        return t.Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                      }
                    }
                    catch
                    {
                      return 0M;
                    }
                  }
                  else
                  {
                    try
                    {
                      var t = memberRunningCosts.Where(x =>
                        x.IndustryProfileReport.ReportYear == reportYear && x.RunningCostGroup.Id == linkedId1);

                      if (memberId > 0 && memberBranchId > 0)
                      {
                        return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                      }

                      if (memberId > 0 && memberBranchId == 0)
                      {
                        return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                      }

                      if (memberId == 0 && memberBranchId == 0)
                      {
                        return t.Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                      }
                    }
                    catch
                    {
                      return 0M;
                    }
                  }
                }

                break;
            }
          }
          else
          {
            return 0;
          }

          return 0;

        case "RUNNINGCOSTSUNDRY":
          {
            if (linkedId1 == 27 && industryCostGroupId == 2)
            {
              if (reportYear == null)
              {
                try
                {
                  var t = memberRunningCostSundries.Where(x => x.IndustryProfileReport.Id == industryProfileReportId);

                  if (memberId > 0 && memberBranchId > 0)
                  {
                    return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                  }

                  if (memberId > 0 && memberBranchId == 0)
                  {
                    return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                  }

                  if (memberId == 0 && memberBranchId == 0)
                  {
                    return t.Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                  }
                }
                catch
                {
                  return 0M;
                }
              }
              else
              {
                try
                {
                  var t = memberRunningCostSundries.Where(x => x.IndustryProfileReport.ReportYear == reportYear);

                  if (memberId > 0 && memberBranchId > 0)
                  {
                    return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                  }

                  if (memberId > 0 && memberBranchId == 0)
                  {
                    return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                  }

                  if (memberId == 0 && memberBranchId == 0)
                  {
                    return t.Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                  }
                }
                catch
                {
                  return 0M;
                }
              }
            }

            if (linkedId1 == 27 && industryCostGroupId == 3)
            {
              if (reportYear == null)
              {
                try
                {
                  var t = memberRunningCostSundries.Where(x => x.IndustryProfileReport.Id == industryProfileReportId);

                  if (memberId > 0 && memberBranchId > 0)
                  {
                    return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                  }

                  if (memberId > 0 && memberBranchId == 0)
                  {
                    return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                  }

                  if (memberId == 0 && memberBranchId == 0)
                  {
                    return t.Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                  }
                }
                catch
                {
                  return 0M;
                }
              }
              else
              {
                try
                {
                  var t = memberRunningCostSundries.Where(x => x.IndustryProfileReport.ReportYear == reportYear);

                  if (memberId > 0 && memberBranchId > 0)
                  {
                    return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                  }

                  if (memberId > 0 && memberBranchId == 0)
                  {
                    return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                  }

                  if (memberId == 0 && memberBranchId == 0)
                  {
                    return t.Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                  }
                }
                catch
                {
                  return 0M;
                }
              }
            }

            if (linkedId1 == 27 && industryCostGroupId == 4)
            {
              if (reportYear == null)
              {
                try
                {
                  var t = memberRunningCostSundries.Where(x => x.IndustryProfileReport.Id == industryProfileReportId);

                  if (memberId > 0 && memberBranchId > 0)
                  {
                    return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                  }

                  if (memberId > 0 && memberBranchId == 0)
                  {
                    return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                  }

                  if (memberId == 0 && memberBranchId == 0)
                  {
                    return t.Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                  }
                }
                catch
                {
                  return 0M;
                }
              }
              else
              {
                try
                {
                  var t = memberRunningCostSundries.Where(x => x.IndustryProfileReport.ReportYear == reportYear);

                  if (memberId > 0 && memberBranchId > 0)
                  {
                    return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                  }

                  if (memberId > 0 && memberBranchId == 0)
                  {
                    return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                  }

                  if (memberId == 0 && memberBranchId == 0)
                  {
                    return t.Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                  }
                }
                catch
                {
                  return 0M;
                }
              }
            }

            if (linkedId1 == 27 && industryCostGroupId == 5)
            {
              if (reportYear == null)
              {
                try
                {
                  var t = memberRunningCostSundries.Where(x => x.IndustryProfileReport.Id == industryProfileReportId);

                  if (memberId > 0 && memberBranchId > 0)
                  {
                    return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                  }

                  if (memberId > 0 && memberBranchId == 0)
                  {
                    return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                  }

                  if (memberId == 0 && memberBranchId == 0)
                  {
                    return t.Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                  }
                }
                catch
                {
                  return 0M;
                }
              }
              else
              {
                try
                {
                  var t = memberRunningCostSundries.Where(x => x.IndustryProfileReport.ReportYear == reportYear);

                  if (memberId > 0 && memberBranchId > 0)
                  {
                    return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                  }

                  if (memberId > 0 && memberBranchId == 0)
                  {
                    return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                  }

                  if (memberId == 0 && memberBranchId == 0)
                  {
                    return t.Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                  }
                }
                catch
                {
                  return 0M;
                }
              }
            }

            return 0;
          }

        case "RUNNINGCOSTFLEET":
          {
            if (linkedId1 == 0 && industryCostGroupId == 7)
            {
              if (reportYear == null)
              {
                try
                {
                  var t = memberRunningCostsFleet.Where(x => x.IndustryProfileReport.Id == industryProfileReportId);
                  var u = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == industryProfileReportId);

                  if (memberId > 0 && memberBranchId > 0)
                  {
                    return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                      .Sum(sum => (decimal?)sum.Value ?? 0M) + u
                      .Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                      .Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                  }

                  if (memberId > 0 && memberBranchId == 0)
                  {
                    return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.Value ?? 0M) + u
                      .Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                  }

                  if (memberId == 0 && memberBranchId == 0)
                  {
                    return t.Sum(sum => (decimal?)sum.Value ?? 0M) +
                           u.Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                  }
                }
                catch
                {
                  return 0M;
                }
              }
              else
              {
                try
                {
                  var t = memberRunningCostsFleet.Where(x => x.IndustryProfileReport.ReportYear == reportYear);
                  var u = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear);

                  if (memberId > 0 && memberBranchId > 0)
                  {
                    return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                             .Sum(sum => (decimal?)sum.Value ?? 0M) +
                           u.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                             .Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                  }

                  if (memberId > 0 && memberBranchId == 0)
                  {
                    return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.Value ?? 0M) + u
                      .Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                  }

                  if (memberId == 0 && memberBranchId == 0)
                  {
                    return t.Sum(sum => (decimal?)sum.Value ?? 0M) + u.Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                  }
                }
                catch
                {
                  return 0M;
                }
              }
            }

            return 0;
          }

        default:
          return 0;
      }
    }

    public async Task<ICollection<IpBaseGroupModel>> GetIpBaseGroups(bool showAll = true)
    {
      var results = await _applicationRepository.QueryAsync<IpBaseGroup>().ConfigureAwait(false);

      if (!showAll)
      {
        results = results.Where(x => x.Active);
      }

      return _mapper.Map<IEnumerable<IpBaseGroup>, List<IpBaseGroupModel>>(results);
    }

    public async Task<ICollection<IpBaseItemModel>> GetIpBaseItems(bool showAll = true)
    {
      var results = await _applicationRepository.QueryAsync<IpBaseItem>().ConfigureAwait(false);

      if (!showAll)
      {
        results = results.Where(x => x.Active);
      }

      return _mapper.Map<IEnumerable<IpBaseItem>, List<IpBaseItemModel>>(results);
    }

    public async Task SaveOrUpdateIpBaseItem(int id, decimal value)
    {
      var item = await _applicationRepository.QuerySingleAsync<MemberIpBaseItem>(x => x.Id == id).ConfigureAwait(false);
      if (item.Value != value)
      {
        item.Value = value;
        var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(item);
      }
    }

    public async Task<ICollection<IpAdminGroupModel>> GetIpAdminGroups(bool showAll = true)
    {
      var results = await _applicationRepository.QueryAsync<IpAdminGroup>().ConfigureAwait(false);

      if (!showAll)
      {
        results = results.Where(x => x.Active);
      }

      return _mapper.Map<IEnumerable<IpAdminGroup>, List<IpAdminGroupModel>>(results);
    }

    public async Task<ICollection<IpAdminItemModel>> GetIpAdminItems(bool showAll = true)
    {
      var results = await _applicationRepository.QueryAsync<IpAdminItem>().ConfigureAwait(false);

      if (!showAll)
      {
        results = results.Where(x => x.Active);
      }

      return _mapper.Map<IEnumerable<IpAdminItem>, List<IpAdminItemModel>>(results);
    }

    public async Task SaveOrUpdateIpAdminItem(int id, decimal value)
    {
      var item = await _applicationRepository.QuerySingleAsync<MemberIpAdminItem>(x => x.Id == id).ConfigureAwait(false);
      if (item.Value != value)
      {
        item.Value = value;
        var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(item);
      }
    }

    public async Task SaveOrUpdateIpAdditionalItem(int id, decimal value)
    {
      var item = await _applicationRepository.QuerySingleAsync<MemberIpAdditionalItem>(x => x.Id == id).ConfigureAwait(false);
      if (item.Value != value)
      {
        item.Value = value;
        var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(item);
      }
    }

    public async Task<ICollection<IpAdditionalItemModel>> GetIpAdditionalItems(bool showAll = true)
    {
      var results = await _applicationRepository.QueryAsync<IpAdditionalItem>().ConfigureAwait(false);

      if (!showAll)
      {
        results = results.Where(x => x.Active);
      }

      return _mapper.Map<IEnumerable<IpAdditionalItem>, List<IpAdditionalItemModel>>(results);
    }

    public async Task<ICollection<RawMaterialTotalCostModel>> GetRawMaterialCost(bool showAll = true)
    {
      var results = await _applicationRepository.QueryAsync<RawMaterialTotalCost>().ConfigureAwait(false);

      if (!showAll)
      {
        results = results.Where(x => x.Active);
      }

      return _mapper.Map<IEnumerable<RawMaterialTotalCost>, List<RawMaterialTotalCostModel>>(results);
    }

    public async Task SaveOrUpdateRunningCostItem(int id, int groupId, int itemId, int reportId, CostingGroup costingGroup, decimal value)
    {
      var memberRunningCostItem = new MemberRunningCost();

      //var items = await _applicationRepository.QueryAsync<MemberRunningCost>().ConfigureAwait(false);

      if (groupId > 0 && itemId > 0 && reportId > 0)
      {
        memberRunningCostItem = _applicationRepository.QueryAsync<MemberRunningCost>().Result.FirstOrDefault(x => x.RunningCost!.Id == itemId && x.RunningCostGroup.Id == groupId && x.IndustryProfileReport.Id == reportId);
      }
      else
      {
        memberRunningCostItem = _applicationRepository.QueryAsync<MemberRunningCost>().Result.FirstOrDefault(x => x.Id == id);
      }

      switch (costingGroup)
      {
        case CostingGroup.AdministrationValue:
          if (memberRunningCostItem!.AdministrationValue != value)
          {
            memberRunningCostItem.AdministrationValue = value;
          }
          break;

        case CostingGroup.ProductionValue:
          if (memberRunningCostItem!.ProductionValue != value)
          {
            memberRunningCostItem.ProductionValue = value;
          }
          break;

        case CostingGroup.RoadTransportValue:
          if (memberRunningCostItem!.RoadTransportValue != value)
          {
            memberRunningCostItem.RoadTransportValue = value;
          }
          break;

        case CostingGroup.SalesValue:
          if (memberRunningCostItem!.SalesValue != value)
          {
            memberRunningCostItem.SalesValue = value;
          }
          break;
      }

      var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(memberRunningCostItem!).ConfigureAwait(false);
    }

    public async Task<ICollection<RunningCostGroupModel>> GetRunningCostGroups(bool showAll = true)
    {
      var results = await _applicationRepository.QueryAsync<RunningCostGroup>().ConfigureAwait(false);

      if (!showAll)
      {
        results = results.Where(x => x.Active);
      }

      return _mapper.Map<IEnumerable<RunningCostGroup>, List<RunningCostGroupModel>>(results);
    }

    public async Task<ICollection<RunningCostRentedFleetModel>> GetRunningCostRentedFleetItems(bool showAll = true)
    {
      var results = await _applicationRepository.QueryAsync<RunningCostRentedFleet>().ConfigureAwait(false);

      if (!showAll)
      {
        results = results.Where(x => x.Active);
      }

      return _mapper.Map<IEnumerable<RunningCostRentedFleet>, List<RunningCostRentedFleetModel>>(results);
    }

    public async Task<ICollection<RunningCostSundryExpenseModel>> GetRunningCostSundryExpenseItems(bool showAll = true)
    {
      var results = await _applicationRepository.QueryAsync<RunningCostSundryExpense>().ConfigureAwait(false);

      if (!showAll)
      {
        results = results.Where(x => x.Active);
      }

      return _mapper.Map<IEnumerable<RunningCostSundryExpense>, List<RunningCostSundryExpenseModel>>(results);
    }

    public async Task<ICollection<PcbCostItemModel>> GetPcbCostItems(bool showAll = true)
    {
      var results = await _applicationRepository.QueryAsync<PcbCostItem>().ConfigureAwait(false);

      if (!showAll)
      {
        results = results.Where(x => x.Active);
      }

      return _mapper.Map<IEnumerable<PcbCostItem>, List<PcbCostItemModel>>(results);
    }

    public async Task<ICollection<PcbCostItemGroupModel>> GetPcbCostItemGroups(bool showAll = true)
    {
      var results = await _applicationRepository.QueryAsync<PcbCostItemGroup>().ConfigureAwait(false);

      if (!showAll)
      {
        results = results.Where(x => x.Active);
      }

      return _mapper.Map<IEnumerable<PcbCostItemGroup>, List<PcbCostItemGroupModel>>(results);
    }

    public async Task<ICollection<IndustryGroupItemModel>> GetIndustryCostItems(bool showAll = true)
    {
      var results = await _applicationRepository.QueryAsync<IndustryGroupItem>().ConfigureAwait(false);

      if (!showAll)
      {
        results = results.Where(x => x.Active);
      }

      return _mapper.Map<IEnumerable<IndustryGroupItem>, List<IndustryGroupItemModel>>(results);
    }

    public async Task SaveOrUpdateRunningCostFleetItem(int id, decimal value)
    {
      var item = await _applicationRepository.QuerySingleAsync<MemberRunningCostFleet>(x => x.Id == id).ConfigureAwait(false);
      if (item.Value != value)
      {
        item.Value = value;
        var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(item);
      }
    }

    public async Task SaveOrUpdatePcbCostItem(int id, decimal value)
    {
      var item = await _applicationRepository.QuerySingleAsync<MemberPcbCostItem>(x => x.Id == id).ConfigureAwait(false);
      if (item.Value != value)
      {
        item.Value = value;
        var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(item);
      }
    }

    public async Task SaveOrUpdateRunningCostSundryItem(int id, CostingGroup costingGroup, decimal value)
    {
      var item = await _applicationRepository.QuerySingleAsync<MemberRunningCostSundry>(x => x.Id == id).ConfigureAwait(false);

      switch (costingGroup)
      {
        case CostingGroup.AdministrationValue:
          if (item.AdministrationValue != value)
          {
            item.AdministrationValue = value;
            //var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(item);
          }
          break;

        case CostingGroup.ProductionValue:
          if (item.ProductionValue != value)
          {
            item.ProductionValue = value;
            //var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(item);
          }
          break;

        case CostingGroup.SalesValue:
          if (item.SalesValue != value)
          {
            item.SalesValue = value;
            //var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(item);
          }
          break;

        case CostingGroup.RoadTransportValue:
          if (item.RoadTransportValue != value)
          {
            item.RoadTransportValue = value;
          }
          break;
      }

      var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(item);
    }


    public async Task<ICollection<IpAdditionalGroupModel>> GetIpAdditionalGroups(bool showAll = true)
    {
      var results = await _applicationRepository.QueryAsync<IpAdditionalGroup>().ConfigureAwait(false);

      if (!showAll)
      {
        results = results.Where(x => x.Active);
      }

      return _mapper.Map<IEnumerable<IpAdditionalGroup>, List<IpAdditionalGroupModel>>(results);
    }

    #endregion

    #region Dashboards

    public async Task<IEnumerable<DashboardViewModel>> GenerateFeedsalesDashboard()
    {
      var dashboards = new List<DashboardViewModel>();
      var currentMonth = DateTime.Now.Month;
      var previousMonth = DateTime.Now.AddMonths(-1).Month;
      var reportYear = 2022;

      //Dashboard for current year
      var reports =
        await _applicationRepository.QueryAsync<FeedsalesReport>(x => x.ReportYear == reportYear)
          .ConfigureAwait(false);

      //reports.SelectMany(x => x.ReportItems.Where(m => m.Material != null).Select(a => a.Id)).Count();

      dashboards.Add(new DashboardViewModel
      {
        DashboardName = $"Feedsales Submissions - Current Year {reportYear}",
        DashboardCode = "cy",
        DisplayOrder = 1,
        Module = ModuleEnum.Feedsales,
        ReportCount = reports.Count(),
        Submitted = reports.Count(x => x.IsSubmitted),
        NotSubmitted = reports.Count(x => !x.IsSubmitted),
        TotalMaterials = reports.SelectMany(x => x.ReportItems.Where(m => m.Material != null).Select(a => a.Id)).Count(),
        TotalProducts = reports.SelectMany(x => x.ReportItems.Where(m => m.Product != null).Select(a => a.Id)).Count(),
      });

      //Dashboard for current month
      dashboards.Add(new DashboardViewModel
      {
        DashboardName = $"Feedsales Submissions - Current Month {currentMonth}",
        DashboardCode = "cm",
        DisplayOrder = 2,
        Module = ModuleEnum.Feedsales,
        ReportCount = reports.Count(x => x.ReportMonth == currentMonth),
        Submitted = reports.Count(x => x.ReportMonth == currentMonth && x.IsSubmitted),
        NotSubmitted = reports.Count(x => x.ReportMonth == currentMonth && !x.IsSubmitted),
        TotalMaterials = reports.Where(x => x.ReportMonth == currentMonth).SelectMany(x => x.ReportItems.Where(m => m.Material != null).Select(a => a.Id)).Count(),
        TotalProducts = reports.Where(x => x.ReportMonth == currentMonth).SelectMany(x => x.ReportItems.Where(m => m.Product != null).Select(a => a.Id)).Count(),
      });

      //Dashboard for previous month
      dashboards.Add(new DashboardViewModel
      {
        DashboardName = $"Feedsales Submissions - Previous Month {previousMonth}",
        DashboardCode = "pm",
        DisplayOrder = 3,
        Module = ModuleEnum.Feedsales,
        ReportCount = reports.Count(x => x.ReportMonth == previousMonth),
        Submitted = reports.Count(x => x.ReportMonth == previousMonth && x.IsSubmitted),
        NotSubmitted = reports.Count(x => x.ReportMonth == previousMonth && !x.IsSubmitted),
        TotalMaterials = reports.Where(x => x.ReportMonth == previousMonth).SelectMany(x => x.ReportItems.Where(m => m.Material != null).Select(a => a.Id)).Count(),
        TotalProducts = reports.Where(x => x.ReportMonth == previousMonth).SelectMany(x => x.ReportItems.Where(m => m.Product != null).Select(a => a.Id)).Count(),
      });

      return dashboards;
    }

    public async Task<IEnumerable<DashboardViewModel>> GenerateSalmonellaDashboard()
    {
      var dashboards = new List<DashboardViewModel>();
      var currentMonth = DateTime.Now.Month;
      var previousMonth = DateTime.Now.AddMonths(-1).Month;
      var reportYear = 2022;

      //Dashboard for current year
      var reports = await _applicationRepository.QueryAsync<SalmonellaReport>(x => x.ReportYear == reportYear).ConfigureAwait(false);

      //reports.SelectMany(x => x.ReportItems.Where(m => m.Material != null).Select(a => a.Id)).Count();

      dashboards.Add(new DashboardViewModel
      {
        DashboardName = $"Salmonella Submissions - Current Year {reportYear}",
        DashboardCode = "cy",
        DisplayOrder = 1,
        Module = ModuleEnum.Salmonella,
        ReportCount = reports.Count(),
        Submitted = reports.Count(x => x.IsSubmitted),
        NotSubmitted = reports.Count(x => !x.IsSubmitted),
      });

      //Dashboard for current month
      dashboards.Add(new DashboardViewModel
      {
        DashboardName = $"Salmonella Submissions - Current Month {currentMonth}",
        DashboardCode = "cm",
        DisplayOrder = 2,
        Module = ModuleEnum.Salmonella,
        ReportCount = reports.Count(x => x.ReportMonth == currentMonth),
        Submitted = reports.Count(x => x.ReportMonth == currentMonth && x.IsSubmitted),
        NotSubmitted = reports.Count(x => x.ReportMonth == currentMonth && !x.IsSubmitted),
      });

      //Dashboard for previous month
      dashboards.Add(new DashboardViewModel
      {
        DashboardName = $"Salmonella Submissions - Previous Month {previousMonth}",
        DashboardCode = "pm",
        DisplayOrder = 3,
        Module = ModuleEnum.Salmonella,
        ReportCount = reports.Count(x => x.ReportMonth == previousMonth),
        Submitted = reports.Count(x => x.ReportMonth == previousMonth && x.IsSubmitted),
        NotSubmitted = reports.Count(x => x.ReportMonth == previousMonth && !x.IsSubmitted),
      });

      return dashboards;
    }

    public async Task<ClientDashboardItemViewModel> GenerateClientItemDashboard(int memberId, int memberBranchId, ModuleEnum module)
    {
      var dashboardItemViewModel = new ClientDashboardItemViewModel();

      switch (module)
      {
        case ModuleEnum.Feedsales:
          var feedsalesReports = await _applicationRepository
            .QueryAsync<FeedsalesReportLookup>(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
            .ConfigureAwait(false);

          dashboardItemViewModel.Submitted = feedsalesReports.Count(x => x.IsSubmitted);
          dashboardItemViewModel.NotSubmitted = feedsalesReports.Count(x => !x.IsSubmitted);
          dashboardItemViewModel.ReportCount = feedsalesReports.Count();
          dashboardItemViewModel.Overdue = feedsalesReports.Count(x => !x.IsSubmitted && x.DueDate > DateTime.Now);
          dashboardItemViewModel.CurrentDue =
            feedsalesReports.Count(x => !x.IsSubmitted && x.DueDate > DateTime.Now && x.DueDate < DateTime.Now.AddDays(30));
          dashboardItemViewModel.DashboardImage = "mdi-paw";
          dashboardItemViewModel.DashboardName = "Feedsales Report Summary";
          return dashboardItemViewModel;

        case ModuleEnum.Salmonella:
          var salmonellaReports = await _applicationRepository
            .QueryAsync<SalmonellaReport>(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
            .ConfigureAwait(false);

          dashboardItemViewModel.Submitted = salmonellaReports.Count(x => x.IsSubmitted);
          dashboardItemViewModel.NotSubmitted = salmonellaReports.Count(x => !x.IsSubmitted);
          dashboardItemViewModel.ReportCount = salmonellaReports.Count();
          dashboardItemViewModel.Overdue = salmonellaReports.Count(x => !x.IsSubmitted && x.DueDate > DateTime.Now);
          dashboardItemViewModel.CurrentDue =
            salmonellaReports.Count(x => !x.IsSubmitted && x.DueDate > DateTime.Now && x.DueDate < DateTime.Now.AddDays(30));
          dashboardItemViewModel.DashboardImage = "mdi-biohazard";
          dashboardItemViewModel.DashboardName = "Salmonella Report Summary";
          return dashboardItemViewModel;

        case ModuleEnum.IndustryProfile:
          var industryProfileReports = await _applicationRepository
            .QueryAsync<IndustryProfileReport>(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
            .ConfigureAwait(false);

          dashboardItemViewModel.Submitted = industryProfileReports.Count(x => x.Submitted);
          dashboardItemViewModel.NotSubmitted = industryProfileReports.Count(x => !x.Submitted);
          dashboardItemViewModel.ReportCount = industryProfileReports.Count();
          dashboardItemViewModel.Overdue = industryProfileReports.Count(x => !x.Submitted && x.ReportYear != DateTime.Now.Year);
          dashboardItemViewModel.CurrentDue = industryProfileReports.Count(x => !x.Submitted && x.ReportYear == DateTime.Now.Year);
          dashboardItemViewModel.DashboardImage = "mdi-factory";
          dashboardItemViewModel.DashboardName = "Industry Profile Report Summary";
          return dashboardItemViewModel;

        case ModuleEnum.Mycotoxin:
          var mycotoxinReports = await _applicationRepository
            .QueryAsync<MycotoxinReport>(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
            .ConfigureAwait(false);

          dashboardItemViewModel.Submitted = mycotoxinReports.Count(x => x.IsSubmitted);
          dashboardItemViewModel.NotSubmitted = mycotoxinReports.Count(x => !x.IsSubmitted);
          dashboardItemViewModel.ReportCount = mycotoxinReports.Count();
          dashboardItemViewModel.Overdue = mycotoxinReports.Count(x => !x.IsSubmitted && x.DueDate > DateTime.Now);
          dashboardItemViewModel.CurrentDue =
            mycotoxinReports.Count(x => !x.IsSubmitted && x.DueDate > DateTime.Now && x.DueDate < DateTime.Now.AddDays(30));
          dashboardItemViewModel.DashboardImage = "mdi-bug";
          dashboardItemViewModel.DashboardName = "Mycotoxin Report Summary";
          return dashboardItemViewModel;
      }

      throw new Exception("No module specified");
    }

    #endregion

    #region Audit

    #region Member Module Access

    public async Task<ICollection<MemberModuleAccessViewModel>> GenerateMemberModuleAccessMatrix()
    {
      var memberAccessViewModel = new List<MemberModuleAccessViewModel>();
      var members = await GetMembersSummary().ConfigureAwait(false);
      var activeMembers = members.Where(x => x.Active).OrderBy(o => o.Name);
      var modules = await GetModules().ConfigureAwait(false);
      var activeModules = modules.OrderBy(x => x.Description).ToList();

      foreach (var member in activeMembers)
      {
        var vm = new MemberModuleAccessViewModel
        {
          Member = member.Name,
          MemberId = member.Id,
          MemberType = member.MemberType,
        };

        foreach (var module in activeModules)
        {
          vm.MemberModules.Add(new MemberModuleViewModel
          {
            Module = module.Description!,
            HasAccess = await CheckMemberModule(member.Id, (ModuleEnum)module.Id).ConfigureAwait(false),
            ModuleId = (ModuleEnum)module.Id,
          });
        }

        memberAccessViewModel.Add(vm);
      }

      return memberAccessViewModel;
    }

    public async Task<ICollection<AccountModuleAccessViewModel>> GenerateAccountModuleAccessMatrix()
    {
      var accountAccessViewModel = new List<AccountModuleAccessViewModel>();
      var accounts = await GetMemberAccounts().ConfigureAwait(false);
      var activeAccounts = accounts.Where(x => x.Active)
        .OrderBy(o => o.Member)
        .ThenBy(o => o.MemberBranch)
        .ThenBy(o => o.FullName);
      var modules = await GetModules().ConfigureAwait(false);
      var activeModules = modules.OrderBy(x => x.Description).ToList();

      foreach (var account in activeAccounts)
      {
        var vm = new AccountModuleAccessViewModel
        {
          Member = account.Member,
          MemberId = account.MemberId,
          MemberType = GetMemberMemberType(account.MemberId).Result.Description,
          MemberBranchId = account.MemberBranchId,
          MemberBranch = account.MemberBranch,
          MemberAccountEmail = account.Email,
          MemberAccountFullName = account.FullName,
          MemberAccountId = account.Id,
          UserName = account.UserName,
        };

        foreach (var module in activeModules)
        {
          vm.MemberModules.Add(new MemberModuleViewModel
          {
            Module = module.Description!,
            HasAccess = await CheckMemberAccountModule(account.Id, (ModuleEnum)module.Id).ConfigureAwait(false),
            ModuleId = (ModuleEnum)module.Id,
          });
        }

        accountAccessViewModel.Add(vm);
      }

      return accountAccessViewModel;
    }

    #endregion

    #region Login History

    public async Task AddToLoginHistory(string userId, string userName, string ipAddress, string hostName,
      string member, string memberBranch, string userType, string browser)
    {
      var loginHistoryModel = new LoginHistoryModel
      {
        UserId = userId,
        UserName = userName,
        LoginDate = DateTime.Now,
        IpAddress = ipAddress,
        HostName = hostName,
        Member = member,
        Memberbranch = memberBranch,
        UserType = userType,
        Browser = browser,
      };

      var mapped = _mapper.Map<LoginHistoryModel, LoginHistory>(loginHistoryModel);
      await _applicationRepository.SaveOrUpdateAndCommitAsync(mapped).ConfigureAwait(false);
    }

    public async Task<IEnumerable<LoginHistoryModel>> GetLoginHistoryPaged(AlphaPageRequest pageRequest)
    {
      var result = await _applicationRepository.QueryPagedAsync<LoginHistory>(pageRequest);
      var resultMapped = _mapper.Map<IEnumerable<LoginHistory>, List<LoginHistoryModel>>(result.PageData);
      return new StaticPagedList<LoginHistoryModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }


    public async Task<IEnumerable<LoginHistoryModel>> GetLoginHistory()
    {
      var result = await _applicationRepository.QueryAsync<LoginHistory>().ConfigureAwait(false);
      var mapped =
        _mapper.Map<IEnumerable<LoginHistory>, List<LoginHistoryModel>>(result.OrderByDescending(x => x.LoginDate)
          .Take(5000));

      return mapped;
    }

    #endregion

    #region Versioning

    public string ApplicationVersion()
    {
      var asm = Assembly.GetExecutingAssembly();
      var version = _applicationSettings.Version;
      var compileDate = File.GetLastWriteTime(asm.Location);

      return
        $"<span>{version} | Build: {compileDate:dd/MM/yyyy HH:mm:ss}</span>";
    }

    #endregion

    #region Test Site Indicator

    public bool IsTestSite()
    {
      return _applicationSettings.TestSite;
    }

    #endregion

    #region Member Account Audit

    public async Task<IEnumerable<MemberAccountAuditViewModel>> GetMemberAccountAudit()
    {
      var accounts = await _applicationRepository.QueryAsync<MemberAccount>();
      var memberAccountAuditViewModel = new List<MemberAccountAuditViewModel>();
      foreach (var account in accounts.OrderBy(o => o.Member.Name))
      {
        memberAccountAuditViewModel.Add(new MemberAccountAuditViewModel
        {
          MemberAccountId = account.Id,
          MemberId = account.Member.Id,
          MemberName = account.Member.Name,
          MemberBranchId = 0,
          MemberBranchName = string.Join("<br />", account.Member.Branches.Select(p => $"{p.Id}|{p.Name}")),
          MemberAccountBranchId = account.MemberBranch.Id,
          MemberAccountBranchName = $"{account.MemberBranch.Id}|{account.MemberBranch.Name}",
          AccountEmailAddress = account.ApplicationUser.Email,
          MemberAccountActive = account.Active,
          UserId = account.ApplicationUser.Id,
          MemberAccountValid = account.Member.Branches!.Select(x => x.Id).Contains(account.MemberBranch.Id),
          ValidationMessage = "",
        });
      }

      return memberAccountAuditViewModel;
    }

    #endregion

    #endregion
  }
}

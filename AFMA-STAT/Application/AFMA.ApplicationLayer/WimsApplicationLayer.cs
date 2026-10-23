namespace AFMA.ApplicationLayer
{
  using AFMA.ApplicationLayer.Expressions;
  using AFMA.ApplicationLayer.Helpers;
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Model.Wims;
  using AFMA.ApplicationLayer.ViewModels.Wims;
  using AFMA.Data.Entities.Wims;
  using AFMA.Infrastructure.Options;
  using AFMA.Infrastructure.Settings;
  using AFMA.Shared.Infrastructure.Settings;
  using Alpha.Services.Extensions.Attributes;
  using Alpha.Services.Models.Data.PageData;
  using Alpha.Services.NHibernate.Repository.Services;
  using AutoMapper;
  using EmailServiceReference;
  using Microsoft.AspNetCore.Mvc.Rendering;
  using Microsoft.Extensions.Caching.Memory;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.Options;
  using X.PagedList;

  public class WimsApplicationLayer(
      IApplicationRepository<WimsSessionFactory> applicationRepository,
      IOptions<ApplicationSettings> applicationSettings,
      IOptions<SmtpSettings> smtpSettings,
      IMapper mapper,
      IMemoryCache memoryCache,
      ILogger<WimsApplicationLayer> logger)
    : IWimsApplicationLayer
  {
    private readonly ApplicationSettings _applicationSettings = applicationSettings.Value;
    private readonly IApplicationRepository<WimsSessionFactory> _applicationRepository = applicationRepository;
    private readonly SmtpSettings _smtpSettings = smtpSettings.Value;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<WimsApplicationLayer> _logger = logger;
    private IMemoryCache _memoryCache { get; } = memoryCache;

    private const string MenuItemCacheKey = "MenuItems";

    #region Select Lists

    public async Task<IEnumerable<SelectListItem>> GenerateMemberContactTypeSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<MemberContactType>();
      return results.OrderBy(o => o.Description).ToList().ToSelectList(x => x.Id, x => x.Description!, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateMemberTypeSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<MemberType>();
      return results.OrderBy(o => o.Description).ToList().ToSelectList(x => x.Id, x => x.Description!, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateProvinceSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<Province>();
      return results.OrderBy(o => o.Description).ToList().ToSelectList(x => x.Id, x => x.Description!, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateDistributionGroupSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<DistributionGroup>();
      return results.OrderBy(o => o.Name).ToList().ToSelectList(x => x.Id, x => x.Description!, selectedValue);
    }

    #endregion

    #region Animal Feed Products

    public async Task<IEnumerable<AnimalFeedProductModel>> GetAnimalFeedProducts()
    {
      var result = await _applicationRepository.QueryAsync<AnimalFeedProduct>().ConfigureAwait(false);
      return _mapper.Map<IEnumerable<AnimalFeedProduct>, List<AnimalFeedProductModel>>(result.OrderBy(name => name.Description));
    }

    public async Task<IEnumerable<ServiceProviderModel>> GetServiceProviders()
    {
      var result = await _applicationRepository.QueryAsync<ServiceProvider>().ConfigureAwait(false);
      return _mapper.Map<IEnumerable<ServiceProvider>, List<ServiceProviderModel>>(result.OrderBy(name => name.Description));
    }

    #endregion

    #region Members

    public async Task<IEnumerable<MemberModel>> GetMembers(AlphaPageRequest pageRequest, int? memberTypeId = null, string? searchText = null)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterWimsMembers(memberTypeId, searchText ?? "")).ConfigureAwait(false);
      var resultMapped = _mapper.Map<IEnumerable<Member>, List<MemberModel>>(result.PageData);
      return new StaticPagedList<MemberModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    public async Task<MemberModel> GetMember(int memberId)
    {
      var result = await _applicationRepository.QuerySingleAsync<Member>(x => x.Id == memberId);
      return _mapper.Map<Member, MemberModel>(result);
    }

    public async Task<MemberBranchModel> GetMemberBranch(int memberBranchId)
    {
      var result = await _applicationRepository.QuerySingleAsync<MemberBranch>(x => x.Id == memberBranchId);
      var mapped = _mapper.Map<MemberBranch, MemberBranchModel>(result);
      mapped.CategorizedContacts = await GetCategorizedContacts(memberBranchId);
      return mapped;
    }

    public async Task<ICollection<MemberBranchModel>> GetMemberBranches(int memberId)
    {
      var result = await _applicationRepository.QueryAsync<MemberBranch>(x => x.Member.Id == memberId);
      return _mapper.Map<IEnumerable<MemberBranch>, List<MemberBranchModel>>(result);
    }

    public async Task<ICollection<MemberBranchContactModel>> GetMemberBranchContacts(int memberBranchId)
    {
      var result = await _applicationRepository.QueryAsync<MemberBranchContact>(x => x.MemberBranch.Id == memberBranchId);
      return _mapper.Map<IEnumerable<MemberBranchContact>, List<MemberBranchContactModel>>(result);
    }

    public async Task<MemberBranchContactModel> GetMemberBranchContact(int memberBranchContactId)
    {
      var result = await _applicationRepository.QuerySingleAsync<MemberBranchContact>(x => x.Id == memberBranchContactId);
      return _mapper.Map<MemberBranchContact, MemberBranchContactModel>(result);
    }

    public async Task DeleteAllMemberAnimalFeedProducts(int memberBranchId)
    {
      var result = await _applicationRepository
        .QueryAsync<MemberBranchAnimalFeedProduct>(x => x.MemberBranch.Id == memberBranchId)
        .ConfigureAwait(false);

      foreach (var product in result)
      {
        await _applicationRepository.DeleteAndCommitAsync(product).ConfigureAwait(false);
      }
    }

    public async Task DeleteAllServiceProviders(int memberBranchId)
    {
      var result = await _applicationRepository
        .QueryAsync<MemberBranchServiceProvider>(x => x.MemberBranch.Id == memberBranchId)
        .ConfigureAwait(false);

      foreach (var product in result)
      {
        await _applicationRepository.DeleteAndCommitAsync(product).ConfigureAwait(false);
      }
    }

    private async Task<ICollection<MemberBranchContactTypeViewModel>> GetCategorizedContacts(int memberBranchId)
    {
      var categorizedContacts = new List<MemberBranchContactTypeViewModel>();

      var contacts = _mapper.Map<IEnumerable<MemberBranchContact>, List<MemberBranchContactModel>>(await _applicationRepository.QueryAsync<MemberBranchContact>(x => x.MemberBranch!.Id == memberBranchId || x.CompanyContact));

      var contactTypes = await _applicationRepository.QueryAsync<MemberContactType>();

      foreach (var contactType in contactTypes)
      {
        var contactTypeViewModel = new MemberBranchContactTypeViewModel
        {
          Id = contactType.Id,
          Description = contactType.Description!,
          MemberBranchContacts = contacts.Where(x => x.MemberContactTypeId == contactType.Id).ToList()
        };

        categorizedContacts.Add(contactTypeViewModel);
      }

      return categorizedContacts;
    }

    #endregion

    #region Branch Contacts

    public async Task<IEnumerable<SelectListItem>> GenerateMemberSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<Member>();

      return results.OrderBy(o => o.MemberTradeName).ToList().ToSelectList(x => x.Id, x => x.MemberTradeName, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateMemberBranchSelectList(int? memberId, int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<MemberBranch>(x => x.Member.Id == memberId);

      results = memberId == null ? results.Where(x => x.Member!.Id == 0) : results.Where(x => x.Member!.Id == memberId);

      return results.OrderBy(o => o.BranchName).ToList().ToSelectList(x => x.Id, x => x.BranchName, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateMemberAccountSelectList(int? memberBranchId, int? selectedValue)
    {
      //TODO: Add summary model
      var results = await _applicationRepository.QueryAsync<MemberBranchContact>();

      if (memberBranchId.HasValue)
      {
        results = results.Where(x => x.MemberBranch.Id == memberBranchId);
      }

      return results.OrderBy(o => o.FirstName).ToList().ToSelectList(x => x.Id, x => x.FirstName, selectedValue);
    }

    #endregion

    #region Distribution Groups

    public async Task<IEnumerable<DistributionGroupModel>> GetDistributionGroups(AlphaPageRequest pageRequest, string? searchText = null)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterDistributionGroups(searchText ?? "")).ConfigureAwait(false);
      var resultMapped = _mapper.Map<IEnumerable<DistributionGroup>, List<DistributionGroupModel>>(result.PageData);
      return new StaticPagedList<DistributionGroupModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    public async Task<DistributionGroupModel> GetDistributionGroup(int distributionGroupId)
    {
      var result = await _applicationRepository.QuerySingleAsync<DistributionGroup>(x => x.Id == distributionGroupId);
      return _mapper.Map<DistributionGroup, DistributionGroupModel>(result);
    }

    public async Task<DistributionGroupMemberModel> GetDistributionGroupMember(int distributionGroupMemberId)
    {
      var result = await _applicationRepository.QuerySingleAsync<DistributionGroupMember>(x => x.Id == distributionGroupMemberId);
      return _mapper.Map<DistributionGroupMember, DistributionGroupMemberModel>(result);
    }

    #endregion

    #region Bulk Sending

    public async Task<IEnumerable<BatchModel>> GetBatches(AlphaPageRequest pageRequest, string? searchText = null)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterBatches(searchText ?? "")).ConfigureAwait(false);
      var resultMapped = _mapper.Map<IEnumerable<Batch>, List<BatchModel>>(result.PageData);
      return new StaticPagedList<BatchModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    public async Task<BatchModel> GetBatch(int batchId)
    {
      var result = await _applicationRepository.QuerySingleAsync<Batch>(x => x.Id == batchId);
      return _mapper.Map<Batch, BatchModel>(result);
    }

    public async Task SubmitBatch(int batchId)
    {
      var batch = await _applicationRepository.QuerySingleAsync<Batch>(x => x.Id == batchId);
      var emailService = new AFMA.Service.Communication.EmailService("EAAAAN1pSIE7NPd2YEZYlua4fRDLOHPdEtOAmzsdI5GHBsfNF2cJsVkT2eBNgzrbMR6xbQ==");
      emailService.ProcessAndSendBatch(new MessageSchema());
      batch.SubmissionDate = DateTime.Now;
      batch.Status = (int)BatchStatus.Submitted;
      var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(batch).ConfigureAwait(false);
    }

    public async Task<IEnumerable<BatchRecipientModel>> GetBatchRecipients(int batchId, int distributionGroupId)
    {
      var batchRecipients = new List<BatchRecipientModel>();

      var distributionList =
        await _applicationRepository.QueryAsync<DistributionGroupMember>(x =>
          x.DistributionGroup.Id == distributionGroupId).ConfigureAwait(false);

      foreach (var item in distributionList)
      {
        batchRecipients.Add(new BatchRecipientModel
        {
          Id = item.Id,
          BatchId = batchId,
          ToEmail = item.EmailAddress,
          ToName = item.ContactName
        });
      }

      return batchRecipients;
    }

    public async Task AddBatchRecipient(int batchId, string emailAddress, string contactName)
    {
      var batchRecipient = new BatchRecipient
      {
        Batch = new Batch { Id = batchId },
        ToEmail = emailAddress,
        ToName = contactName
      };

      var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(batchRecipient).ConfigureAwait(false);
    }

    public async Task<BatchAttachmentModel> GetBatchAttachment(int batchAttachmentId)
    {
      var result = await _applicationRepository.QuerySingleAsync<BatchAttachment>(x => x.Id == batchAttachmentId);
      return _mapper.Map<BatchAttachment, BatchAttachmentModel>(result);
    }

    public async Task DeleteBatchAttachment(int batchAttachmentId)
    {
      var batchAttachment = await _applicationRepository.QuerySingleAsync<BatchAttachment>(x => x.Id == batchAttachmentId);
      await _applicationRepository.DeleteAndCommitAsync(batchAttachment).ConfigureAwait(false);
    }

    #endregion

    #region Generic Methods

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

    public async Task<SaveOrUpdateResult> Delete<TSource>(TSource source)
    {
      var result = await _applicationRepository.DeleteAndCommitAsync(_mapper.Map(source, source!.GetType(), source!.GetAttribute<MapToAttribute>()!.EntityType));

      return await Task.FromResult(new SaveOrUpdateResult
      {
        Success = result.Succeeded,
        ErrorMessage = result.ExceptionMessage,
      });
    }

    #endregion
  }
}

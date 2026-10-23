namespace AFMA.ApplicationLayer
{
  using System.Security.Claims;
  using AFMA.ApplicationLayer.Expressions;
  using AFMA.ApplicationLayer.Helpers;
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Model.Wims;
  using AFMA.ApplicationLayer.Templating.Models;
  using AFMA.ApplicationLayer.ViewModels.Wims;
  using AFMA.Data.Entities.Wims;
  using AFMA.Infrastructure.Options;
  using AFMA.Infrastructure.Settings;
  using AFMA.Shared.Infrastructure.Settings;
  using Alpha.Services.Extensions.Attributes;
  using Alpha.Services.Models.Data.PageData;
  using Alpha.Services.NHibernate.Repository.Services;
  using Alpha.Services.Notifications.Smtp;
  using AutoMapper;
  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Mvc.Rendering;
  using Microsoft.Extensions.Caching.Memory;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.Options;
  using Razor.Templating.Core;
  using X.PagedList;

  public class WimsApplicationLayer(
      IApplicationRepository<WimsSessionFactory> applicationRepository,
      IOptions<ApplicationSettings> applicationSettings,
      IOptions<SmtpSettings> smtpSettings,
      IMapper mapper,
      IMemoryCache memoryCache,
      ILogger<WimsApplicationLayer> logger,
      IWimsIdentityLayer wimsIdentityLayer,
      IRazorTemplateEngine razorTemplateEngine,
      IEmailClient emalClient,
      IHttpContextAccessor httpContextAccessor)
      : IWimsApplicationLayer
  {
    private readonly ApplicationSettings _applicationSettings = applicationSettings.Value;
    private readonly IApplicationRepository<WimsSessionFactory> _applicationRepository = applicationRepository;
    private readonly SmtpSettings _smtpSettings = smtpSettings.Value;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<WimsApplicationLayer> _logger = logger;
    private readonly IWimsIdentityLayer _wimsIdentityLayer = wimsIdentityLayer;
    private readonly IRazorTemplateEngine _razorTemplateEngine = razorTemplateEngine;
    private readonly IEmailClient _emailClient = emalClient;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

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

    public async Task<IEnumerable<SelectListItem>> GenerateNotificationTypeSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<NotificationType>();
      return results.OrderBy(o => o.Description).ToList().ToSelectList(x => x.Id, x => x.Description!, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateNotificationTemplateSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<NotificationTemplate>();
      return results.OrderBy(o => o.Description).ToList().ToSelectList(x => x.Id, x => x.Description!, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateDocumentTemplateSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<DocumentTemplate>();
      return results.OrderBy(o => o.Name).ToList().ToSelectList(x => x.Id, x => x.Name!, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateDocumentTypeSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<DocumentType>();
      return results.OrderBy(o => o.Description).ToList().ToSelectList(x => x.Id, x => x.Description!, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateCommitteeSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<Committee>();
      return results.OrderBy(o => o.Name).ToList().ToSelectList(x => x.Id, x => x.Name!, selectedValue);
    }

    public IEnumerable<SelectListItem> GenerateActiveInactiveList(bool? selectedValue = null)
    {
      return new List<SelectListItem>
      {
          new SelectListItem { Value = "true",  Text = "Active",   Selected = selectedValue == true },
          new SelectListItem { Value = "false", Text = "Inactive", Selected = selectedValue == false }
      };
    }

    public IEnumerable<SelectListItem> GenerateYesNoList(bool? selectedValue = null)
    {
      return new List<SelectListItem>
      {
          new SelectListItem { Value = "true",  Text = "Yes",   Selected = selectedValue == true },
          new SelectListItem { Value = "false", Text = "No", Selected = selectedValue == false }
      };
    }

    #endregion

    # region Email Sending

    public async Task<bool> SendGenericEmail(string recipientEmail, string subject, string body)
    {
      try
      {
        var emailMessage = await this._razorTemplateEngine.RenderAsync("/Templates/GenericEmail.cshtml",
          new GenericEmailModel
          {
            BaseUrl = this._applicationSettings.BaseUrl,
            Body = body
          }).ConfigureAwait(false);

        var recipient = recipientEmail;

        if (this._smtpSettings.Debug && !string.IsNullOrEmpty(this._smtpSettings.DebugEmail))
        {
          recipient = this._smtpSettings.DebugEmail;
        }

        await _emailClient.SendAsync(recipient,
            subject,
            emailMessage, null, new List<string> { "" })
            .ConfigureAwait(false);

        return true;
      }
      catch (Exception ex)
      {
        this._logger.LogError(ex, "Could not send email");
        throw new Exception(ex.Message, ex);
      }
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

    public async Task<IEnumerable<ServiceModel>> GetServices()
    {
      var result = await _applicationRepository.QueryAsync<Service>().ConfigureAwait(false);
      return _mapper.Map<IEnumerable<Service>, List<ServiceModel>>(result.OrderBy(name => name.Description));
    }

    public async Task<IEnumerable<ManufacturerModel>> GetManufacturers()
    {
      var result = await _applicationRepository.QueryAsync<Manufacturer>().ConfigureAwait(false);
      return _mapper.Map<IEnumerable<Manufacturer>, List<ManufacturerModel>>(result.OrderBy(name => name.Description));
    }

    public async Task<IEnumerable<SupplierModel>> GetSuppliers()
    {
      var result = await _applicationRepository.QueryAsync<Supplier>().ConfigureAwait(false);
      return _mapper.Map<IEnumerable<Supplier>, List<SupplierModel>>(result.OrderBy(name => name.Description));
    }

    #endregion

    #region Members

    public async Task<IEnumerable<MemberModel>> GetMembers(int? memberTypeId = null, int? provinceId = null, string? memberStatus = null, string? searchText = null)
    {
      var result = await _applicationRepository.QueryAsync<Member>(SearchExpression.FilterWimsMembers(memberTypeId, provinceId, memberStatus.ToNullableStatusBool(), searchText ?? "")).ConfigureAwait(false);
      return _mapper.Map<IEnumerable<Member>, List<MemberModel>>(result)
        .OrderBy(x => x.MemberTradeName)
        .ThenBy(x => x.MemberLegalName);
    }

    public async Task<IEnumerable<MemberModel>> GetMembers(AlphaPageRequest pageRequest, int? memberTypeId = null, int? provinceId = null, string? memberStatus = null, string? searchText = null)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterWimsMembers(memberTypeId, provinceId, memberStatus.ToNullableStatusBool(), searchText ?? "")).ConfigureAwait(false);
      var resultMapped = _mapper.Map<IEnumerable<Member>, List<MemberModel>>(result.PageData);
      return new StaticPagedList<MemberModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    public async Task<MemberModel> GetMember(int memberId)
    {
      var result = await _applicationRepository.QuerySingleAsync<Member>(x => x.Id == memberId);
      return _mapper.Map<Member, MemberModel>(result);
    }

    public async Task<bool> DeleteMember(int memberId)
    {
      var memberToDelete = await _applicationRepository.QuerySingleAsync<Member>(x => x.Id == memberId).ConfigureAwait(false);
      var result = await _applicationRepository.DeleteAndCommitAsync(memberToDelete).ConfigureAwait(false);
      return result.Succeeded;
    }

    public async Task<MemberBranchModel> GetMemberBranch(int memberBranchId)
    {
      var result = await _applicationRepository.QuerySingleAsync<MemberBranch>(x => x.Id == memberBranchId);
      var mapped = _mapper.Map<MemberBranch, MemberBranchModel>(result);
      mapped.CategorizedContacts = await GetCategorizedContacts(memberBranchId, result.Member.Id);
      return mapped;
    }

    public async Task<bool> DeleteBranch(int memberBranchId)
    {
      var branchToDelete = await _applicationRepository.QuerySingleAsync<MemberBranch>(x => x.Id == memberBranchId).ConfigureAwait(false);
      var result = await _applicationRepository.DeleteAndCommitAsync(branchToDelete).ConfigureAwait(false);
      return result.Succeeded;
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

    public async Task<ICollection<MemberBranchContactModel>> GetMemberContacts(int memberId)
    {
      var result = await _applicationRepository.QueryAsync<MemberBranchContact>(x => x.Member.Id == memberId);
      return _mapper.Map<IEnumerable<MemberBranchContact>, List<MemberBranchContactModel>>(result);
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

    public async Task DeleteAllServices(int memberBranchId)
    {
      var result = await _applicationRepository
        .QueryAsync<MemberBranchService>(x => x.MemberBranch.Id == memberBranchId)
        .ConfigureAwait(false);

      foreach (var service in result)
      {
        await _applicationRepository.DeleteAndCommitAsync(service).ConfigureAwait(false);
      }
    }

    public async Task DeleteAllSuppliers(int memberBranchId)
    {
      var result = await _applicationRepository
        .QueryAsync<MemberBranchSupplier>(x => x.MemberBranch.Id == memberBranchId)
        .ConfigureAwait(false);

      foreach (var supplier in result)
      {
        await _applicationRepository.DeleteAndCommitAsync(supplier).ConfigureAwait(false);
      }
    }

    public async Task DeleteAllManufacturers(int memberBranchId)
    {
      var result = await _applicationRepository
        .QueryAsync<MemberBranchManufacturer>(x => x.MemberBranch.Id == memberBranchId)
        .ConfigureAwait(false);

      foreach (var manufacturer in result)
      {
        await _applicationRepository.DeleteAndCommitAsync(manufacturer).ConfigureAwait(false);
      }
    }

    public async Task<ICollection<MemberBranchContactTypeViewModel>> GetCategorizedContacts(int memberBranchId, int memberId)
    {
      var categorizedContacts = new List<MemberBranchContactTypeViewModel>();

      var branchContacts = _mapper.Map<List<MemberBranchContactModel>>(
          await _applicationRepository.QueryAsync<MemberBranchContact>(
              x => x.MemberBranch!.Id == memberBranchId && !x.CompanyContact));

      var companyContacts = _mapper.Map<List<MemberBranchContactModel>>(
          await _applicationRepository.QueryAsync<MemberBranchContact>(
              x => x.Member!.Id == memberId && x.CompanyContact));

      var allContacts = branchContacts
          .Concat(companyContacts)
          .ToList();

      var contactTypes = await _applicationRepository.QueryAsync<MemberContactType>();

      foreach (var contactType in contactTypes)
      {
        categorizedContacts.Add(new MemberBranchContactTypeViewModel
        {
          Id = contactType.Id,
          Description = contactType.Description!,
          MemberBranchContacts = allContacts
                .Where(x => x.MemberContactTypeId == contactType.Id)
                .ToList()
        });
      }

      return categorizedContacts;
    }

    public async Task<int> CreateBranchFromMember(int memberId)
    {
      var member = await _applicationRepository.QuerySingleAsync<Member>(x => x.Id == memberId);

      var memberBranch = new MemberBranch
      {
        BranchName = member.Branch ?? "Not specified",
        DateOfIssue = null,
        Member = new Member { Id = member.Id },
        MemberIdentificationId = member.MemberIdentificationId,
        PhysicalAddress = member.PhysicalAddress ?? string.Empty,
        PostalAddress = member.PostalAddress ?? string.Empty,
        Province = new Province { Id = member.Province.Id },
        TelephoneNumber = member.TelephoneNo ?? string.Empty,
      };

      var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(memberBranch).ConfigureAwait(false);

      return result.SavedObjectId;
    }

    public async Task<int> CreateBranchFromMember()
    {
      var members = await _applicationRepository.QueryAsync<Member>().ConfigureAwait(false);

      foreach (var member in members)
      {
        if (!await _applicationRepository.QueryAnyAsync<MemberBranch>(x => x.Member.Id == member.Id))
        {
          try
          {
            var memberBranch = new MemberBranch
            {
              BranchName = member.Branch ?? "Not specified",
              DateOfIssue = null,
              Member = new Member { Id = member.Id },
              MemberIdentificationId = member.MemberIdentificationId,
              PhysicalAddress = member.PhysicalAddress ?? string.Empty,
              PostalAddress = member.PostalAddress ?? string.Empty,
              Province = new Province { Id = member.Province.Id },
              TelephoneNumber = member.TelephoneNo ?? string.Empty,
            };

            await _applicationRepository.SaveOrUpdateAndCommitAsync(memberBranch).ConfigureAwait(false);
          }
          catch (Exception ex)
          {
            return member.Id;
          }
        }
      }

      return 1234567;
    }

    public async Task<MemberTypeEnum> GetMemberType(int memberId)
    {
      var member = await _applicationRepository.QuerySingleAsync<Member>(x => x.Id == memberId);
      return (MemberTypeEnum)member.MemberType.Id;
    }

    #endregion

    #region Member Documents

    public async Task<MemberDocumentModel> GetMemberDocument(int memberDocumentId)
    {
      var result = await _applicationRepository.QuerySingleAsync<MemberDocument>(x => x.Id == memberDocumentId);
      var mapped = _mapper.Map<MemberDocument, MemberDocumentModel>(result);
      return mapped;
    }

    public async Task<bool> DeleteMemberDocument(int memberDocumentId)
    {
      var documentToDelete = await _applicationRepository.QuerySingleAsync<MemberDocument>(x => x.Id == memberDocumentId).ConfigureAwait(false);
      var result = await _applicationRepository.DeleteAndCommitAsync(documentToDelete).ConfigureAwait(false);
      return result.Succeeded;
    }

    #endregion

    #region Branch Contacts

    public async Task<IEnumerable<SelectListItem>> GenerateMemberSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<Member>();

      return results.OrderBy(o => o.MemberLegalName).ToList().ToSelectList(x => x.Id, x => x.MemberLegalName, selectedValue);
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
      //var results = await _applicationRepository.QueryAsync<MemberBranchContact>();

      //if (memberBranchId.HasValue)
      //{
      //  results = results.Where(x => x.MemberBranch.Id == memberBranchId);
      //}

      //var list = results.OrderBy(o => o.FirstName ?? "").ToList();
      //var selectList = list.ToSelectList(x => x.Id, x => x.FirstName, selectedValue);

      //return selectList;
      var results = await _applicationRepository.QueryAsync<MemberBranchContact>();

      if (memberBranchId.HasValue)
      {
        results = results.Where(x =>
            x.MemberBranch != null &&
            x.MemberBranch.Id == memberBranchId.Value
        );
      }

      var list = results.OrderBy(o => o.FirstName ?? "").ToList();

      //var selectList = list.ToSelectList(
      //    x => x.Id,
      //    x => $"{x.FirstName ?? ""} {x.Surname ?? ""} ({x.Member?.MemberTradeName ?? ""})",
      //    selectedValue
      //);

      var selectList = list.ToSelectList(
      x => x.Id,
      x => $"{x.FirstName} {x.Surname}",
      selectedValue);


      return selectList;
    }

    public async Task<IEnumerable<SelectListItem>> GenerateMemberAccountSelectListByMember(int? memberId, int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<MemberBranchContact>();

      var list = results.Where(x =>
            x.MemberBranch != null &&
            x.MemberBranch.Member.Id == memberId).OrderBy(o => o.FirstName ?? "").ToList();

      var selectList = list.ToSelectList(
      x => x.Id,
      x => $"{x.FirstName} {x.Surname}",
      selectedValue);

      return selectList;
    }

    public async Task<bool> DeleteBranchContact(int memberBranchContactId)
    {
      var contactToDelete = await _applicationRepository.QuerySingleAsync<MemberBranchContact>(x => x.Id == memberBranchContactId).ConfigureAwait(false);
      var result = await _applicationRepository.DeleteAndCommitAsync(contactToDelete).ConfigureAwait(false);
      return result.Succeeded;
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

      var memberUsers = await this.GetMemberUserDistributionGroups(distributionGroupId).ConfigureAwait(false);

      foreach (var memberUser in memberUsers)
      {
        result.DistributionGroupMembers.Add(new DistributionGroupMember
        {
          Id = memberUser.Id,
          ContactName = $"{memberUser.FirstName} {memberUser.LastName} (Member User)",
          ContactNumber = string.Empty,
          EmailAddress = memberUser.EmailAddress,
        });
      }

      var mapped = _mapper.Map<DistributionGroup, DistributionGroupModel>(result);

      return mapped;
    }

    public async Task<DistributionGroupMemberModel> GetDistributionGroupMember(int distributionGroupMemberId)
    {
      var result = await _applicationRepository.QuerySingleAsync<DistributionGroupMember>(x => x.Id == distributionGroupMemberId);
      return _mapper.Map<DistributionGroupMember, DistributionGroupMemberModel>(result);
    }

    public async Task<bool> DeleteDistributionGroup(int distributionGroupId)
    {
      var recordToDelete = await _applicationRepository.QuerySingleAsync<DistributionGroup>(x => x.Id == distributionGroupId).ConfigureAwait(false);
      var result = await _applicationRepository.DeleteAndCommitAsync(recordToDelete).ConfigureAwait(false);
      return result.Succeeded;
    }

    public async Task<bool> DeleteDistributionGroupMember(int distributionGroupMemberId)
    {
      var recordToDelete = await _applicationRepository.QuerySingleAsync<DistributionGroupMember>(x => x.Id == distributionGroupMemberId).ConfigureAwait(false);
      var result = await _applicationRepository.DeleteAndCommitAsync(recordToDelete).ConfigureAwait(false);
      return result.Succeeded;
    }

    public async Task<bool> DeleteDistributionGroupMemberUser(int distributionGroupMemberId)
    {
      var recordToDelete = await _applicationRepository.QuerySingleAsync<MemberUserDistributionGroup>(x => x.Id == distributionGroupMemberId).ConfigureAwait(false);
      var result = await _applicationRepository.DeleteAndCommitAsync(recordToDelete).ConfigureAwait(false);
      return result.Succeeded;
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

    public async Task BatchSubmitted(int batchId)
    {
      var batch = await _applicationRepository.QuerySingleAsync<Batch>(x => x.Id == batchId).ConfigureAwait(false);
      batch.Status = (int)BatchStatus.Submitted;
      batch.SubmissionDate = DateTime.Now;
      await _applicationRepository.SaveOrUpdateAndCommitAsync(batch).ConfigureAwait(false);
    }

    public async Task<IEnumerable<BatchRecipientModel>> GetBatchRecipients(int batchId, int? distributionGroupId, int? committeeId)
    {
      var batchRecipients = new List<BatchRecipientModel>();

      if (distributionGroupId.HasValue)
      {
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
      }

      if (committeeId.HasValue)
      {
        var committeeList = await _applicationRepository.QueryAsync<MemberCommittee>(x => x.Committee.Id == committeeId).ConfigureAwait(false);

        foreach (var item in committeeList)
        {
          batchRecipients.Add(new BatchRecipientModel
          {
            Id = item.Id,
            BatchId = batchId,
            ToEmail = item.EmailAddress,
            ToName = item.MemberName
          });
        }
      }

      return batchRecipients;
    }

    public async Task AddBatchRecipient(int batchId, string emailAddress, string contactName)
    {
      var batchRecipient = new BatchRecipient
      {
        Batch = new Batch { Id = batchId },
        ToEmail = emailAddress,
        ToName = contactName ?? emailAddress,
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

    public async Task DeleteBatchRecipient(int batchRecipientId)
    {
      var result = await _applicationRepository.QuerySingleAsync<BatchRecipient>(x => x.Id == batchRecipientId);
      await _applicationRepository.DeleteAndCommitAsync(result).ConfigureAwait(false);
    }

    public async Task<bool> DeleteBatch(int batchId)
    {
      var recordToDelete = await _applicationRepository.QuerySingleAsync<Batch>(x => x.Id == batchId).ConfigureAwait(false);
      var result = await _applicationRepository.DeleteAndCommitAsync(recordToDelete).ConfigureAwait(false);
      return result.Succeeded;
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

    public async Task<SaveOrUpdateResult> Merge<TSource>(TSource source)
    {
      var mapped = _mapper.Map(source, source!.GetType(), source!.GetAttribute<MapToAttribute>()!.EntityType);
      var result = await _applicationRepository.MergeAndCommitAsync(mapped);

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

    #region Notifications

    public async Task<IEnumerable<NotificationTemplateModel>> GetNotificationTemplates(AlphaPageRequest pageRequest, string? searchText = null)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterNotificationTemplates(searchText ?? "")).ConfigureAwait(false);
      var resultMapped = _mapper.Map<IEnumerable<NotificationTemplate>, List<NotificationTemplateModel>>(result.PageData);
      return new StaticPagedList<NotificationTemplateModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    public async Task<NotificationTemplateModel> GetNotificationTemplate(int notificationTemplateId)
    {
      var result = await _applicationRepository.QuerySingleAsync<NotificationTemplate>(x => x.Id == notificationTemplateId);
      return _mapper.Map<NotificationTemplate, NotificationTemplateModel>(result);
    }

    public async Task<NotificationContactModel> GetNotificationContact(int notificationContactId)
    {
      var result = await _applicationRepository.QuerySingleAsync<NotificationContact>(x => x.Id == notificationContactId);
      return _mapper.Map<NotificationContact, NotificationContactModel>(result);
    }

    public async Task<bool> DeleteNotificationTemplate(int notificationTemplateId)
    {
      var recordToDelete = await _applicationRepository.QuerySingleAsync<NotificationTemplate>(x => x.Id == notificationTemplateId).ConfigureAwait(false);
      var result = await _applicationRepository.DeleteAndCommitAsync(recordToDelete).ConfigureAwait(false);
      return result.Succeeded;
    }

    public async Task<bool> DeleteNotificationContact(int notificationContactId)
    {
      var recordToDelete = await _applicationRepository.QuerySingleAsync<NotificationContact>(x => x.Id == notificationContactId).ConfigureAwait(false);
      var result = await _applicationRepository.DeleteAndCommitAsync(recordToDelete).ConfigureAwait(false);
      return result.Succeeded;
    }

    #endregion

    #region Document Types

    public async Task<IEnumerable<DocumentTypeModel>> GetDocumentTypes(AlphaPageRequest pageRequest, string? searchText = null)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterDocumentTypes(searchText ?? "")).ConfigureAwait(false);
      var resultMapped = _mapper.Map<IEnumerable<DocumentType>, List<DocumentTypeModel>>(result.PageData);
      return new StaticPagedList<DocumentTypeModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    public async Task<DocumentTypeModel> GetDocumentType(int documentTypeId)
    {
      var result = await _applicationRepository.QuerySingleAsync<DocumentType>(x => x.Id == documentTypeId);
      return _mapper.Map<DocumentType, DocumentTypeModel>(result);
    }

    public async Task<IEnumerable<DocumentTemplateModel>> GetDocumentTemplates(AlphaPageRequest pageRequest, string? searchText = null)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterDocumentTemplates(searchText ?? "")).ConfigureAwait(false);
      var resultMapped = _mapper.Map<IEnumerable<DocumentTemplate>, List<DocumentTemplateModel>>(result.PageData);
      return new StaticPagedList<DocumentTemplateModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    public async Task<DocumentTemplateModel> GetDocumentTemplate(int documentTemplateId)
    {
      var result = await _applicationRepository.QuerySingleAsync<DocumentTemplate>(x => x.Id == documentTemplateId);
      return _mapper.Map<DocumentTemplate, DocumentTemplateModel>(result);
    }

    public async Task<bool> DeleteDocumentType(int documentTypeId)
    {
      var recordToDelete = await _applicationRepository.QuerySingleAsync<DocumentType>(x => x.Id == documentTypeId).ConfigureAwait(false);
      var result = await _applicationRepository.DeleteAndCommitAsync(recordToDelete).ConfigureAwait(false);
      return result.Succeeded;
    }

    public async Task<bool> DeleteDocumentTemplate(int documentTemplateId)
    {
      var recordToDelete = await _applicationRepository.QuerySingleAsync<DocumentTemplate>(x => x.Id == documentTemplateId).ConfigureAwait(false);
      var result = await _applicationRepository.DeleteAndCommitAsync(recordToDelete).ConfigureAwait(false);
      return result.Succeeded;
    }

    #endregion

    #region Committes

    public async Task<IEnumerable<CommitteeModel>> GetCommittees(AlphaPageRequest pageRequest, string? searchText = null)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterCommittees(searchText ?? "")).ConfigureAwait(false);
      var resultMapped = _mapper.Map<IEnumerable<Committee>, List<CommitteeModel>>(result.PageData);
      return new StaticPagedList<CommitteeModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    public async Task<CommitteeModel> GetCommittee(int committeeId)
    {
      var result = await _applicationRepository.QuerySingleAsync<Committee>(x => x.Id == committeeId);
      return _mapper.Map<Committee, CommitteeModel>(result);
    }

    public async Task<MemberCommitteeModel> GetMemberCommittees(int memberId)
    {
      var result = await _applicationRepository.QuerySingleAsync<MemberCommittee>(x => x.Member.Id == memberId);
      return _mapper.Map<MemberCommittee, MemberCommitteeModel>(result);
    }

    public async Task<CommitteeDocumentModel> GetCommitteeDocuments(int committeeId)
    {
      var result = await _applicationRepository.QuerySingleAsync<CommitteeDocument>(x => x.Committee.Id == committeeId);
      return _mapper.Map<CommitteeDocument, CommitteeDocumentModel>(result);
    }

    public async Task<CommitteeDocumentModel> GetCommitteeDocument(int committeeDocumentId)
    {
      var result = await _applicationRepository.QuerySingleAsync<CommitteeDocument>(x => x.Id == committeeDocumentId);
      return _mapper.Map<CommitteeDocument, CommitteeDocumentModel>(result);
    }

    public async Task<MemberCommitteeModel> GetCommitteeMember(int committeeMemberId)
    {
      var result = await _applicationRepository.QuerySingleAsync<MemberCommittee>(x => x.Id == committeeMemberId);
      return _mapper.Map<MemberCommittee, MemberCommitteeModel>(result);
    }

    public async Task<bool> DeleteMemberCommittee(int memberCommitteeId)
    {
      var recordToDelete = await _applicationRepository.QuerySingleAsync<MemberCommittee>(x => x.Id == memberCommitteeId).ConfigureAwait(false);
      var result = await _applicationRepository.DeleteAndCommitAsync(recordToDelete).ConfigureAwait(false);
      return result.Succeeded;
    }

    public async Task<bool> DeleteMemberCommitteeDocument(int memberCommitteeDocumentId)
    {
      var recordToDelete = await _applicationRepository.QuerySingleAsync<CommitteeDocument>(x => x.Id == memberCommitteeDocumentId).ConfigureAwait(false);
      var result = await _applicationRepository.DeleteAndCommitAsync(recordToDelete).ConfigureAwait(false);
      return result.Succeeded;
    }

    public async Task<bool> DeleteCommittee(int committeeId)
    {
      var recordToDelete = await _applicationRepository.QuerySingleAsync<Committee>(x => x.Id == committeeId).ConfigureAwait(false);
      var result = await _applicationRepository.DeleteAndCommitAsync(recordToDelete).ConfigureAwait(false);
      return result.Succeeded;
    }

    public async Task<bool> DeleteCommitteeDocument(int committeeDocumentId)
    {
      var recordToDelete = await _applicationRepository.QuerySingleAsync<CommitteeDocument>(x => x.Id == committeeDocumentId).ConfigureAwait(false);
      var result = await _applicationRepository.DeleteAndCommitAsync(recordToDelete).ConfigureAwait(false);
      return result.Succeeded;
    }

    public async Task<bool> DeleteCommitteeMember(int committeeMemberId)
    {
      var recordToDelete = await _applicationRepository.QuerySingleAsync<MemberCommittee>(x => x.Id == committeeMemberId).ConfigureAwait(false);
      var result = await _applicationRepository.DeleteAndCommitAsync(recordToDelete).ConfigureAwait(false);
      return result.Succeeded;
    }

    #endregion

    #region Member Users and Branches

    public async Task<IEnumerable<MemberUserModel>> GetMemberUsers(AlphaPageRequest pageRequest, string? searchText = null)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterMemberUsers(searchText ?? "")).ConfigureAwait(false);
      var resultMapped = _mapper.Map<IEnumerable<MemberUser>, List<MemberUserModel>>(result.PageData);
      return new StaticPagedList<MemberUserModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    public async Task<MemberUserModel> GetMemberUser(int memberUserId)
    {
      var result = await _applicationRepository.QuerySingleAsync<MemberUser>(x => x.Id == memberUserId);

      var mapped = _mapper.Map<MemberUser, MemberUserModel>(result);

      mapped.MemberBranches.MemberId = mapped.MemberId;
      mapped.MemberBranches.MemberUserId = memberUserId;
      mapped.MemberBranches.MemberBranches = mapped.Branches!;

      return mapped;
    }

    public async Task<MemberUserModel> GetMemberUser(string userId)
    {
      var result = await _applicationRepository.QuerySingleAsync<MemberUser>(x => x.User.Id == userId);

      var mapped = _mapper.Map<MemberUser, MemberUserModel>(result);

      mapped.MemberBranches.MemberId = mapped.MemberId;
      mapped.MemberBranches.MemberUserId = mapped.Id;
      mapped.MemberBranches.MemberBranches = mapped.Branches ?? new List<MemberUserBranchModel>();

      return mapped;
    }

    public async Task<SaveOrUpdateResult> SaveOrUpdateMemberUser(MemberUserModel memberUserModel)
    {
      WimsApplicationUser user = new WimsApplicationUser()
      {
        Email = memberUserModel.Email,
        UserName = memberUserModel.Email,
        FirstName = memberUserModel.FirstName,
        LastName = memberUserModel.LastName,
        IsActive = true,
      };

      var identityResult = await _wimsIdentityLayer.Identity.CreateUserAsync(user, memberUserModel.Password).ConfigureAwait(false);

      if (identityResult.Succeeded)
      {
        await _wimsIdentityLayer.Identity.AddUserToRoleAsync(user.Id, "Member");
      }

      var mapped = _mapper.Map<MemberUserModel, MemberUser>(memberUserModel);
      mapped.User = new WimsApplicationUser { Id = user.Id };
      var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(mapped);

      if (result.Succeeded)
      {

      }

      return new SaveOrUpdateResult
      {
        Success = result.Succeeded,
        ErrorMessage = result.ExceptionMessage,
        SavedObjectId = result.SavedObjectId,
      };
    }

    public async Task DeleteAllMemberBranchUsers(int memberUserId)
    {
      var result = await _applicationRepository
        .QueryAsync<MemberUserBranch>(x => x.MemberUser.Id == memberUserId)
        .ConfigureAwait(false);

      foreach (var memberUserBranch in result)
      {
        await _applicationRepository.DeleteAndCommitAsync(memberUserBranch).ConfigureAwait(false);
      }
    }

    public async Task<ICollection<MemberUserBranchModel>> GetMemberUserBranches(int memberUserId)
    {
      var result = await _applicationRepository.QueryAsync<MemberUserBranch>(x => x.MemberUser.Id == memberUserId);
      return _mapper.Map<IEnumerable<MemberUserBranch>, List<MemberUserBranchModel>>(result);
    }

    public async Task<SaveOrUpdateResult> ResetMemberUserPassword(int memberUserId)
    {
      var memberUser = await _applicationRepository.QuerySingleAsync<MemberUser>(x => x.Id == memberUserId).ConfigureAwait(false);
      var identityUser = await _wimsIdentityLayer.Identity.GetUserByIdAsync(memberUser.User.Id).ConfigureAwait(false);
      var newPassword = PasswordGenerator.GeneratePassword(true, true, true, true, false, 10);
      var identityResult = await _wimsIdentityLayer.Identity.ChangePasswordAsync(identityUser, newPassword).ConfigureAwait(false);

      if (identityResult.Succeeded)
      {
        await this.SendGenericEmail(identityUser.Email, "Your password has been reset", $"Your new password for AFMA WIMS is: {newPassword}");
      }

      return new SaveOrUpdateResult
      {
        Success = identityResult.Succeeded,
        ErrorMessage = identityResult.Errors.FirstOrDefault()?.Description,
        SavedObjectId = memberUserId,
      };
    }

    public async Task<bool> CanUserEditMember(int memberId)
    {
      var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var memberUser = await _applicationRepository.QuerySingleAsync<MemberUser>(x => x.Member.Id == memberId && x.User.Id == userId).ConfigureAwait(false);
      return memberUser.CanEdit;
    }

    public async Task<bool> VerifyMemberUserPermission(int memberId)
    {
      var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var memberUser = await _applicationRepository.QueryAnyAsync<MemberUser>(x => x.Member.Id == memberId && x.User.Id == userId).ConfigureAwait(false);
      return memberUser ? true : false;
    }

    public async Task<bool> VerifyMemberUserBranchPermission(int memberBranchId)
    {
      var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var memberUser = await _applicationRepository.QuerySingleAsync<MemberUser>(x => x.User.Id == userId).ConfigureAwait(false);
      var memberUserBranch = await _applicationRepository.QueryAnyAsync<MemberUserBranch>(x => x.MemberUser.Id == memberUser.Id && x.MemberBranch.Id == memberBranchId).ConfigureAwait(false);
      return memberUserBranch ? true : false;
    }

    public async Task<int> GetMemberUserIdFromUserId()
    {
      var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var memberUser = await _applicationRepository.QuerySingleAsync<MemberUser>(x => x.User.Id == userId).ConfigureAwait(false);
      return memberUser.Id;
    }

    public async Task<bool> DeleteMemberUser(int memberUserId)
    {
      var recordToDelete = await _applicationRepository.QuerySingleAsync<MemberUser>(x => x.Id == memberUserId).ConfigureAwait(false);
      var result = await _applicationRepository.DeleteAndCommitAsync(recordToDelete).ConfigureAwait(false);
      var identityUser = await _wimsIdentityLayer.Identity.GetUserByIdAsync(recordToDelete.User.Id).ConfigureAwait(false);
      var identityResult = await _wimsIdentityLayer.Identity.UserManager.DeleteAsync(identityUser).ConfigureAwait(false);
      return result.Succeeded && identityResult.Succeeded;
    }

    #endregion

    #region Member Distribution

    public async Task<ICollection<DistributionGroupModel>> GetPublicDistributionGroups()
    {
      var result = await _applicationRepository.QueryAsync<DistributionGroup>(x => !x.Private);
      return _mapper.Map<IEnumerable<DistributionGroup>, List<DistributionGroupModel>>(result);
    }

    public async Task<ICollection<MemberUserDistributionGroupModel>> GetMemberUserDistributionGroups(int distributionGroupId)
    {
      var result = await _applicationRepository.QueryAsync<MemberUserDistributionGroup>(x => x.DistributionGroup.Id == distributionGroupId);
      return _mapper.Map<IEnumerable<MemberUserDistributionGroup>, List<MemberUserDistributionGroupModel>>(result);
    }

    public async Task<ICollection<MemberUserDistributionGroupModel>> GetMemberUserDistributionGroups()
    {
      var memberUserId = await this.GetMemberUserIdFromUserId().ConfigureAwait(false);
      var result = await _applicationRepository.QueryAsync<MemberUserDistributionGroup>(x => x.MemberUser.Id == memberUserId);
      return _mapper.Map<IEnumerable<MemberUserDistributionGroup>, List<MemberUserDistributionGroupModel>>(result);
    }


    public async Task DeleteAllMemberUserDistributionGroups()
    {
      var memberUserId = await this.GetMemberUserIdFromUserId().ConfigureAwait(false);

      var result = await _applicationRepository
        .QueryAsync<MemberUserDistributionGroup>(x => x.MemberUser.Id == memberUserId)
        .ConfigureAwait(false);

      foreach (var group in result)
      {
        await _applicationRepository.DeleteAndCommitAsync(group).ConfigureAwait(false);
      }
    }

    #endregion
  }
}

namespace AFMA.ApplicationLayer.Interface
{
  using AFMA.ApplicationLayer.Model.Wims;
  using Alpha.Services.Models.Data.PageData;
  using Microsoft.AspNetCore.Mvc.Rendering;
  using MemberBranchModel = AFMA.ApplicationLayer.Model.Wims.MemberBranchModel;
  using MemberModel = AFMA.ApplicationLayer.Model.Wims.MemberModel;

  public interface IWimsApplicationLayer
  {
    #region Select Lists

    Task<IEnumerable<SelectListItem>> GenerateMemberTypeSelectList(int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateMemberContactTypeSelectList(int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateProvinceSelectList(int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateDistributionGroupSelectList(int? selectedValue);

    #endregion

    #region Members

    Task<MemberModel> GetMember(int memberId);
    Task<IEnumerable<MemberModel>> GetMembers(AlphaPageRequest pageRequest, int? memberTypeId = null, string? searchText = null);
    Task<MemberBranchModel> GetMemberBranch(int memberBranchId);
    Task<IEnumerable<AnimalFeedProductModel>> GetAnimalFeedProducts();
    Task DeleteAllMemberAnimalFeedProducts(int memberBranchId);
    Task<IEnumerable<ServiceProviderModel>> GetServiceProviders();
    Task DeleteAllServiceProviders(int memberBranchId);

    #endregion

    #region Distribution Groups

    Task<IEnumerable<DistributionGroupModel>> GetDistributionGroups(AlphaPageRequest pageRequest,
      string? searchText = null);

    Task<DistributionGroupModel> GetDistributionGroup(int distributionGroupId);

    Task<DistributionGroupMemberModel> GetDistributionGroupMember(int distributionGroupMemberId);

    Task<IEnumerable<SelectListItem>> GenerateMemberSelectList(int? selectedValue);

    Task<IEnumerable<SelectListItem>> GenerateMemberBranchSelectList(int? memberId,
      int? selectedValue);

    Task<IEnumerable<SelectListItem>> GenerateMemberAccountSelectList(int? memberBranchId,
      int? selectedValue);

    Task<ICollection<MemberBranchModel>> GetMemberBranches(int memberId);
    Task<ICollection<MemberBranchContactModel>> GetMemberBranchContacts(int memberBranchId);

    #endregion

    #region Bulk Sending

    Task<IEnumerable<BatchModel>> GetBatches(AlphaPageRequest pageRequest, string? searchText = null);
    Task<BatchModel> GetBatch(int batchId);
    Task<IEnumerable<BatchRecipientModel>> GetBatchRecipients(int batchId, int distributionGroupId);
    Task AddBatchRecipient(int batchId, string emailAddress, string contactName);
    Task SubmitBatch(int batchId);
    Task<BatchAttachmentModel> GetBatchAttachment(int batchAttachmentId);
    Task DeleteBatchAttachment(int batchAttachmentId);

    #endregion

    #region Generic Methods

    Task<SaveOrUpdateResult> SaveOrUpdate<TSource>(TSource source);
    Task<SaveOrUpdateResult> Delete<TSource>(TSource source);
    Task<MemberBranchContactModel> GetMemberBranchContact(int memberBranchContactId);

    #endregion
  }
}

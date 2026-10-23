namespace AFMA.ApplicationLayer.Interface
{
  using AFMA.ApplicationLayer.Helpers;
  using AFMA.ApplicationLayer.Model.Wims;
  using AFMA.ApplicationLayer.ViewModels.Wims;
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
    Task<IEnumerable<SelectListItem>> GenerateNotificationTypeSelectList(int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateNotificationTemplateSelectList(int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateDocumentTemplateSelectList(int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateDocumentTypeSelectList(int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateCommitteeSelectList(int? selectedValue);

    IEnumerable<SelectListItem> GenerateActiveInactiveList(bool? selectedValue = null);
    IEnumerable<SelectListItem> GenerateYesNoList(bool? selectedValue = null);
    Task<IEnumerable<SelectListItem>> GenerateMemberAccountSelectListByMember(int? memberId, int? selectedValue);

    #endregion

    #region Email Sending

    Task<bool> SendGenericEmail(string recipientEmail, string subject, string body);

    #endregion

    #region Members

    Task<MemberModel> GetMember(int memberId);
    Task<bool> DeleteMember(int memberId);
    Task<IEnumerable<MemberModel>> GetMembers(int? memberTypeId = null, int? provinceId = null, string? memberStatus = null, string? searchText = null);
    Task<IEnumerable<MemberModel>> GetMembers(AlphaPageRequest pageRequest, int? memberTypeId = null, int? provinceId = null, string? memberStatus = null, string? searchText = null);
    Task<MemberBranchModel> GetMemberBranch(int memberBranchId);
    Task<ICollection<MemberBranchContactModel>> GetMemberContacts(int memberId);
    Task<bool> DeleteBranchContact(int memberBranchContactId);
    Task<bool> DeleteBranch(int memberBranchId);
    Task<IEnumerable<AnimalFeedProductModel>> GetAnimalFeedProducts();
    Task<IEnumerable<ServiceModel>> GetServices();
    Task<IEnumerable<ManufacturerModel>> GetManufacturers();
    Task<IEnumerable<SupplierModel>> GetSuppliers();
    Task DeleteAllMemberAnimalFeedProducts(int memberBranchId);
    Task DeleteAllServices(int memberBranchId);
    Task DeleteAllSuppliers(int memberBranchId);
    Task DeleteAllManufacturers(int memberBranchId);
    Task<IEnumerable<ServiceProviderModel>> GetServiceProviders();
    Task DeleteAllServiceProviders(int memberBranchId);
    Task<int> CreateBranchFromMember(int memberId);
    Task<int> CreateBranchFromMember();
    Task<ICollection<MemberBranchContactTypeViewModel>> GetCategorizedContacts(int memberBranchId, int memberId);
    Task<MemberTypeEnum> GetMemberType(int memberId);

    #endregion

    #region Member Documents

    Task<MemberDocumentModel> GetMemberDocument(int memberDocumentId);
    Task<bool> DeleteMemberDocument(int memberDocumentId);

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

    Task<bool> DeleteDistributionGroup(int distributionGroupId);
    Task<bool> DeleteDistributionGroupMember(int distributionGroupMemberId);
    Task<bool> DeleteDistributionGroupMemberUser(int distributionGroupMemberId);

    #endregion

    #region Bulk Sending

    Task<IEnumerable<BatchModel>> GetBatches(AlphaPageRequest pageRequest, string? searchText = null);
    Task<BatchModel> GetBatch(int batchId);
    Task<IEnumerable<BatchRecipientModel>> GetBatchRecipients(int batchId, int? distributionGroupId, int? committeeId);
    Task AddBatchRecipient(int batchId, string emailAddress, string contactName);
    Task<BatchAttachmentModel> GetBatchAttachment(int batchAttachmentId);
    Task DeleteBatchAttachment(int batchAttachmentId);
    Task BatchSubmitted(int batchId);
    Task<bool> DeleteBatch(int batchId);
    Task DeleteBatchRecipient(int batchRecipientId);

    #endregion

    #region Generic Methods

    Task<SaveOrUpdateResult> SaveOrUpdate<TSource>(TSource source);
    Task<SaveOrUpdateResult> Delete<TSource>(TSource source);
    Task<MemberBranchContactModel> GetMemberBranchContact(int memberBranchContactId);
    Task<SaveOrUpdateResult> Merge<TSource>(TSource source);

    #endregion

    #region Notifications

    Task<IEnumerable<NotificationTemplateModel>> GetNotificationTemplates(AlphaPageRequest pageRequest,
      string? searchText = null);

    Task<NotificationTemplateModel> GetNotificationTemplate(int notificationTemplateId);

    Task<NotificationContactModel> GetNotificationContact(int notificationContactId);
    Task<bool> DeleteNotificationTemplate(int notificationTemplateId);
    Task<bool> DeleteNotificationContact(int notificationContactId);

    #endregion

    #region Document Types

    Task<IEnumerable<DocumentTypeModel>> GetDocumentTypes(AlphaPageRequest pageRequest, string? searchText = null);
    Task<DocumentTypeModel> GetDocumentType(int documentTypeId);

    Task<IEnumerable<DocumentTemplateModel>> GetDocumentTemplates(AlphaPageRequest pageRequest,
      string? searchText = null);

    Task<DocumentTemplateModel> GetDocumentTemplate(int documentTemplateId);
    Task<bool> DeleteDocumentType(int documentTypeId);
    Task<bool> DeleteDocumentTemplate(int documentTemplateId);

    #endregion

    #region Committees

    Task<IEnumerable<CommitteeModel>> GetCommittees(AlphaPageRequest pageRequest, string? searchText = null);
    Task<CommitteeModel> GetCommittee(int committeeId);
    Task<MemberCommitteeModel> GetMemberCommittees(int memberId);
    Task<CommitteeDocumentModel> GetCommitteeDocuments(int committeeId);
    Task<CommitteeDocumentModel> GetCommitteeDocument(int committeeDocumentId);
    Task<MemberCommitteeModel> GetCommitteeMember(int committeeMemberId);
    Task<bool> DeleteMemberCommittee(int memberCommitteeId);
    Task<bool> DeleteMemberCommitteeDocument(int memberCommitteeDocumentId);
    Task<bool> DeleteCommittee(int committeeId);
    Task<bool> DeleteCommitteeDocument(int committeeDocumentId);
    Task<bool> DeleteCommitteeMember(int committeeMemberId);

    #endregion

    #region Member Users and Branches

    Task<IEnumerable<MemberUserModel>> GetMemberUsers(AlphaPageRequest pageRequest, string? searchText = null);
    Task<MemberUserModel> GetMemberUser(int memberUserId);
    Task<MemberUserModel> GetMemberUser(string userId);
    Task<SaveOrUpdateResult> SaveOrUpdateMemberUser(MemberUserModel memberUserModel);
    Task DeleteAllMemberBranchUsers(int memberUserId);
    Task<SaveOrUpdateResult> ResetMemberUserPassword(int memberUserId);
    Task<bool> CanUserEditMember(int memberId);
    Task<bool> VerifyMemberUserPermission(int memberId);
    Task<bool> VerifyMemberUserBranchPermission(int memberBranchId);
    Task<int> GetMemberUserIdFromUserId();
    Task<ICollection<MemberUserBranchModel>> GetMemberUserBranches(int memberUserId);
    Task<bool> DeleteMemberUser(int memberUserId);

    #endregion

    #region Member User Distribution Lists

    Task<ICollection<DistributionGroupModel>> GetPublicDistributionGroups();
    Task<ICollection<MemberUserDistributionGroupModel>> GetMemberUserDistributionGroups();
    Task<ICollection<MemberUserDistributionGroupModel>> GetMemberUserDistributionGroups(int distributionGroupId);
    Task DeleteAllMemberUserDistributionGroups();

    #endregion
  }
}

namespace AFMA.ApplicationLayer.Expressions
{
  using AFMA.Data.Entities;
  using System;
  using System.Linq.Expressions;

  public static class SearchExpression
  {
    #region Stats Expressions

    public static Expression<Func<MemberSummary, bool>> FilterMembers(int? memberTypeId, string searchTerm)
    {
      if (memberTypeId != null) { return (x) => x.Name.Contains(searchTerm) && (x.MemberType != null && x.MemberType.Id == memberTypeId); }
      return (x) => x.Name.Contains(searchTerm);
    }

    public static Expression<Func<FeedsalesReportLookup, bool>> FilterFeedsalesReports(int? memberId, int? branchId, bool? isSubmitted)
    {
      return (x) => (memberId == null || (x.Member != null && x.Member.Id == memberId)) &&
                    (branchId == null || (x.MemberBranch != null && x.MemberBranch.Id == branchId)) &&
                    (isSubmitted == null || (x.IsSubmitted != null && x.IsSubmitted == isSubmitted));
    }

    public static Expression<Func<SalmonellaReportLookup, bool>> FilterSalmonellaReports(int memberId, int branchId, int accountId, bool? isSubmitted)
    {
      return (x => x.Member.Id == memberId && x.MemberBranch.Id == branchId && x.MemberAccount.Id == accountId && (isSubmitted == null || (x.IsSubmitted != null && x.IsSubmitted == isSubmitted)));
    }

    public static Expression<Func<SalmonellaReportLookup, bool>> FilterSalmonellaReports(int? memberId, int? branchId, bool? isSubmitted)
    {
      return (x) => (memberId == null || (x.Member != null && x.Member.Id == memberId)) &&
                    (branchId == null || (x.MemberBranch != null && x.MemberBranch.Id == branchId)) &&
                    (isSubmitted == null || (x.IsSubmitted != null && x.IsSubmitted == isSubmitted));
    }

    public static Expression<Func<FeedsalesReportLookup, bool>> FilterFeedsalesReports(int memberId, int branchId, int accountId, bool? isSubmitted)
    {
      return (x => x.Member.Id == memberId && x.MemberBranch.Id == branchId && x.MemberAccount.Id == accountId && (isSubmitted == null || (x.IsSubmitted != null && x.IsSubmitted == isSubmitted)));
    }

    public static Expression<Func<ApplicationUser, bool>> FilterUsers(string searchTerm)
    {
      return (x) => x.FirstName.Contains(searchTerm) || x.LastName.Contains(searchTerm) ||
                    x.Email.Contains(searchTerm) || x.UserName.Contains(searchTerm);
    }

    public static Expression<Func<SalmonellaSampleTypeItem, bool>> FilterMainLocations(int? salmonellaSampleTypeId, string searchTerm)
    {
      if (salmonellaSampleTypeId != null) { return (x) => x.Name.Contains(searchTerm) && (x.SalmonellaSampleType != null && x.SalmonellaSampleType.Id == salmonellaSampleTypeId); }
      return (x) => x.Name.Contains(searchTerm);
    }

    public static Expression<Func<SalmonellaSampleTypeSubItem, bool>> FilterSubLocations(int? salmonellaSampleTypeId, string searchTerm)
    {
      if (salmonellaSampleTypeId != null) { return (x) => x.Name.Contains(searchTerm) && (x.SalmonellaSampleType != null && x.SalmonellaSampleType.Id == salmonellaSampleTypeId); }
      return (x) => x.Name.Contains(searchTerm);
    }

    public static Expression<Func<SalmonellaType, bool>> FilterSalmonellaTypes(int? salmonellaGroupId, string searchTerm)
    {
      if (salmonellaGroupId != null) { return (x) => x.Name!.Contains(searchTerm) && (x.SalmonellaGroup != null && x.SalmonellaGroup.Id == salmonellaGroupId); }
      return (x) => x.Name!.Contains(searchTerm);
    }

    public static Expression<Func<SalmonellaGroup, bool>> FilterSalmonellaGroups(string searchTerm)
    {
      return (x) => x.Name!.Contains(searchTerm);
    }

    public static Expression<Func<Laboratory, bool>> FilterLaboratories(string searchTerm)
    {
      return (x) => x.Name!.Contains(searchTerm);
    }

    public static Expression<Func<MemberType, bool>> FilterMemberTypes(string searchTerm)
    {
      return (x) => x.Description!.Contains(searchTerm);
    }

    public static Expression<Func<Material, bool>> FilterRawMaterials(int? materialCategoryId, string searchTerm)
    {
      if (materialCategoryId != null) { return (x) => x.Name!.Contains(searchTerm) && (x.MaterialCategory != null && x.MaterialCategory.Id == materialCategoryId); }
      return (x) => x.Name!.Contains(searchTerm);
    }

    public static Expression<Func<Product, bool>> FilterFinishedProducts(int? productGroupId, string searchTerm)
    {
      if (productGroupId != null) { return (x) => x.Name!.Contains(searchTerm) && (x.ProductGroup != null && x.ProductGroup.Id == productGroupId); }
      return (x) => x.Name!.Contains(searchTerm);
    }

    public static Expression<Func<ProductGroup, bool>> FilterFinishedProductGroups(string searchTerm)
    {
      return (x) => x.Name!.Contains(searchTerm);
    }

    public static Expression<Func<MycotoxinReportLookup, bool>> FilterMycotoxinReports(int? memberId, int? branchId, bool? isSubmitted)
    {
      return (x) => (memberId == null || (x.Member != null && x.Member.Id == memberId)) &&
                    (branchId == null || (x.MemberBranch != null && x.MemberBranch.Id == branchId)) &&
                    (isSubmitted == null || (x.IsSubmitted != null && x.IsSubmitted == isSubmitted));
    }

    public static Expression<Func<MycotoxinReport, bool>> FilterMycotoxinReports(int memberId, int branchId, int accountId, bool? isSubmitted)
    {
      return (x => x.Member.Id == memberId && x.MemberBranch.Id == branchId && x.MemberAccount.Id == accountId && (isSubmitted == null || (x.IsSubmitted != null && x.IsSubmitted == isSubmitted)));
    }

    public static Expression<Func<MycotoxinClass, bool>> FilterMycotoxinClasses(string searchTerm)
    {
      return (x) => x.Name!.Contains(searchTerm);
    }

    public static Expression<Func<MycotoxinType, bool>> FilterMycotoxinTypes(int? mycotoxinClassId, string searchTerm)
    {
      if (mycotoxinClassId != null) { return (x) => x.Name!.Contains(searchTerm) && (x.MycotoxinClass != null && x.MycotoxinClass.Id == mycotoxinClassId); }
      return (x) => x.Name!.Contains(searchTerm);
    }

    public static Expression<Func<IndustryProfileReport, bool>> FilterIndustryProfileReports(int? memberId, int? branchId, bool? isSubmitted)
    {
      return (x) => (memberId == null || (x.Member != null && x.Member.Id == memberId)) &&
                    (branchId == null || (x.MemberBranch != null && x.MemberBranch.Id == branchId)) &&
                    (isSubmitted == null || (x.Submitted != null && x.Submitted == isSubmitted));
    }

    #endregion

    #region Wims Expressions

    public static Expression<Func<AFMA.Data.Entities.Wims.Member, bool>> FilterWimsMembers(int? memberTypeId, int? provinceId, bool? memberStatus, string searchTerm)
    {
      return x =>
          (x.MemberTradeName.Contains(searchTerm) || x.MemberIdentificationId.Contains(searchTerm)) &&
          (memberTypeId == null || (x.MemberType != null && x.MemberType.Id == memberTypeId)) &&
          (provinceId == null || (x.Province != null && x.Province.Id == provinceId)) &&
          (memberStatus == null || x.Active == memberStatus);
    }

    public static Expression<Func<AFMA.Data.Entities.Wims.DistributionGroup, bool>> FilterDistributionGroups(string searchTerm)
    {
      return (x) => x.Name!.Contains(searchTerm);
    }

    public static Expression<Func<AFMA.Data.Entities.Wims.Batch, bool>> FilterBatches(string searchTerm)
    {
      return (x) => x.Description!.Contains(searchTerm);
    }

    public static Expression<Func<AFMA.Data.Entities.Wims.NotificationTemplate, bool>> FilterNotificationTemplates(string searchTerm)
    {
      return (x) => x.Description.Contains(searchTerm);
    }

    public static Expression<Func<AFMA.Data.Entities.Wims.DocumentType, bool>> FilterDocumentTypes(string searchTerm)
    {
      return (x) => x.Description.Contains(searchTerm);
    }

    public static Expression<Func<AFMA.Data.Entities.Wims.DocumentTemplate, bool>> FilterDocumentTemplates(string searchTerm)
    {
      return (x) => x.Name.Contains(searchTerm);
    }

    public static Expression<Func<AFMA.Data.Entities.Wims.Committee, bool>> FilterCommittees(string searchTerm)
    {
      return (x) => x.Name.Contains(searchTerm);
    }

    public static Expression<Func<AFMA.Data.Entities.Wims.CommitteeDocument, bool>> FilterCommitteeDocuments(string searchTerm)
    {
      return (x) => x.Name.Contains(searchTerm);
    }

    public static Expression<Func<AFMA.Data.Entities.Wims.MemberUser, bool>> FilterMemberUsers(string searchTerm)
    {
      return (x) => x.FirstName.Contains(searchTerm) || x.LastName.Contains(searchTerm);
    }

    #endregion
  }
}

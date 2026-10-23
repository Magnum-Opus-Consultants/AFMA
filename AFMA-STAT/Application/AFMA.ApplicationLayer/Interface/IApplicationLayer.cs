namespace AFMA.ApplicationLayer.Interface
{
  using AFMA.ApplicationLayer.Helpers;
  using AFMA.Data.Entities.Interface;
  using Alpha.Services.Models.Data.PageData;
  using Alpha.Services.Models.Repository;
  using Microsoft.AspNetCore.Mvc.Rendering;
  using ViewModels;

  /// <summary>
  /// The ApplicationLayer interface.
  /// </summary>
  public interface IApplicationLayer
  {
    #region Select Lists

    Task<IEnumerable<SelectListItem>> GenerateMaterialCategorySelectList(int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateSalmonellaReportGroupSelectList(int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateFeedsalesReportGroupSelectList(int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateProductGroupSelectList(int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateProductSelectList(int productGroupId, int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateMaterialSelectList(int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateSalmonellaSampleTypeSelectList(int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateSalmonellaSampleTypeItemSelectList(int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateSalmonellaSampleTypeSubItemSelectList(int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateProductSelectList(int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateLaboratorySelectList(int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateGenericSelectList<T>(int? selectedValue) where T : class;

    Task<IEnumerable<SelectListItem>> GenerateMemberSelectList(bool showActiveOnly, ModuleEnum? module, int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateMemberBranchSelectList(bool showActiveOnly, int? memberId, int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateMemberAccountSelectList(bool showActiveOnly, int? memberBranchId, int? selectedValue);
    IEnumerable<SelectListItem> GenerateReportStatusSelectList(bool? selectedValue);
    IEnumerable<SelectListItem> GenerateSampleTypeSelectList(int? selectedValue);

    Task<IEnumerable<SelectListItem>> GenerateMaterialOriginSelectList(int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateProvinceSelectList(int? selectedValue);

    Task<IEnumerable<SelectListItem>> GenerateMemberTypeSelectList(int? selectedValue);

    #endregion

    #region Lookups

    Task<string> GetMemberNameFromId(int memberId);
    Task<string> GetMemberBranchNameFromId(int memberBranchId);

    #endregion

    #region Users

    /// <summary>
    /// Gets a list of system users.
    /// </summary>
    /// <returns>The users.</returns>
    IEnumerable<UserModel> GetUsers();

    /// <summary>
    /// Gets a list of all current users, with their roles.
    /// </summary>
    /// <returns></returns>
    //Task<ICollection<UserModel>> GetUsersWithRoles();

    Task<IEnumerable<UserModel>> GetUsersWithRoles(AlphaPageRequest pageRequest, string? searchText = null);

    #endregion

    #region Members

    /// <summary>
    /// Gets a paged list of members.
    /// </summary>
    /// <param name="pageRequest">The Page Request.</param>
    /// <param name="memberTypeId">The member type Id.</param>
    /// <param name="searchText">The search text.</param>
    /// <returns></returns>
    Task<IEnumerable<MemberSummaryModel>> GetMembersSummary(AlphaPageRequest pageRequest, int? memberTypeId = null, string? searchText = null);

    /// <summary>
    /// Gets a list members.
    /// </summary>
    /// <returns>The members.</returns>
    Task<IEnumerable<MemberModel>> GetMembers();

    /// <summary>
    /// Gets a list members.
    /// </summary>
    /// <returns>The members.</returns>
    Task<IEnumerable<MemberSummaryModel>> GetMembersSummary();

    /// <summary>
    /// Gets a <see cref="MemberModel"/> based on the Member Id.
    /// </summary>
    /// <param name="memberId">The Member Id.</param>
    /// <returns>The <see cref="MemberModel"/><</returns>
    Task<MemberModel> GetMember(int memberId);

    /// <summary>
    /// Checks if a member has access to a module.
    /// </summary>
    /// <param name="memberId">The Member Id.</param>
    /// <param name="module">The module to check.</param>
    /// <returns>The value indicating if the member has access to the module.</returns>
    Task<bool> CheckMemberModule(int memberId, ModuleEnum module);

    /// <summary>
    /// Adds or removes a selected/deselected member module.
    /// </summary>
    /// <param name="memberId">The member Id.</param>
    /// <param name="memberModules">The list of modules to add or remove.</param>
    void UpdateMemberModules(int memberId, IEnumerable<MemberModuleModel> memberModules);

    /// <summary>
    /// Deletes a member
    /// </summary>
    /// <param name="id">The member Id.</param>
    /// <returns>The result</returns>
    Task<RepositoryResult> DeleteMember(int id);

    /// <summary>
    /// Gets the member type for a member.
    /// </summary>
    /// <param name="memberId">The member Id</param>
    /// <returns>The member type object.</returns>
    Task<MemberTypeModel> GetMemberMemberType(int memberId);

    #endregion

    #region Member Branches

    Task<IEnumerable<MemberBranchModel>> GetMemberBranches();
    Task<MemberBranchModel> GetMemberBranch(int memberBranchId);
    Task<IEnumerable<MemberBranchSummaryModel>> GetMemberBranchesSummary();
    Task<IEnumerable<MemberBranchSummaryModel>> GetMemberBranchesSummaryByMemberType(int memberTypeId);
    Task<IEnumerable<MemberBranchModel>> GetActiveMemberBranchesSummaryByModule(int moduleId);
    Task<IEnumerable<MemberBranchSummaryModel>> GetMemberBranchesSummary(int memberId);
    Task<IEnumerable<MemberBranchModel>> GetMemberBranchesSummaryByModule(int moduleId);
    Task<ICollection<MemberBranchModel>> GetMemberBranchesByModule(int moduleId);
    Task<RepositoryResult> DeleteMemberBranch(int id, int memberId);

    #endregion

    #region Member Accounts

    Task<ICollection<MemberAccountModel>> GetMemberAccounts();
    Task<MemberAccountModel> GetMemberAccount(int memberAccountId);
    Task<MemberAccountModel?> GetMemberAccount(string userId);
    Task<ICollection<MemberAccountModel>> GetMemberAccountsByModule(int moduleId, bool showActive = false);
    Task<bool> AddMemberAccountToModule(int memberAccountId, int moduleId);
    Task<bool> RemoveMemberAccountFromModule(int memberAccountId, int moduleId);
    Task<bool> CheckMemberAccountModule(int memberAccountId, ModuleEnum module);
    Task<RepositoryResult> DeleteMemberAccount(int id, int memberId);

    Task<MemberAccessViewModel> GetCurrentMemberAccess();

    /// <summary>
    /// Checks if a member account has access to a module.
    /// </summary>
    /// <remarks>
    /// Note that we are not passing in the Member Account Id.
    /// We are resolving the current user and querying the member account by
    /// exposing the session in the background.
    /// Refer to the <see cref="Alpha.Repository.Services.Identity.IdentityService{TIdentityUser}"/>
    /// </remarks>
    /// <param name="module">The module to check.</param>
    /// <returns>The value indicating if the member account has access to the module.</returns>
    Task<bool> CheckMemberAccountModule(ModuleEnum module);

    Task<IEnumerable<MemberAccountModel>> GetMemberBranchAccounts(int memberBranchId);

    Task<IEnumerable<MemberAccountModel>> GetMemberBranchAccounts(int memberBranchId, ModuleEnum module);

    Task<IEnumerable<MemberAccountModel>> GetActiveMemberBranchAccounts(int memberBranchId, ModuleEnum module);

    Task<IEnumerable<MemberSummaryModel>> GetMembersSummaryByModule(int moduleId);

    #endregion

    #region Member Types

    /// <summary>
    /// Returns a list of member types.
    /// </summary>
    /// <returns>Tje list of member types.</returns>
    Task<IEnumerable<MemberTypeModel>> GetMemberTypes();

    /// <summary>
    /// Returns a paged and filtered list of member types.
    /// </summary>
    /// <param name="pageRequest">The page request.</param>
    /// <param name="searchText">The search text.</param>
    /// <returns>The list of member types.</returns>
    Task<IEnumerable<MemberTypeModel>> GetMemberTypes(AlphaPageRequest pageRequest, string? searchText = null);

    /// <summary>
    /// Returns a single member type.
    /// </summary>
    /// <param name="memberTypeId">The member type ID.</param>
    /// <returns>The member type.</returns>
    Task<MemberTypeModel> GetMemberType(int memberTypeId);

    /// <summary>
    /// Deletes a member type.
    /// </summary>
    /// <param name="memberTypeId">The member type ID.</param>
    /// <returns>The delete result.</returns>
    Task<RepositoryResult> DeleteMemberType(int memberTypeId);

    #endregion

    #region Raw Materials

    /// <summary>
    /// Gets a list of raw materials
    /// </summary>
    /// <returns>The raw materials</returns>
    Task<IEnumerable<MaterialModel>> GetRawMaterials();

    /// <summary>
    /// Retrieves a filtered and paged list of raw materials
    /// </summary>
    /// <param name="pageRequest">The page request.</param>
    /// <param name="materialCategoryId">The material category ID.</param>
    /// <param name="searchText">The search text.</param>
    /// <returns>The list of raw materials.</returns>
    Task<IEnumerable<MaterialModel>> GetRawMaterials(AlphaPageRequest pageRequest, int? materialCategoryId = null,
      string? searchText = null);

    /// <summary>
    /// Gets a list of raw materials for by category
    /// </summary>
    /// <param name="materialCategoryId">The material category</param>
    /// <returns>The raw materials</returns>
    Task<IEnumerable<MaterialModel>> GetRawMaterials(int materialCategoryId);

    /// <summary>
    /// Gets a <see cref="MaterialModel"/> based on the material Id.
    /// </summary>
    /// <param name="materialId">The material Id.</param>
    /// <returns>The <see cref="MaterialModel"/></returns>
    Task<MaterialModel> GetRawMaterial(int materialId);

    #endregion

    #region Finished Products

    /// <summary>
    /// Gets a list of finished products.
    /// </summary>
    /// <returns>The finished products</returns>
    Task<IEnumerable<ProductModel>> GetFinishedProducts();

    /// <summary>
    /// Retrieves a list of products by product group.
    /// </summary>
    /// <param name="productGroupId">The product group ID.</param>
    /// <returns>The product group list.</returns>
    Task<IEnumerable<ProductModel>> GetFinishedProducts(int productGroupId);

    /// <summary>
    /// Retrieves a filtered and paged list of finished products.
    /// </summary>
    /// <param name="pageRequest">The page request.</param>
    /// <param name="productGroupId">The product group ID.</param>
    /// <param name="searchText">The search text.</param>
    /// <returns>The product list.</returns>
    Task<IEnumerable<ProductModel>> GetFinishedProducts(AlphaPageRequest pageRequest, int? productGroupId = null,
      string? searchText = null);

    /// <summary>
    /// Gets a <see cref="ProductModel"/> based on the product Id.
    /// </summary>
    /// <param name="productId">The product Id.</param>
    /// <returns>The <see cref="ProductModel"/></returns>
    Task<ProductModel> GetFinishedProduct(int productId);

    Task<IEnumerable<AvailableFinishedProductsViewModel>> AvailableFinishedProducts(int reportId, int productGroupId);

    #endregion

    #region Finished Product Groups

    Task<IEnumerable<ProductGroupModel>> GetFinishedProductGroups();

    Task<IEnumerable<ProductGroupModel>> GetFinishedProductGroups(AlphaPageRequest pageRequest,
      string? searchText = null);

    Task<ProductGroupModel> GetFinishedProductGroup(int productGroupId);

    Task<RepositoryResult> DeleteFinishedProductGroup(int productGroupId);

    #endregion

    #region Modules

    /// <summary>
    /// Gets a list of modules
    /// </summary>
    /// <returns>The modules</returns>
    Task<IEnumerable<ModuleModel>> GetModules();

    /// <summary>
    /// Gets a <see cref="ModuleModel"/> based on the module Id.
    /// </summary>
    /// <param name="moduleId">The product Id.</param>
    /// <returns>The <see cref="ModuleModel"/></returns>
    Task<ModuleModel> GetModule(int moduleId);

    #endregion

    #region Provinces

    /// <summary>
    /// Gets a list of provinces
    /// </summary>
    /// <returns>The provinces</returns>
    Task<IEnumerable<ProvinceModel>> GetProvinces();

    /// <summary>
    /// Gets a <see cref="ProvinceModel"/> based on the province Id.
    /// </summary>
    /// <param name="provinceId">The province Id.</param>
    /// <returns>The <see cref="ProvinceModel"/></returns>
    Task<ProvinceModel> GetProvince(int provinceId);

    #endregion

    #region Laboratories

    /// <summary>
    /// Gets a list of all laboratories.
    /// </summary>
    /// <returns>The list.</returns>
    Task<IEnumerable<LaboratoryModel>> GetLaboratories();

    /// <summary>
    /// Retrieves a paged and filtered list of laboratories.
    /// </summary>
    /// <param name="pageRequest">The page request.</param>
    /// <param name="searchText">The search text.</param>
    /// <returns>The list.</returns>
    Task<IEnumerable<LaboratoryModel>> GetLaboratories(AlphaPageRequest pageRequest, string? searchText = null);

    /// <summary>
    /// Retrieves a single laboratory.
    /// </summary>
    /// <param name="laboratoryId">The laboratory ID.</param>
    /// <returns>The laboratory.</returns>
    Task<LaboratoryModel> GetLaboratory(int laboratoryId);

    /// <summary>
    /// Deletes a laboratory.
    /// </summary>
    /// <param name="laboratoryId">The laboratory ID.</param>
    /// <returns>The delete result.</returns>
    Task<RepositoryResult> DeleteLaboratory(int laboratoryId);

    #endregion

    #region Main Locations

    /// <summary>
    /// Gets all main locations.
    /// </summary>
    /// <returns>The Main Locations.</returns>
    Task<IEnumerable<SalmonellaSampleTypeItemModel>> GetMainLocations();

    /// <summary>
    /// Gets a paged and filtered list of main locations.
    /// </summary>
    /// <param name="pageRequest"></param>
    /// <param name="salmonellaSampleTypeId"></param>
    /// <param name="searchText"></param>
    /// <returns></returns>
    Task<IEnumerable<SalmonellaSampleTypeItemModel>> GetMainLocations(AlphaPageRequest pageRequest, int? salmonellaSampleTypeId = null, string? searchText = null);

    /// <summary>
    /// Gets a main location.
    /// </summary>
    /// <param name="mainLocationId">The Main Location ID.</param>
    /// <returns>The Main Location.</returns>
    Task<SalmonellaSampleTypeItemModel> GetMainLocation(int mainLocationId);

    /// <summary>
    /// Deletes a main location.
    /// </summary>
    /// <param name="mainLocationId">The Main Location ID.</param>
    /// <returns>The delete result.</returns>
    Task<RepositoryResult> DeleteMainLocation(int mainLocationId);

    #endregion

    #region Sub Locations

    Task<IEnumerable<SalmonellaSampleTypeSubItemModel>> GetSubLocations();
    Task<SalmonellaSampleTypeSubItemModel> GetSubLocation(int subLocationId);

    /// <summary>
    /// Deletes a salmonella Sub Location.
    /// </summary>
    /// <param name="subLocationId">the Sub Location ID</param>
    /// <returns>The delete result.</returns>
    Task<RepositoryResult> DeleteSubLocation(int subLocationId);

    /// <summary>
    /// Gets a pages and filtered list of Salmonella Sub Locations.
    /// </summary>
    /// <param name="pageRequest"></param>
    /// <param name="salmonellaSampleTypeId"></param>
    /// <param name="searchText"></param>
    /// <returns></returns>
    Task<IEnumerable<SalmonellaSampleTypeSubItemModel>> GetSubLocations(AlphaPageRequest pageRequest,
      int? salmonellaSampleTypeId = null, string? searchText = null);

    #endregion

    #region Sample Types

    Task<IEnumerable<SalmonellaSampleTypeModel>> GetSampleTypes();
    Task<SalmonellaSampleTypeModel> GetSampleType(int sampleTypeId);

    #endregion

    #region Feedsales Admin Reports

    Task<IEnumerable<FeedsalesReportModel>> GetFeedsalesReports();

    //Task<IEnumerable<FeedsalesReportModel>> GetFeedsalesReports(int? memberId, int? memberBranchId, bool? isSubmitted, int? month, int? year);

    Task<IEnumerable<FeedsalesReportLookupModel>> GetFeedsalesReports(int? memberId, int? memberBranchId, bool? isSubmitted, int? month, int? year);
    Task<FeedsalesReportModel> GetFeedsalesReport(int id);
    Task<FeedsalesReportItemModel> GetFeedsalesReportItem(int id);
    Task<ICollection<FeedsalesReportItemModel>> GetFeedsalesReportItems(int feedsalesReportId);
    Task<ICollection<FeedsalesReportItemLookupModel>> GetFeedsalesReportItems();
    Task<FeedsalesReportModel> GetPreviousFeedsalesReport(int memberBranchId, DateTime infoFor, int month, int year);
    Task<IEnumerable<FeedsalesReportLookupModel>> GetFeedsalesReportsLookup();

    //Task<IEnumerable<FeedsalesReportLookupModel>> GetFeedsalesReportsLookup(int? memberId, int? memberBranchId, bool? isSubmitted, int? month = null, int? year = null);
    Task<IEnumerable<FeedsalesReportLookupModel>> GetFeedsalesReportsLookup(AlphaPageRequest pageRequest, int? memberId, int? memberBranchId, bool? isSubmitted);

    Task<IEnumerable<FeedsalesReportLookupModel>> GetFeedsalesReportsLookup(AlphaPageRequest pageRequest, int memberId,
      int memberBranchId, int memberAccountId, bool? isSubmitted);

    Task<IEnumerable<FeedsalesReportWithItemsLookupModel>> GetFeedsalesReportsWithItems(int? memberId, int? memberBranchId, int? memberAccountId, bool? isSubmitted, int? month, int? year);

    Task<bool> DeleteFeedsalesReportItem(int id);
    Task<RepositoryResult> DeleteFeedsalesReport(int id);
    Task<SaveOrUpdateResult> UpdateFeedsalesReportStatus(int id, bool submitStatus);

    Task<ICollection<FeedsalesReportGroupModel>> GetFeedsalesReportGroups();

    //Task<bool> SendFeedsalesSubmissionReminder(int feedsalesReportId);
    Task<SaveOrUpdateResult> EditFeedsalesReport(int id, string differenceReason, string comment);

    #endregion

    #region Mycotoxin Admin Reports

    Task<bool> PopulateMycotoxinReports(int memberId, int memberBranchId, int memberAccountId, DateTime startDate,
      DateTime endDate);

    #endregion

    #region CRUD Methods

    T? ExecuteSqlQuery<T>(string sqlQuery);

    /// <summary>
    /// Saves or updates an entity.
    /// </summary>
    /// <typeparam name="TSource">The source object type.</typeparam>
    /// <param name="source">The data to be saved or updated.</param>
    /// <returns>The save or update result.</returns>
    Task<SaveOrUpdateResult> SaveOrUpdate<TSource>(TSource source);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSource">The source object type.</typeparam>
    /// <param name="source">The source data.</param>
    /// <returns>The delete result.</returns>
    Task<SaveOrUpdateResult> Delete<TSource>(TSource source);

    #endregion

    decimal GetTotalFinishedProductByGroup(int year, int month, int branchId, int productGroupId, bool isAdmin);
    decimal GetFeedsalesByMemberByDate(int year, int month, int branchId);
    decimal GetRawMaterialsByMemberByDate(int year, int month, int branchId);

    decimal GetTotalFeedsalesProductByProvinceByYear(DateTime startDate, DateTime endDate, int provinceId, int productId, int memberTypeId);

    decimal GetTotalFeedsalesProductGroupByProvinceByYear(DateTime startDate, DateTime endDate, int provinceId, int productGroupId, int memberTypeId);

    decimal GetTotalFeedsalesProductGroupByProvinceByYearTotal(DateTime startDate, DateTime endDate, int provinceId,
      int memberTypeId);

    decimal GetFinishedProductGroupTotalSales(DateTime startDate, DateTime endDate, int productGroupId, int memberTypeId, int? memberBranchId = 0);

    decimal GetFinishedProductsTotalSales(DateTime startDate, DateTime endDate, int productId, int memberTypeId, int? memberBranchId = 0);

    decimal GetTotalFeedsalesGroupByMonth(int year, int month, int memberTypeId, int? memberBranchId = 0);

    decimal GetTotalFeedsalesGroupByYear(DateTime startDate, DateTime endDate, int memberTypeId, int? memberBranchId = 0);

    decimal GetTotalRawMaterialUsageByCategoryByYear(DateTime startDate, DateTime endDate, int reportGroupCategoryId, int memberTypeId, int? memberBranchId = 0);

    decimal GetTotalRawMaterialUsageByYear(DateTime startDate, DateTime endDate, int reportGroupId, int reportCategoryId, int memberTypeId, int? memberBranchId = 0);
    decimal GetTotalFinishedProduct(int year, int month, int productId, int memberTypeId, int? memberBranchId = 0);
    decimal GetTotalRawMaterial(int year, int month, int materialId, int memberTypeId, int? memberBranchId = 0);

    Task<IEnumerable<ReportModel>> GetReports();

    Task<SalmonellaSampleModel> GetSalmonellaSampleModelById(int id);

    Task<FeedsalesReportItemModel> GetFeedsalesReportItemModelById(int id);

    Task<int> GetCurrentMemberAccountId();
    Task<string> GetCurrentUserId();

    Task<IEnumerable<SubmissionStatsViewModel>> GetFeedsalesSubmissions(DateTime period);
    Task<SalmonellaReportLookupModel> GetSalmonellaReportForPeriod(DateTime period, int branchId);
    Task<IEnumerable<SubmissionStatsViewModel>> GetSalmonellaSubmissions(DateTime period);

    Task<bool> PopulateFeedsalesReports(int memberId, int memberBranchId, int memberAccountId, DateTime startDate, DateTime endDate);
    Task<bool> PopulateSalmonellaReports(int memberId, int memberBranchId, int memberAccountId, DateTime startDate, DateTime endDate);

    #region Industry Profile Reports

    Task<IEnumerable<IndustryProfileReportModel>> GetIndustryProfileReports();
    Task<IEnumerable<IndustryProfileReportModel>> GetIndustryProfileReports(int memberId, int memberBranchId, bool submitted);

    Task<IEnumerable<IndustryProfileReportModel>> GetIndustryProfileReports(AlphaPageRequest pageRequest, int? memberId,
      int? memberBranchId, bool? isSubmitted);
    Task<IndustryProfileReportModel> GetIndustryProfileReport(int industryProfileReportId);

    Task<IndustryProfileReportModel> GetIndustryProfileReport(int industryProfileReportId, int memberId,
      int memberBranchId);
    Task<ICollection<MemberIpBaseItemModel>> GetMemberIpBaseItems(IndustryProfileDisplayType displayType, int memberId,
      int memberBranchId, int industryProfileReportId, int memberTypeId, int reportYear = 0);

    Task<ICollection<MemberIpAdminItemModel>> GetMemberIpAdminItems(IndustryProfileDisplayType displayType,
      int memberId, int memberBranchId, int industryProfileReportId, int memberTypeId, int reportYear = 0);

    Task<ICollection<MemberIpAdditionalItemModel>> GetMemberIpAdditionalItems(IndustryProfileDisplayType displayType,
      int memberId, int memberBranchId, int industryProfileReportId, int memberTypeId, int reportYear = 0);

    Task<ICollection<MemberRunningCostViewModel>> GetMemberRunningCosts(IndustryProfileDisplayType displayType,
      int memberId, int memberBranchId, int industryProfileReportId, int memberTypeId, int reportYear = 0);

    Task<ICollection<MemberRunningCostFleetModel>> GetMemberRunningCostsFleet(IndustryProfileDisplayType displayType,
      int memberId, int memberBranchId, int industryProfileReportId, int memberTypeId, int reportYear = 0);

    Task<ICollection<MemberRunningCostSundryModel>> GetMemberRunningCostsSundries(
      IndustryProfileDisplayType displayType, int memberId, int memberBranchId, int industryProfileReportId,
      int memberTypeId, int reportYear = 0);

    Task<ICollection<MemberTotalRawMaterialCostModel>> GetMemberTotalRawMaterialCosts(
      IndustryProfileDisplayType displayType, int memberId, int memberBranchId, int industryProfileReportId,
      int memberTypeId, int reportYear = 0);

    Task<ICollection<MemberPcbCostItemModel>> GetMemberPcbCostItems(PcbItem pcbItem,
      IndustryProfileDisplayType displayType, int memberId, int memberBranchId, int industryProfileReportId,
      int memberTypeId, int reportYear = 0);

    Task<ICollection<IndustryCostGroupModel>> GetIndustryCostGroups(bool showAll = true);

    Task<decimal> GetValueFromTable<TEntity>(int industryProfileReportId, int groupId, int itemId,
      CostingGroup costingGroup, int memberId, int memberBranchId, int year = 0)
      where TEntity : class, IIndustryCostMultiple;

    Task UpdateValueToTable<TEntity>(int id, int industryProfileReportId, int groupId, int itemId, decimal value)
      where TEntity : class, IIndustryCostSingle;

    Task<ICollection<IpBaseGroupModel>> GetIpBaseGroups(bool showAll = true);
    Task<ICollection<IpBaseItemModel>> GetIpBaseItems(bool showAll = true);
    Task<ICollection<IpAdminGroupModel>> GetIpAdminGroups(bool showAll = true);
    Task<ICollection<IpAdminItemModel>> GetIpAdminItems(bool showAll = true);
    Task<ICollection<IpAdditionalGroupModel>> GetIpAdditionalGroups(bool showAll = true);
    Task<ICollection<IpAdditionalItemModel>> GetIpAdditionalItems(bool showAll = true);

    Task<ICollection<RawMaterialTotalCostModel>> GetRawMaterialCost(bool showAll = true);
    Task<ICollection<RunningCostGroupModel>> GetRunningCostGroups(bool showAll = true);
    Task<ICollection<RunningCostRentedFleetModel>> GetRunningCostRentedFleetItems(bool showAll = true);
    Task<ICollection<RunningCostSundryExpenseModel>> GetRunningCostSundryExpenseItems(bool showAll = true);
    Task<ICollection<PcbCostItemModel>> GetPcbCostItems(bool showAll = true);
    Task<ICollection<PcbCostItemGroupModel>> GetPcbCostItemGroups(bool showAll = true);
    Task<ICollection<IndustryGroupItemModel>> GetIndustryCostItems(bool showAll = true);

    Task<decimal> GetCostFromTable(int linkedId1, int? linkedId2, int industryProfileReportId, string tableName,
      int industryCostId, RunningCostType runningCostType, int? reportYear, int? industryCostGroupId, int memberId = 0,
      int memberBranchId = 0);

    Task SaveOrUpdateIpBaseItem(int id, decimal value);
    Task SaveOrUpdateIpAdminItem(int id, decimal value);
    Task SaveOrUpdateIpAdditionalItem(int id, decimal value);

    Task SaveOrUpdateRunningCostItem(int id, int groupId, int itemId, int reportId, CostingGroup costingGroup,
      decimal value);

    Task SaveOrUpdateRunningCostFleetItem(int id, decimal value);
    Task SaveOrUpdateRunningCostSundryItem(int id, CostingGroup costingGroup, decimal value);
    Task SaveOrUpdatePcbCostItem(int id, decimal value);

    #endregion

    #region Dashboards

    Task<IEnumerable<DashboardViewModel>> GenerateFeedsalesDashboard();
    Task<IEnumerable<DashboardViewModel>> GenerateSalmonellaDashboard();
    Task<ClientDashboardItemViewModel> GenerateClientItemDashboard(int memberId, int memberBranchId, ModuleEnum module);

    #endregion

    #region Member Module Access

    Task<ICollection<MemberModuleAccessViewModel>> GenerateMemberModuleAccessMatrix();
    Task<ICollection<AccountModuleAccessViewModel>> GenerateAccountModuleAccessMatrix();

    #endregion

    #region Login History

    Task AddToLoginHistory(string userId, string userName, string ipAddress, string hostName,
      string member, string memberBranch, string userType, string browser);

    Task<IEnumerable<LoginHistoryModel>> GetLoginHistoryPaged(AlphaPageRequest pageRequest);
    Task<IEnumerable<LoginHistoryModel>> GetLoginHistory();

    #endregion

    #region Versioning

    string ApplicationVersion();

    #endregion

    #region Test Site Indicator

    bool IsTestSite();

    #endregion

    #region Member Account Audit

    Task<IEnumerable<MemberAccountAuditViewModel>> GetMemberAccountAudit();

    #endregion
  }
}

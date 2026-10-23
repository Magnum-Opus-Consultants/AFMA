namespace AFMA.ApplicationLayer.Interface
{
  using Alpha.Services.Models.Data.PageData;
  using Microsoft.AspNetCore.Mvc.Rendering;
  using Model;

  public interface ILookupLayer
  {
    #region Member Types

    Task<IEnumerable<SelectListItem>> GetMemberTypeSelectList(int? selectedValue);

    #endregion

    #region Modules

    Task<IEnumerable<SelectListItem>> GetModuleSelectList(int? selectedValue);
    Task<IEnumerable<ModuleModel>> GetModules();

    #endregion

    #region Provinces

    Task<IEnumerable<SelectListItem>> GetProvinceSelectList(int? selectedValue);
    Task<IEnumerable<ProvinceModel>> GetProvinces();

    #endregion

    #region Feedale Reports

    //Task<AlphaPageResult<FeedsalesReportLookupModel>> GetFeedsaleReportLookupsByMemberAccountIdPagedAsync(AlphaPageRequest pageRequest, int memberAccountId);

    //Task<FeedsalesReportLookupModel> GetFeedsaleReportLookupByFeedsaleReportIdAsync(int feedsaleReportId);

    #endregion

    #region Salmonella Reports

    Task<AlphaPageResult<SalmonellaReportLookupModel>> GetSalmonellaReportLookupsByMemberAccountIdPagedAsync(AlphaPageRequest pageRequest, int memberAccountId);

    Task<SalmonellaReportLookupModel> GetSalmonellaReportLookupBySalmonellaReportIdAsync(int salmonellaReportId);

    #endregion

    #region Salmonella Samples

    Task<List<SalmonellaSampleModel>> GetSalmonelleSamplesBySalmonellaReportId(int salmonellaReportId);

    Task<List<SalmonellaSampleTypeSelectLookupModel>> GetSalmonellaSampleTypeSelectLookups();

    Task<List<SalmonellaSampleTypeItemSelectLookupModel>> GetSalmonellaSampleTypeItemSelectLookups();

    Task<List<SalmonellaSampleTypeSubItemSelectLookupModel>> GetSalmonellaSampleTypeSubItemSelectLookups();

    Task<List<SalmonellaTypeSelectLookupModel>> GetSalmonellaTypeSelectLookups();

    #endregion

    #region Feedsale Report Items

    Task<List<FeedsalesReportItemLookupModel>> GetFeedsaleReportItemLookupsByFeedsaleReportId(int feedsaleReportId);

    #endregion

    #region Product

    Task<List<ProductSelectLookupModel>> GetProductSelectLookupsByProductGroupId(int productGroupId);

    #endregion

    #region Material

    Task<List<MaterialSelectLookupModel>> GetMaterialSelectLookups();

    #endregion

    #region Product Groups

    Task<List<ProductGroupLookupModel>> GetFeedsaleProductGroupLookups(bool forMember = true);

    Task<List<ProductGroupSelectLookupModel>> GetProductGroupSelectLookups();

    #endregion

    #region Laboratory

    Task<List<LaboratorySelectLookupModel>> GetLaboratorySelectLookups();

    #endregion
  }
}

namespace AFMA.ApplicationLayer
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Model;
  using AFMA.Data.Entities;
  using AFMA.Infrastructure.Options;
  using Alpha.Services.Models.Data.PageData;
  using Alpha.Services.NHibernate.Repository.Services;
  using AutoMapper;
  using Microsoft.AspNetCore.Mvc.Rendering;
  using Resolvetech.NetCore.Common.Helpers;

  public class LookupLayer : ILookupLayer
  {
    private readonly IApplicationRepository<SessionFactory> _applicationRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationLayer"/> class.
    /// </summary>
    /// <param name="applicationRepository">The application repository.</param>
    public LookupLayer(IApplicationRepository<SessionFactory> applicationRepository, IMapper mapper)
    {
      this._applicationRepository = applicationRepository;
      this._mapper = mapper;
    }

    #region member Types

    public async Task<IEnumerable<SelectListItem>> GetMemberTypeSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<MemberType>();
      return results.ToList().ToSelectList(a => a.Id, a => a.Description, selectedValue);
    }

    #endregion

    #region Modules

    public async Task<IEnumerable<SelectListItem>> GetModuleSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<Module>();
      return results.ToList().ToSelectList(a => a.Id, a => a.Description, selectedValue);
    }

    public async Task<IEnumerable<ModuleModel>> GetModules()
    {
      var results = await _applicationRepository.QueryAsync<Module>();
      return _mapper.Map<IEnumerable<Module>, List<ModuleModel>>(results);
    }

    #endregion

    #region Provinces

    public async Task<IEnumerable<SelectListItem>> GetProvinceSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<Province>();
      return results.ToList().ToSelectList(a => a.Id, a => a.Description, selectedValue);
    }

    public async Task<IEnumerable<ProvinceModel>> GetProvinces()
    {
      var results = await _applicationRepository.QueryAsync<Province>();
      return _mapper.Map<IEnumerable<Province>, List<ProvinceModel>>(results);
    }

    #endregion

    #region Feedsale Reports

    //public async Task<AlphaPageResult<FeedsalesReportLookupModel>> GetFeedsaleReportLookupsByMemberAccountIdPagedAsync(AlphaPageRequest pageRequest, int memberAccountId)
    //{
    //  var result = await _applicationRepository.QueryPagedAsync<FeedsalesReport>(pageRequest, x => x.MemberAccount.Id == memberAccountId).ConfigureAwait(false);

    //  return _mapper.Map<AlphaPageResult<FeedsalesReportLookupModel>>(result);
    //}

    //public async Task<FeedsalesReportLookupModel> GetFeedsaleReportLookupByFeedsaleReportIdAsync(int feedsaleReportId)
    //{
    //  var result = await _applicationRepository.QueryAsync<FeedsalesReport>(x => x.Id == feedsaleReportId).ConfigureAwait(false);

    //  return _mapper.Map<FeedsalesReportLookupModel>(result);
    //}

    #endregion

    #region Salmonella Reports

    public async Task<AlphaPageResult<SalmonellaReportLookupModel>> GetSalmonellaReportLookupsByMemberAccountIdPagedAsync(AlphaPageRequest pageRequest, int memberAccountId)
    {
      var result = await _applicationRepository.QueryPagedAsync<SalmonellaReport>(pageRequest, x => x.MemberAccount.Id == memberAccountId).ConfigureAwait(false);

      return _mapper.Map<AlphaPageResult<SalmonellaReportLookupModel>>(result);
    }

    public async Task<SalmonellaReportLookupModel> GetSalmonellaReportLookupBySalmonellaReportIdAsync(int salmonellaReportId)
    {
      var result = await _applicationRepository.QueryAsync<SalmonellaReport>(x => x.Id == salmonellaReportId).ConfigureAwait(false);

      return _mapper.Map<SalmonellaReportLookupModel>(result);
    }

    #endregion

    #region Salmonella Samples

    public async Task<List<SalmonellaSampleModel>> GetSalmonelleSamplesBySalmonellaReportId(int salmonellaReportId)
    {
      var result = await _applicationRepository.QueryAsync<SalmonellaSample>(x => x.SalmonellaReport.Id == salmonellaReportId).ConfigureAwait(false);

      return _mapper.Map<List<SalmonellaSampleModel>>(result);
    }

    public async Task<List<SalmonellaSampleTypeSelectLookupModel>> GetSalmonellaSampleTypeSelectLookups()
    {
      var result = await _applicationRepository.QueryAsync<SalmonellaSampleType>().ConfigureAwait(false);

      return _mapper.Map<List<SalmonellaSampleTypeSelectLookupModel>>(result);
    }

    public async Task<List<SalmonellaSampleTypeItemSelectLookupModel>> GetSalmonellaSampleTypeItemSelectLookups()
    {
      var result = await _applicationRepository.QueryAsync<SalmonellaSampleTypeItem>().ConfigureAwait(false);

      return _mapper.Map<List<SalmonellaSampleTypeItemSelectLookupModel>>(result.OrderBy(x => x.DisplayOrder));
    }

    public async Task<List<SalmonellaSampleTypeSubItemSelectLookupModel>> GetSalmonellaSampleTypeSubItemSelectLookups()
    {
      var result = await _applicationRepository.QueryAsync<SalmonellaSampleTypeSubItem>().ConfigureAwait(false);

      return _mapper.Map<List<SalmonellaSampleTypeSubItemSelectLookupModel>>(result.OrderBy(x => x.DisplayOrder));
    }

    public async Task<List<SalmonellaTypeSelectLookupModel>> GetSalmonellaTypeSelectLookups()
    {
      var result = await _applicationRepository.QueryAsync<SalmonellaType>().ConfigureAwait(false);

      return _mapper.Map<List<SalmonellaTypeSelectLookupModel>>(result.OrderBy(x => x.DisplayOrder));
    }

    #endregion

    #region Feedsale Report Items

    public async Task<List<FeedsalesReportItemLookupModel>> GetFeedsaleReportItemLookupsByFeedsaleReportId(int feedsaleReportId)
    {
      var result = await _applicationRepository.QueryAsync<FeedsalesReportItem>(x => x.FeedsalesReport.Id == feedsaleReportId);

      return _mapper.Map<List<FeedsalesReportItemLookupModel>>(result);
    }

    #endregion

    #region Product

    public async Task<List<ProductSelectLookupModel>> GetProductSelectLookupsByProductGroupId(int productGroupId)
    {
      var result = await this._applicationRepository.QueryAsync<Product>(x => x.ProductGroup.Id == productGroupId);

      return _mapper.Map<List<ProductSelectLookupModel>>(result.OrderByDescending(x => x.DisplayOrder));
    }

    #endregion

    #region Material

    public async Task<List<MaterialSelectLookupModel>> GetMaterialSelectLookups()
    {
      var result = await this._applicationRepository.QueryAsync<Material>();

      return _mapper.Map<List<MaterialSelectLookupModel>>(result.OrderByDescending(x => x.DisplayOrder));
    }

    #endregion

    #region Product Groups

    public async Task<List<ProductGroupLookupModel>> GetFeedsaleProductGroupLookups(bool forMember = true)
    {
      var result = await _applicationRepository.QueryAsync<ProductGroup>(x => forMember ? x.Active && x.VisibleToMembers : true);

      return _mapper.Map<List<ProductGroupLookupModel>>(result.OrderBy(x => x.DisplayOrder));
    }

    public async Task<List<ProductGroupSelectLookupModel>> GetProductGroupSelectLookups()
    {
      var result = await _applicationRepository.QueryAsync<ProductGroup>();

      return _mapper.Map<List<ProductGroupSelectLookupModel>>(result.OrderBy(x => x.DisplayOrder));
    }

    #endregion

    #region Laboratory

    public async Task<List<LaboratorySelectLookupModel>> GetLaboratorySelectLookups()
    {
      var result = await this._applicationRepository.QueryAsync<Laboratory>();

      return _mapper.Map<List<LaboratorySelectLookupModel>>(result);
    }

    #endregion
  }
}

namespace AFMA.AutoMapping
{
  using AFMA.ApplicationLayer.Model;
  using AFMA.Data.Entities;
  using AutoMapper;

  public class IndustryProfileMapping : Profile
  {
    public IndustryProfileMapping()
    {
      #region Industry Profile

      this.CreateMap<IndustryProfileReport, IndustryProfileReportModel>()
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.Name))
        .ForMember(d => d.MemberBranchId, m => m.MapFrom(s => s.MemberBranch.Id))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => s.MemberBranch.Name))
        .ForMember(d => d.MemberAccountId, m => m.MapFrom(s => s.MemberAccount.Id))
        .ForMember(d => d.MemberAccount, m => m.MapFrom(s => s.MemberAccount.ApplicationUser.FirstName))
        .ForMember(d => d.MemberTypeId, m => m.MapFrom(s => s.MemberType.Id))
        .ForMember(d => d.MemberType, m => m.MapFrom(s => s.MemberType.Description));

      this.CreateMap<IndustryProfileReportModel, IndustryProfileReport>()
        .ForMember(d => d.Member, m => m.MapFrom(s => new Member { Id = s.MemberId }))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => new MemberBranch { Id = s.MemberBranchId }))
        .ForMember(d => d.MemberAccount, m => m.MapFrom(s => new MemberAccount { Id = s.MemberAccountId }))
        .ForMember(d => d.MemberType, m => m.MapFrom(s => new MemberType { Id = s.MemberTypeId }));

      #endregion

      #region Industry Profile Items

      this.CreateMap<IpAdditionalGroup, IpAdditionalGroupModel>().ReverseMap();

      this.CreateMap<IpAdditionalItem, IpAdditionalItemModel>()
        .ForMember(d => d.IpAdditionalGroupId, m => m.MapFrom(s => s.IpAdditionalGroup.Id))
        .ForMember(d => d.IpAdditionalGroup, m => m.MapFrom(s => s.IpAdditionalGroup.GroupName));

      this.CreateMap<IpAdditionalItemModel, IpAdditionalItem>()
        .ForMember(d => d.IpAdditionalGroup, m => m.MapFrom(s => new IpAdditionalGroup { Id = s.IpAdditionalGroupId }));

      this.CreateMap<IpAdminGroup, IpAdminGroupModel>().ReverseMap();

      this.CreateMap<IpAdminItem, IpAdminItemModel>()
        .ForMember(d => d.IpAdminGroupId, m => m.MapFrom(s => s.IpAdminGroup.Id))
        .ForMember(d => d.IpAdminGroup, m => m.MapFrom(s => s.IpAdminGroup.GroupName));

      this.CreateMap<IpAdminItemModel, IpAdminItem>()
        .ForMember(d => d.IpAdminGroup, m => m.MapFrom(s => new IpAdminGroup { Id = s.IpAdminGroupId }));

      this.CreateMap<IpBaseGroup, IpBaseGroupModel>().ReverseMap();

      this.CreateMap<IpBaseItem, IpBaseItemModel>()
        .ForMember(d => d.IpBaseGroupId, m => m.MapFrom(s => s.IpBaseGroup.Id))
        .ForMember(d => d.IpBaseGroup, m => m.MapFrom(s => s.IpBaseGroup.GroupName));

      this.CreateMap<IpBaseItemModel, IpBaseItem>()
        .ForMember(d => d.IpBaseGroup, m => m.MapFrom(s => new IpBaseGroup { Id = s.IpBaseGroupId }));

      #endregion

      #region Pellet Crumb Bag (PCB)

      this.CreateMap<PcbCostMainGroup, PcbCostMainGroupModel>().ReverseMap();

      this.CreateMap<PcbCostSubGroup, PcbCostSubGroupModel>()
        .ForMember(d => d.PcbCostMainGroupId, m => m.MapFrom(s => s.PcbCostMainGroup.Id))
        .ForMember(d => d.PcbCostMainGroup, m => m.MapFrom(s => s.PcbCostMainGroup.GroupName));

      this.CreateMap<PcbCostSubGroupModel, PcbCostSubGroup>()
        .ForMember(d => d.PcbCostMainGroup, m => m.MapFrom(s => new PcbCostMainGroup { Id = s.PcbCostMainGroupId }));

      this.CreateMap<PcbCostItemGroup, PcbCostItemGroupModel>()
        .ForMember(d => d.PcbCostSubGroupId, m => m.MapFrom(s => s.PcbCostSubGroup.Id))
        .ForMember(d => d.PcbCostSubGroup, m => m.MapFrom(s => s.PcbCostSubGroup.SubGroupName));

      this.CreateMap<PcbCostItemGroupModel, PcbCostItemGroup>()
        .ForMember(d => d.PcbCostSubGroup, m => m.MapFrom(s => new PcbCostSubGroup { Id = s.PcbCostSubGroupId }));

      this.CreateMap<PcbCostItem, PcbCostItemModel>()
        .ForMember(d => d.PcbCostItemGroupId, m => m.MapFrom(s => s.PcbCostItemGroup.Id))
        .ForMember(d => d.PcbCostItemGroup, m => m.MapFrom(s => s.PcbCostItemGroup.ItemGroupName))
        .ForMember(d => d.PcbCostItemMainGroupId, m => m.MapFrom(s => s.PcbCostItemGroup.PcbCostSubGroup.PcbCostMainGroup.Id));

      this.CreateMap<PcbCostItemModel, PcbCostItem>()
        .ForMember(d => d.PcbCostItemGroup, m => m.MapFrom(s => new PcbCostItemGroup { Id = s.PcbCostItemGroupId }));

      #endregion

      #region Running Cost

      this.CreateMap<RunningCostGroup, RunningCostGroupModel>().ReverseMap();

      this.CreateMap<RunningCostRentedFleet, RunningCostRentedFleetModel>().ReverseMap();

      this.CreateMap<RunningCostSundryExpense, RunningCostSundryExpenseModel>().ReverseMap();

      this.CreateMap<RunningCost, RunningCostModel>()
        .ForMember(d => d.RunningCostGroupId, m => m.MapFrom(s => s.RunningCostGroup.Id))
        .ForMember(d => d.RunningCostGroup, m => m.MapFrom(s => s.RunningCostGroup.GroupName));

      this.CreateMap<RunningCostModel, RunningCost>()
        .ForMember(d => d.RunningCostGroup, m => m.MapFrom(s => new RunningCostGroup { Id = s.RunningCostGroupId }));

      #endregion

      #region Raw Material Total Cost

      this.CreateMap<RawMaterialTotalCost, RawMaterialTotalCostModel>().ReverseMap();

      #endregion

      #region Member Industry Profile Reports

      this.CreateMap<MemberIpAdditionalItem, MemberIpAdditionalItemModel>()
        .ForMember(d => d.IndustryProfileReportId, m => m.MapFrom(s => s.IndustryProfileReport.Id))
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.Name))
        .ForMember(d => d.MemberBranchId, m => m.MapFrom(s => s.MemberBranch.Id))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => s.MemberBranch.Name))
        .ForMember(d => d.IpAdditionalGroupId, m => m.MapFrom(s => s.IpAdditionalGroup.Id))
        .ForMember(d => d.IpAdditionalGroup, m => m.MapFrom(s => s.IpAdditionalGroup.GroupName))
        .ForMember(d => d.IpAdditionalItemId, m => m.MapFrom(s => s.IpAdditionalItem.Id))
        .ForMember(d => d.IpAdditionalItem, m => m.MapFrom(s => s.IpAdditionalItem.ItemName))
        .ForMember(d => d.Description, m => m.MapFrom(s => s.IpAdditionalItem.Description));

      this.CreateMap<MemberIpAdditionalItemModel, MemberIpAdditionalItem>()
        .ForMember(d => d.IndustryProfileReport, m => m.MapFrom(s => new IndustryProfileReport { Id = s.IndustryProfileReportId }))
        .ForMember(d => d.Member, m => m.MapFrom(s => new Member { Id = s.MemberId }))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => new MemberBranch { Id = s.MemberBranchId }))
        .ForMember(d => d.IpAdditionalGroup, m => m.MapFrom(s => new IpAdditionalGroup { Id = s.IpAdditionalGroupId }))
        .ForMember(d => d.IpAdditionalItem, m => m.MapFrom(s => new IpAdditionalItem { Id = s.IpAdditionalItemId }));

      this.CreateMap<MemberIpAdminItem, MemberIpAdminItemModel>()
        .ForMember(d => d.IndustryProfileReportId, m => m.MapFrom(s => s.IndustryProfileReport.Id))
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.Name))
        .ForMember(d => d.MemberBranchId, m => m.MapFrom(s => s.MemberBranch.Id))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => s.MemberBranch.Name))
        .ForMember(d => d.IpAdminGroupId, m => m.MapFrom(s => s.IpAdminGroup.Id))
        .ForMember(d => d.IpAdminGroup, m => m.MapFrom(s => s.IpAdminGroup.GroupName))
        .ForMember(d => d.IpAdminItemId, m => m.MapFrom(s => s.IpAdminItem.Id))
        .ForMember(d => d.IpAdminItem, m => m.MapFrom(s => s.IpAdminItem.ItemName))
        .ForMember(d => d.Description, m => m.MapFrom(s => s.IpAdminItem.Description));

      this.CreateMap<MemberIpAdminItemModel, MemberIpAdminItem>()
        .ForMember(d => d.IndustryProfileReport, m => m.MapFrom(s => new IndustryProfileReport { Id = s.IndustryProfileReportId }))
        .ForMember(d => d.Member, m => m.MapFrom(s => new Member { Id = s.MemberId }))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => new MemberBranch { Id = s.MemberBranchId }))
        .ForMember(d => d.IpAdminGroup, m => m.MapFrom(s => new IpAdminGroup { Id = s.IpAdminGroupId }))
        .ForMember(d => d.IpAdminItem, m => m.MapFrom(s => new IpAdminItem { Id = s.IpAdminItemId }));

      this.CreateMap<MemberIpBaseItem, MemberIpBaseItemModel>()
        .ForMember(d => d.IndustryProfileReportId, m => m.MapFrom(s => s.IndustryProfileReport.Id))
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.Name))
        .ForMember(d => d.MemberBranchId, m => m.MapFrom(s => s.MemberBranch.Id))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => s.MemberBranch.Name))
        .ForMember(d => d.IpBaseGroupId, m => m.MapFrom(s => s.IpBaseGroup.Id))
        .ForMember(d => d.IpBaseGroup, m => m.MapFrom(s => s.IpBaseGroup.GroupName))
        .ForMember(d => d.IpBaseItemId, m => m.MapFrom(s => s.IpBaseItem.Id))
        .ForMember(d => d.IpBaseItem, m => m.MapFrom(s => s.IpBaseItem.ItemName))
        .ForMember(d => d.Description, m => m.MapFrom(s => s.IpBaseItem.Description));

      this.CreateMap<MemberIpBaseItemModel, MemberIpBaseItem>()
        .ForMember(d => d.IndustryProfileReport,
          m => m.MapFrom(s => new IndustryProfileReport { Id = s.IndustryProfileReportId }))
        .ForMember(d => d.Member, m => m.MapFrom(s => new Member { Id = s.MemberId }))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => new MemberBranch { Id = s.MemberBranchId }))
        .ForMember(d => d.IpBaseGroup, m => m.MapFrom(s => new IpBaseGroup { Id = s.IpBaseGroupId }))
        .ForMember(d => d.IpBaseItem, m => m.MapFrom(s => new IpBaseItem { Id = s.IpBaseItemId }));

      this.CreateMap<MemberPcbCostItem, MemberPcbCostItemModel>()
        .ForMember(d => d.IndustryProfileReportId, m => m.MapFrom(s => s.IndustryProfileReport.Id))
        .ForMember(d => d.MemberTypeId, m => m.MapFrom(s => s.IndustryProfileReport.MemberType.Id))
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.Name))
        .ForMember(d => d.MemberBranchId, m => m.MapFrom(s => s.MemberBranch.Id))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => s.MemberBranch.Name))
        .ForMember(d => d.PcbCostMainGroupId, m => m.MapFrom(s => s.PcbCostMainGroup.Id))
        .ForMember(d => d.PcbCostMainGroup, m => m.MapFrom(s => s.PcbCostMainGroup.GroupName))
        .ForMember(d => d.PcbCostItemId, m => m.MapFrom(s => s.PcbCostItem.Id))
        .ForMember(d => d.PcbCostItem, m => m.MapFrom(s => s.PcbCostItem.ItemDescription))
        .ForMember(d => d.PcbCostItemGroupId, m => m.MapFrom(s => s.PcbCostItem.PcbCostItemGroup.Id));

      this.CreateMap<MemberPcbCostItemModel, MemberPcbCostItem>()
        .ForMember(d => d.IndustryProfileReport, m => m.MapFrom(s => new IndustryProfileReport { Id = s.IndustryProfileReportId }))
        .ForMember(d => d.Member, m => m.MapFrom(s => new Member { Id = s.MemberId }))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => new MemberBranch { Id = s.MemberBranchId }))
        .ForMember(d => d.PcbCostMainGroup, m => m.MapFrom(s => new PcbCostMainGroup { Id = s.PcbCostMainGroupId }))
        .ForMember(d => d.PcbCostItem, m => m.MapFrom(s => new PcbCostItem { Id = s.PcbCostItemId }));

      this.CreateMap<MemberRunningCostFleet, MemberRunningCostFleetModel>()
        .ForMember(d => d.IndustryProfileReportId, m => m.MapFrom(s => s.IndustryProfileReport.Id))
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.Name))
        .ForMember(d => d.MemberBranchId, m => m.MapFrom(s => s.MemberBranch.Id))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => s.MemberBranch.Name))
        .ForMember(d => d.RunningCostRentedFleetId, m => m.MapFrom(s => s.RunningCostRentedFleet.Id))
        .ForMember(d => d.RunningCostRentedFleetCode, m => m.MapFrom(s => s.RunningCostRentedFleet.ItemCode))
        .ForMember(d => d.RunningCostRentedFleet, m => m.MapFrom(s => s.RunningCostRentedFleet.ItemName))
        .ForMember(d => d.Note, m => m.MapFrom(s => s.RunningCostRentedFleet.Note));

      this.CreateMap<MemberRunningCostFleetModel, MemberRunningCostFleet>()
        .ForMember(d => d.IndustryProfileReport,
          m => m.MapFrom(s => new IndustryProfileReport { Id = s.IndustryProfileReportId }))
        .ForMember(d => d.Member, m => m.MapFrom(s => new Member { Id = s.MemberId }))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => new MemberBranch { Id = s.MemberBranchId }))
        .ForMember(d => d.RunningCostRentedFleet,
          m => m.MapFrom(s => new RunningCostRentedFleet { Id = s.RunningCostRentedFleetId }));

      this.CreateMap<MemberRunningCost, MemberRunningCostModel>()
        .ForMember(d => d.IndustryProfileReportId, m => m.MapFrom(s => s.IndustryProfileReport.Id))
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.Name))
        .ForMember(d => d.MemberBranchId, m => m.MapFrom(s => s.MemberBranch.Id))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => s.MemberBranch.Name))
        .ForMember(d => d.RunningCostGroupId, m => m.MapFrom(s => s.RunningCostGroup.Id))
        .ForMember(d => d.RunningCostGroup, m => m.MapFrom(s => s.RunningCostGroup.GroupName))
        .ForMember(d => d.RunningCostId, m => m.MapFrom(s => s.RunningCost.Id))
        .ForMember(d => d.RunningCost, m => m.MapFrom(s => s.RunningCost.ItemName));


      this.CreateMap<MemberRunningCostModel, MemberRunningCost>()
        .ForMember(d => d.IndustryProfileReport,
          m => m.MapFrom(s => new IndustryProfileReport { Id = s.IndustryProfileReportId }))
        .ForMember(d => d.Member, m => m.MapFrom(s => new Member { Id = s.MemberId }))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => new MemberBranch { Id = s.MemberBranchId }))
        .ForMember(d => d.RunningCostGroup, m => m.MapFrom(s => new RunningCostGroup { Id = s.RunningCostGroupId }))
        .ForMember(d => d.RunningCost,
          m => m.MapFrom(s => s.RunningCostId == null ? null : new RunningCost { Id = (int)s.RunningCostId }));


      this.CreateMap<MemberRunningCostSundry, MemberRunningCostSundryModel>()
        .ForMember(d => d.IndustryProfileReportId, m => m.MapFrom(s => s.IndustryProfileReport.Id))
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.Name))
        .ForMember(d => d.MemberBranchId, m => m.MapFrom(s => s.MemberBranch.Id))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => s.MemberBranch.Name))
        .ForMember(d => d.RunningCostSundryExpenseId, m => m.MapFrom(s => s.RunningCostSundryExpense.Id))
        .ForMember(d => d.RunningCostSundryExpense, m => m.MapFrom(s => s.RunningCostSundryExpense.ItemName))
        .ForMember(d => d.ProductionEnabled, m => m.MapFrom(s => s.RunningCostSundryExpense.ProductionEnabled))
        .ForMember(d => d.MarketingEnabled, m => m.MapFrom(s => s.RunningCostSundryExpense.MarketingEnabled))
        .ForMember(d => d.AdministrationEnabled, m => m.MapFrom(s => s.RunningCostSundryExpense.AdministrationEnabled))
        .ForMember(d => d.RoadTransportEnabled, m => m.MapFrom(s => s.RunningCostSundryExpense.RoadTransportEnabled));


      this.CreateMap<MemberRunningCostSundryModel, MemberRunningCostSundry>()
        .ForMember(d => d.IndustryProfileReport,
          m => m.MapFrom(s => new IndustryProfileReport { Id = s.IndustryProfileReportId }))
        .ForMember(d => d.Member, m => m.MapFrom(s => new Member { Id = s.MemberId }))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => new MemberBranch { Id = s.MemberBranchId }))
        .ForMember(d => d.RunningCostSundryExpense,
          m => m.MapFrom(s => new RunningCostSundryExpense { Id = s.RunningCostSundryExpenseId }));

      this.CreateMap<MemberTotalRawMaterialCost, MemberTotalRawMaterialCostModel>()
        .ForMember(d => d.IndustryProfileReportId, m => m.MapFrom(s => s.IndustryProfileReport.Id))
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.Name))
        .ForMember(d => d.MemberBranchId, m => m.MapFrom(s => s.MemberBranch.Id))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => s.MemberBranch.Name))
        .ForMember(d => d.RawMaterialTotalCostId, m => m.MapFrom(s => s.RawMaterialTotalCost.Id))
        .ForMember(d => d.RawMaterialTotalCost, m => m.MapFrom(s => s.RawMaterialTotalCost.ItemName));


      this.CreateMap<MemberTotalRawMaterialCostModel, MemberTotalRawMaterialCost>()
        .ForMember(d => d.IndustryProfileReport,
          m => m.MapFrom(s => new IndustryProfileReport { Id = s.IndustryProfileReportId }))
        .ForMember(d => d.Member, m => m.MapFrom(s => new Member { Id = s.MemberId }))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => new MemberBranch { Id = s.MemberBranchId }))
        .ForMember(d => d.RawMaterialTotalCost,
          m => m.MapFrom(s => new RawMaterialTotalCost { Id = s.RawMaterialTotalCostId }));

      this.CreateMap<IndustryCostGroup, IndustryCostGroupModel>().ReverseMap();

      this.CreateMap<IndustryGroupItem, IndustryGroupItemModel>()
        .ForMember(d => d.IndustryCostGroupId, m => m.MapFrom(s => s.IndustryCostGroup.Id));

      this.CreateMap<IndustryGroupItemModel, IndustryGroupItem>()
        .ForMember(d => d.IndustryCostGroup,
          m => m.MapFrom(s => new IndustryCostGroup { Id = s.IndustryCostGroupId }));

      #endregion
    }
  }
}

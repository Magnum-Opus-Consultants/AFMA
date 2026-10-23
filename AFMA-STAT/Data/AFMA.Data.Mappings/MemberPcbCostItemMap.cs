namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class MemberPcbCostItemMap : ClassMap<MemberPcbCostItem>
  {
    public MemberPcbCostItemMap()
    {
      Table("MemberPcbCostItem");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.IndustryProfileReport, "IndustryProfileReportId").Cascade.Delete().Not.Nullable();
      References(x => x.Member, "MemberId").Cascade.Delete().Not.Nullable();
      References(x => x.MemberBranch, "MemberBranchId").Cascade.Delete().Not.Nullable();
      References(x => x.PcbCostItem, "ItemId").Cascade.Delete().Not.Nullable();
      References(x => x.PcbCostMainGroup, "MainGroupId").Cascade.Delete().Not.Nullable();
      Map(x => x.Value).Not.Nullable();
      Map(x => x.ItemDescription);
      Map(x => x.Active).Not.Nullable();
      Map(x => x.DisplayOrder).Not.Nullable();
      Map(x => x.Readonly).Not.Nullable();
      Map(x => x.AddToTotal).Not.Nullable();
      Map(x => x.DisplayValue).Not.Nullable();
    }
  }
}

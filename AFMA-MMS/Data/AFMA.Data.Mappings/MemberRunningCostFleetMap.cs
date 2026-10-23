namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class MemberRunningCostFleetMap : ClassMap<MemberRunningCostFleet>
  {
    public MemberRunningCostFleetMap()
    {
      Table("MemberRunningCostFleet");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.IndustryProfileReport, "IndustryProfileReportId").Cascade.Delete().Not.Nullable();
      References(x => x.Member, "MemberId").Cascade.Delete().Not.Nullable();
      References(x => x.MemberBranch, "MemberBranchId").Cascade.Delete().Not.Nullable();
      References(x => x.RunningCostRentedFleet, "ItemId").Cascade.Delete().Not.Nullable();
      Map(x => x.Value).Not.Nullable();
      Map(x => x.ItemDescription);
      Map(x => x.Active).Not.Nullable();
      Map(x => x.DisplayOrder).Not.Nullable();
    }
  }
}

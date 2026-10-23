namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class MemberRunningCostMap : ClassMap<MemberRunningCost>
  {
    public MemberRunningCostMap()
    {
      Table("MemberRunningCost");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.IndustryProfileReport, "IndustryProfileReportId").Cascade.None().Not.Nullable();
      References(x => x.Member, "MemberId").Cascade.None().Not.Nullable();
      References(x => x.MemberBranch, "MemberBranchId").Cascade.None().Not.Nullable();
      References(x => x.RunningCostGroup, "GroupId").Cascade.None().Not.Nullable();
      References(x => x.RunningCost, "ItemId").Cascade.None();
      Map(x => x.ProductionValue).Not.Nullable();
      Map(x => x.SalesValue).Not.Nullable();
      Map(x => x.AdministrationValue).Not.Nullable();
      Map(x => x.RoadTransportValue).Not.Nullable();
      Map(x => x.ItemDescription);
      Map(x => x.Active).Not.Nullable();
      Map(x => x.DisplayOrder).Not.Nullable();
      Map(x => x.CalculationType).Not.Nullable();
      Map(x => x.ReportYear).Not.Nullable();
    }
  }
}

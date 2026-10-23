namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class MemberRunningCostSundryMap : ClassMap<MemberRunningCostSundry>
  {
    public MemberRunningCostSundryMap()
    {
      Table("MemberRunningCostSundry");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.IndustryProfileReport, "IndustryProfileReportId").Cascade.Delete().Not.Nullable();
      References(x => x.Member, "MemberId").Cascade.Delete().Not.Nullable();
      References(x => x.MemberBranch, "MemberBranchId").Cascade.Delete().Not.Nullable();
      References(x => x.RunningCostSundryExpense, "ItemId").Cascade.Delete().Not.Nullable();
      Map(x => x.ProductionValue).Not.Nullable();
      Map(x => x.SalesValue).Not.Nullable();
      Map(x => x.AdministrationValue).Not.Nullable();
      Map(x => x.RoadTransportValue).Not.Nullable();
      Map(x => x.ItemDescription);
      Map(x => x.Active).Not.Nullable();
      Map(x => x.DisplayOrder).Not.Nullable();
      Map(x => x.ReportYear).Not.Nullable();
    }
  }
}

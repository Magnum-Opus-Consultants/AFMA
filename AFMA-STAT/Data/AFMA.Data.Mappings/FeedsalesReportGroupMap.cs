namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class FeedsalesReportGroupMap : ClassMap<FeedsalesReportGroup>
  {
    public FeedsalesReportGroupMap()
    {
      Table("FeedsalesReportGroup");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Name).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
      References(x => x.FeedsalesReportGroupCategory, "FeedsalesReportGroupCategoryId").Cascade.None();
      Map(x => x.ReportDisplayGroup);
    }
  }
}

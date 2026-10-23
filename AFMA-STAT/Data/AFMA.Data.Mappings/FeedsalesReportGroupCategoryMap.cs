namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class FeedsalesReportGroupCategoryMap : ClassMap<FeedsalesReportGroupCategory>
  {
    public FeedsalesReportGroupCategoryMap()
    {
      Table("FeedsalesReportGroupCategory");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Name).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
    }
  }
}

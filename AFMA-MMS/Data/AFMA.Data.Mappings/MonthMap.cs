namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class MonthMap : ClassMap<Month>
  {
    public MonthMap()
    {
      Table("Month");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.MonthNumber).Not.Nullable();
      Map(x => x.MonthName).Not.Nullable();
      Map(x => x.Season).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
    }
  }
}

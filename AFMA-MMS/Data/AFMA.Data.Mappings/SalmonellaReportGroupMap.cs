namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class SalmonellaReportGroupMap : ClassMap<SalmonellaReportGroup>
  {
    public SalmonellaReportGroupMap()
    {
      Table("SalmonellaReportGroup");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Name).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
    }
  }
}

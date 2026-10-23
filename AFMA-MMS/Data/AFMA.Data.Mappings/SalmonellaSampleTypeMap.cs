namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class SalmonellaSampleTypeMap : ClassMap<SalmonellaSampleType>
  {
    public SalmonellaSampleTypeMap()
    {
      Table("SalmonellaSampleType");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.ShortCode).Not.Nullable();
      Map(x => x.Name).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
    }
  }
}

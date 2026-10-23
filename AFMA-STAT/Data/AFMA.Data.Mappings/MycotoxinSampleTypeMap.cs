namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class MycotoxinSampleTypeMap : ClassMap<MycotoxinSampleType>
  {
    public MycotoxinSampleTypeMap()
    {
      Table("MycotoxinSampleType");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Name).Not.Nullable();
    }
  }
}

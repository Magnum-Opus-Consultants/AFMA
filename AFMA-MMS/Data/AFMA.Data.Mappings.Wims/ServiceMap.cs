namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class ServiceMap : ClassMap<Service>
  {
    public ServiceMap()
    {
      Table("Service");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Description).Not.Nullable();
    }
  }
}

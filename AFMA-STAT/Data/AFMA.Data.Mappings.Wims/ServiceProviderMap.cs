namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class ServiceProviderMap : ClassMap<ServiceProvider>
  {
    public ServiceProviderMap()
    {
      Table("ServiceProvider");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Description).Not.Nullable();

    }
  }
}

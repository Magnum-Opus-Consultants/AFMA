namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class AnimalFeedProductMap : ClassMap<AnimalFeedProduct>
  {
    public AnimalFeedProductMap()
    {
      Table("AnimalFeedProduct");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Description).Not.Nullable();
    }
  }
}

namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;


  public class ProvinceMap : ClassMap<Province>
  {
    public ProvinceMap()
    {
      Table("Province");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Description).Not.Nullable();
    }
  }
}

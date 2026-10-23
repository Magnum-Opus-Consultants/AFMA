namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class LaboratoryMap : ClassMap<Laboratory>
  {
    public LaboratoryMap()
    {
      Table("Laboratory");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Name).Not.Nullable();
      Map(x => x.Address).CustomSqlType("varchar(MAX)");
      Map(x => x.Active).Not.Nullable();
    }
  }
}

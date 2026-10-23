namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class SupplierMap : ClassMap<Supplier>
  {
    public SupplierMap()
    {
      Table("Supplier");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Description).Not.Nullable();
    }
  }
}

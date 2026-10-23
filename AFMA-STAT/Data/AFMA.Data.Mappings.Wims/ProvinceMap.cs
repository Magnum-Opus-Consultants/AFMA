namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class ProvinceMap : ClassMap<Province>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ProvinceMap"/> class.
    /// </summary>
    public ProvinceMap()
    {
      Table("Province");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Description).Not.Nullable();
    }
  }
}

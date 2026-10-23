namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class MemberTypeMap : ClassMap<MemberType>
  {
    public MemberTypeMap()
    {
      Table("MemberType");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Description).Not.Nullable();
    }
  }
}

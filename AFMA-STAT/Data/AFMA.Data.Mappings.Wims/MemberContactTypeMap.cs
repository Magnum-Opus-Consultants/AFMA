namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class MemberContactTypeMap : ClassMap<MemberContactType>
  {
    public MemberContactTypeMap()
    {
      Table("MemberContactType");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Description).Not.Nullable();
    }
  }
}

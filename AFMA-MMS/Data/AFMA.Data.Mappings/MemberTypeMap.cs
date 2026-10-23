namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class MemberTypeMap : ClassMap<MemberType>
  {
    public MemberTypeMap()
    {
      Table("MemberType");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Description).Not.Nullable();
      Map(x => x.Deletable).Not.Nullable();
    }
  }
}

namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  /// <summary>
  /// The member module mapping class.
  /// </summary>
  public class MemberModuleMap : ClassMap<MemberModule>
  {
    public MemberModuleMap()
    {
      Table("MemberModule");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.Member, "MemberId").Not.Nullable().Cascade.None();
      References(x => x.Module, "ModuleId").Not.Nullable().Cascade.None();
    }
  }
}

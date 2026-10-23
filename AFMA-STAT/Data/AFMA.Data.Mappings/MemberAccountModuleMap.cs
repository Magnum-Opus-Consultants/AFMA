namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  /// <summary>
  /// The member mapping class.
  /// </summary>
  public class MemberAccountModuleMap : ClassMap<MemberAccountModule>
  {
    public MemberAccountModuleMap()
    {
      Table("MemberAccountModule");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.MemberAccount, "MemberAccountId").Cascade.None();
      References(x => x.Module, "ModuleId").Cascade.None();
    }
  }
}

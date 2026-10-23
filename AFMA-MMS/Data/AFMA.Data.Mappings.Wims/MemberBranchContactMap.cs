namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class MemberBranchContactMap : ClassMap<MemberBranchContact>
  {
    public MemberBranchContactMap()
    {
      Table("MemberBranchContact");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.Member, "MemberId").Not.Nullable().Cascade.None();
      References(x => x.MemberBranch, "MemberBranchId").Not.Nullable().Cascade.None();
      References(x => x.MemberContactType, "MemberContactTypeId").Not.Nullable().Cascade.None();
      Map(x => x.Title).Length(50).Not.Nullable();
      Map(x => x.FirstName).Length(150).Not.Nullable();
      Map(x => x.Surname).Length(150).Not.Nullable();
      Map(x => x.TelephoneNo).Length(50).Not.Nullable();
      Map(x => x.MobileNo).Length(50);
      Map(x => x.EmailAddress).Length(150).Not.Nullable();
      Map(x => x.CompanyContact);
      Map(x => x.Designation).Length(200).Not.Nullable();
    }
  }
}

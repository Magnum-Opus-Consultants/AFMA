namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class MemberBranchMap : ClassMap<MemberBranch>
  {
    public MemberBranchMap()
    {
      Table("MemberBranch");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.Member, "MemberId").Not.Nullable().Cascade.None();
      References(x => x.Province, "ProvinceId").Not.Nullable().Cascade.None();
      Map(x => x.MemberIdentificationId).Length(50);
      Map(x => x.BranchName).Not.Nullable().Length(150);
      Map(x => x.PhysicalAddress).Not.Nullable();
      Map(x => x.PostalAddress).Not.Nullable();
      Map(x => x.TelephoneNumber).Not.Nullable().Length(150);
      Map(x => x.DateOfMembership);
      Map(x => x.DateOfIssue);
      Map(x => x.OtherMemberIdentificationId).Length(50);
      Map(x => x.Comments);
      Map(x => x.GpsCoordinates).Length(255);
      HasMany(x => x.AnimalFeedProducts).KeyColumn("MemberBranchId").Cascade.None().Inverse();
      HasMany(x => x.ServiceProviders).KeyColumn("MemberBranchId").Cascade.None().Inverse();
      HasMany(x => x.Contacts).KeyColumn("MemberBranchId").Cascade.None().Inverse();
    }
  }
}

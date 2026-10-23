namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class MemberMap : ClassMap<Member>
  {
    public MemberMap()
    {
      Table("Member");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.Province, "ProvinceId").Not.Nullable().Cascade.None();
      References(x => x.MemberType, "MemberTypeId").Not.Nullable().Cascade.None();
      Map(x => x.MemberIdentificationId).Not.Nullable().Length(50);
      Map(x => x.MemberTradeName).Length(255);
      Map(x => x.MemberLegalName).Not.Nullable().Length(255);
      Map(x => x.MemberOtherName).Length(255);
      Map(x => x.Branch).Length(50);
      Map(x => x.RegistrationNo).Length(50);
      Map(x => x.VatRegistrationNo).Length(50);
      Map(x => x.PhysicalAddress).Not.Nullable();
      Map(x => x.PostalAddress).Not.Nullable();
      Map(x => x.TelephoneNo).Not.Nullable().Length(20);
      Map(x => x.Website).Length(255);
      Map(x => x.Active).Not.Nullable();
      Map(x => x.GpsCoordinates).Length(255);
      HasMany(x => x.Branches).KeyColumn("MemberId").Cascade.Delete().Inverse();
      HasMany(x => x.Committees).KeyColumn("MemberId").Cascade.Delete().Inverse();
      HasMany(x => x.Documents).KeyColumn("MemberId").Cascade.Delete().Inverse();
      HasMany(x => x.Users).KeyColumn("MemberId").Cascade.Delete().Inverse();
    }
  }
}

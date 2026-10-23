namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class MemberDocumentMap : ClassMap<MemberDocument>
  {
    public MemberDocumentMap()
    {
      Table("MemberDocument");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Name).Not.Nullable();
      References(x => x.DocumentType, "DocumentTypeId").Cascade.None().Not.Nullable();
      References(x => x.Member, "MemberId").Cascade.None().Not.Nullable();
      Map(x => x.Visible).Not.Nullable();
      Map(x => x.ExpiryDate).Nullable();
      Map(x => x.LastNotificationSent).Nullable();
      Map(x => x.DocumentContent).CustomSqlType("varbinary(MAX)").Length(int.MaxValue).Not.Nullable();
      Map(x => x.DocumentContentType).Not.Nullable();
      Map(x => x.DocumentFileName).Not.Nullable();
      Map(x => x.DocumentFileSize).Not.Nullable();
    }
  }
}

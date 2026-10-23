namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  /// <summary>
  /// The committee document mapping class.
  /// </summary>
  public class CommitteeDocumentMap : ClassMap<CommitteeDocument>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CommitteeDocumentMap"/> class.
    /// </summary>
    public CommitteeDocumentMap()
    {
      Table("CommitteeDocument");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.Committee, "CommitteeId").Cascade.None().Not.Nullable();
      References(x => x.DocumentType, "DocumentTypeId").Cascade.None().Not.Nullable();
      Map(x => x.Name).Not.Nullable();
      Map(x => x.Description).Nullable();
      Map(x => x.DocumentContent).CustomSqlType("varbinary(MAX)").Length(int.MaxValue).Not.Nullable();
      Map(x => x.DocumentContentType).Not.Nullable();
      Map(x => x.DocumentFileName).Not.Nullable();
      Map(x => x.DocumentFileSize).Not.Nullable();
    }
  }
}

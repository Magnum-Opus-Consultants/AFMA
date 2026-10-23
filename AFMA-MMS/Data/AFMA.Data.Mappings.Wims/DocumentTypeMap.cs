namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  /// <summary>
  /// The Document Type mapping class.
  /// </summary>
  public class DocumentTypeMap : ClassMap<DocumentType>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentTypeMap"/> class.
    /// </summary>
    public DocumentTypeMap()
    {
      Table("DocumentType");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Description).Not.Nullable();
      Map(x => x.RequireTemplate).Not.Nullable();
      References(x => x.DocumentTemplate, "DocumentTemplateId").Cascade.None();
    }
  }
}

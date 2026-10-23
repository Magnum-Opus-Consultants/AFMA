namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  /// <summary>
  /// The Document Template mapping class.
  /// </summary>
  public class DocumentTemplateMap : ClassMap<DocumentTemplate>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentTemplateMap"/> class.
    /// </summary>
    public DocumentTemplateMap()
    {
      Table("DocumentTemplate");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Name).Not.Nullable();
      Map(x => x.TemplateContent).Not.Nullable().CustomSqlType("varbinary(MAX)").Length(int.MaxValue);
      Map(x => x.TemplateContentType).Not.Nullable();
      Map(x => x.TemplateFileName).Not.Nullable();
      Map(x => x.TemplateFileSize).Not.Nullable();
    }
  }
}

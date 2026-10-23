namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class BatchAttachmentMap : ClassMap<BatchAttachment>
  {
    public BatchAttachmentMap()
    {
      Table("BatchAttachment");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.Batch, "BatchId").Cascade.None();
      Map(x => x.AttachmentFileName).Not.Nullable();
      Map(x => x.AttachmentContentType).Not.Nullable();
      Map(x => x.Attachment).Not.Nullable().CustomSqlType("varbinary(MAX)").Length(int.MaxValue);
      Map(x => x.AttachmentFileSize).Not.Nullable();
    }
  }
}

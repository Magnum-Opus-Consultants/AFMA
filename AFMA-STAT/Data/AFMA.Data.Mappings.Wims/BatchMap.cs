namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class BatchMap : ClassMap<Batch>
  {
    public BatchMap()
    {
      Table("Batch");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.DistributionGroup, "DistributionGroupId").Cascade.None().Not.Nullable();
      Map(x => x.Message).Not.Nullable().CustomSqlType("varchar(MAX)").CustomType("StringClob");
      Map(x => x.UserId).Not.Nullable();
      Map(x => x.Description).Not.Nullable();
      Map(x => x.BatchType).Not.Nullable();
      Map(x => x.Status).Not.Nullable();
      Map(x => x.MessageCount).Not.Nullable();
      Map(x => x.SubmissionDate);
      Map(x => x.CreatedDate).Not.Nullable();
      Map(x => x.FromName).Not.Nullable();
      Map(x => x.FromEmail).Not.Nullable();
      Map(x => x.Subject).Not.Nullable();
      Map(x => x.RecipientCount).Not.Nullable();
      HasMany(x => x.Recipients).KeyColumn("BatchId").Cascade.None().Inverse();
      HasMany(x => x.Attachments).KeyColumn("BatchId").Cascade.None().Inverse();
    }
  }
}

namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class BatchRecipientMap : ClassMap<BatchRecipient>
  {
    public BatchRecipientMap()
    {
      Table("BatchRecipient");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.Batch, "BatchId").Cascade.None();
      Map(x => x.FromEmail).Not.Nullable();
      Map(x => x.FromName).Not.Nullable();
      Map(x => x.ToEmail).Not.Nullable();
      Map(x => x.ToName).Not.Nullable();
    }
  }
}

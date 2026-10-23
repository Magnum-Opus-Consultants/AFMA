namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class SalmonellaSampleTypeSubItemMap : ClassMap<SalmonellaSampleTypeSubItem>
  {
    public SalmonellaSampleTypeSubItemMap()
    {
      Table("SalmonellaSampleTypeSubItem");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.SalmonellaSampleType, "SalmonellaSampleTypeId").Cascade.None().Not.Nullable();
      Map(x => x.Name).Not.Nullable();
      Map(x => x.ShortCode).Not.Nullable();
      Map(x => x.DisplayOrder).Not.Nullable();
      Map(x => x.VisibleToMembers).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
    }
  }
}

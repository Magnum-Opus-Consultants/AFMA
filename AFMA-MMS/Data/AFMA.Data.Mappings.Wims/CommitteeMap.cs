namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  /// <summary>
  /// The committee mapping class.
  /// </summary>
  public class CommitteeMap : ClassMap<Committee>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CommitteeMap"/> class.
    /// </summary>
    public CommitteeMap()
    {
      Table("Committee");
      Id(x => x.Id).GeneratedBy.Identity().Not.Nullable();
      Map(x => x.Name).Not.Nullable();
      HasMany(x => x.Members).KeyColumn("CommitteeId").Inverse().Cascade.Delete().LazyLoad();
      HasMany(x => x.Documents).KeyColumn("CommitteeId").Inverse().Cascade.Delete().LazyLoad();
    }
  }
}

namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class MycotoxinAnalysisMap : ClassMap<MycotoxinAnalysis>
  {
    public MycotoxinAnalysisMap()
    {
      Table("MycotoxinAnalysis");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Name).Not.Nullable();
      Map(x => x.Other).Not.Nullable();
    }
  }
}

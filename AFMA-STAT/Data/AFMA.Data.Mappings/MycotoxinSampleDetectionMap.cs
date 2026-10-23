namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class MycotoxinSampleDetectionMap : ClassMap<MycotoxinSampleDetection>
  {
    public MycotoxinSampleDetectionMap()
    {
      Table("MycotoxinSampleDetection");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.MycotoxinSample, "MycotoxinSampleId").Cascade.None().Not.Nullable();
      References(x => x.MycotoxinClass, "MycotoxinClassId").Cascade.None().Not.Nullable();
      References(x => x.MycotoxinType, "MycotoxinTypeId").Cascade.None().Not.Nullable();
      Map(x => x.Concentration);
      Map(x => x.Lod);
    }
  }
}

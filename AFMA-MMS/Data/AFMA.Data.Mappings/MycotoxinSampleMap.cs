namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class MycotoxinSampleMap : ClassMap<MycotoxinSample>
  {
    public MycotoxinSampleMap()
    {
      Table("MycotoxinSample");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.MycotoxinReport, "MycotoxinReportId").Not.Nullable().Cascade.None();
      References(x => x.MycotoxinSampleType, "MycotoxinSampleTypeId").Not.Nullable().Cascade.None();
      References(x => x.Product, "ProductId").Cascade.None();
      References(x => x.Material, "MaterialId").Cascade.None();
      References(x => x.MaterialOrigin, "MaterialOriginId").Cascade.None();
      References(x => x.Province, "ProvinceId").Cascade.None();
      Map(x => x.MaterialCountryOfOrigin);
      Map(x => x.Note).CustomSqlType("varchar(MAX)").CustomType("StringClob");
      References(x => x.MycotoxinAnalysis, "MycotoxinAnalysisId").Not.Nullable().Cascade.None();
      Map(x => x.MycotoxinAnalysisOther);
      Map(x => x.MycotoxinDetected).Not.Nullable();
      HasMany(x => x.Detections).KeyColumn("MycotoxinSampleId").Cascade.Delete().Inverse();
    }
  }
}

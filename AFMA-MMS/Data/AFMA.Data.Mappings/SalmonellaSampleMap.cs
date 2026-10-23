namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class SalmonellaSampleMap : ClassMap<SalmonellaSample>
  {
    public SalmonellaSampleMap()
    {
      Table("SalmonellaSample");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.SalmonellaReport, "SalmonellaReportId").Cascade.None().Not.Nullable();
      References(x => x.SalmonellaSampleType, "SalmonellaSampleTypeId").Cascade.None().Not.Nullable();
      Map(x => x.SalmonellaDetected).Not.Nullable();
      References(x => x.Product, "ProductId").Cascade.None();
      References(x => x.Material, "MaterialId").Cascade.None();
      Map(x => x.AdditionalInfo);
      Map(x => x.Note);
      References(x => x.SalmonellaSampleTypeItem, "SalmonellaSampleTypeItemId").Cascade.None();
      References(x => x.SalmonellaSampleTypeSubItem, "SalmonellaSampleTypeSubItemId").Cascade.None();
      References(x => x.SalmonellaGroup, "SalmonellaGroupId").Cascade.None();
      References(x => x.SalmonellaType, "SalmonellaTypeId").Cascade.None();
      References(x => x.ProductGroup, "ProductGroupId").Cascade.None();
    }
  }
}

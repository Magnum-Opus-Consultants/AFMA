namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class MaterialMap : ClassMap<Material>
  {
    public MaterialMap()
    {
      Table("Material");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Name).Not.Nullable();
      Map(x => x.DisplayOrder).Not.Nullable();
      References(x => x.MaterialCategory, "MaterialCategoryId").Cascade.None();
      Map(x => x.Code).Not.Nullable();
      Map(x => x.Active).Not.Nullable().Not.Nullable();
      References(x => x.SalmonellaReportGroup, "SalmonellaReportGroupId").Cascade.None();
      References(x => x.FeedsalesReportGroup, "FeedsalesReportGroupId").Cascade.None();
      Map(x => x.HideOnFeedsales).Not.Nullable();
      Map(x => x.HideOnSalmonella).Not.Nullable();
      Map(x => x.VisibleToMembers).Not.Nullable();
      Map(x => x.Other).Not.Nullable();
      Map(x => x.AllowDuplicates).Not.Nullable();
      Map(x => x.ShowAlert).Not.Nullable();
      Map(x => x.AlertMessage).CustomSqlType("varchar(MAX)");
    }
  }
}

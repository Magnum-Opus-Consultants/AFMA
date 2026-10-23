namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class FeedsalesReportItemMap : ClassMap<FeedsalesReportItem>
  {
    public FeedsalesReportItemMap()
    {
      Table("FeedsalesReportItem");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.FeedsalesReport, "FeedsalesReportId").Cascade.None().Not.Nullable();
      References(x => x.Product, "ProductId").Cascade.None();
      References(x => x.Material, "MaterialId").Cascade.None();
      Map(x => x.Value).Not.Nullable();
      References(x => x.Member, "MemberId").Cascade.None().Not.Nullable();
      References(x => x.MemberBranch, "MemberBranchId").Cascade.None().Not.Nullable();
      References(x => x.MemberAccount, "MemberAccountId").Cascade.None();
      Map(x => x.Other).CustomSqlType("varchar(MAX)");
    }
  }

  public class FeedsalesReportItemLookupMap : ClassMap<FeedsalesReportItemLookup>
  {
    public FeedsalesReportItemLookupMap()
    {
      Table("FeedsalesReportItem");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.FeedsalesReport, "FeedsalesReportId").Cascade.None().Not.Nullable();
      References(x => x.Product, "ProductId").Cascade.None();
      References(x => x.Material, "MaterialId").Cascade.None();
      Map(x => x.Value).Not.Nullable();
      Map(x => x.Other).CustomSqlType("varchar(MAX)");
    }
  }
}

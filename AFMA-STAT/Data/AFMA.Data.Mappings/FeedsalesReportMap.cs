namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class FeedsalesReportMap : ClassMap<FeedsalesReport>
  {
    public FeedsalesReportMap()
    {
      Table("FeedsalesReport");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.IsSubmitted).Not.Nullable();
      Map(x => x.SubmittedDate);
      References(x => x.Member, "MemberId").Cascade.None().Not.Nullable();
      References(x => x.MemberBranch, "MemberBranchId").Cascade.None().Not.Nullable();
      References(x => x.MemberAccount, "MemberAccountId").Cascade.None();
      Map(x => x.CreatedBy).Not.Nullable();
      Map(x => x.CreatedDate).Not.Nullable();
      Map(x => x.DueDate).Not.Nullable();
      Map(x => x.InfoFor);
      Map(x => x.ReportYear);
      Map(x => x.ReportMonth);
      Map(x => x.Comment).CustomSqlType("varchar(MAX)");
      Map(x => x.DifferenceReason).CustomSqlType("varchar(MAX)").CustomType("StringClob");
      HasMany(x => x.ReportItems).KeyColumn("FeedsalesReportId").Cascade.Delete().Inverse();
    }
  }

  public class FeedsalesReportLookupMap : ClassMap<FeedsalesReportLookup>
  {
    public FeedsalesReportLookupMap()
    {
      Table("FeedsalesReport");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.IsSubmitted).Not.Nullable();
      Map(x => x.SubmittedDate);
      References(x => x.Member, "MemberId").Cascade.None().Not.Nullable();
      References(x => x.MemberBranch, "MemberBranchId").Cascade.None().Not.Nullable();
      References(x => x.MemberAccount, "MemberAccountId").Cascade.None();
      Map(x => x.CreatedBy).Not.Nullable();
      Map(x => x.CreatedDate).Not.Nullable();
      Map(x => x.DueDate).Not.Nullable();
      Map(x => x.InfoFor);
      Map(x => x.ReportYear);
      Map(x => x.ReportMonth);
      Map(x => x.Comment).CustomSqlType("varchar(MAX)");
    }
  }

  public class FeedsalesReportWithItemsLookupMap : ClassMap<FeedsalesReportWithItemsLookup>
  {
    public FeedsalesReportWithItemsLookupMap()
    {
      Table("FeedsalesReport");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.IsSubmitted).Not.Nullable();
      Map(x => x.SubmittedDate);
      References(x => x.Member, "MemberId").Cascade.None().Not.Nullable();
      References(x => x.MemberBranch, "MemberBranchId").Cascade.None().Not.Nullable();
      References(x => x.MemberAccount, "MemberAccountId").Cascade.None();
      Map(x => x.CreatedBy).Not.Nullable();
      Map(x => x.CreatedDate).Not.Nullable();
      Map(x => x.DueDate).Not.Nullable();
      Map(x => x.InfoFor);
      Map(x => x.ReportYear);
      Map(x => x.ReportMonth);
      Map(x => x.Comment).CustomSqlType("varchar(MAX)");
      HasMany(x => x.ReportItems).KeyColumn("FeedsalesReportId").Cascade.Delete().Inverse();
    }
  }
}

namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class SalmonellaReportMap : ClassMap<SalmonellaReport>
  {
    public SalmonellaReportMap()
    {
      Table("SalmonellaReport");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.Laboratory, "LaboratoryId").Cascade.None();
      References(x => x.Member, "MemberId").Cascade.None().Not.Nullable();
      References(x => x.MemberBranch, "MemberBranchId").Cascade.None().Not.Nullable();
      References(x => x.MemberAccount, "MemberAccountId").Cascade.None();
      Map(x => x.SubmittedDate);
      Map(x => x.IsSubmitted).Not.Nullable();
      Map(x => x.OrderNumber);
      Map(x => x.Sampler);
      Map(x => x.LaboratoryReportNo);
      Map(x => x.SiteCode);
      Map(x => x.DueDate).Not.Nullable();
      Map(x => x.InfoFor).Not.Nullable();
      Map(x => x.ReportYear);
      Map(x => x.ReportMonth);
      Map(x => x.Comment).CustomSqlType("varchar(MAX)");
      HasMany(x => x.ReportItems).KeyColumn("SalmonellaReportId").Cascade.Delete().Inverse();
    }
  }

  public class SalmonellaReportLookupMap : ClassMap<SalmonellaReportLookup>
  {
    public SalmonellaReportLookupMap()
    {
      Table("SalmonellaReport");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.Laboratory, "LaboratoryId").Cascade.None();
      References(x => x.Member, "MemberId").Cascade.None().Not.Nullable();
      References(x => x.MemberBranch, "MemberBranchId").Cascade.None().Not.Nullable();
      References(x => x.MemberAccount, "MemberAccountId").Cascade.None();
      Map(x => x.SubmittedDate);
      Map(x => x.IsSubmitted).Not.Nullable();
      Map(x => x.OrderNumber);
      Map(x => x.Sampler);
      Map(x => x.LaboratoryReportNo);
      Map(x => x.SiteCode);
      Map(x => x.DueDate).Not.Nullable();
      Map(x => x.InfoFor).Not.Nullable();
      Map(x => x.ReportYear);
      Map(x => x.ReportMonth);
      Map(x => x.Comment).CustomSqlType("varchar(MAX)");
    }
  }
}

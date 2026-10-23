namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class MycotoxinReportMap : ClassMap<MycotoxinReport>
  {
    public MycotoxinReportMap()
    {
      Table("MycotoxinReport");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.Member, "MemberId").Cascade.None().Not.Nullable();
      References(x => x.MemberBranch, "MemberBranchId").Cascade.None().Not.Nullable();
      References(x => x.MemberAccount, "MemberAccountId").Cascade.None().Not.Nullable();
      References(x => x.Laboratory, "LaboratoryId").Cascade.None();
      Map(x => x.SubmittedDate);
      Map(x => x.IsSubmitted).Not.Nullable();
      Map(x => x.LaboratoryReportNo);
      Map(x => x.DueDate).Not.Nullable();
      Map(x => x.InfoFor).Not.Nullable();
      Map(x => x.ReportYear);
      Map(x => x.ReportMonth);
      Map(x => x.Comment).CustomSqlType("varchar(MAX)");
      HasMany(x => x.Samples).KeyColumn("MycotoxinReportId").Cascade.Delete().Inverse();
    }
  }

  public class MycotoxinReportLookupMap : ClassMap<MycotoxinReportLookup>
  {
    public MycotoxinReportLookupMap()
    {
      Table("MycotoxinReport");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.Member, "MemberId").Cascade.Delete().Not.Nullable();
      References(x => x.MemberBranch, "MemberBranchId").Cascade.Delete().Not.Nullable();
      References(x => x.MemberAccount, "MemberAccountId").Cascade.Delete().Not.Nullable();
      Map(x => x.IsSubmitted).Not.Nullable();
      Map(x => x.InfoFor).Not.Nullable();
      Map(x => x.ReportYear);
      Map(x => x.ReportMonth);
      Map(x => x.Comment).CustomSqlType("varchar(MAX)");
    }
  }
}

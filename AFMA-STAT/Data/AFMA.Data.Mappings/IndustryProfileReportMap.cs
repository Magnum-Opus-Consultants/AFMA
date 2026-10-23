namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class IndustryProfileReportMap : ClassMap<IndustryProfileReport>
  {
    public IndustryProfileReportMap()
    {
      Table("IndustryProfileReport");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.Member, "MemberId").Not.Nullable().Cascade.None();
      References(x => x.MemberBranch, "MemberBranchId").Not.Nullable().Cascade.None();
      References(x => x.MemberAccount, "MemberAccountId").Not.Nullable().Cascade.None();
      Map(x => x.ReportYear).Not.Nullable();
      Map(x => x.SubmittedBy);
      Map(x => x.Submitted).Not.Nullable();
      Map(x => x.AnnualProductionAverage);
      References(x => x.MemberType, "MemberTypeId").Not.Nullable().Cascade.None();
      Map(x => x.MemberTypeDescription);
      Map(x => x.CreatedBy).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
    }
  }
}

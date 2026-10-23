namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class MemberBranchAnimalFeedProductMap : ClassMap<MemberBranchAnimalFeedProduct>
  {
    public MemberBranchAnimalFeedProductMap()
    {
      Table("MemberBranchAnimalFeedProduct");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.AnimalFeedProduct, "AnimalFeedProductId").Not.Nullable().Cascade.None();
      References(x => x.MemberBranch, "MemberBranchId").Not.Nullable().Cascade.None();
    }
  }
}

namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class LoginHistoryMap : ClassMap<LoginHistory>
  {
    public LoginHistoryMap()
    {
      Table("LoginHistory");
      Id(x => x.Id).GeneratedBy.Identity().Not.Nullable();
      Map(x => x.UserId).Not.Nullable();
      Map(x => x.UserName).Not.Nullable();
      Map(x => x.LoginDate).Not.Nullable();
      Map(x => x.IpAddress).Not.Nullable();
      Map(x => x.HostName).Not.Nullable();
      Map(x => x.Member).Not.Nullable();
      Map(x => x.Memberbranch).Not.Nullable();
      Map(x => x.UserType).Not.Nullable();
      Map(x => x.Browser).Not.Nullable();
    }
  }
}

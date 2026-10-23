namespace AFMA.Infrastructure.Options
{
  using Alpha.Services.NHibernate.Repository.Options;
  using Microsoft.Extensions.Configuration;

  public class SessionFactory : SessionFactoryOptionsBase
  {
    public SessionFactory(IConfiguration configuration)
    {
      var x = configuration.GetConnectionString("ApplicationRepository");
      this.WithConnectionString(configuration.GetConnectionString("ApplicationRepository"));
      this.WithSessionName("StatsApplication");
    }
  }
}

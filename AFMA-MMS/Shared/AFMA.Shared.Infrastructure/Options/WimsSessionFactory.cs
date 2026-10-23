namespace AFMA.Infrastructure.Options
{
  using Alpha.Services.NHibernate.Repository.Options;
  using Microsoft.Extensions.Configuration;

  public class WimsSessionFactory : SessionFactoryOptionsBase
  {
    public WimsSessionFactory(IConfiguration configuration)
    {
      this.WithConnectionString(configuration.GetConnectionString("WimsApplicationRepository"));
      this.WithSessionName("WimsApplication");
    }
  }
}

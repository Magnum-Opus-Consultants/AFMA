using AFMA.Data.Entities;
using FluentNHibernate.Mapping;

namespace AFMA.Data.Mappings
{
  public class UserMap : ClassMap<User>
  {
    public UserMap()
    {
      Table("User");
      Id(x => x.UserId);
    }
  }
}

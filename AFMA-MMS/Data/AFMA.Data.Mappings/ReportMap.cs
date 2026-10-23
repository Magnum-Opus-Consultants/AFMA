using AFMA.Data.Entities;
using FluentNHibernate.Mapping;

namespace AFMA.Data.Mappings
{
  public class ReportMap : ClassMap<Report>
  {
    public ReportMap()
    {
      Id(x => x.Id);
    }
  }
}

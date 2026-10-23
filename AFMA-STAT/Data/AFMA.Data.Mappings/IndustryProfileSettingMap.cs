namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class IndustryProfileSettingMap : ClassMap<IndustryProfileSetting>
  {
    public IndustryProfileSettingMap()
    {
      Table("IndustryProfileSetting");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.SettingName).Not.Nullable();
      Map(x => x.SettingType).Not.Nullable();
      Map(x => x.SettingValue).Not.Nullable();
      Map(x => x.Editable).Not.Nullable();
    }
  }
}

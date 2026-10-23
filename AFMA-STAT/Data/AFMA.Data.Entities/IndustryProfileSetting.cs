namespace AFMA.Data.Entities
{
  public class IndustryProfileSetting
  {
    public virtual int Id { get; set; }
    public virtual string? SettingName { get; set; }
    public virtual string? SettingType { get; set; }
    public virtual string? SettingValue { get; set; }
    public virtual bool Editable { get; set; }
  }
}

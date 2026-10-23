namespace AFMA.ApplicationLayer.Model
{
  public class MaterialSelectLookupModel
  {
    public int Id { get; set; }
    public string? Name { get; set; }
    public string DisplayName => $"{Code} - {Name}";
    public string? Code { get; set; }
    public int DisplayOrder { get; set; }
    public bool Other { get; set; }
  }
}

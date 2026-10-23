namespace AFMA.ApplicationLayer.Model
{
  public class MaterialCategoryModel
  {
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? MaterialGroupId { get; set; }
    public string? MaterialGroup { get; set; }
    public string? Code { get; set; }
    public bool Active { get; set; }
  }
}

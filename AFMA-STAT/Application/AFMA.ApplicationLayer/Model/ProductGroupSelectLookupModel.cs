namespace AFMA.ApplicationLayer.Model
{
  public class ProductGroupSelectLookupModel
  {
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Code { get; set; }

    public string? DisplayName => $"{this.Code} - {this.Name}";
  }
}

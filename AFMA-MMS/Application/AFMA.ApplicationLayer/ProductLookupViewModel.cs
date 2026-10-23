namespace AFMA.ApplicationLayer
{
  using Helpers;

  public class ProductMaterialLookupViewModel
  {
    public string Code { get; set; }
    public int? MaterialId { get; set; }
    public string? Material { get; set; }
    public int? ProductId { get; set; }
    public string? Product { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal PreviousValue { get; set; }
    public decimal Difference => GeneralHelpers.GetPercentageIncreaseOrDecrease(this.CurrentValue, this.PreviousValue);
    public IncreaseDecrease IncreaseDecrease => this.PreviousValue > this.CurrentValue ? IncreaseDecrease.Decrease : IncreaseDecrease.Increase;
  }
}

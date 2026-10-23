namespace AFMA.ApplicationLayer.ViewModels
{
  using System.Collections.Generic;

  public class FinishedProductAddViewModel
  {
    public FinishedProductAddViewModel()
    {
      AvailableFinishedProducts = new List<AvailableFinishedProductsViewModel>();
    }

    public int Id { get; set; }
    public int ReportId { get; set; }
    public int ProductGroupId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Value { get; set; }
    public string Other { get; set; }
    public ICollection<AvailableFinishedProductsViewModel> AvailableFinishedProducts { get; set; }
  }
}

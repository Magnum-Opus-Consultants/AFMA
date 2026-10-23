namespace AFMA.ApplicationLayer.Model.ExternalApi
{
  using AFMA.ApplicationLayer.Helpers;

  public class FeedsalesApiDashboardModel
  {
    public decimal AnimalFeedProducedCurrentMonth => Data
      .Where(x => x.ReportYear == DateTime.Now.Year && x.ReportMonth == DateTime.Now.AddMonths(-3).Month)
      .Sum(s => s.SubmittedValue);

    public string AnimalFeedProducedCurrentMonthDescription
    {
      get
      {
        var date = DateTime.Now.AddMonths(-3);
        var monthName = date.ToString("MMMM").ToUpper();
        return $"{monthName} {DateTime.Now.Year}";
      }
    }

    public decimal AnimalFeedProducedPreviousMonth => Data
      .Where(x => x.ReportYear == DateTime.Now.Year && x.ReportMonth == DateTime.Now.AddMonths(-4).Month)
      .Sum(s => s.SubmittedValue);

    public string AnimalFeedProducedPreviousMonthDescription
    {
      get
      {
        var date = DateTime.Now.AddMonths(-4);
        var monthName = date.ToString("MMMM").ToUpper();
        return $"{monthName} {DateTime.Now.Year}";
      }
    }

    public decimal AnimalFeedProducedPreviousYearMonth => Data.Where(x =>
        x.ReportYear == DateTime.Now.AddYears(-1).Year && x.ReportMonth == DateTime.Now.AddMonths(-3).Month)
      .Sum(s => s.SubmittedValue);

    public string AnimalFeedProducedPreviousYearMonthDescription
    {
      get
      {
        var date = DateTime.Now.AddYears(-1).AddMonths(-3);
        var monthName = date.ToString("MMMM").ToUpper();
        return $"{monthName} {date.Year}";
      }
    }

    public ICollection<FeedsalesApiYearlyModel> YearlyData { get; set; } = [];
    public ICollection<FeedsalesApiYearlyModel> RmYearlyData { get; set; } = [];
    public ICollection<FeedsalesApiModel> Data { get; set; } = [];
    public List<int> Years { get; set; } = [];
    public List<FeedCategoryModel> FeedCategories
    {
      get
      {
        var list = new List<FeedCategoryModel>
        {
          new FeedCategoryModel
          {
            Id = 1,
            Name = "Ruminant Feed",
            FeedGroupIds = [1, 2]
          },
          new FeedCategoryModel
          {
            Id = 2,
            Name = "Monogastric Livestock Feed",
            FeedGroupIds = [3, 4, 5, 6]
          },
          new FeedCategoryModel
          {
            Id = 3,
            Name = "Equine Feed",
            FeedGroupIds = [7]
          },
          new FeedCategoryModel
          {
            Id = 4,
            Name = "Companion Animal Feed",
            FeedGroupIds = []
          },
          new FeedCategoryModel
          {
            Id = 5,
            Name = "Specialty Animal Feed",
            FeedGroupIds = [11, 12, 18]
          },
          new FeedCategoryModel
          {
            Id = 6,
            Name = "Pet Food",
            FeedGroupIds = [8, 17]
          },
          new FeedCategoryModel
          {
            Id = 7,
            Name = "Other Feed",
            FeedGroupIds = [9]
          },
          new FeedCategoryModel
          {
            Id = 8,
            Name = "Alternative or Functional Feed",
            FeedGroupIds = [10, 14]
          }
        };
        return list;
      }
    }

    public List<FeedCategoryModel> RawMaterialCategories
    {
      get
      {
        var list = new List<FeedCategoryModel>
        {
          new FeedCategoryModel
          {
            Id = 1,
            Name = "Roughage",
            FeedGroupIds = [1, 2]
          },
          new FeedCategoryModel
          {
            Id = 2,
            Name = "Animal Protein Sources",
            FeedGroupIds = [3, 4, 5, 6]
          },
          new FeedCategoryModel
          {
            Id = 3,
            Name = "Fats and Oil",
            FeedGroupIds = [7]
          },
          new FeedCategoryModel
          {
            Id = 4,
            Name = "Plant Protein Source",
            FeedGroupIds = []
          },
          new FeedCategoryModel
          {
            Id = 5,
            Name = "Mineral and Additives",
            FeedGroupIds = [11, 12, 18]
          },
          new FeedCategoryModel
          {
            Id = 6,
            Name = "Cereal Grain and by-products",
            FeedGroupIds = [8, 17]
          },
          new FeedCategoryModel
          {
            Id = 7,
            Name = "Other plant products and by-products",
            FeedGroupIds = [9]
          },
          new FeedCategoryModel
          {
            Id = 8,
            Name = "Other Products",
            FeedGroupIds = [10, 14]
          },
          new FeedCategoryModel
          {
            Id = 9,
            Name = "Maize and Maize Products",
            FeedGroupIds = [10, 14]
          }
        };
        return list;
      }
    }

    public List<MontModel> Months =>
    [
      new() { Id = 1, Name = "January" },
      new() { Id = 2, Name = "February" },
      new() { Id = 3, Name = "March" },
      new() { Id = 4, Name = "April" },
      new() { Id = 5, Name = "May" },
      new() { Id = 6, Name = "June" },
      new() { Id = 7, Name = "July" },
      new() { Id = 8, Name = "August" },
      new() { Id = 9, Name = "September" },
      new() { Id = 10, Name = "October" },
      new() { Id = 11, Name = "November" },
      new() { Id = 12, Name = "December" }
    ];
  }

  public class FeedsalesApiYearlyModel
  {
    public int ReportYear { get; set; }
    public decimal TotalSubmitted { get; set; }
    public decimal Growth { get; set; }
    public ProductMaterial ProductMaterial { get; set; }
  }

  public class FeedsalesApiModel
  {
    public int ReportId { get; set; }
    public int ReportYear { get; set; }
    public int ReportMonth { get; set; }
    public DateTime InfoFor { get; set; }
    public bool IsSubmitted { get; set; }
    public int? ProductGroupId { get; set; }
    public string ProductGroupName { get; set; } = string.Empty;
    public ProductMaterial ProductMaterial { get; set; }
    public decimal SubmittedValue { get; set; }
  }

  public class FeedCategoryModel
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int[] FeedGroupIds = [];
  }

  public class MontModel
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
  }
}
namespace AFMA.ApplicationLayer.ViewModels
{
  using DocumentFormat.OpenXml.VariantTypes;
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Salmonella Year By Month Reversed Excel model
  /// </summary>
  public class SalmonellaYearByMonthExcelReversedViewModel
  {
    public string SampleType { get; set; }
    public int SampleTypeId { get; set; }
    public string Description { get; set; }
    public int[] Years { get; set; }
    public List<YearMonthValue> SamplesTestedPositive { get; set; }
    public List<YearMonthValueTotals> Totals { get; set; }
  }

  public class YearMonthValue
  {
    public int Year { get; set; }
    public int Month { get; set; }
    public string Season { get; set; }
    public int SampleTypeId { get; set; }
    public string SampleType { get; set; }
    public int TotalTested { get; set; }
    public int TotalPositive { get; set; }
    public int TotalPositiveYear { get; set; }
    public float PercentagePositive
    {
      get
      {
        if (this.TotalTested <= 0) return 0;
        var percentage1 = TotalPositive * 100;
        var percentage2 = percentage1 / TotalTested;
        return percentage2;
      }
    }
  }

  public class YearMonthValueTotals
  {
    public int Year { get; set; }
    public int Month { get; set; }
    public int TotalTested { get; set; }
    public int TotalPositive { get; set; }
  }

  public class SalmonellaSeasonalViewModel
  {
    public string SampleType { get; set; }
    public string Description { get; set; }
    public int[] Years { get; set; }
    public List<SalmonellaSeasonalDetail> SamplesTestedPositive { get; set; }
    public List<YearMonthValueTotals> Totals { get; set; }
  }

  public class SalmonellaSeasonalDetail
  {
    public int Year { get; set; }
    public string SampleType { get; set; }
    public string Season { get; set; }
    public int TotalTested { get; set; }
    public int TotalPositive { get; set; }
    public int TotalPositiveYear { get; set; }
    public decimal PercentagePositive
    {
      get
      {
        if (this.TotalTested > 0)
        {
          decimal percentage1 = TotalPositive * 100;
          decimal percentage2 = percentage1 / TotalTested;
          return decimal.Round(percentage2, 2);
        }
        return 0;
      }
    }
  }

  public class SalmonellaMonthlyExcelReversedViewModel
  {
    public int SampleTypeId { get; set; }
    public string SampleType { get; set; }
    public DateTime StartDate { get; set; }
    public int Months { get; set; }
    public List<MonthlyValues> SampleTypes { get; set; }
  }

  public class MonthlyValues
  {
    public int SampleTypeId { get; set; }
    public string SampleType { get; set; }
    public string Code { get; set; }
    public string Sample { get; set; }
    public List<MonthlyValueTotals> SamplesTestedPositive { get; set; }
  }

  public class MonthlyValueTotals
  {
    public int Year { get; set; }
    public int MonthNumber { get; set; }
    public string MonthName { get; set; }
    public int TotalTested { get; set; }
    public int TotalPositive { get; set; }
    public decimal PercentagePositive
    {
      get
      {
        if (this.TotalTested > 0)
        {
          decimal percentage1 = TotalPositive * 100;
          decimal percentage2 = percentage1 / TotalTested;
          return percentage2;
        }
        return 0;
      }
    }
  }

  public class SalmonellaYearlyExcelReversedViewModel
  {
    public string SampleType { get; set; }
    public string Description { get; set; }
    public int[] Years { get; set; }
    public IEnumerable<SalmonellaYearlySamplesTestedViewModel> SamplesTestedPositive { get; set; }
    public List<KeyValuePair<int, int>> RMTotalTested { get; set; }
    public List<KeyValuePair<int, int>> RMTotalPositive { get; set; }
    public List<KeyValuePair<int, float>> RMPositivePercent { get; set; }
    public List<KeyValuePair<int, int>> FPTotalTested { get; set; }
    public List<KeyValuePair<int, int>> FPTotalPositive { get; set; }
    public List<KeyValuePair<int, float>> FPPositivePercent { get; set; }
    public List<KeyValuePair<int, int>> DSTotalTested { get; set; }
    public List<KeyValuePair<int, int>> DSTotalPositive { get; set; }
    public List<KeyValuePair<int, float>> DSPositivePercent { get; set; }
  }

  public class SalmonellaYearlySamplesTestedViewModel
  {
    public string Code { get; set; }
    public string Sample { get; set; }
    public int? ReportingGroupId { get; set; }
    public string? ReportingGroupName { get; set; }
    public int SampleTypeId { get; set; }
    public List<YearValue> YearValues { get; set; }
    public int SampleTotal { get; set; }
    public int TotalForYear { get; set; }
  }

  public class YearValue
  {
    public int Year { get; set; }
    public int TotalTested { get; set; }
    public int TotalPositive { get; set; }
    public float PercentagePositive
    {
      get
      {
        if (this.TotalTested > 0)
        {
          float percentage1 = TotalPositive * 100;
          float percentage2 = percentage1 / TotalTested;
          return percentage2;
        }
        return 0;
      }
    }
  }

  public class SalmonellaRawMaterialGroupReportViewModel
  {
    public string GroupName { get; set; }
    public int GroupId { get; set; }
    public List<SalmonellaRawMaterialGroupSamplesViewModel> TestedPositiveSamples { get; set; }
  }

  public class SalmonellaRawMaterialGroupSamplesViewModel
  {
    public int Year { get; set; }
    public int TotalTested { get; set; }
    public int TotalPositive { get; set; }
    public decimal PercentagePositive
    {
      get
      {
        try
        {
          var retValue = (decimal)this.TotalPositive / (decimal)this.TotalTested * 100;
          return retValue;
        }
        catch
        {
          return 0M;
        }
      }
    }
  }

  public class SalmonellaDetailSubmissionSummaryViewModel
  {
    public int ReportId { get; set; }
    public DateTime InfoFor { get; set; }
    public int? SampleIdentifier { get; set; }
    public string Member { get; set; }
    public string BranchName { get; set; }
    public string SampleType { get; set; }
    public string SampleId { get; set; }
    public string SampleDetail { get; set; }
    public int? MaterialId { get; set; }
    public int? ProductId { get; set; }
    public int? ProductGroupId { get; set; }
    public string ProductGroup { get; set; }
    public string AdditionalInfo { get; set; }
    public string Note { get; set; }
    public bool? SalmonellaDetected { get; set; }
    public string TypeName { get; set; }
    public string GroupName { get; set; }
  }
}

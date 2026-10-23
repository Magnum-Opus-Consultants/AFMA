namespace AFMA.ApplicationLayer.Helpers
{
  using Microsoft.AspNetCore.Mvc.Rendering;
  using System.Globalization;

  /// <summary>
  /// The GeneralHelpers class.
  /// </summary>
  public static class GeneralHelpers
  {
    public class ItemList
    {
      public string? Text { get; set; }
      public int Value { get; set; }
    }

    public static IEnumerable<SelectListItem> ToSelectList<T>(this IList<T> entities, Func<T, object> funcToGetValue, Func<T, object> funcToGetText, object? selectedValue)
    {
      return entities.Select(x => new SelectListItem
      {
        Value = funcToGetValue(x).ToString(),
        Text = funcToGetText(x).ToString(),
        Selected = selectedValue != null && funcToGetValue(x).ToString() == selectedValue.ToString()
      });
    }

    public static IEnumerable<ItemList> ToSelectList<T>(this IList<T> entities, Func<T, object> funcToGetValue, Func<T, object> funcToGetText)
    {
      return entities.Select(x => new ItemList
      {
        Value = (int)funcToGetValue(x),
        Text = funcToGetText(x).ToString(),
      });
    }

    public static decimal GetPercentageIncreaseOrDecrease(decimal thisMonth, decimal lastMonth)
    {
      try
      {
        var decrease = lastMonth - thisMonth;
        var result = decrease / lastMonth * 100;
        if (result < 0)
        {
          return result * -1;
        }

        return result;

      }
      catch (DivideByZeroException ex)
      {
        return 100M;
      }
    }

    public static int? ToNullableInt(this object s)
    {
      int i;
      if (int.TryParse(s.ToString(), out i)) return i;
      return null;
    }

    public static bool? ToNullableBool(this object b)
    {
      bool i;
      if (bool.TryParse(b.ToString(), out i)) return i;
      return null;
    }

    public static bool ToBoolean(this bool? value)
    {
      return value != null && value != false;
    }

    public static IEnumerable<DateTime> MonthsBetween(DateTime d0, DateTime d1)
    {
      try
      {
        return Enumerable.Range(0, (d1.Year - d0.Year) * 12 + (d1.Month - d0.Month + 1))
          .Select(m => new DateTime(d0.Year, d0.Month, 1).AddMonths(m));
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public static IEnumerable<SelectListItem> GenerateMonthSelectList(int? selectedValue)
    {
      var itemList = new List<ItemList>();
      for (int i = 1; i < 13; i++)
      {
        itemList.Add(new ItemList
        {
          Value = i,
          Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
        });
      }

      return itemList.ToSelectList(x => x.Value, x => x.Text!, selectedValue);
    }

    public static IEnumerable<SelectListItem> GenerateYearSelectList(int? selectedValue)
    {
      var itemList = new List<ItemList>();
      for (int i = 2000; i <= DateTime.Now.Year; i++)
      {
        itemList.Add(new ItemList
        {
          Value = i,
          Text = i.ToString(),
        });
      }

      return itemList.OrderByDescending(y => y.Value).ToList().ToSelectList(x => x.Value, x => x.Text!, selectedValue);
    }
  }
}
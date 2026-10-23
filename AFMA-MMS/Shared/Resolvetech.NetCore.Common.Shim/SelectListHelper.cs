namespace Resolvetech.NetCore.Common.Helpers
{
  using Microsoft.AspNetCore.Mvc.Rendering;

  /// <summary>
  /// Local stand-in for the Resolvetech.NetCore.Common package, which is not
  /// available on the local NuGet feed. Only the members the solution actually
  /// consumes are implemented.
  /// </summary>
  public static class SelectListHelper
  {
    /// <summary>
    /// Projects a sequence into <see cref="SelectListItem"/>s, marking the item
    /// whose value matches <paramref name="selectedValue"/> as selected.
    /// </summary>
    public static IEnumerable<SelectListItem> ToSelectList<TSource, TValue>(
      this IEnumerable<TSource> source,
      Func<TSource, TValue> valueSelector,
      Func<TSource, string> textSelector,
      object? selectedValue = null)
    {
      var selected = Convert.ToString(selectedValue);

      return source.Select(item =>
      {
        var value = Convert.ToString(valueSelector(item)) ?? string.Empty;

        return new SelectListItem
        {
          Value = value,
          Text = textSelector(item),
          Selected = !string.IsNullOrEmpty(selected) && value == selected,
        };
      }).ToList();
    }
  }
}

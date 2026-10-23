namespace AFMA.Stats.Client.ModelBinders;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

/// <summary>
/// Binds <see cref="DateTime"/> form values independently of the server's regional settings.
/// </summary>
/// <remarks>
/// The views render dates as "dd/MM/yyyy" and "MMMM yyyy" (Salmonella/Mycotoxin Edit) and HTML date
/// inputs post "yyyy-MM-dd". The default binder parses with the server culture, so on an en-US server
/// "20/08/2026" fails to parse and the model silently receives 0001-01-01, which SQL Server rejects on
/// save ("could not execute batch command"). This binder accepts the formats the application itself
/// produces, then falls back to the default culture-based parse for anything else.
/// </remarks>
public class InvariantDateTimeModelBinderProvider : IModelBinderProvider
{
  public IModelBinder? GetBinder(ModelBinderProviderContext context)
  {
    if (context.Metadata.ModelType == typeof(DateTime) || context.Metadata.ModelType == typeof(DateTime?))
    {
      return new InvariantDateTimeModelBinder();
    }

    return null;
  }
}

public class InvariantDateTimeModelBinder : IModelBinder
{
  /// <summary>
  /// Formats the application renders into form fields, in the order they should be tried.
  /// </summary>
  private static readonly string[] Formats =
  {
    "yyyy-MM-dd",
    "yyyy-MM-ddTHH:mm",
    "yyyy-MM-ddTHH:mm:ss",
    "yyyy-MM-dd HH:mm",
    "yyyy-MM-dd HH:mm:ss",
    "dd/MM/yyyy",
    "d/M/yyyy",
    "dd/MM/yyyy HH:mm",
    "dd/MM/yyyy HH:mm:ss",
    "yyyy/MM/dd",
    "yyyy/MM/dd HH:mm:ss",
    "MMMM yyyy",
    "MMM yyyy",
    "MM/yyyy",
  };

  public Task BindModelAsync(ModelBindingContext bindingContext)
  {
    var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

    if (valueProviderResult == ValueProviderResult.None)
    {
      return Task.CompletedTask;
    }

    bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

    var value = valueProviderResult.FirstValue;

    if (string.IsNullOrWhiteSpace(value))
    {
      if (bindingContext.ModelMetadata.ModelType == typeof(DateTime?))
      {
        bindingContext.Result = ModelBindingResult.Success(null);
      }

      return Task.CompletedTask;
    }

    value = value.Trim();

    if (DateTime.TryParseExact(value, Formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var exact))
    {
      bindingContext.Result = ModelBindingResult.Success(exact);
    }
    else if (DateTime.TryParse(value, valueProviderResult.Culture,
               DateTimeStyles.AdjustToUniversal | DateTimeStyles.AllowWhiteSpaces, out var parsed))
    {
      // Same behaviour as ASP.NET Core's default DateTimeModelBinder.
      bindingContext.Result = ModelBindingResult.Success(parsed);
    }
    else
    {
      bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid date value.");
    }

    return Task.CompletedTask;
  }
}

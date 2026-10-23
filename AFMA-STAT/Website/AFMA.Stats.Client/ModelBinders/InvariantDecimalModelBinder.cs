namespace AFMA.Stats.Client.ModelBinders;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

public class InvariantDecimalModelBinderProvider : IModelBinderProvider
{
  public IModelBinder? GetBinder(ModelBinderProviderContext context)
  {
    if (context.Metadata.ModelType == typeof(decimal) || context.Metadata.ModelType == typeof(decimal?))
    {
      return new InvariantDecimalModelBinder();
    }

    return null;
  }
}

public class InvariantDecimalModelBinder : IModelBinder
{
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
      if (bindingContext.ModelMetadata.ModelType == typeof(decimal?))
      {
        bindingContext.Result = ModelBindingResult.Success(null);
        return Task.CompletedTask;
      }

      return Task.CompletedTask;
    }

    // Normalize: replace comma with dot so both separators work
    value = value.Replace(",", ".");

    if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
    {
      bindingContext.Result = ModelBindingResult.Success(result);
    }
    else
    {
      bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid decimal value.");
    }

    return Task.CompletedTask;
  }
}

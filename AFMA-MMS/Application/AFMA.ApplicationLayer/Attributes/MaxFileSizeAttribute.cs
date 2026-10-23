namespace AFMA.ApplicationLayer.Attributes
{
  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

  public class MaxFileSizeAttribute : ValidationAttribute, IClientModelValidator
  {
    private readonly int _maxFileSizeInBytes;

    public MaxFileSizeAttribute(int maxFileSizeInBytes)
    {
      _maxFileSizeInBytes = maxFileSizeInBytes;
      ErrorMessage = $"File size cannot exceed {_maxFileSizeInBytes / (1024 * 1024)} MB.";
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (value is IFormFile file && file.Length > _maxFileSizeInBytes)
      {
        return new ValidationResult(ErrorMessage);
      }

      return ValidationResult.Success;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
      context.Attributes.Add("data-val", "true");
      context.Attributes.Add("data-val-maxfilesize", ErrorMessage);
      context.Attributes.Add("data-val-maxfilesize-size", _maxFileSizeInBytes.ToString());
    }
  }
}

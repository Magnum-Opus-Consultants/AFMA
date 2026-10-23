namespace AFMA.ApplicationLayer.Services
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Model.Export;
  using OfficeOpenXml;
  using OfficeOpenXml.Style;
  using System.Collections;
  using System.ComponentModel;
  using System.Drawing;
  using System.Reflection;
  using System.Text.RegularExpressions;

  public partial class ExcelExportService : IExcelExportService
  {
    private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public ExportFileModel CreateWorkbook(ExcelWorkbookExportModel workbookExportModel)
    {
      ArgumentNullException.ThrowIfNull(workbookExportModel);

      if (string.IsNullOrWhiteSpace(workbookExportModel.FileName))
      {
        throw new ArgumentException("A file name is required for the export.", nameof(workbookExportModel));
      }

      if (workbookExportModel.Worksheets.Count == 0)
      {
        throw new ArgumentException("At least one worksheet is required for the export.", nameof(workbookExportModel));
      }

      using var package = new ExcelPackage();
      var worksheetNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

      foreach (var worksheetExportModel in workbookExportModel.Worksheets)
      {
        var worksheetName = GetWorksheetName(worksheetExportModel.Name, worksheetNames);
        var worksheet = package.Workbook.Worksheets.Add(worksheetName);

        WriteWorksheet(worksheet, worksheetExportModel);
      }

      return new ExportFileModel
      {
        Content = package.GetAsByteArray(),
        ContentType = ExcelContentType,
        FileName = GetFileName(workbookExportModel.FileName),
      };
    }

    private static void WriteWorksheet(ExcelWorksheet worksheet, ExcelWorksheetExportModel worksheetExportModel)
    {
      ArgumentNullException.ThrowIfNull(worksheetExportModel);
      ArgumentNullException.ThrowIfNull(worksheetExportModel.Data);

      var rows = GetRows(worksheetExportModel.Data, out var rowType);
      if (IsDictionaryRowType(rowType))
      {
        WriteDictionaryWorksheet(worksheet, rows, worksheetExportModel);
        return;
      }

      var properties = GetProperties(rowType, worksheetExportModel.PropertyNames);

      if (properties.Count == 0)
      {
        throw new InvalidOperationException($"Worksheet '{worksheetExportModel.Name}' does not contain any exportable columns.");
      }

      for (var columnIndex = 0; columnIndex < properties.Count; columnIndex++)
      {
        var cell = worksheet.Cells[1, columnIndex + 1];
        cell.Value = GetColumnName(properties[columnIndex]);
      }

      var headerRange = worksheet.Cells[1, 1, 1, properties.Count];
      headerRange.Style.Font.Bold = true;
      headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
      headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);

      var rowIndex = 2;
      foreach (var row in rows.Where(x => x is not null))
      {
        for (var columnIndex = 0; columnIndex < properties.Count; columnIndex++)
        {
          worksheet.Cells[rowIndex, columnIndex + 1].Value = GetCellValue(properties[columnIndex], row);
        }

        rowIndex++;
      }

      FormatWorksheet(worksheet);
    }

    private static List<object?> GetRows(object data, out Type rowType)
    {
      if (data is IEnumerable enumerable and not string and not byte[])
      {
        var rows = enumerable.Cast<object?>().ToList();
        rowType = rows.FirstOrDefault(x => x is not null)?.GetType()
          ?? GetEnumerableItemType(data.GetType())
          ?? throw new InvalidOperationException("Unable to determine the worksheet row type.");

        return rows;
      }

      rowType = data.GetType();
      return [data];
    }

    private static List<PropertyInfo> GetProperties(Type rowType, ICollection<string> propertyNames)
    {
      var availableProperties = rowType
        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
        .Where(x => x.CanRead && x.GetIndexParameters().Length == 0 && IsSupportedExportType(x.PropertyType))
        .OrderBy(x => x.MetadataToken)
        .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

      if (propertyNames.Count == 0)
      {
        return availableProperties.Values.ToList();
      }

      var properties = new List<PropertyInfo>(propertyNames.Count);
      foreach (var propertyName in propertyNames)
      {
        if (!availableProperties.TryGetValue(propertyName, out var propertyInfo))
        {
          throw new InvalidOperationException($"Property '{propertyName}' could not be exported from type '{rowType.Name}'.");
        }

        properties.Add(propertyInfo);
      }

      return properties;
    }

    private static bool IsSupportedExportType(Type propertyType)
    {
      var normalizedType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

      return normalizedType.IsPrimitive
        || normalizedType.IsEnum
        || normalizedType == typeof(string)
        || normalizedType == typeof(decimal)
        || normalizedType == typeof(DateTime)
        || normalizedType == typeof(DateTimeOffset)
        || normalizedType == typeof(DateOnly)
        || normalizedType == typeof(TimeOnly)
        || normalizedType == typeof(TimeSpan)
        || normalizedType == typeof(Guid);
    }

    private static bool IsDictionaryRowType(Type rowType)
    {
      return typeof(IDictionary<string, object>).IsAssignableFrom(rowType)
        || rowType.GetInterfaces().Any(x =>
          x.IsGenericType
          && x.GetGenericTypeDefinition() == typeof(IDictionary<,>)
          && x.GetGenericArguments()[0] == typeof(string));
    }

    private static void WriteDictionaryWorksheet(ExcelWorksheet worksheet, List<object?> rows, ExcelWorksheetExportModel worksheetExportModel)
    {
      var dictionaryRows = rows
        .Where(x => x is not null)
        .Cast<IDictionary<string, object>>()
        .ToList();

      var columns = worksheetExportModel.PropertyNames.Count > 0
        ? worksheetExportModel.PropertyNames.ToList()
        : dictionaryRows
          .SelectMany(x => x.Keys)
          .Distinct(StringComparer.OrdinalIgnoreCase)
          .ToList();

      if (columns.Count == 0)
      {
        throw new InvalidOperationException($"Worksheet '{worksheetExportModel.Name}' does not contain any exportable columns.");
      }

      for (var columnIndex = 0; columnIndex < columns.Count; columnIndex++)
      {
        worksheet.Cells[1, columnIndex + 1].Value = columns[columnIndex];
      }

      var headerRange = worksheet.Cells[1, 1, 1, columns.Count];
      headerRange.Style.Font.Bold = true;
      headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
      headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);

      var rowIndex = 2;
      foreach (var row in dictionaryRows)
      {
        for (var columnIndex = 0; columnIndex < columns.Count; columnIndex++)
        {
          row.TryGetValue(columns[columnIndex], out var value);
          worksheet.Cells[rowIndex, columnIndex + 1].Value = GetFormattedCellValue(value);
        }

        rowIndex++;
      }

      FormatWorksheet(worksheet);
    }

    private static object? GetCellValue(PropertyInfo propertyInfo, object? row)
    {
      var value = propertyInfo.GetValue(row);
      return GetFormattedCellValue(value);
    }

    private static object? GetFormattedCellValue(object? value)
    {
      return value switch
      {
        null => string.Empty,
        DateOnly dateOnly => dateOnly.ToString("yyyy-MM-dd"),
        TimeOnly timeOnly => timeOnly.ToString("HH:mm:ss"),
        _ => value,
      };
    }

    private static string GetColumnName(PropertyInfo propertyInfo)
    {
      var displayAttribute = propertyInfo.GetCustomAttribute<DisplayAttribute>();
      if (!string.IsNullOrWhiteSpace(displayAttribute?.GetName()))
      {
        return displayAttribute.GetName()!;
      }

      var displayNameAttribute = propertyInfo.GetCustomAttribute<DisplayNameAttribute>();
      if (!string.IsNullOrWhiteSpace(displayNameAttribute?.DisplayName))
      {
        return displayNameAttribute.DisplayName;
      }

      return HumanizePropertyNameRegex().Replace(propertyInfo.Name, "$1 $2");
    }

    private static void FormatWorksheet(ExcelWorksheet worksheet)
    {
      if (worksheet.Dimension is not null)
      {
        worksheet.Cells[worksheet.Dimension.Address].AutoFilter = true;
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
      }

      worksheet.View.FreezePanes(2, 1);
    }

    private static Type? GetEnumerableItemType(Type collectionType)
    {
      if (collectionType.IsArray)
      {
        return collectionType.GetElementType();
      }

      var enumerableType = collectionType
        .GetInterfaces()
        .Append(collectionType)
        .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));

      return enumerableType?.GetGenericArguments()[0];
    }

    private static string GetFileName(string fileName)
    {
      var name = fileName.Trim();
      foreach (var invalidCharacter in Path.GetInvalidFileNameChars())
      {
        name = name.Replace(invalidCharacter, '-');
      }

      if (!name.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
      {
        name += ".xlsx";
      }

      if (name == ".xlsx")
      {
        name = "Export.xlsx";
      }

      return name;
    }

    private static string GetWorksheetName(string worksheetName, ISet<string> worksheetNames)
    {
      var name = string.IsNullOrWhiteSpace(worksheetName) ? "Sheet1" : worksheetName.Trim();

      foreach (var invalidCharacter in Path.GetInvalidFileNameChars().Concat(['[', ']', ':']))
      {
        name = name.Replace(invalidCharacter, ' ');
      }

      name = string.Join(" ", name.Split(' ', StringSplitOptions.RemoveEmptyEntries));
      if (string.IsNullOrWhiteSpace(name))
      {
        name = "Sheet1";
      }

      const int maxWorksheetLength = 31;
      if (name.Length > maxWorksheetLength)
      {
        name = name[..maxWorksheetLength];
      }

      var candidate = name;
      var suffix = 2;
      while (!worksheetNames.Add(candidate))
      {
        var duplicateSuffix = $" ({suffix})";
        var trimmedName = name.Length + duplicateSuffix.Length > maxWorksheetLength
          ? name[..(maxWorksheetLength - duplicateSuffix.Length)]
          : name;

        candidate = $"{trimmedName}{duplicateSuffix}";
        suffix++;
      }

      return candidate;
    }

    [GeneratedRegex("([a-z0-9])([A-Z])")]
    private static partial Regex HumanizePropertyNameRegex();
  }
}

namespace AFMA.Shared.Infrastructure.Commands;

/// <summary>
/// Represents the Authenticate Command Result.
/// </summary>
public class SaveOrUpdateResult
{
  /// <summary>
  /// Initializes a new instance of the <see cref="SaveOrUpdateResult"/> class.
  /// </summary>
  /// <param name="success"> Operation Success Indication. </param>
  /// <param name="errorMessage"> Error Message. </param>
  /// <param name="data"> Additional Data.</param>
  /// <param name="savedObjectId"> Saved Object Id. </param>
  public SaveOrUpdateResult(bool success, string errorMessage = null!, object data = null!, int? savedObjectId = null)
  {
    this.Success = success;
    this.ErrorMessage = errorMessage;
    this.Data = data;
    this.SavedObjectId = savedObjectId;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="SaveOrUpdateResult"/> class.
  /// </summary>
  public SaveOrUpdateResult()
  {
  }

  /// <summary>
  /// Gets a value indicating whether the command was a success.
  /// </summary>
  public bool Success { get; set; }

  /// <summary>
  /// Gets the Error.
  /// </summary>
  public string? ErrorMessage { get; set; }

  /// <summary>
  /// Gets the Saved Object Id.
  /// </summary>
  public int? SavedObjectId { get; set; }

  /// <summary>
  /// Gets any Additonal Data.
  /// </summary>
  public object Data { get; set; } = null!;
}
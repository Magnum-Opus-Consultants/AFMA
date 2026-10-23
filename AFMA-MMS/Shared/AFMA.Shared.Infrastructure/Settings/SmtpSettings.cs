namespace AFMA.Shared.Infrastructure.Settings
{
  /// <summary>
  /// The SmtpSettings class.
  /// </summary>
  public class SmtpSettings
  {
    /// <summary>
    /// Gets or sets the SMTP Host name.
    /// </summary>
    public string? Host { get; set; }

    /// <summary>
    /// Gets or sets the SMTP Port number.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets the SMTP Username.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the SMTP Password.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the From name.
    /// </summary>
    public string? FromName { get; set; }

    /// <summary>
    /// Gets or sets the From email address.
    /// </summary>
    public string? FromAddress { get; set; }

    /// <summary>
    /// Gets or sets the authentication indicator.
    /// </summary>
    public bool Auth { get; set; } = false;

    /// <summary>
    /// Gets or sets the debug mode.
    /// </summary>
    public bool Debug { get; set; } = false;

    /// <summary>
    /// Gets or sets the email address for debugging.
    /// </summary>
    public string? DebugEmail { get; set; }

    /// <summary>
    /// Gets or sets the email address to cc submission stats.
    /// </summary>
    public string? SubmissionStatsEmail { get; set; }
  }
}

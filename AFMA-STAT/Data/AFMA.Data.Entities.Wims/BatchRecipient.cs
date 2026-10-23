namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The BatchRecipient class represents a recipient of a batch message, including details about the member, distribution group, and recipient information.
  /// </summary>
  public class BatchRecipient
  {
    /// <summary>
    /// Gets or sets the unique identifier for the batch recipient.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the member associated with this batch recipient.
    /// </summary>
    public virtual Batch Batch { get; set; } = new();

    /// <summary>
    /// Gets or sets the email address from which the email ius sent.
    /// </summary>
    public virtual string FromEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the sender.
    /// </summary>
    public virtual string FromName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the recipient email address.
    /// </summary>
    public virtual string ToEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the recipient.
    /// </summary>
    public virtual string ToName { get; set; } = string.Empty;
  }
}

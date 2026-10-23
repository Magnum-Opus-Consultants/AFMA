namespace AFMA.Data.Entities.Wims
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// The Batch class represents a batch of messages to be sent, including details about the sender, recipients, and attachments.
  /// </summary>
  public class Batch
  {
    /// <summary>
    /// Gets or sets the unique identifier for the batch.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// GEts or sets the name of the batch.
    /// </summary>
    public virtual string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the sender.
    /// </summary>
    public virtual string FromName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address from which the batch messages are sent.
    /// </summary>
    public virtual string FromEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subject of the batch messages.
    /// </summary>
    public virtual string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message content of the batch.
    /// </summary>
    public virtual string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user who created the batch.
    /// </summary>
    public virtual string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the batch, indicating whether it is an email batch or a text message batch.
    /// </summary>
    public virtual int BatchType { get; set; }

    /// <summary>
    /// Gets or sets the status of the batch, indicating its current state (e.g., queued, sent, error).
    /// </summary>
    public virtual int Status { get; set; }

    /// <summary>
    /// Gets or sets the collection of recipients for the batch.
    /// </summary>
    public virtual ICollection<BatchRecipient> Recipients { get; set; } = [];

    /// <summary>
    /// Gets or sets the count of recipients in the batch.
    /// </summary>
    public virtual int RecipientCount { get; set; }

    /// <summary>
    /// Gets or sets the collection of attachments associated with the batch.
    /// </summary>
    public virtual ICollection<BatchAttachment> Attachments { get; set; } = [];

    /// <summary>
    /// Gets or sets the count of attachments in the batch.
    /// </summary>
    public virtual int MessageCount { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the batch was submitted.
    /// </summary>
    public virtual DateTime? SubmissionDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the batch was created.
    /// </summary>
    public virtual DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the batch was last updated.
    /// </summary>
    public virtual DistributionGroup DistributionGroup { get; set; } = new DistributionGroup();
  }
}

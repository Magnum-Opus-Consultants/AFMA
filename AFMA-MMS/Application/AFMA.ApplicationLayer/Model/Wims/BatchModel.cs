namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.ApplicationLayer.Helpers;
  using AFMA.Data.Entities.Wims;
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations.Schema;

  /// <summary>
  /// The BatchModel class represents a batch of messages to be sent, including details about the sender, recipients, and attachments.
  /// </summary>
  [MapTo(typeof(Batch))]
  public class BatchModel
  {
    /// <summary>
    /// Gets or sets the unique identifier for the batch.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// GEts or sets the name of the batch.
    /// </summary>
    [Display(Name = "Batch Description")]
    [Required(ErrorMessage = "Batch Description is required")]
    [StringLength(255, ErrorMessage = "Batch Description cannot exceed 250 characters")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the sender.
    /// </summary>
    [Display(Name = "From Name")]
    [Required(ErrorMessage = "From Name is required")]
    [StringLength(200, ErrorMessage = "From Name cannot exceed 200 characters")]
    public string FromName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address from which the batch messages are sent.
    /// </summary>
    [Display(Name = "From Email")]
    [Required(ErrorMessage = "From Email is required")]
    [StringLength(200, ErrorMessage = "From Email cannot exceed 200 characters")]
    public string FromEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subject of the batch messages.
    /// </summary>
    [Display(Name = "Message Subject")]
    [Required(ErrorMessage = "Subject is required")]
    [StringLength(200, ErrorMessage = "Subject cannot exceed 200 characters")]
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message content of the batch.
    /// </summary>
    [Display(Name = "Message Content")]
    public string? Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user who created the batch.
    /// </summary>
    [Required(ErrorMessage = "User is required")]
    [StringLength(255, ErrorMessage = "User cannot exceed 200 characters")]
    public string User { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the batch, indicating whether it is an email batch or a text message batch.
    /// </summary>
    [Display(Name = "Batch Type")]
    public int BatchType { get; set; }

    /// <summary>
    /// Gets or sets the status of the batch, indicating its current state (e.g., queued, sent, error).
    /// </summary>
    [Display(Name = "Batch Status")]
    public BatchStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the collection of recipients for the batch.
    /// </summary>
    public ICollection<BatchRecipientModel> Recipients { get; set; } = [];

    /// <summary>
    /// Gets or sets the count of recipients in the batch.
    /// </summary>
    public int RecipientCount => Recipients.Count;

    /// <summary>
    /// Gets or sets the collection of attachments associated with the batch.
    /// </summary>
    public ICollection<BatchAttachmentModel> Attachments { get; set; } = [];

    /// <summary>
    /// Gets or sets the count of attachments in the batch.
    /// </summary>
    [Display(Name = "Message Count")]
    public int MessageCount { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the batch was submitted.
    /// </summary>
    [Display(Name = "Submission Date")]
    public DateTime? SubmissionDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the batch was created.
    /// </summary>
    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the distribution group to which this batch belongs.
    /// </summary>
    [Display(Name = "Distribution Group")]
    public int? DistributionGroupId { get; set; }

    /// <summary>
    /// Gets or sets the distribution group to which this batch belongs.
    /// </summary>
    [Display(Name = "Distribution Group")]
    public string DistributionGroup { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the committee to which this batch belongs.
    /// </summary>
    [Display(Name = "Committee")]
    public int? CommitteeId { get; set; }

    /// <summary>
    /// Gets or sets the committee to which this batch belongs.
    /// </summary>
    [Display(Name = "Committee")]
    public string Committee { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the recipient selector, which indicates the criteria used to select recipients for the batch (e.g., all members, specific distribution group, specific committee).
    /// </summary>
    [NotMapped]
    public int RecipientSelector { get; set; }
  }
}

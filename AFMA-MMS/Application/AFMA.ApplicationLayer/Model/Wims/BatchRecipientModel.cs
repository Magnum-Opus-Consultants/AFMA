namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The BatchRecipientModel class represents a recipient of a batch message, including details about the sender and recipient.
  /// </summary>
  [MapTo(typeof(BatchRecipient))]
  public class BatchRecipientModel
  {
    /// <summary>
    /// Gets or sets the unique identifier for the batch recipient.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the member associated with this batch recipient.
    /// </summary>
    public int BatchId { get; set; }

    /// <summary>
    /// Gets or sets the name of the batch to which this recipient belongs.
    /// </summary>
    public string Batch { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address from which the email ius sent.
    /// </summary>
    [Display(Name = "From Email")]
    [Required(ErrorMessage = "From Email is required")]
    [StringLength(100, ErrorMessage = "From Email exceed 100 characters")]
    public string FromEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the sender.
    /// </summary>
    [Display(Name = "From Name")]
    [Required(ErrorMessage = "From Name is required")]
    [StringLength(100, ErrorMessage = "From Name exceed 100 characters")]
    public string FromName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the recipient email address.
    /// </summary>
    [Display(Name = "To Email")]
    [Required(ErrorMessage = "To Email is required")]
    [StringLength(100, ErrorMessage = "To Email exceed 100 characters")]
    public string ToEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the recipient.
    /// </summary>
    [Display(Name = "To Name")]
    [Required(ErrorMessage = "To Name is required")]
    [StringLength(100, ErrorMessage = "To Name exceed 100 characters")]
    public string ToName { get; set; } = string.Empty;
  }
}

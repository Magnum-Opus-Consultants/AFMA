namespace AFMA.Service.Communication.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public class EmailMessageModel
  {
    public EmailMessageModel()
    {
      Attachments = new List<EmailAttachmentModel>();
    }

    public string RecipientName { get; set; }
    public string RecipientEmail { get; set; }
    public string Subject { get; set; }
    public int CustomerId { get; set; }
    public int CostCentre { get; set; }
    public ICollection<EmailAttachmentModel> Attachments { get; set; }
    public DateTime? DateSent { get; set; }
    public DateTime DateReceived { get; set; }
    public string DeliveryStatus { get; set; }
    public int DeliveryStatusId { get; set; }
    public int Retries { get; set; }
    public string ServerMessage { get; set; }
    public DateTime? LastRetryDate { get; set; }
    public string MessageText { get; set; }

    public int AttachmentCount => Attachments.Count;
    public long AttachmentSize => Attachments.Sum(s => s.FileSize);
  }

  public class EmailAttachmentModel
  {
    public string FileName { get; set; }
    public long FileSize { get; set; }
  }
}

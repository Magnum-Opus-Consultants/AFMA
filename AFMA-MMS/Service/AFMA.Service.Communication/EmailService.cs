namespace AFMA.Service.Communication
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Model.Wims;
  using AFMA.Service.Communication.Model;
  using EmailServiceReference;
  using System.Net.Mail;

  public class EmailService
  {
    private readonly EmailServiceClient _client;
    private readonly IWimsApplicationLayer _wimsApplicationLayer;
    private readonly string _apiKey;

    public EmailService(string apiKey, IWimsApplicationLayer wimsApplicationLayer)
    {
      _client = new EmailServiceClient();
      _wimsApplicationLayer = wimsApplicationLayer;
      _apiKey = apiKey;

    }

    public IEnumerable<EmailMessageModel> GetQueue()
    {
      return _client.GetQueueAsync(_apiKey, 0).Result.Select(s => new
          EmailMessageModel()
      {
        CostCentre = s.CostCentre,
        CustomerId = s.CustomerId,
        DateReceived = s.DateReceived,
        DateSent = s.DateSent,
        DeliveryStatus = s.DeliveryStatus,
        DeliveryStatusId = s.DeliveryStatusId,
        LastRetryDate = s.LastRetryDate,
        RecipientEmail = s.RecipientEmail,
        RecipientName = s.RecipientName,
        Retries = s.Retries,
        ServerMessage = s.ServerMessage,
        Subject = s.Subject,
      }).ToList();
    }

    public IEnumerable<EmailMessageModel> GetQueue(int patientId)
    {
      return GetQueue().Where(x => x.CustomerId == patientId);
    }

    public IEnumerable<EmailMessageModel> GetSent()
    {
      return _client.GetSentAsync(_apiKey, 0).Result.Select(s => new
          EmailMessageModel()
      {
        CostCentre = s.CostCentre,
        CustomerId = s.CustomerId,
        DateReceived = s.DateReceived,
        DateSent = s.DateSent,
        DeliveryStatus = s.DeliveryStatus,
        DeliveryStatusId = s.DeliveryStatusId,
        LastRetryDate = s.LastRetryDate,
        RecipientEmail = s.RecipientEmail,
        RecipientName = s.RecipientName,
        Retries = s.Retries,
        ServerMessage = s.ServerMessage,
        Subject = s.Subject,
      }).ToList();
    }

    public IEnumerable<EmailMessageModel> GetSent(int patientId)
    {
      return GetSent().Where(x => x.CustomerId == patientId);
    }

    public IEnumerable<EmailMessageModel> GetError()
    {
      return _client.GetErrorAsync(_apiKey, 0).Result.Select(s => new
          EmailMessageModel()
      {
        CostCentre = s.CostCentre,
        CustomerId = s.CustomerId,
        DateReceived = s.DateReceived,
        DateSent = s.DateSent,
        DeliveryStatus = s.DeliveryStatus,
        DeliveryStatusId = s.DeliveryStatusId,
        LastRetryDate = s.LastRetryDate,
        RecipientEmail = s.RecipientEmail,
        RecipientName = s.RecipientName,
        Retries = s.Retries,
        ServerMessage = s.ServerMessage,
        Subject = s.Subject,
      }).ToList();
    }

    public IEnumerable<EmailMessageModel> GetError(int patientId)
    {
      return GetError().Where(x => x.CustomerId == patientId);
    }

    public bool ProcessAndSendBatch(BatchModel batch)
    {
      try
      {
        var emailMessage = new MessageSchema()
        {
          CostCentre = 1,
          UserId = 1009,
          FromEmail = batch.FromEmail,
          FromName = batch.FromName,
          MessageText = batch.Message,
          MessageType = "Email",
          Subject = batch.Subject,
          Attachments = batch.Attachments.Select(a => new AttachmentSchema()
          {
            FileName = a.AttachmentFileName,
            FileContent = a.Attachment,
            FileSize = a.AttachmentFileSize,
            FileType = a.AttachmentContentType,
          }).ToArray(),
          Recipients = batch.Recipients.Select(r => new RecipientSchema()
          {
            DeliveryStatusId = 1,
            ToEmail = r.ToEmail,
            ToName = r.ToName
          }).ToArray()
        };

        var result = _client.SendMessageAsync(_apiKey, emailMessage).Result;

        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public void SendSmtpNotification(string to, string subject, string message)
    {
      var client = new SmtpClient("", 25);
      var mailMessage = new MailMessage()
      {
        From = new MailAddress("afma@afma.co.za"),
        To = { to },
        Subject = subject,
        Body = message,
        IsBodyHtml = true
      };

      client.Send(mailMessage);
    }

    /// <summary>
    /// Sends a batch of emails directly via SMTP (Mailgun), using the batch's own
    /// From address so each message is sent from the individual sender. Throws on
    /// failure so the caller can log and surface the error instead of failing silently.
    /// </summary>
    public static void SendBatchViaSmtp(
      BatchModel batch,
      string host,
      int port,
      string username,
      string password,
      string defaultFromName)
    {
      using var client = new SmtpClient(host, port)
      {
        EnableSsl = true,
        DeliveryMethod = SmtpDeliveryMethod.Network,
        Credentials = new System.Net.NetworkCredential(username, password),
        Timeout = 60000,
      };

      var fromName = string.IsNullOrWhiteSpace(batch.FromName) ? defaultFromName : batch.FromName;

      foreach (var recipient in batch.Recipients)
      {
        using var mailMessage = new MailMessage
        {
          From = new MailAddress(batch.FromEmail, fromName),
          Subject = batch.Subject,
          Body = batch.Message,
          IsBodyHtml = true,
        };
        mailMessage.To.Add(new MailAddress(recipient.ToEmail, recipient.ToName));

        foreach (var attachment in batch.Attachments)
        {
          var stream = new MemoryStream(attachment.Attachment);
          mailMessage.Attachments.Add(
            new Attachment(stream, attachment.AttachmentFileName, attachment.AttachmentContentType));
        }

        client.Send(mailMessage);
      }
    }
  }
}

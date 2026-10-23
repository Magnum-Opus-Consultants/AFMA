namespace AFMA.Service.Communication
{
  using AFMA.Service.Communication.Model;
  using EmailServiceReference;
  using System.Net.Mail;

  public class EmailService
  {
    private readonly EmailServiceClient _client;
    //private readonly iwims IApplicationLayer _applicationLayer;
    private readonly string _apiKey;

    public EmailService(string apiKey)
    {
      _client = new EmailServiceClient();
      //_applicationLayer = applicationLayer;
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
        //Attachments = s.Attachments.Select(a => new EmailAttachmentModel()
        //{
        //  FileName = a.FileName,
        //  FileSize = a.FileSize
        //}).ToList()
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
        //Attachments = s.Attachments.Select(a => new EmailAttachmentModel()
        //{
        //  FileName = a.FileName,
        //  FileSize = a.FileSize
        //}).ToList()
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
        //Attachments = s.Attachments.Select(a => new EmailAttachmentModel()
        //{
        //  FileName = a.FileName,
        //  FileSize = a.FileSize
        //}).ToList()
      }).ToList();
    }

    public IEnumerable<EmailMessageModel> GetError(int patientId)
    {
      return GetError().Where(x => x.CustomerId == patientId);
    }

    public bool ProcessAndSendBatch(MessageSchema batch)
    {
      //Logger.Write("Start processing batch...");

      List<RecipientSchema> rs = new List<RecipientSchema>()
      {
        new RecipientSchema
        {
          ToName = "Jacques Swart",
          ToEmail = "jacques@greengecko.co.za"
        }
      };

      batch.Attachments = new List<AttachmentSchema>().ToArray();
      batch.Recipients = rs.ToArray();
      batch.FromEmail = "afma@afma.co.za";
      batch.FromName = "AFMA";
      batch.Subject = "Test subject for submission";

      try
      {
        var emailMessage = new MessageSchema()
        {
          CostCentre = 1,
          UserId = 1009,
          FromEmail = batch.FromEmail,
          FromName = batch.FromName,
          MessageText = "Test Message",
          MessageType = "Email",
          Subject = batch.Subject,
          Attachments = batch.Attachments,
          //Attachments = batch.Attachments.Select(a => new AttachmentSchema()
          //{
          //  FileName = a.AttachmentFileName,
          //  FileContent = a.Attachment,
          //  FileSize = a.AttachmentFileSize,
          //  FileType = a.AttachmentContentType
          //}).ToArray(),
          Recipients = batch.Recipients.Select(r => new RecipientSchema()
          {
            DeliveryStatusId = 1,
            ToEmail = r.ToEmail,
            ToName = r.ToName
          }).ToArray()
        };

        var result = _client.SendMessageAsync(_apiKey, emailMessage).Result;

        //Logger.Write($"Batch processing completed: {result}");

        return true;
      }
      catch (Exception ex)
      {
        //Logger.Write(ex);
        return false;
      }
    }

    public void SendSmtpNotification(string to, string subject, string message)
    {
      var client = new SmtpClient("relay03.geckomail.co.za", 25);
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


  }
}

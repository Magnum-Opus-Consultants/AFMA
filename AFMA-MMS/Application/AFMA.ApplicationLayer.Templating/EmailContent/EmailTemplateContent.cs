namespace AFMA.ApplicationLayer.Templating.EmailContent
{
  using System.Globalization;
  using System.Text;
  using System.Text.Encodings.Web;

  public static class Submissions
  {
    public static string GenerateFeedsalesEmail(int month, int year, string recipient, string memberName)
    {
      var sb = new StringBuilder();
      sb.Append("<p>Dear AFMA Member</p>");
      sb.Append("<p>The current arrangement is that Feed Sales information must be submitted by the 20th of the month following the month in which the animal feeds have been manufactured.</p>");
      sb.Append($"<p>Your return for feed sales for {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)} {year} is currently outstanding.</p>");
      sb.Append("<p>It would be appreciated if you could submit the outstanding information as soon as possible.</p>");
      //sb.Append("<br>");
      sb.Append($"<p class='smaller' style='text-align:center'>This email was sent to {recipient} from {memberName}</p>");
      return sb.ToString();
    }

    public static string GenerateSalmonellaEmail(int month, int year, string recipient, string memberName)
    {
      var sb = new StringBuilder();
      sb.Append("<p>Dear AFMA Member</p>");
      sb.Append("<p>The current arrangement is that Salmonella reports should be submitted by the 20th of the month following the month in which the samples have been analysed.</p>");
      sb.Append($"<p>Your Salmonella report for {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)} {year} is currently outstanding.</p>");
      sb.Append("<p>Please submit the outstanding reports as soon as possible.</p>");
      //sb.Append("<br>");
      sb.Append($"<p class='smaller' style='text-align:center'>This email was sent to {recipient} from {memberName}</p>");
      return sb.ToString();
    }

    public static string GenerateMycotoxinEmail(int month, int year, string recipient, string memberName)
    {
      var sb = new StringBuilder();
      sb.Append("<p>Dear AFMA Member</p>");
      sb.Append("<p>The current arrangement is that Mycotoxin reports should be submitted by the 20th of the month following the month in which the samples have been analysed.</p>");
      sb.Append($"<p>Your Mycotoxin report for {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)} {year} is currently outstanding.</p>");
      sb.Append("<p>Please submit the outstanding reports as soon as possible.</p>");
      sb.Append($"<p class='smaller' style='text-align:center'>This email was sent to {recipient} from {memberName}</p>");
      return sb.ToString();
    }
  }

  public static class Accounts
  {
    public static string GenerateResetPasswordEmail(string accessUrl, string userName, string password)
    {
      var sb = new StringBuilder();
      sb.Append("<p>Dear AFMA Member</p>");
      sb.Append("<p>Welcome to the AFMA statistics portal. Please find your access details below.</p>");
      sb.Append("<p>");
      sb.Append("<ul>");
      sb.Append($"<li>Website URL: {accessUrl}</li>");
      sb.Append($"<li>Username: {userName}</li>");
      sb.Append($"<li>Password: {password}</li>");
      sb.Append("</ul>");
      sb.Append("</p>");
      return sb.ToString();
    }

    public static string GenerateResetPasswordRequestEmail(string callbackUrl)
    {
      var sb = new StringBuilder();
      sb.Append("<p>Dear AFMA Member</p>");
      sb.Append("<p>We have received a request to reset your password.</p>");
      sb.Append("<p>If you did not request a password reset, you can safely ignore this request.</p>");
      sb.Append("<p>Your password has not yet been changed and your account is still secure.</p>");
      sb.Append("<p>");
      sb.Append($"Click on this link to continue to reset your password: {HtmlEncoder.Default.Encode(callbackUrl)}");
      sb.Append("</p>");
      sb.Append("<p>For more information or if you have any queries or concerns, please contact the AFMA office.</p>");
      return sb.ToString();
    }
  }
}

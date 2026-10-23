namespace AFMA.Stats.Admin.ControllerExtensions
{
  using ApplicationLayer.Helpers;
  using Microsoft.AspNetCore.Mvc;

  public abstract class BaseController : Controller
  {
    public void Alert(string message, string title, Enums.NotificationType notificationType)
    {
      var msg = "<script language='javascript'>swal('" + title.ToUpper() + "', '" + message + "','" + notificationType + "')" + "</script>";
      TempData["notification"] = msg;
    }

    public void Message(string message, Enums.NotificationType notifyType)
    {
      TempData["Notification2"] = message;

      switch (notifyType)
      {
        case Enums.NotificationType.success:
          TempData["NotificationCSS"] = "alert-box success";
          break;
        case Enums.NotificationType.error:
          TempData["NotificationCSS"] = "alert-box errors";
          break;
        case Enums.NotificationType.warning:
          TempData["NotificationCSS"] = "alert-box warning";
          break;
        case Enums.NotificationType.info:
          TempData["NotificationCSS"] = "alert-box notice";
          break;
      }
    }
  }
}

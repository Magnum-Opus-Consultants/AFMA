namespace AFMA.Web.Admin.Controllers
{
  using AFMA.Infrastructure.Extensions;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;

  [Authorize(Roles = "Admin")]
  public class CertificateController : BaseController
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}

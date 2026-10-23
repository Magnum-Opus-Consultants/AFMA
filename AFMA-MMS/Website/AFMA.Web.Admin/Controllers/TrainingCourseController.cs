namespace AFMA.Web.Admin.Controllers
{
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;


  [Authorize(Roles = "Admin")]
  public class TrainingCourseController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}

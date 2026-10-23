namespace AFMA.Web.Admin.Controllers
{
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;

  [Authorize]
  public class MenuController() : Controller
  {
    public async Task<IActionResult> Index()
    {
      return View();
    }

    public IActionResult Create()
    {
      return View();
    }

    public async Task<IActionResult> Edit(int id)
    {
      return View();

    }
  }
}

using Microsoft.AspNetCore.Mvc;

namespace WebAppMVC.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "Users";

            ViewBag.Country = "CA";

            return View();
        }
    }
}

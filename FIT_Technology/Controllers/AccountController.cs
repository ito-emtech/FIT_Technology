using Microsoft.AspNetCore.Mvc;

namespace FIT_Technology.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(nameof(AccountController.Login));
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return View(nameof(AccountController.Logout));
        }

        [HttpGet]
        public IActionResult Menu()
        {
            return View(nameof(AccountController.Menu));
        }
    }
}

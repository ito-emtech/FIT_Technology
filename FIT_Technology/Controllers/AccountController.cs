using Microsoft.AspNetCore.Mvc;

namespace FIT_Technology.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

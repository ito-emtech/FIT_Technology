using Microsoft.AspNetCore.Mvc;

namespace FIT_Technology.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

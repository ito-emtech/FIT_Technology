using Microsoft.AspNetCore.Mvc;

namespace FIT_Technology.Controllers
{
    public class EmployeeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(nameof(EmployeeController.Index));
        }

        [HttpGet]
        public IActionResult List()
        {
            return View(nameof(EmployeeController.List));
        }

        [HttpGet]
        public IActionResult Insert()
        {
            return View(nameof(EmployeeController.Insert));
        }

        [HttpGet]
        public IActionResult Alert()
        {
            return View(nameof(EmployeeController.Alert));
        }

        [HttpGet]
        public IActionResult Update()
        {
            return View(nameof(EmployeeController.Update));
        }
    }
}

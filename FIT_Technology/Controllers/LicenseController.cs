using Microsoft.AspNetCore.Mvc;

namespace FIT_Technology.Controllers
{
    public class LicenseController : Controller
    {

        [HttpGet]
        public IActionResult Index()
        {
            return View(nameof(LicenseController.Index));
        }

        [HttpGet]
        public IActionResult LicenseMenu()
        {
            return View(nameof(LicenseController.LicenseMenu));
        }

        [HttpGet]
        public IActionResult Insert()
        {
            return View(nameof(LicenseController.Insert));
        }

        [HttpGet]
        public IActionResult List()
        {
            return View(nameof(LicenseController.List));
        }

        [HttpGet]
        public IActionResult Alert()
        {
            return View(nameof(LicenseController.Alert));
        }
    }
}

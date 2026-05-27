using DynamicDll.Db;
using Microsoft.AspNetCore.Mvc;
using FIT_Technology.Models.Services;
using FIT_Technology.Models.Entities;
using FIT_Technology.Models.Constants;

namespace FIT_Technology.Controllers
{
    public class AccountController(AccountService service) : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string name)
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(nameof(AccountController.Login));
        }

        [HttpPost]
        public IActionResult Login(UserEntity user)
        {
            if(string.IsNullOrEmpty(user.UserId) || string.IsNullOrEmpty(user.Password))
            {
                return RedirectToAction(nameof(AccountController.Login));
            }

            if (service.Authenticate(user.UserId, user.Password))
            {
                HttpContext.Session.SetString(DbConstants.SessionKeys.UserId, user.UserId);
                return RedirectToAction(nameof(AccountController.Menu));
            }
            else
            {
                return RedirectToAction(nameof(ResultController.Index));
            }
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

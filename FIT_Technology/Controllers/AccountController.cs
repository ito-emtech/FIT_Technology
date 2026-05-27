using DynamicDll.Db;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using FIT_Technology.Models.Daos;

namespace FIT_Technology.Controllers
{
    public class AccountController : Controller
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
        public IActionResult Login(string userid, string password)
        {
            throw new NotImplementedException();

            using (TranMng tm = TranMng.BeginTransaction("empdb"))
            {
                AccountDao accountDao = new AccountDao();

                var userName = AccountDao.Login(userid, password);

                if (string.IsNullOrEmpty(userName))
                {
                    return RedirectToAction(nameof(Error));
                }
                else
                {
                    HttpContext.Session.SetString(nameof(userName), userName);
                    HttpContext.Session.SetString(nameof(userid), userid);
                    return RedirectToAction(nameof(HomeController.Menu));
                }
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

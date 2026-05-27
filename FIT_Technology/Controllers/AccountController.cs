using FIT_Technology.Filters;
using FIT_Technology.Models.Constants;
using FIT_Technology.Models.Entities;
using FIT_Technology.Models.Helpers;
using FIT_Technology.Models.Services;
using Microsoft.AspNetCore.Mvc;

namespace FIT_Technology.Controllers
{
    public class AccountController(AccountService service) : Controller
    {
        // ログインしていない状態でアクセスしても、FilterがLoginへ飛ばしてくれる
        [HttpGet]
        [SessionCheck]
        public IActionResult Index()
        {
            ViewBag.Title = "ホーム画面";
            return View(nameof(AccountController.Menu));
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Title = "ログイン画面";
            return View(nameof(AccountController.Login));
        }

        // 重要：[SessionCheck]は付けない（ログイン前なので）
        [HttpPost]
        public IActionResult Login(UserEntity user)
        {
            ViewBag.Title = "再入力画面";

            if (!ModelState.IsValid) { return View(nameof(AccountController.Login)); }

            if (service.Authenticate(user.UserId, user.Password))
            {
                HttpContext.Session.SetString(DbConstants.SessionKeys.UserId, user.UserId);
                return RedirectToAction(nameof(AccountController.Menu), Ctrl.Get<AccountController>());
            }
            else
            {
                ViewBag.ViewTitle = "ログインエラー";
                ViewBag.Msg = "ログインに失敗しました";
                return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
            }
        }

        [HttpGet]
        [SessionCheck]
        public IActionResult Logout()
        {
            ViewBag.Title = "ログアウト確認画面";
            // 💡 セッションチェックはFilterに任せたので、ここはViewを返すだけでOK
            return View(nameof(AccountController.Logout));
        }

        [HttpPost]
        [SessionCheck]
        public IActionResult Logout(string btn_action)
        {
            ViewBag.Title = "ログアウト画面";

            var userId = HttpContext.Session.GetString(DbConstants.SessionKeys.UserId);
            ViewBag.Msg = $"{userId}様、お疲れさまでした";

            HttpContext.Session.Clear();
            return RedirectToAction(nameof(AccountController.Login), Ctrl.Get<AccountController>());
        }

        [HttpGet]
        [SessionCheck]
        public IActionResult Menu()
        {
            ViewBag.Title = "従業員管理システム画面";
            return View(nameof(AccountController.Menu));
        }

        [HttpPost]
        [SessionCheck]
        public IActionResult Menu(string btn_action)
        {
            ViewBag.Title = "従業員管理システム画面";

            switch (btn_action)
            {
                case "logout":
                    return RedirectToAction(nameof(AccountController.Logout), Ctrl.Get<AccountController>());
                case "insert":
                case "list":
                    return RedirectToAction(nameof(EmployeeController.Index), Ctrl.Get<EmployeeController>());
                case "license":
                    return RedirectToAction(nameof(LicenseController.Index), Ctrl.Get<LicenseController>());
                default:
                    ViewBag.Caption = "不正な入力を検知";
                    ViewBag.Msg = "不正コマンドを検出しました";
                    return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
            }
        }
    }
}
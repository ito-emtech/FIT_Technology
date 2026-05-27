using FIT_Technology.Models.Constants;
using Microsoft.AspNetCore.Mvc;
using FIT_Technology.Models.Helpers; // Ctrlクラスの名前空間を追加

namespace FIT_Technology.Controllers
{
    public class ResultController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Title = "エラー画面";

            // 直接このURLを叩かれた場合のデフォルト
            if (ViewBag.Msg == null)
            {
                ViewBag.ViewTitle = "案内";
                ViewBag.Msg = "表示するメッセージはありません。";
            }
            return View(nameof(ResultController.Index));
        }

        [HttpPost]
        public IActionResult Index(string btn_action)
        {
            ViewBag.Title = "案内画面";

            if (string.IsNullOrEmpty(HttpContext.Session.GetString(DbConstants.SessionKeys.UserId)))
            {
                ViewBag.ViewTitle = "ログインエラー";
                ViewBag.Msg = "すでにログアウトされているか、セッションが切れています";

                return RedirectToAction(nameof(AccountController.Index), Ctrl.Get<AccountController>());
            }

            return RedirectToAction(nameof(AccountController.Menu), Ctrl.Get<AccountController>());
        }
    }
}
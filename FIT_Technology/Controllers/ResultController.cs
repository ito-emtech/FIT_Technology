using FIT_Technology.Filters;
using FIT_Technology.Models.Constants;
using FIT_Technology.Models.Helpers; // Ctrlクラスの名前空間を追加
using Microsoft.AspNetCore.Mvc;

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
        [SessionCheck]
        public IActionResult Index(string btn_action)
        {
            ViewBag.Title = ViewBag.Title ?? "案内画面";
            ViewBag.Msg = ViewBag.Msg ?? "表示するメッセージはありません";

            return RedirectToAction(nameof(AccountController.Menu), Ctrl.Get<AccountController>());
        }
    }
}
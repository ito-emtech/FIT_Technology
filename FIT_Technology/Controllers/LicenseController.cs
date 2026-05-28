using FIT_Technology.Models.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace FIT_Technology.Controllers
{
    public class LicenseController : Controller
    {

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(LicenseController.LicenseMenu));
        }

        [HttpGet]
        public IActionResult LicenseMenu()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LicenseMenu(string btn_action)
        {
            switch (btn_action)
            {
                case "insert":
                    // 「登録」ボタンが押されたら登録画面へ
                    //return RedirectToAction("Insert");
                    return RedirectToAction(nameof(LicenseController.Insert), Ctrl.Get<LicenseController>());
                case "list":
                    // 「資格一覧表示」ボタンが押されたら、一覧画面へ
                    //return RedirectToAction("List");
                    return RedirectToAction(nameof(LicenseController.List), Ctrl.Get<LicenseController>());
                case "cancel":
                    // 「戻る」ボタンが押されたらIndex（トップ画面）へ
                    //return RedirectToAction("Index", "License");
                    //下の意味と同じ
                    return RedirectToAction(nameof(LicenseController.Index), Ctrl.Get<AccountController>());

                default:
                    // どれにも当てはまらない場合は、今のメニュー画面を再表示
                    return View();
            }
        }

        [HttpGet]
        public IActionResult Insert()
        {
            return View(nameof(LicenseController.Insert));
        }

        [HttpPost]
        public IActionResult Insert(string btn_action)
        {
            switch (btn_action)
            {
                case "result":
                    // 「登録」ボタンが押されたら完了画面へ

                    return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());

                case "cancel":
                    // 「戻る」ボタンが押されたら保有資格管理画面へ

                    return RedirectToAction(nameof(LicenseController.LicenseMenu), Ctrl.Get<LicenseController>());

                default:
                    // どれにも当てはまらない場合は、今のメニュー画面を再表示
                    return View();
            }
        }

        [HttpGet]
        public IActionResult List()
        {

            return View();

        }

        [HttpPost]
        public IActionResult List(string btn_action)
        {
            switch (btn_action)
            {
                case "alert":
                    // 「削除」ボタンが押されたら削除警告画面へ
                    return RedirectToAction(nameof(LicenseController.Alert), Ctrl.Get<LicenseController>());

                case "cancel":
                    // 「戻る」ボタンが押されたら保有資格管理画面へ
                    return RedirectToAction(nameof(LicenseController.LicenseMenu), Ctrl.Get<LicenseController>());

                default:
                    // どれにも当てはまらない場合は、今のメニュー画面を再表示
                    return View();
            }
        }

        [HttpGet]
        public IActionResult Alert()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Alert(string btn_action)
        {
            switch (btn_action)
            {
                case "result":
                    // 「削除」ボタンが押されたら完了画面へ

                    return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());

                case "cancel":
                    // 「キャンセル」ボタンが押されたら保有資格一覧画面へ
                    return RedirectToAction(nameof(LicenseController.List), Ctrl.Get<LicenseController>());

                default:
                    // どれにも当てはまらない場合は、今のメニュー画面を再表示
                    return View();
            }
        }
    }
}

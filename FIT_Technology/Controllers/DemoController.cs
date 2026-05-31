using FIT_Technology.Filters;
using FIT_Technology.Models.Constants;
using FIT_Technology.Models.Entities;
using FIT_Technology.Models.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace FIT_Technology.Controllers
{
    [SessionCheck]
    public class DemoController : Controller
    {
        public static class TempKeys
        {
            public const string CreateEmployee = "createEmployee";
            public const string DeleteLicense = "deleteLicense";
        }

        /// <summary>
        /// 従業員管理システムのデフォルト入り口
        /// </summary>
        /// <remarks>
        /// トップ画面にリダイレクトします
        /// </remarks>
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Title = "デフォルト画面";
            return RedirectToAction(
                nameof(DemoController.LicenseMenu),
                Ctrl.Get<DemoController>());

            /* トップメニュー画面 */
            //return RedirectToAction(
            //    nameof(AccountController.Menu),
            //    Ctrl.Get<AccountController>()
            //    );
        }

        /// <summary>
        /// 保有資格の管理メニュー画面を表示します
        /// </summary>
        /// <remarks>
        /// 従業員の一覧を表示します
        /// チェックボタンで従業員の選択が行えます
        /// 選択された従業員に対する処理を送信ボタンで決定します
        /// 戻るボタンでトップメニューに戻れます
        /// </remarks>
        [HttpGet]
        public IActionResult LicenseMenu()
        {
            // タグに表示されるタイトル
            ViewBag.Title = "保有資格管理システム画面";
            // 画面上に表示されるタイトル
            ViewBag.ViewTitle = "保有資格管理システム";

            List<EmployeeEntity> entities = new List<EmployeeEntity>();

            return View(entities);
        }

        /// <summary>
        /// 保有資格の管理メニュー画面からのアクション振り分け
        /// </summary>
        /// <remarks>
        /// 従業員の一覧を表示します
        /// チェックボタンで従業員の選択が行えます
        /// 選択された従業員に対する処理を送信ボタンで決定します
        /// ・従業員の新規登録
        /// ・選択者の保有資格一覧を表示
        /// </remarks>
        [HttpPost]
        public IActionResult LicenseMenu(string btn_action, string emp_cd)
        {
            ViewBag.Title = "保有資格管理システム";

            if (string.IsNullOrEmpty(btn_action))
            {
                ViewBag.ErrorMsg = "操作を選択してください";
                return View();
            }

            // デモとして追加
            if(btn_action == "employeeInsert")
            {
                // 従業員の新規登録
                return RedirectToAction(
                    nameof(DemoController.EmployeeInsert),
                    Ctrl.Get<DemoController>());
            }

            if(string.IsNullOrEmpty(emp_cd))
            {
                // ErrorMsgがある時はタイトルの下に表示
                ViewBag.ErrorMsg = "従業員が選択されていません";
                View();
            }

            switch (btn_action)
            {
                case ActionValues.Insert:
                    // 選択者の資格登録
                    TempData[TempKeys.CreateEmployee] = emp_cd ?? "D001";
                    return RedirectToAction(
                        nameof(DemoController.Insert), 
                        Ctrl.Get<DemoController>());

                case ActionValues.List:
                    // 選択者の資格一覧
                    return RedirectToAction(
                        nameof(DemoController.List), 
                        Ctrl.Get<DemoController>());

                default:
                    return this.RedirectToResult(
                        viewTitle: "不正な入力を検知",
                        msg: "メニュー画面から不正コマンドを検出しました。",
                        caption: "再度トップメニュー画面から操作を行ってください");
            }
        }

        /// <summary>
        /// 従業員新規登録画面を表示
        /// </summary>
        /// <remarks>
        /// 新規従業員の入力を行う画面です
        /// リンクで"LicenseMenu"に戻れます
        /// </remarks>
        [HttpGet]
        public IActionResult EmployeeInsert()
        {
            ViewBag.Title = "従業員新規登録";

            return View();
        }

        /// <summary>
        /// 従業員新規登録を行う
        /// </summary>
        /// <remarks>
        /// 入力項目のチェックを行います
        /// 従業員の登録を行います
        /// </remarks>
        [HttpPost]
        public IActionResult EmployeeInsert(EmployeeEntity entity)
        {
            ViewBag.Title = "再入力画面";
            ViewBag.ViewTitle = "従業員新規登録";

            //if (!ModelState.IsValid) { return View(); }

            return this.RedirectToResult(
                viewTitle: "従業員新規登録",
                msg: "従業員の新規登録が完了しました。",
                caption: "管理メニューで従業員の確認が可能です");
        }

        /// <summary>
        /// 保有資格の登録画面
        /// </summary>
        /// <remarks>
        /// 選択された従業員に対して、新規資格の登録を行います
        /// 登録資格の入力を行う画面です
        /// リンクで"LicenseMenu"に戻れます
        /// </remarks>
        [HttpGet]
        public IActionResult Insert()
        {
            ViewBag.Title = "資格登録画面";
            ViewBag.ViewTitle = "資格登録";

            // タイトルの下に登録者名を表示
            // 従業員コード[NNNN] : 氏名（フリガナ）
            string emp_cd = (string)(TempData[TempKeys.CreateEmployee] ?? string.Empty);
            ViewBag.ViewName = emp_cd;

            return View();
        }

        /// <summary>
        /// 保有資格の登録画面
        /// </summary>
        /// <remarks>
        /// 選択された従業員に対して、新規資格の登録を行います
        /// 登録資格の入力を行う画面です
        /// </remarks>
        [HttpPost]
        public IActionResult Insert(GetLicenseEntity entity)
        {
            //if (!ModelState.IsValid) { return View(); }

            return this.RedirectToResult(
                viewTitle: "資格登録完了",
                msg: "資格の新規登録が完了しました。",
                caption: "管理メニューから保有資格の確認が可能です");
        }

        /// <summary>
        /// 保有資格の管理メニュー画面を表示します
        /// </summary>
        /// <remarks>
        /// 選択された従業員の資格一覧を表示します
        /// チェックボタンで保有資格の選択が行えます
        /// 選択された資格に対する処理を送信ボタンで決定します
        /// リンクで"LicenseMenu"に戻れます
        /// </remarks>
        [HttpGet]
        public IActionResult List()
        {
            List<GetLicenseEntity> entities = new List<GetLicenseEntity>();
            return View(entities);
        }

        /// <summary>
        /// 保有資格の管理メニュー画面を表示します
        /// </summary>
        /// <remarks>
        /// 選択された資格に対する処理を送信ボタンで決定します
        /// </remarks>
        [HttpPost]
        public IActionResult List(string btn_action, string[] license_cd)
        {
            if (string.IsNullOrEmpty(btn_action) || license_cd == null)
            {
                ViewBag.ErrorMsg = "操作を選択してください";
                return View();
            }

            if(license_cd.Length <= 0)
            {
                ViewBag.ErrorMsg = "資格が選択されていません";
                return View();
            }

            // 現在は削除ボタンのみ実装予定
            switch (btn_action)
            {
                case ActionValues.Alert:
                    TempData[TempKeys.DeleteLicense] = license_cd;

                    return RedirectToAction(
                        nameof(DemoController.Alert),
                        Ctrl.Get<DemoController>());
                default:
                    return this.RedirectToResult(
                        viewTitle: "不正な入力を検知",
                        msg: "保有資格メニュー画面から不正コマンドを検出しました。",
                        caption: "再度トップメニュー画面から操作を行ってください");
            }
        }

        /// <summary>
        /// 保有資格の削除確認画面を表示します
        /// </summary>
        /// <remarks>
        /// 選択された資格を表示します
        /// リンクで"List"に戻れます
        /// </remarks>
        [HttpGet]
        public IActionResult Alert()
        {
            string[] license_codes = (string[])(TempData[TempKeys.DeleteLicense] ?? string.Empty);

            List<GetLicenseEntity> entities = new List<GetLicenseEntity>();
            foreach (string code in license_codes)
            {
                var entity = new GetLicenseEntity();
                entities.Add(entity);
            }

            return View(entities);
        }

        /// <summary>
        /// 保有資格の削除を行います
        /// </summary>
        /// <remarks>
        /// 選択された資格に対する処理を送信ボタンで決定します
        /// </remarks>
        [HttpPost]
        public IActionResult Alert(List<GetLicenseEntity> entities)
        {
            return this.RedirectToResult(
                viewTitle: "資格削除完了",
                msg: "保有資格の削除が完了しました。",
                caption: "管理メニューから保有資格の確認が可能です");
        }
    }
}
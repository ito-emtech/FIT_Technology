using FIT_Technology.Filters;
using FIT_Technology.Models.Constants;
using FIT_Technology.Models.Entities;
using FIT_Technology.Models.Helpers;
using FIT_Technology.Models.Services;
using FIT_Technology.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace FIT_Technology.Controllers
{
    [SessionCheck]
    public class DemoController : Controller
    {
        // 2つのサービスをプライベート変数として保持
        private readonly DemoEmployeeService _employeeService;
        private readonly DemoGetLicenseService _licenseService; // ★追加

        /// <summary>
        /// コンストラクタで各サービスをインスタンス化
        /// </summary>
        public DemoController()
        {
            _employeeService = new DemoEmployeeService();
            _licenseService = new DemoGetLicenseService(); // ★追加
        }

        public static class TempKeys
        {
            public const string CreateEmployee = "createEmployee";
            public const string DeleteLicense = "deleteLicense";
            public const string TargetEmpCd = "targetEmpCd"; // ★遷移先での表示名構築用キー
        }

        /// <summary>
        /// 従業員管理システムのデフォルト入り口
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Title = "デフォルト画面";
            return RedirectToAction(
                nameof(DemoController.LicenseMenu),
                Ctrl.Get<DemoController>());
        }

        /// <summary>
        /// 保有資格の管理メニュー画面を表示します（従業員一覧表示）
        /// </summary>
        [HttpGet]
        public IActionResult LicenseMenu()
        {
            ViewBag.Title = "保有資格管理システム画面";
            ViewBag.ViewTitle = "保有資格管理システム";

            List<LicenseMenuRowViewModel> model = _employeeService.GetLicenseMenuRows();
            return View(model);
        }

        /// <summary>
        /// 保有資格の管理メニュー画面からのアクション振り分け
        /// </summary>
        [HttpPost]
        public IActionResult LicenseMenu(string btn_action, string emp_cd)
        {
            ViewBag.Title = "保有資格管理システム";
            ViewBag.ViewTitle = "保有資格管理システム";

            if (string.IsNullOrEmpty(btn_action))
            {
                ViewBag.ErrorMsg = "操作を選択してください";
                return View(_employeeService.GetLicenseMenuRows());
            }

            if (btn_action == "employeeInsert")
            {
                return RedirectToAction(
                    nameof(DemoController.EmployeeInsert),
                    Ctrl.Get<DemoController>());
            }

            if (string.IsNullOrEmpty(emp_cd))
            {
                ViewBag.ErrorMsg = "従業員が選択されていません";
                return View(_employeeService.GetLicenseMenuRows());
            }

            // 後続の「資格登録」「資格一覧」画面で、誰の情報を扱っているか識別・表示するためにTempDataに保存
            TempData[TempKeys.TargetEmpCd] = emp_cd;

            switch (btn_action)
            {
                case ActionValues.Insert:
                    TempData[TempKeys.CreateEmployee] = emp_cd;
                    return RedirectToAction(
                        nameof(DemoController.Insert),
                        Ctrl.Get<DemoController>());

                case ActionValues.List:
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
        [HttpGet]
        public IActionResult EmployeeInsert()
        {
            ViewBag.Title = "従業員新規登録";
            ViewBag.ViewTitle = "従業員新規登録";

            ViewBag.Sections = _employeeService.GetSections();
            ViewBag.Genders = _employeeService.GetGenders();

            return View(new EmployeeEntity());
        }

        /// <summary>
        /// 従業員新規登録を行う
        /// </summary>
        [HttpPost]
        public IActionResult EmployeeInsert(EmployeeEntity entity)
        {
            ViewBag.Title = "再入力画面";
            ViewBag.ViewTitle = "従業員新規登録";

            if (entity == null)
            {
                ViewBag.ErrorMsg = "入力データが不正です。";
                ViewBag.Sections = _employeeService.GetSections();
                ViewBag.Genders = _employeeService.GetGenders();
                return View(new EmployeeEntity());
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Sections = _employeeService.GetSections();
                ViewBag.Genders = _employeeService.GetGenders();
                return View(entity);
            }

            bool isSuccess = _employeeService.RegisterEmployee(entity);

            if (!isSuccess)
            {
                ViewBag.ErrorMsg = "従業員コードが既に登録されているか、登録処理中にエラーが発生しました。";
                ViewBag.Sections = _employeeService.GetSections();
                ViewBag.Genders = _employeeService.GetGenders();
                return View(entity);
            }

            return this.RedirectToResult(
                viewTitle: "従業員新規登録",
                msg: "従業員の新規登録が完了しました。",
                caption: "管理メニューで従業員の確認が可能です");
        }

        /// <summary>
        /// 保有資格の登録画面
        /// </summary>
        [HttpGet]
        public IActionResult Insert()
        {
            ViewBag.Title = "資格登録画面";
            ViewBag.ViewTitle = "資格登録";

            string emp_cd = (string)(TempData[TempKeys.CreateEmployee] ?? string.Empty);

            EmployeeEntity emp = _employeeService.GetEmployee(emp_cd);
            string name = emp != null ? $"{emp.LastNm} {emp.FirstNm}" : "未知の従業員";
            ViewBag.ViewName = $"［{emp_cd}］{name}";

            // ★追加：セレクトボックス用の資格マスタ一覧を取得して渡す
            ViewBag.Licenses = _licenseService.GetLicenses();

            TempData.Keep(TempKeys.CreateEmployee);
            TempData.Keep(TempKeys.TargetEmpCd);

            var entity = new GetLicenseEntity
            {
                EmpCd = emp_cd,
                GetLicenseDate = DateTime.Today
            };

            return View(entity);
        }

        /// <summary>
        /// 保有資格の登録を行う
        /// </summary>
        [HttpPost]
        public IActionResult Insert(GetLicenseEntity entity)
        {
            ViewBag.Title = "資格登録画面";
            ViewBag.ViewTitle = "資格登録";

            if (entity == null)
            {
                ViewBag.ErrorMsg = "入力データが不正です。";
                ViewBag.Licenses = _licenseService.GetLicenses(); // ★追加
                return View(new GetLicenseEntity());
            }

            if (!ModelState.IsValid)
            {
                string emp_cd = entity.EmpCd ?? string.Empty;
                EmployeeEntity emp = _employeeService.GetEmployee(emp_cd);
                string name = emp != null ? $"{emp.LastNm} {emp.FirstNm}" : "未知の従業員";
                ViewBag.ViewName = $"［{emp_cd}］{name}";

                // ★追加：入力エラーで画面に戻る際も選択肢が消えないよう再取得
                ViewBag.Licenses = _licenseService.GetLicenses();

                return View(entity);
            }

            bool isSuccess = _licenseService.RegisterLicense(entity);
            if (!isSuccess)
            {
                ViewBag.ErrorMsg = "この資格は既に登録されているか、登録処理中にエラーが発生しました。";

                string emp_cd = entity.EmpCd ?? string.Empty;
                EmployeeEntity emp = _employeeService.GetEmployee(emp_cd);
                string name = emp != null ? $"{emp.LastNm} {emp.FirstNm}" : "未知の従業員";
                ViewBag.ViewName = $"［{emp_cd}］{name}";

                // ★追加：エラーで画面に戻る際も選択肢が消えないよう再取得
                ViewBag.Licenses = _licenseService.GetLicenses();

                return View(entity);
            }

            return this.RedirectToResult(
                viewTitle: "資格登録完了",
                msg: "資格の新規登録が完了しました。",
                caption: "管理メニューから保有資格の確認が可能です");
        }

        /// <summary>
        /// 保有資格の一覧画面を表示します
        /// </summary>
        [HttpGet]
        public IActionResult List()
        {
            // ★モック固定値「D001」を廃止し、メニューで選ばれた従業員コードを取得
            string emp_cd = (string)(TempData[TempKeys.TargetEmpCd] ?? string.Empty);

            EmployeeEntity emp = _employeeService.GetEmployee(emp_cd);
            string name = emp != null ? $"{emp.LastNm} {emp.FirstNm}" : "未知の従業員";
            ViewBag.ViewName = $"［{emp_cd}］{name}";

            TempData.Keep(TempKeys.TargetEmpCd);

            // ★サービスを呼び出して、この従業員が持つリアルな保有資格リストをViewに渡す
            List<GetLicenseEntity> entities = _licenseService.GetLicensesByEmpCd(emp_cd);
            return View(entities);
        }

        /// <summary>
        /// 保有資格の一覧画面からのアクション振り分け
        /// </summary>
        [HttpPost]
        public IActionResult List(string btn_action, string[] license_cd)
        {
            string emp_cd = (string)(TempData[TempKeys.TargetEmpCd] ?? string.Empty);
            EmployeeEntity emp = _employeeService.GetEmployee(emp_cd);
            string name = emp != null ? $"{emp.LastNm} {emp.FirstNm}" : "未知の従業員";
            ViewBag.ViewName = $"［{emp_cd}］{name}";

            TempData.Keep(TempKeys.TargetEmpCd);

            if (string.IsNullOrEmpty(btn_action) || license_cd == null)
            {
                ViewBag.ErrorMsg = "操作を選択してください";
                return View(_licenseService.GetLicensesByEmpCd(emp_cd));
            }

            if (license_cd.Length <= 0)
            {
                ViewBag.ErrorMsg = "資格が選択されていません";
                return View(_licenseService.GetLicensesByEmpCd(emp_cd));
            }

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
        /// 保有資格の削除確認画面を表示します（資格名・正確な取得日付き）
        /// </summary>
        [HttpGet]
        public IActionResult Alert()
        {
            string[] license_codes = (string[])(TempData[TempKeys.DeleteLicense] ?? Array.Empty<string>());
            string emp_cd = (string)(TempData[TempKeys.TargetEmpCd] ?? string.Empty);

            EmployeeEntity emp = _employeeService.GetEmployee(emp_cd);
            string name = emp != null ? $"{emp.LastNm} {emp.FirstNm}" : "未知の従業員";
            ViewBag.ViewName = $"［{emp_cd}］{name}";

            List<GetLicenseEntity> entities = new List<GetLicenseEntity>();

            foreach (string code in license_codes)
            {
                // ★修正点: new ではなく、DAO(サービス経由)でDBから資格名や正しい取得日が入った完全なデータを取得します
                GetLicenseEntity entity = _licenseService.GetLicense(emp_cd, code);

                if (entity != null)
                {
                    entities.Add(entity);
                }
            }

            TempData.Keep(TempKeys.DeleteLicense);
            TempData.Keep(TempKeys.TargetEmpCd);

            return View(entities);
        }

        /// <summary>
        /// 保有資格の削除を行います
        /// </summary>
        [HttpPost]
        public IActionResult Alert(List<GetLicenseEntity> entities)
        {
            if (entities == null || entities.Count == 0)
            {
                return this.RedirectToResult(
                    viewTitle: "エラー",
                    msg: "削除対象の情報が正常に送信されませんでした。",
                    caption: "再度一覧画面からやり直してください");
            }

            if (!ModelState.IsValid)
            {
                return View(entities);
            }

            // ★サービスの一括削除メソッド（トランザクション制御付き）を適用！
            bool isSuccess = _licenseService.RemoveLicenses(entities);
            if (!isSuccess)
            {
                return this.RedirectToResult(
                    viewTitle: "エラー",
                    msg: "対象の資格を削除できませんでした。既に削除されている可能性があります。",
                    caption: "再度一覧画面から確認を行ってください");
            }

            return this.RedirectToResult(
                viewTitle: "資格削除完了",
                msg: "保有資格の削除が完了しました。",
                caption: "管理メニューから保有資格の確認が可能です");
        }
    }
}
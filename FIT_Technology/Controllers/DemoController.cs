using FIT_Technology.Filters;
using FIT_Technology.Models.Constants;
using FIT_Technology.Models.Entities;
using FIT_Technology.Models.Helpers;
using FIT_Technology.Models.Services;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace FIT_Technology.Controllers
{
    [SessionCheck]
    public class DemoController : Controller
    {
        // 従業員サービスを保持するプライベート変数
        private readonly DemoEmployeeService _employeeService;

        /// <summary>
        /// コンストラクタでサービスを注入します
        /// </summary>
        public DemoController()
        {
            // DIコンテナを導入していないシンプルな構造を想定し、手動でインスタンス化
            // （DIが設定されている環境であれば、引数に DemoEmployeeService service を定義してください）
            _employeeService = new DemoEmployeeService();
        }

        public static class TempKeys
        {
            public const string CreateEmployee = "createEmployee";
            public const string DeleteLicense = "deleteLicense";
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

            // 【サービス適用】データベースから本物の従業員一覧を取得してViewに渡す
            List<EmployeeEntity> entities = _employeeService.GetEmployees();

            return View(entities);
        }

        /// <summary>
        /// 保有資格の管理メニュー画面からのアクション振り分け
        /// </summary>
        [HttpPost]
        public IActionResult LicenseMenu(string btn_action, string emp_cd)
        {
            ViewBag.Title = "保有資格管理システム";
            ViewBag.ViewTitle = "保有資格管理システム";

            // btn_action の null または 空チェック
            if (string.IsNullOrEmpty(btn_action))
            {
                ViewBag.ErrorMsg = "操作を選択してください";
                // 再表示用に現在の最新一覧を渡す
                return View(_employeeService.GetEmployees());
            }

            // デモとして追加
            if (btn_action == "employeeInsert")
            {
                return RedirectToAction(
                    nameof(DemoController.EmployeeInsert),
                    Ctrl.Get<DemoController>());
            }

            // emp_cd の null または 空チェック
            if (string.IsNullOrEmpty(emp_cd))
            {
                ViewBag.ErrorMsg = "従業員が選択されていません";
                // 再表示用に現在の最新一覧を渡す
                return View(_employeeService.GetEmployees());
            }

            switch (btn_action)
            {
                case ActionValues.Insert:
                    TempData[TempKeys.CreateEmployee] = emp_cd;
                    return RedirectToAction(
                        nameof(DemoController.Insert),
                        Ctrl.Get<DemoController>());

                case ActionValues.List:
                    // 本来は選択された emp_cd を次画面に引き継ぐため TempData やクエリパラメータにセットするべきですが、
                    // 現状の構成を維持しています。
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

            // 1. 引数オブジェクト自体の null チェック
            if (entity == null)
            {
                ViewBag.ErrorMsg = "入力データが不正です。";
                return View(new EmployeeEntity());
            }

            // 2. DataAnnotations に基づく入力検証 (ModelState.IsValid)
            if (!ModelState.IsValid)
            {
                // 検証エラーがある場合は、入力内容を保持したまま登録画面へ戻す
                return View(entity);
            }

            // 【サービス適用】DBに新従業員を登録する
            bool isSuccess = _employeeService.RegisterEmployee(entity);

            if (!isSuccess)
            {
                // 重複コードやシステムエラーがあった場合のハンドリング
                ViewBag.ErrorMsg = "従業員コードが既に登録されているか、登録処理中にエラーが発生しました。";
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

            // 1. TempDataから4文字の純粋な従業員コードを取得
            string emp_cd = (string)(TempData[TempKeys.CreateEmployee] ?? string.Empty);
            string name = "田中 太郎";
            ViewBag.ViewName = $"［{emp_cd}］{name}";

            // TempData は一度読み込むと消えるため、再入力に備えてキープしておく
            TempData.Keep(TempKeys.CreateEmployee);

            var entity = new GetLicenseEntity
            {
                EmpCd = emp_cd
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

            // 1. 引数オブジェクト自体の null チェック
            if (entity == null)
            {
                ViewBag.ErrorMsg = "入力データが不正です。";
                return View(new GetLicenseEntity());
            }

            // 2. 入力検証 (ModelState.IsValid)
            if (!ModelState.IsValid)
            {
                // 再表示用に表示名（ViewBag.ViewName）を再構築
                string emp_cd = entity.EmpCd ?? string.Empty;
                string name = "田中 太郎";
                ViewBag.ViewName = $"［{emp_cd}］{name}";

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
            string emp_cd = "D001";
            string name = "田中 太郎";
            ViewBag.ViewName = $"［{emp_cd}］{name}";

            List<GetLicenseEntity> entities = new List<GetLicenseEntity>();
            return View(entities);
        }

        /// <summary>
        /// 保有資格の一覧画面からのアクション振り分け
        /// </summary>
        [HttpPost]
        public IActionResult List(string btn_action, string[] license_cd)
        {
            string emp_cd = "D001";
            string name = "田中 太郎";
            ViewBag.ViewName = $"［{emp_cd}］{name}";

            // btn_action および 配列自体の null チェック
            if (string.IsNullOrEmpty(btn_action) || license_cd == null)
            {
                ViewBag.ErrorMsg = "操作を選択してください";
                return View(new List<GetLicenseEntity>());
            }

            // 配列の要素数チェック
            if (license_cd.Length <= 0)
            {
                ViewBag.ErrorMsg = "資格が選択されていません";
                return View(new List<GetLicenseEntity>());
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
        /// 保有資格の削除確認画面を表示します
        /// </summary>
        [HttpGet]
        public IActionResult Alert()
        {
            // TempDataの null チェックと取り出し
            string[] license_codes = (string[])(TempData[TempKeys.DeleteLicense] ?? Array.Empty<string>());

            List<GetLicenseEntity> entities = new List<GetLicenseEntity>();
            foreach (string code in license_codes)
            {
                var entity = new GetLicenseEntity
                {
                    LicenseCd = code,
                    EmpCd = "D001", // デモ用固定値
                    GetLicenseDate = DateTime.Now
                };
                entities.Add(entity);
            }

            // 再入力やリフレッシュに備えてTempDataを保持
            TempData.Keep(TempKeys.DeleteLicense);

            return View(entities);
        }

        /// <summary>
        /// 保有資格の削除を行います
        /// </summary>
        [HttpPost]
        public IActionResult Alert(List<GetLicenseEntity> entities)
        {
            // 1. 引数リスト自体の null チェック
            if (entities == null || entities.Count == 0)
            {
                return this.RedirectToResult(
                    viewTitle: "エラー",
                    msg: "削除対象の情報が正常に送信されませんでした。",
                    caption: "再度一覧画面からやり直してください");
            }

            // 2. リスト内各オブジェクトの検証 (ModelState.IsValid)
            if (!ModelState.IsValid)
            {
                // 削除確認画面でエラーになるケースは稀ですが、安全のために追加
                return View(entities);
            }

            return this.RedirectToResult(
                viewTitle: "資格削除完了",
                msg: "保有資格の削除が完了しました。",
                caption: "管理メニューから保有資格の確認が可能です");
        }
    }
}
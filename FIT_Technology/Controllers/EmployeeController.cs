using DynamicDll.Db;
using FIT_Technology.Models.Daos;
using FIT_Technology.Models.Entities;
using FIT_Technology.Models.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FIT_Technology.Controllers
{
    /// <summary>
    /// 従業員管理に関する画面遷移とビジネスロジックを制御するコントローラー
    /// </summary>
    public class EmployeeController : Controller
    {
        /// <summary>
        /// [GET] 従業員管理のデフォルト入り口
        /// </summary>
        /// <remarks>直接List画面へリダイレクト（転送）します</remarks>
        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(EmployeeController.List));
        }

        /// <summary>
        /// [GET] 従業員情報一覧画面の表示
        /// </summary>
        [HttpGet]
        public IActionResult List()
        {
            List<EmployeeEntity> info = new List<EmployeeEntity>();

            // データベースから全従業員情報を取得
            using (TranMng mng = TranMng.BeginTransaction("empdb"))
            {
                EmployeeDao dao = new EmployeeDao();
                info = dao.FindAll(); // Dao経由で全件取得
                mng.Commit();
            }

            // 取得したリストをViewBagに格納してView（HTML）側へ渡す
            ViewBag.Info = info;

            return View(nameof(EmployeeController.List), Ctrl.Get<EmployeeController>());
        }

        /// <summary>
        /// [POST] 従業員情報一覧画面からのボタン押下時の処理
        /// </summary>
        /// <param name="empcd">HTML側でチェックされた従業員コードのリスト（name="Entity" または name="empcd" と一致させる必要があります）</param>
        /// <param name="btn_action">押されたボタンのvalue値（"update", "alert", "cancel"など）</param>
        [HttpPost]
        public IActionResult List(List<string> empcd, string btn_action)
        {
            // 💡 アドバイス: 次の画面へ進む際、現状だと「誰がチェックされたか(empcd)」の情報が
            // リダイレクト先（UpdateやAlert）に引き継がれず消えてしまいます。
            // 渡したい場合は TempData["CheckedCds"] = empcd; などをここに挟むとベストです！

            // 「変更」ボタンが押された場合
            if (btn_action == "update")
            {
                return RedirectToAction(nameof(EmployeeController.Update), Ctrl.Get<EmployeeController>());
            }
            // 「削除」ボタンが押された場合（※View側でvalue="alert"になっているボタン）
            else if (btn_action == "alert")
            {
                return RedirectToAction(nameof(EmployeeController.Alert), Ctrl.Get<EmployeeController>());
            }
            // 「戻る」ボタン、またはそれ以外が押された場合はメニュー画面へ戻る
            else
            {
                return RedirectToAction(nameof(AccountController.Menu), Ctrl.Get<AccountController>());
            }
        }

        /// <summary>
        /// [GET] 従業員新規登録画面の表示
        /// </summary>
        [HttpGet]
        public IActionResult Insert()
        {
            return View(nameof(EmployeeController.Insert));
        }

        /// <summary>
        /// [POST] 従業員新規登録画面からのボタン押下時の処理
        /// </summary>
        [HttpPost]
        public IActionResult Insert(EmployeeEntity employee, string btn_action)
        {
            // 「登録（結果）」ボタンが押された場合
            if (btn_action == "result")
            {
                return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
            }
            // 「戻る」ボタンなどが押された場合はメニュー画面へ戻る
            else
            {
                return RedirectToAction(nameof(AccountController.Menu), Ctrl.Get<AccountController>());
            }
        }

        /// <summary>
        /// [GET] 従業員削除確認（アラート）画面の表示
        /// </summary>
        [HttpGet]
        public IActionResult Alert()
        {
            List<EmployeeEntity> info = new List<EmployeeEntity>();

            // 💡 補足: 現状は一覧画面と同じく全件データを表示する動きになっています。
            // もし「チェックされた人だけ」を表示したい場合は、ここで TempData などから
            // 従業員コードのリストを受け取り、特定のレコードだけを取得する処理に変えるとバッチリです。
            using (TranMng mng = TranMng.BeginTransaction("empdb"))
            {
                EmployeeDao dao = new EmployeeDao();
                info = dao.FindAll();
                mng.Commit();
            }
            ViewBag.Info = info;
            return View(nameof(EmployeeController.Alert));
        }

        /// <summary>
        /// [POST] 従業員削除確認画面からのボタン押下時の処理
        /// </summary>
        [HttpPost]
        public IActionResult Alert(string btn_action)
        {
            // 「削除確定」などの処理を経て結果画面へ
            if (btn_action == "result")
            {
                return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
            }
            // 「キャンセル（戻る）」の場合は一覧画面へ戻る
            else
            {
                return RedirectToAction(nameof(EmployeeController.List), Ctrl.Get<EmployeeController>());
            }
        }

        /// <summary>
        /// [GET] 従業員情報変更画面の表示
        /// </summary>
        [HttpGet]
        public IActionResult Update()
        {
            List<EmployeeEntity> info = new List<EmployeeEntity>();

            // 💡 補足: Alert同様、ここも現状は全件取得になっています。
            // 変更対象として選ばれた人のデータを初期値として表示する処理を今後入れるとスムーズです。
            using (TranMng mng = TranMng.BeginTransaction("empdb"))
            {
                EmployeeDao dao = new EmployeeDao();
                info = dao.FindAll();
                mng.Commit();
            }
            ViewBag.Info = info;
            return View(nameof(EmployeeController.Update));
        }

        /// <summary>
        /// [POST] 従業員情報変更画面からのボタン押下時の処理
        /// </summary>
        [HttpPost]
        public IActionResult Update(string btn_action)
        {
            // 「更新確定」などの処理を経て結果画面へ
            if (btn_action == "result")
            {
                return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
            }
            // 「キャンセル（戻る）」の場合は一覧画面へ戻る
            else
            {
                return RedirectToAction(nameof(EmployeeController.List), Ctrl.Get<EmployeeController>());
            }
        }
    }
}
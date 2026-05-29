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
                if(empcd.Count == 1)
                {
                    TempData["UpdateInfo"] = empcd;
                    return RedirectToAction(nameof(EmployeeController.Update), Ctrl.Get<EmployeeController>());
                }
                else
                {
                    return RedirectToAction(nameof(EmployeeController.List), Ctrl.Get<EmployeeController>());
                }
            }
            // 「削除」ボタンが押された場合（※View側でvalue="alert"になっているボタン）
            else if (btn_action == "alert")
            {
                if(empcd.Count == 0)
                {
                    return RedirectToAction(nameof(EmployeeController.List), Ctrl.Get<EmployeeController>());
                }
                else
                {
                    TempData["DeleteInfo"] = empcd;
                    return RedirectToAction(nameof(EmployeeController.Alert), Ctrl.Get<EmployeeController>());
                }
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
                using (TranMng mng = TranMng.BeginTransaction("empdb"))
                {
                    EmployeeDao dao = new EmployeeDao();
                    dao.Insert(employee);
                    mng.Commit();
                }
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
            string[] dele = TempData["DeleteInfo"] as string[];
            List<EmployeeEntity> info = new List<EmployeeEntity>();
            using (TranMng mng = TranMng.BeginTransaction("empdb"))
            {
                for (int i = 0; i < dele.Length; i++)
                    {
                    EmployeeDao dao = new EmployeeDao();
                    info.Add(dao.Find(dele[i]));
                    }
            }
                
            ViewBag.Info = info;
            return View(nameof(EmployeeController.Alert));
        }

        /// <summary>
        /// [POST] 従業員削除確認画面からのボタン押下時の処理
        /// </summary>
        [HttpPost]
        public IActionResult Alert(List<EmployeeEntity> deleteEmp, string btn_action)
        {
            // 「削除確定」などの処理を経て結果画面へ
            if (btn_action == "result")
            {
                using (TranMng mng = TranMng.BeginTransaction("empdb"))
                {
                    EmployeeDao dao = new EmployeeDao();
                    for (int i = 0; i < deleteEmp.Count; i++)
                    {
                        dao.Delete(deleteEmp[i]);
                    }
                    mng.Commit();
                }
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
            string[] up = TempData["UpdateInfo"] as string[];
            List<EmployeeEntity> info = new List<EmployeeEntity>();
            using (TranMng mng = TranMng.BeginTransaction("empdb"))
            {
                for (int i = 0; i < up.Length; i++)
                {
                    EmployeeDao dao = new EmployeeDao();
                    info.Add(dao.Find(up[i]));
                }
            }

            ViewBag.Info = info;
            return View(nameof(EmployeeController.Update));
        }

        /// <summary>
        /// [POST] 従業員情報変更画面からのボタン押下時の処理
        /// </summary>
        [HttpPost]
        public IActionResult Update(EmployeeEntity employee, string btn_action)
        {
            // 「更新確定」などの処理を経て結果画面へ
            if (btn_action == "result")
            {
                if (!ModelState.IsValid)
                {
                    // 入力エラー（カタカナじゃない、空っぽなど）がある場合
                    // そのまま入力内容を保持して登録画面（Insert.cshtml）を再表示
                    return View(nameof(EmployeeController.Insert), employee);
                }
                using (TranMng mng = TranMng.BeginTransaction("empdb"))
                {
                    EmployeeDao dao = new EmployeeDao();
                   
                    dao.Update(employee);
                    
                    mng.Commit();
                }
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
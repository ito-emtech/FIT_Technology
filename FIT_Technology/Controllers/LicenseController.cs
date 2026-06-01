using DynamicDll.Db;
using FIT_Technology.Models.Constants;
using FIT_Technology.Models.Daos;
using FIT_Technology.Models.Entities;
using FIT_Technology.Models.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace FIT_Technology.Controllers
{
    public class LicenseController : Controller
    {
        /// <summary>
        /// デフォルトゲート
        /// </summary>
        /// <remarks>
        /// LicenseMenuにリダイレクトを行います
        /// </remarks>
        /// <returns>
        /// LicenseMenuが表示されます
        /// </returns>
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Title = "案内画面";

            return RedirectToAction(
                nameof(LicenseController.LicenseMenu));
        }

        /// <summary>
        /// 保有資格管理画面を表示します
        /// </summary>
        /// <remarks>
        /// 従業員の一覧を表示します<br/>
        /// ラジオボタンで従業員を選択できます<br/>
        /// 従業員に対する操作をボタンで選択できます<br/>
        /// リンクでトップメニュー(Account/Menu)に戻れる
        /// </remarks>
        /// <returns>
        /// LicenseMenuが表示されます
        /// </returns>
        [HttpGet]
        public IActionResult LicenseMenu()
        {
            ViewBag.ErrorMsg = TempData["ErrorMsg"] ?? null;

            List<EmployeeEntity> list = new List<EmployeeEntity>();

            using (TranMng tm = TranMng.BeginTransaction(DbConstants.EmpDbConnection))
            {
                // ※実際のDaoクラス名（例: EmployeeDao）に合わせてください
                EmployeeDao employeeDao = new EmployeeDao();
                try
                {
                    // 全件取得、あるいは必要な検索メソッドを呼び出す（例: FindAll や Select 等）
                    list = employeeDao.FindAll();

                    tm.Commit();
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                    tm.Rollback();
                    // 必要に応じてエラーメッセージをViewBagに入れるなど
                    ViewBag.ErrorMsg = "従業員データの取得に失敗しました。";
                }
                return View(list);
            }

        }


        /// <summary>
        /// 選択者に対する処理を分配します
        /// </summary>
        /// <remarks>
        /// 選択された従業員に対する処理を行います<br/>
        /// 処理はボタンアクションで選択します<br/>
        /// 従業員が選択されていなかったらエラーを表示します<br/>
        /// 選択者に対して資格登録と保有資格一覧表示を行えます<br/>
        /// </remarks>
        /// <returns>
        /// ボタンアクションに応じた画面をリダイレクトします。
        /// </returns>
        [HttpPost]
        public IActionResult LicenseMenu(string btn_action, string emp_code)
        {

            if (emp_code == null)
            {
                TempData["ErrorMsg"] = "従業員を選択してください";
                return RedirectToAction(nameof(LicenseController.LicenseMenu));
            }

            TempData["emp_code"] = emp_code;


            switch (btn_action)
            {
                case "insert":
                    // 「登録」ボタンが押されたら登録画面へ

                    if (emp_code == null)
                    {
                        ViewBag.ErrorMsg = "従業員を選択してください";

                        return View();
                    }

                    TempData["emp_code"] = emp_code;

                    //return RedirectToAction("Insert");
                    return RedirectToAction(nameof(LicenseController.Insert), Ctrl.Get<LicenseController>());
                case "list":
                    // 「資格一覧表示」ボタンが押されたら、一覧画面へ
                    if (emp_code == null)
                    {
                        ViewBag.ErrorMsg = "従業員を選択してください";

                        return View();
                    }

                    TempData["emp_code"] = emp_code;

                    //return RedirectToAction("List", "License");
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
            // TempData から従業員コードを取得。Peek() を使うことで、値を消さずに維持します。
            string emp_code = (string?)TempData.Peek("emp_code") ?? string.Empty;

            // 画面に渡す「空の」エンティティ（モデル）を生成
            var model = new GetLicenseEntity
            {
                EmpCd = emp_code,
                // 初期値として本日の日付をセット（必要に応じて）
                GetLicenseDate = DateTime.Today
            };

            // ビュー側の @(ViewBag.ViewName) や @(ViewBag.EmpCd) に値を引き渡す
            ViewBag.ViewName = emp_code;
            ViewBag.EmpCd = emp_code;

            ViewBag.EmpName = "取得した従業員名";

            // 【ポイント】先ほどの単一モデル仕様のビューに合わせて、
            // リストではなく「model」単体を引数に渡して View を返します。
            return View(model);
        }

        [HttpPost]

        public IActionResult Insert(string btn_action, string license_cd, string get_license_date)
        {
            switch (btn_action)
            {
                case "result":
                    // 「登録」ボタンが押されたら完了画面へ

                    string emp_code = (string?)TempData.Peek("emp_code") ?? string.Empty;


                    List<GetLicenseEntity> list = new List<GetLicenseEntity>();

                    using (TranMng tm = TranMng.BeginTransaction("empdb"))
                    {
                        GetLicenseDao getLicense = new GetLicenseDao();
                        try
                        {
                            //getLicense.Insert();
                            //検索
                            //list = getLicense.FindWhere(emp_cd);
                            list = getLicense.FindWhere(emp_code);//””を外した9時17分
                            tm.Commit();
                            ViewBag.ViewName = emp_code;

                            return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
                        }
                        catch (Exception e)
                        {
                            System.Console.WriteLine(e.Message);
                            tm.Rollback();
                        }

                        return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
                    }


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
            string emp_code = (string?)TempData.Peek("emp_code") ?? string.Empty;
            // ---- データの表示(userid + password) ----
            List<GetLicenseEntity> list = new List<GetLicenseEntity>();

            using (TranMng tm = TranMng.BeginTransaction("empdb"))
            {
                GetLicenseDao getLicense = new GetLicenseDao();
                try
                {
                    //検索
                    //list = getLicense.FindWhere(emp_cd);
                    list = getLicense.FindWhere(emp_code);//””を外した9時17分
                    tm.Commit();
                    ViewBag.ViewName = emp_code;

                    return View(list);
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                    tm.Rollback();
                }
                //List<DateTime> dateList = new List<DateTime>();

                //foreach (var item in list)
                //{
                //    dateList.Add(item.GetLicenseDate); // 1件ずつ手作業で追加
                //}

                //TempData["get_license_date"] = dateList;
            }

            //TempData["get_"] = emp_code;

            //9時15に変更
            return RedirectToAction("Index", "Result");

        }

        [HttpPost]
        public IActionResult List(string btn_action, List<string> license_cd)
        {
            string emp_code = (string?)TempData.Peek("emp_code") ?? string.Empty;

            switch (btn_action)
            {
                case "alert":
                    // 「削除」ボタンが押されたら削除警告画面へ
                    TempData["license_cd"] = license_cd;

                    return RedirectToAction(
                        nameof(LicenseController.Alert),
                        Ctrl.Get<LicenseController>());

                case "cancel":
                    // 「戻る」ボタンが押されたら保有資格管理画面へ
                    return RedirectToAction(
                        nameof(LicenseController.LicenseMenu),
                        Ctrl.Get<LicenseController>());

                default:
                    // どれにも当てはまらない場合は、今のメニュー画面を再表示
                    return View();
            }
        }

        [HttpGet]
        public IActionResult Alert()
        {
            string emp_code = (string?)TempData.Peek("emp_code") ?? string.Empty;
            ViewBag.ViewName = emp_code;

            // 変更前:
            // List<string> license_cd = (List<string>)TempData.Peek("license_cd");

            // 変更後:
            // 一度 string[] や IEnumerable として安全にキャストを試み、List に変換します
            var rawLicenseCd = TempData.Peek("license_cd");
            List<string> license_cd = rawLicenseCd switch
            {
                List<string> list => list,
                string[] array => array.ToList(),
                _ => new List<string>() // ヌルや想定外の型だった場合のフォールバック
            };

            List<GetLicenseEntity> licenseList = new List<GetLicenseEntity>();

            using (TranMng tm = TranMng.BeginTransaction("empdb"))
            {
                GetLicenseDao getLicense = new GetLicenseDao();
                try
                {
                    foreach (var item in license_cd)
                    {
                        var entity = getLicense.Find(emp_code, item);
                        licenseList.Add(entity); // 1件ずつ手作業で追加
                    }

                    return View(licenseList);
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                    tm.Rollback();
                }

            }

            //13時21分勝手に書いた
            //List<DateTime> license_dates = (List<DateTime>?)TempData.Peek("get_license_date") ?? new List<DateTime>();
            //ViewBag.LicenseDate = license_dates;

            return View(licenseList);
        }

        [HttpPost]
        public IActionResult Alert(string btn_action)
        {

            switch (btn_action)
            {
                case "result":
                    // 「削除」ボタンが押されたら完了画面へ

                    return RedirectToAction(
                        nameof(ResultController.Index),
                        Ctrl.Get<ResultController>());

                case "cancel":
                    // 「キャンセル」ボタンが押されたら保有資格一覧画面へ
                    return RedirectToAction(
                        nameof(LicenseController.List),
                        Ctrl.Get<LicenseController>());

                default:
                    // どれにも当てはまらない場合は、今のメニュー画面を再表示
                    return View();
            }
        }
    }
}

using DynamicDll.Db;
using FIT_Technology.Filters;
using FIT_Technology.Models.Daos;
using FIT_Technology.Models.Entities;
using FIT_Technology.Models.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Configuration;

namespace FIT_Technology.Controllers
{
    /// <summary>
    /// 従業員管理に関する画面遷移とビジネスロジックを制御するコントローラー
    /// </summary>
    [SessionCheck]
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
            List<SectionEntity> sectionInfo = new List<SectionEntity>();

            // データベースから全従業員情報を取得
            try
            {
                using (TranMng mng = TranMng.BeginTransaction("empdb"))
                {
                    EmployeeDao dao = new EmployeeDao();
                    SectionDao sectionDao = new SectionDao();
                    info = dao.FindAll(); // Dao経由で全件取得
                    sectionInfo = sectionDao.FindAll();
                    mng.Commit();
                }

            }catch (Exception ex)
            {
                TempData["ViewTitle"] = "エラーが発生しました";
                TempData["Msg"] = "データベースの制約、または通信エラーにより処理を中断しました。データは変更されていません。";

                return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
            }


            // 取得したリストをViewBagに格納してView（HTML）側へ渡す
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            ViewBag.Info = info;
            ViewBag.SectionInfo = sectionInfo;

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
                    TempData["UpdateEmpCd"] = empcd;
                    return RedirectToAction(nameof(EmployeeController.Update), Ctrl.Get<EmployeeController>());
                }else if(empcd.Count == 0)
                {
                    TempData["ErrorMessage"] = "変更したい従業員を1人選択してください。";
                    return RedirectToAction(nameof(EmployeeController.List), Ctrl.Get<EmployeeController>());
                }
                else
                {
                    TempData["ErrorMessage"] = "変更は1人しかできません。変更したい従業員を1人だけ選択してください。";
                    return RedirectToAction(nameof(EmployeeController.List), Ctrl.Get<EmployeeController>());
                }
            }
            // 「削除」ボタンが押された場合（※View側でvalue="alert"になっているボタン）
            else if (btn_action == "alert")
            {
                if(empcd.Count == 0)
                {
                    TempData["ErrorMessage"] = "削除したい従業員を選択してください。";
                    return RedirectToAction(nameof(EmployeeController.List), Ctrl.Get<EmployeeController>());
                }
                else
                {
                    TempData["DeleteEmpCd"] = empcd;
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
            if (TempData.Peek("InsertActive") == null && TempData.ContainsKey("InsertActive"))
            {
                TempData["ViewTitle"] = "登録処理は完了しています";
                TempData["Msg"] = "この画面の登録処理はすでに正常に終了しています。メニューに戻ってください。";
                return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
            }

            // 💡 画面を正常に開いた「証拠」として、削除の「DeleteEmpCd」と同じような目印を置いておく
            TempData["InsertActive"] = "active";

            ViewBag.Title = "登録画面";

            // 💡 削除や変更と同じ仕組みに変えます！
            // 登録完了後にブラウザバックすると、POST側で Remove されているのでここが null になります
            


            // 💡 常に新しい空のインスタンスを渡す
            EmployeeEntity info = new EmployeeEntity();
            List<SectionEntity> sectionInfo = new List<SectionEntity>();
            List<GenderEntity> genderInfo = new List<GenderEntity>();
            info.BirthDate = DateTime.Today; // 生年月日の初期値を今日にする
            info.EmpDate = DateTime.Today;   // 入社日の初期値を今日にする

            using (TranMng mng = TranMng.BeginTransaction("empdb"))
            {
                EmployeeDao dao = new EmployeeDao();
                SectionDao sectionDao = new SectionDao();
                GenderDao genderDao = new GenderDao();
                
                sectionInfo = sectionDao.FindAll();
                genderInfo = genderDao.FindAll();

                mng.Commit();
            }
            ViewBag.SectionInfo = sectionInfo;
            ViewBag.GenderInfo = genderInfo;
            return View(nameof(EmployeeController.Insert), info);
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
                if (ModelState.ContainsKey("BirthDate") && ModelState["BirthDate"].Errors.Count > 0)
                {
                    ModelState["BirthDate"].Errors.Clear(); // 英語のエラーを一旦ゴミ箱に捨てる
                    ModelState.AddModelError("BirthDate", "生年月日を入力してください。"); // 日本語を入れ直す
                }

                // ②「入社日」のチェック
                if (ModelState.ContainsKey("EmpDate") && ModelState["EmpDate"].Errors.Count > 0)
                {
                    ModelState["EmpDate"].Errors.Clear(); // 英語のエラーを一旦ゴミ箱に捨てる
                    ModelState.AddModelError("EmpDate", "入社日を入力してください。"); // 日本語を入れ直す
                }

                if (employee.BirthDate > DateTime.Now)
                {
                    // 画面の「BirthDate」の欄にエラーメッセージを紐付ける
                    ModelState.AddModelError("BirthDate", "生年月日に未来の日付を設定することはできません。");
                }
                if (employee.BirthDate > employee.EmpDate)
                {
                    // 入社日の項目に対してエラーメッセージを紐付けます
                    ModelState.AddModelError("EmpDate", "入社日より後に生まれた生年月日を設定することはできません。");
                }
                if (!ModelState.IsValid)
                {
                    // 🛑 入力エラー（カタカナじゃない、空っぽなど）がある場合
                    // そのまま入力内容を保持して登録画面（Insert.cshtml）を再表示
                    List<SectionEntity> sectionInfo = new List<SectionEntity>();
                    List<GenderEntity> genderInfo = new List<GenderEntity>();

                    using (TranMng mng = TranMng.BeginTransaction("empdb"))
                    {
                        
                        SectionDao sectionDao = new SectionDao();
                        GenderDao genderDao = new GenderDao();

                        sectionInfo = sectionDao.FindAll();
                        genderInfo = genderDao.FindAll();

                        mng.Commit();
                    }
                    ViewBag.SectionInfo = sectionInfo;
                    ViewBag.GenderInfo = genderInfo;
                    return View(nameof(EmployeeController.Insert), employee);
                }
                

                try
                { 
                    using (TranMng mng = TranMng.BeginTransaction("empdb"))
                    {
                        EmployeeDao dao = new EmployeeDao();
                        if (dao.Exists(employee.EmpCd))
                        {
                            // 🛑 重複していたら、手動でエラーを仕込む！
                            // 第1引数：Entityのプロパティ名（大文字小文字を合わせる）
                            // 第2引数：画面に出したいエラーメッセージ
                            List<SectionEntity> sectionInfo = new List<SectionEntity>();
                            List<GenderEntity> genderInfo = new List<GenderEntity>();

                            

                            SectionDao sectionDao = new SectionDao();
                            GenderDao genderDao = new GenderDao();

                            sectionInfo = sectionDao.FindAll();
                            genderInfo = genderDao.FindAll();

                            mng.Commit();
                            
                            ViewBag.SectionInfo = sectionInfo;
                            ViewBag.GenderInfo = genderInfo;
                            ModelState.AddModelError("EmpCd", "入力された従業員コードは既に登録されています。");

                            // 登録画面（Insert.cshtml）に入力内容を保持したまま戻す

                            return View(nameof(EmployeeController.Insert), employee);
                        }
                        dao.Insert(employee);
                        mng.Commit();
                    }

                    TempData.Remove("InsertActive");
                    TempData["ViewTitle"] = "登録が完了しました";
                    TempData["Msg"] = "登録できたよ！！💛";
                    return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
                }
                catch (Exception ex)
                {
                    //return this.RedirectToResult(
                    //    viewTitle: "エラーが発生しました",
                    //    msg: "データベースの制約、または通信エラーにより処理を中断しました。データは変更されていません。",
                    //    caption: ex.ToString());

                    TempData["ViewTitle"] = "エラーが発生しました";
                    TempData["Msg"] = "データベースの制約、または通信エラーにより処理を中断しました。データは変更されていません。";

                    return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
                }
                
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
            string[] dele = TempData.Peek("DeleteEmpCd") as string[];
            if (dele == null || dele.Length == 0)
            {
                TempData["ViewTitle"] = "すでに削除されています";
                TempData["Msg"] = "メニューに戻ってください";

                return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
            }
            List<EmployeeEntity> info = new List<EmployeeEntity>();
            try { using (TranMng mng = TranMng.BeginTransaction("empdb"))
            {
                for (int i = 0; i < dele.Length; i++)
                    {
                    EmployeeDao dao = new EmployeeDao();
                    info.Add(dao.Find(dele[i]));
                    }
            }
            }catch (Exception ex)
            {
                TempData["ViewTitle"] = "エラーが発生しました";
                TempData["Msg"] = "データベースの制約、または通信エラーにより処理を中断しました。データは変更されていません。";

                return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
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
                try
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
                    TempData.Remove("DeleteEmpCd");
                    TempData["ViewTitle"] = "削除が完了しました";
                    string msg = "";
                    for(int i = 0;i < deleteEmp.Count; i++)
                    {
                        msg = msg + deleteEmp[i].EmpCd + " " + deleteEmp[i].LastNm + deleteEmp[i].FirstNm + "　";
                    }
                    TempData["Msg"] = msg + "の削除が完了しました";
                    return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
                }
                catch (Exception ex)
                {
                    TempData["ViewTitle"] = "エラーが発生しました";
                    TempData["Msg"] = "データベースの制約、または通信エラーにより処理を中断しました。データは変更されていません。";

                    return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
                }
                
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
            ViewBag.Title = "変更画面";

            string[] up = TempData.Peek("UpdateEmpCd") as string[];
            if (up == null || up.Length == 0)
            {
                TempData["ViewTitle"] = "すでに変更されています";
                TempData["Msg"] = "メニューに戻ってください";

                return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
            }
            string updatecd = up[0];
            
            EmployeeEntity info = new EmployeeEntity();
            List<SectionEntity> sectionInfo = new List<SectionEntity>();
            List<GenderEntity> genderInfo = new List<GenderEntity>();
            try
            {
                using (TranMng mng = TranMng.BeginTransaction("empdb"))
                {
                    EmployeeDao dao = new EmployeeDao();
                    info = dao.Find(updatecd);
                    SectionDao sectionDao = new SectionDao();
                    GenderDao genderDao = new GenderDao();

                    sectionInfo = sectionDao.FindAll();
                    genderInfo = genderDao.FindAll();

                    mng.Commit();
                }
            }catch (Exception ex)
            {
                TempData["ViewTitle"] = "エラーが発生しました";
                TempData["Msg"] = "データベースの制約、または通信エラーにより処理を中断しました。データは変更されていません。";

                return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
            }

            ViewBag.SectionInfo = sectionInfo;
            ViewBag.GenderInfo = genderInfo;

            return View(nameof(EmployeeController.Update), info);
        }

        /// <summary>
        /// [POST] 従業員情報変更画面からのボタン押下時の処理
        /// </summary>
        [HttpPost]
        public IActionResult Update(EmployeeEntity employee, string btn_action)
        {
            ViewBag.Title = "再入力画面";

            // 「更新確定」などの処理を経て結果画面へ
            if (btn_action == "result")
            {
                List<SectionEntity> sectionInfo = new List<SectionEntity>();
                List<GenderEntity> genderInfo = new List<GenderEntity>();

                if (!ModelState.IsValid)
                {
                    // 入力エラー（カタカナじゃない、空っぽなど）がある場合
                    // そのまま入力内容を保持して登録画面（Insert.cshtml）を再表示
                    using (TranMng mng = TranMng.BeginTransaction("empdb"))
                    {

                        SectionDao sectionDao = new SectionDao();
                        GenderDao genderDao = new GenderDao();

                        sectionInfo = sectionDao.FindAll();
                        genderInfo = genderDao.FindAll();

                        mng.Commit();
                    }
                    ViewBag.SectionInfo = sectionInfo;
                    ViewBag.GenderInfo = genderInfo;
                    return View(nameof(EmployeeController.Update), employee);
                }
                try { using (TranMng mng = TranMng.BeginTransaction("empdb"))
                {
                    EmployeeDao dao = new EmployeeDao();
                   
                    dao.Update(employee);
                    
                    mng.Commit();
                }
                    TempData.Remove("UpdateEmpCd");
                    TempData["ViewTitle"] = "変更が完了しました";
                    TempData["Msg"] = employee.EmpCd + " " + employee.LastNm + employee.FirstNm + "の変更が完了しました";
                    return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
                }
                catch (Exception ex)
                {
                    TempData["ViewTitle"] = "エラーが発生しました";
                    TempData["Msg"] = "データベースの制約、または通信エラーにより処理を中断しました。データは変更されていません。";

                    return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
                }
                
            }
            // 「キャンセル（戻る）」の場合は一覧画面へ戻る
            else
            {
                return RedirectToAction(nameof(EmployeeController.List), Ctrl.Get<EmployeeController>());
            }
        }
    }
}
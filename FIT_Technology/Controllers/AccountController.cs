using FIT_Technology.Filters;
using FIT_Technology.Models.Constants;
using FIT_Technology.Models.Entities;
using FIT_Technology.Models.Helpers;
using FIT_Technology.Models.Services;
using Microsoft.AspNetCore.Mvc;

namespace FIT_Technology.Controllers
{
    /// <summary>
    /// 認証（ログイン・ログアウト）およびメインメニューの制御を行うコントローラーです。
    /// </summary>
    /// <param name="service">ユーザー認証サービス</param>
    public class AccountController(AccountService service) : Controller
    {
        /// <summary>
        /// ルートURLアクセス時のハンドラ。
        /// SessionCheckフィルタにより、未ログイン時はログイン画面へ、ログイン済みの場合はメニュー画面へ誘導します。
        /// </summary>
        [HttpGet]
        [SessionCheck]
        public IActionResult Index()
        {
            ViewBag.Title = "ホーム画面";
            return View(nameof(AccountController.Menu));
        }

        /// <summary>
        /// ログイン画面を表示します。
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Title = "ログイン画面";
            return View(nameof(AccountController.Login));
        }

        /// <summary>
        /// ログイン認証を実行します。
        /// </summary>
        /// <param name="user">入力されたユーザー情報（ID・パスワード）</param>
        /// <remarks>認証成功時はセッションにIDを保存し、メニュー画面へ遷移します。</remarks>
        [HttpPost]
        public IActionResult Login(UserEntity user)
        {
            ViewBag.Title = "再入力画面";

            if (!ModelState.IsValid) { return View(nameof(AccountController.Login)); }

            if (service.Authenticate(user.UserId, user.Password))
            {
                HttpContext.Session.SetString(DbConstants.SessionKeys.UserId, user.UserId);
                return RedirectToAction(nameof(AccountController.Menu), Ctrl.Get<AccountController>());
            }
            else
            {
                // 【修正点】リダイレクト先でも表示できるよう TempData に格納
                TempData["ViewTitle"] = "ログインエラー";
                TempData["Msg"] = "ユーザーIDまたはパスワードが正しくありません。";
                TempData["Caption"] = "入力を確認して再度ログインしてください。";

                return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
            }
        }

        /// <summary>
        /// ログアウト確認画面を表示します。
        /// </summary>
        [HttpGet]
        [SessionCheck]
        public IActionResult Logout()
        {
            ViewBag.Title = "ログアウト確認画面";
            return View(nameof(AccountController.Logout));
        }

        /// <summary>
        /// ログアウト処理を実行し、セッションを破棄します。
        /// </summary>
        /// <param name="btn_action">ボタンアクション（拡張用）</param>
        [HttpPost]
        [SessionCheck]
        public IActionResult Logout(string btn_action)
        {
            // セッションからユーザーIDを取得（クリアする前に行う）
            var userId = HttpContext.Session.GetString(DbConstants.SessionKeys.UserId);

            // 【重要】ViewBagではなくTempDataを使用する
            TempData["InfoMessage"] = $"{userId}様、お疲れさまでした。";

            // セッションをクリア
            HttpContext.Session.Clear();

            // ログイン画面へリダイレクト
            return RedirectToAction(nameof(AccountController.Login), Ctrl.Get<AccountController>());
        }

        /// <summary>
        /// メインメニュー画面を表示します。
        /// </summary>
        [HttpGet]
        [SessionCheck]
        public IActionResult Menu()
        {
            ViewBag.Title = "従業員管理システム画面";
            return View(nameof(AccountController.Menu));
        }

        /// <summary>
        /// メニュー画面からの各機能への遷移を制御します。
        /// </summary>
        /// <param name="btn_action">押下されたボタンに応じたアクション（logout, insert, list, license）</param>
        [HttpPost]
        [SessionCheck]
        public IActionResult Menu(string btn_action)
        {
            ViewBag.Title = "従業員管理システム画面";

            switch (btn_action)
            {
                case "logout":
                    return RedirectToAction(nameof(AccountController.Logout), Ctrl.Get<AccountController>());
                case "insert":
                    // 従業員登録画面へ
                    return RedirectToAction(nameof(EmployeeController.Insert), Ctrl.Get<EmployeeController>());
                case "list":
                    // 従業員管理機能へ
                    return RedirectToAction(nameof(EmployeeController.List), Ctrl.Get<EmployeeController>());
                case "license":
                    // 保有資格管理画面へ
                    return RedirectToAction(nameof(LicenseController.LicenseMenu), Ctrl.Get<LicenseController>());
                default:
                    // 定義外のアクションが送られた場合のエラー処理
                    TempData["ViewTitle"] = "不正な入力を検知";
                    TempData["Msg"] = "従業員管理システム画面から不正コマンドを検出しました";
                    return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
            }
        }
    }
}
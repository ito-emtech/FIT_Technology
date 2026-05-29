using FIT_Technology.Filters;
using FIT_Technology.Models.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace FIT_Technology.Controllers
{
    /// <summary>
    /// 処理結果やエラーメッセージを表示するためのコントローラーです。
    /// </summary>
    public class ResultController : Controller
    {
        /// <summary>
        /// 結果表示画面（初期表示）
        /// </summary>
        /// <returns>Indexビュー</returns>
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Title = "案内画面"; // ブラウザのタブ名など

            // TempDataから値を取り出し、ViewBagにセットする（View側がViewBagを参照しているため）
            ViewBag.ViewTitle = TempData["ViewTitle"] ?? "案内";
            ViewBag.Msg = TempData["Msg"] ?? "表示するメッセージはありません。";
            ViewBag.Caption = TempData["Caption"];

            return View(nameof(ResultController.Index));
        }

        /// <summary>
        /// 画面上のボタン（戻る、確認など）が押下された際の処理
        /// セッションチェックを行い、問題なければメニュー画面へ遷移します。
        /// </summary>
        /// <param name="btn_action">押下されたボタンの識別値</param>
        /// <returns>メニュー画面へのリダイレクト</returns>
        [HttpPost]
        [SessionCheck] // アクション実行前にセッションの有無を検証
        public IActionResult Index(string btn_action)
        {
            // ViewBagが空の場合のフォールバック処理
            ViewBag.ViewTitle = TempData["ViewTitle"] ?? "案内";
            ViewBag.Msg = TempData["Msg"] ?? "表示するメッセージはありません。";
            ViewBag.Caption = TempData["Caption"];

            // Ctrlヘルパーを使用して型安全にメニュー画面（Account/Menu）へリダイレクト
            return RedirectToAction(
                nameof(AccountController.Menu),
                Ctrl.Get<AccountController>()
            );
        }
    }
}
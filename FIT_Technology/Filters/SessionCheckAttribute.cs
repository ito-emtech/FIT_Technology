using FIT_Technology.Controllers;
using FIT_Technology.Models.Constants;
using FIT_Technology.Models.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FIT_Technology.Filters
{
    /// <summary>
    /// セッションの有効性をチェックするアクションフィルタ属性です。
    /// アクションメソッドの実行前に、セッション内にユーザーIDが存在するかを確認します。
    /// </summary>
    public class SessionCheckAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// アクションの実行前に呼び出され、セッション情報のチェックを行います。
        /// </summary>
        /// <param name="context">アクション実行のコンテキスト情報</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // セッションからユーザーIDを取得
            var userId = context.HttpContext.Session.GetString(DbConstants.SessionKeys.UserId);

            // ユーザーIDが取得できない（未ログインまたはセッション切れ）場合の処理
            if (string.IsNullOrEmpty(userId))
            {
                // コントローラーを取得してTempDataにエラーメッセージを格納
                var controller = (Controller)context.Controller;
                controller.TempData["ErrorMessage"] = "セッションの有効期限が切れたか、ログインが必要です。";

                // ログイン画面（AccountControllerのLoginアクション）へリダイレクト
                context.Result = new RedirectToActionResult(
                    nameof(AccountController.Login),
                    Ctrl.Get<AccountController>(),
                    null);
            }

            // 基本クラスのメソッドを呼び出して処理を継続
            base.OnActionExecuting(context);
        }
    }
}
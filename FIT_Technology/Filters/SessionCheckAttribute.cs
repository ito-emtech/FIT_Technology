using FIT_Technology.Controllers;
using FIT_Technology.Models.Constants;
using FIT_Technology.Models.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FIT_Technology.Filters
{
    public class SessionCheckAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetString(DbConstants.SessionKeys.UserId);

            if (string.IsNullOrEmpty(userId))
            {
                // TempDataにメッセージをセット
                var controller = (Controller)context.Controller;
                controller.TempData["ErrorMessage"] = "セッションの有効期限が切れたか、ログインが必要です。";

                context.Result = new RedirectToActionResult(
                    nameof(AccountController.Login),
                    Ctrl.Get<AccountController>(),
                    null);
            }

            base.OnActionExecuting(context);
        }
    }
}
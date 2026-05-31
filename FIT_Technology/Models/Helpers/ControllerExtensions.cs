using FIT_Technology.Controllers;
using FIT_Technology.Models.Constants;
using FIT_Technology.Models.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace FIT_Technology.Models.Helpers
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// 案内画面（Result）に必要なメッセージデータをセットし、Result/Index へリダイレクトします。
        /// </summary>
        public static RedirectToActionResult RedirectToResult(
            this Controller controller, 
            string viewTitle, 
            string msg, 
            string? caption = null, 
            string? buttonView = null)
        {
            // TempDataに安全に値を添付
            controller.TempData[ResultConstants.TempDataKeys.ViewTitle] = viewTitle;
            controller.TempData[ResultConstants.TempDataKeys.Msg] = msg;
            controller.TempData[ResultConstants.TempDataKeys.Caption] = caption;
            controller.TempData[ResultConstants.TempDataKeys.ButtonView] = buttonView;

            // ResultControllerのIndexアクションへのリダイレクトオブジェクトを返却
            return controller.RedirectToAction(
                nameof(ResultController.Index), 
                Ctrl.Get<ResultController>()
            );
        }
    }
}
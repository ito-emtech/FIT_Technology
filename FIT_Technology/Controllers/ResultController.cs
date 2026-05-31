using FIT_Technology.Models.Constants;
using Microsoft.AspNetCore.Mvc;

namespace FIT_Technology.Controllers
{
    /// <summary>
    /// 処理結果やエラーメッセージを表示するためのコントローラーです。
    /// </summary>
    public class ResultController : Controller
    {
        /// <summary>
        /// 結果表示画面
        /// </summary>
        /// <returns>Indexビュー</returns>
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Title = "案内画面";
            ViewBag.ViewTitle 
                = TempData[ResultConstants.TempDataKeys.ViewTitle] ?? "システム通知";
            ViewBag.Msg 
                = TempData[ResultConstants.TempDataKeys.Msg] ?? "表示するメッセージはありません。";
            ViewBag.Caption 
                = TempData[ResultConstants.TempDataKeys.Caption];
            ViewBag.ButtonView 
                = TempData[ResultConstants.TempDataKeys.ButtonView] ?? "次に進む";

            return View(nameof(ResultController.Index));
        }
    }
}
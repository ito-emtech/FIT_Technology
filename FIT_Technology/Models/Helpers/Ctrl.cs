using Microsoft.AspNetCore.Mvc;

namespace FIT_Technology.Models.Helpers
{
    /// <summary>
    /// コントローラー操作に関するヘルパーメソッドを提供する静的クラスです。
    /// </summary>
    public static class Ctrl
    {
        /// <summary>
        /// 指定されたコントローラー型から、ルーティングに使用するコントローラー名を取得します。
        /// クラス名から "Controller" 文字列を除去した値を返します。
        /// </summary>
        /// <typeparam name="T">対象となるコントローラーの型（Controller継承クラス）</typeparam>
        /// <returns>コントローラー名の文字列（例：AccountController の場合は "Account"）</returns>
        /// <example>
        /// usage: 
        /// <code>
        /// string name = Ctrl.Get&lt;AccountController&gt;(); // "Account"
        /// </code>
        /// </example>
        public static string Get<T>() where T : Controller
            => typeof(T).Name.Replace("Controller", "");
    }
}
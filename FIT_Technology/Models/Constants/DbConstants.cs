namespace FIT_Technology.Models.Constants
{
    /// <summary>
    /// アプリケーション全体で使用するデータベース関連の定数管理クラスです。
    /// </summary>
    public static class DbConstants
    {
        /// <summary>
        /// データベース接続文字列のキー名です。
        /// appsettings.json 等の ConnectionStrings セクションに対応します。
        /// </summary>
        /// <remarks>
        /// 本番環境やテスト環境に応じて切り替える場合は、ここを直接書き換えるか、
        /// 環境変数等を利用する設計を検討してください。
        /// </remarks>
        // public const string EmpDbConnection = "empdb"; // 共通環境用
        public const string EmpDbConnection = "local";    // ローカル開発環境用

        /// <summary>
        /// セッション管理で使用するキー名を定義する内部クラスです。
        /// </summary>
        public static class SessionKeys
        {
            /// <summary>
            /// ログインユーザーのIDを保持するためのセッションキーです。
            /// </summary>
            public const string UserId = "UserId";
        }
    }
}
using DynamicDll.Db;
using FIT_Technology.Models.Daos;
using FIT_Technology.Models.Constants;

namespace FIT_Technology.Models.Services
{
    /// <summary>
    /// アカウントに関連する業務ロジック（認証・管理など）を提供するサービスクラスです。
    /// </summary>
    public class AccountService
    {
        /// <summary>
        /// 指定されたユーザーIDとパスワードを用いてログイン認証を行います。
        /// </summary>
        /// <param name="userid">ログインを試行するユーザーID</param>
        /// <param name="password">入力されたパスワード（平文）</param>
        /// <returns>
        /// 認証に成功した場合は true。
        /// ユーザーが見つからない、またはパスワードが一致しない場合は false を返します。
        /// </returns>
        public bool Authenticate(string userid, string password)
        {
            if (string.IsNullOrEmpty(userid) || string.IsNullOrEmpty(password)) { return false; }

            try
            {
                // トランザクションを開始する
                using (TranMng tm = TranMng.BeginTransaction(DbConstants.EmpDbConnection))
                {
                    var dao = new AccountDao();
                    var user = dao.Find(userid);

                    if (user == null) { return false; }
                    return user.Password == password;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
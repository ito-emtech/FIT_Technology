using DynamicDll.Db;
using FIT_Technology.Models.Entities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FIT_Technology.Models.Daos
{
    public class AccountDao : BaseDao<UserEntity>
    {
        public AccountDao() { }

        public override UserEntity Find(params object[] pkeys)
        {
            // 引数チェック: 主キーが渡されていない場合は null を返す
            if (pkeys == null || pkeys.Length == 0 || pkeys[0] == null)
            {
                return new UserEntity();
            }

            string query = @"
                SELECT user_id, password
                FROM m_user 
                WHERE user_id = @UserId";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;

                // パラメータ設定
                cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = pkeys[0];

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        UserEntity user = new UserEntity();
                        user.UserId = reader["user_id"].ToString().Trim();
                        user.Password = reader["password"].ToString().Trim();

                        reader.Close();
                        return user;
                    }
                }
                return new UserEntity();
            }
        }

        public override int Insert(UserEntity entity)
        {
            throw new NotImplementedException();
        }

        public override int Update(UserEntity entity)
        {
            throw new NotImplementedException();
        }

        public override int Delete(UserEntity entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ログインチェックを行います
        /// </summary>
        public string? Login(string userid, string password)
        {
            if (string.IsNullOrEmpty(userid) || string.IsNullOrEmpty(password))
                return null;

            UserEntity user = Find(userid);

            if (user == null) return null;

            if (user.Password != password) return null;

            return user.UserId;
        }
    }
}

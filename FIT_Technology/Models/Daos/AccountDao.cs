using DynamicDll.Db;
using FIT_Technology.Models.Entities;
using FIT_Technology.Models.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FIT_Technology.Models.Daos
{
    /// <summary>
    /// ユーザーアカウントに関するデータアクセスオブジェクト（DAO）です。
    /// </summary>
    public class AccountDao : BaseDao<UserEntity>
    {
        /// <summary>
        /// <see cref="AccountDao"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public AccountDao() { }

        /// <summary>
        /// 主キーを使用して、特定のユーザーエンティティを検索します。
        /// </summary>
        /// <param name="pkeys">主キー。第一引数に UserId を指定してください。</param>
        /// <returns>見つかった場合は <see cref="UserEntity"/>。見つからない場合は null。</returns>
        public override UserEntity Find(params object[] pkeys)
        {
            if (pkeys == null || pkeys.Length == 0 || pkeys[0] == null) return null;

            // 型情報からテーブル名とプロパティを取得
            var type = typeof(UserEntity);
            string tableName = EntityHelpers.GetTableName<UserEntity>();

            // 特定のプロパティ（UserId）のカラム名を取得
            var idProp = type.GetProperty(nameof(UserEntity.UserId))!;
            var passProp = type.GetProperty(nameof(UserEntity.Password))!;

            string idCol = EntityHelpers.GetColumnName(idProp);
            string passCol = EntityHelpers.GetColumnName(passProp);

            // 文字列補完を使ってクエリを構築（リテラルを排除）
            string query = $@"
                SELECT {idCol}, {passCol}
                FROM {tableName}
                WHERE {idCol} = @Id";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;
                // パラメータ名もプロパティ名から生成可能
                cmd.Parameters.Add("@Id", SqlDbType.VarChar).Value = pkeys[0];

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new UserEntity
                        {
                            UserId = reader[idCol].ToString()?.Trim() ?? string.Empty,
                            Password = reader[passCol].ToString()?.Trim() ?? string.Empty
                        };
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// ユーザー情報を新規登録します。（未実装）
        /// </summary>
        /// <param name="entity">登録するエンティティ</param>
        /// <exception cref="NotImplementedException">このメソッドは現在実装されていません。</exception>
        public override int Insert(UserEntity entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 既存のユーザー情報を更新します。（未実装）
        /// </summary>
        /// <param name="entity">更新するエンティティ</param>
        /// <exception cref="NotImplementedException">このメソッドは現在実装されていません。</exception>
        public override int Update(UserEntity entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ユーザー情報を削除します。（未実装）
        /// </summary>
        /// <param name="entity">削除するエンティティ</param>
        /// <exception cref="NotImplementedException">このメソッドは現在実装されていません。</exception>
        public override int Delete(UserEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
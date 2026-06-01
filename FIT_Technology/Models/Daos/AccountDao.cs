using DynamicDll.Db;
using FIT_Technology.Models.Entities;
using FIT_Technology.Models.Helpers; // 拡張メソッドを使うために必要
using Microsoft.Data.SqlClient;
using System.Data;

namespace FIT_Technology.Models.Daos
{
    /// <summary>
    /// ユーザーアカウントに関するデータアクセスオブジェクト（DAO）です。
    /// </summary>
    public class AccountDao : BaseDao<UserEntity>
    {
        public AccountDao() { }

        /// <summary>
        /// 主キーを使用して、特定のユーザーエンティティを検索します。
        /// </summary>
        public override UserEntity Find(params object[] pkeys)
        {
            if (pkeys == null || pkeys.Length == 0 || pkeys[0] == null) return null;

            // 1. EntityMetaHelperを使用してメタデータを取得
            string tableName = EntityMetaHelper.GetTableName<UserEntity>();
            var idProp = typeof(UserEntity).GetProperty(nameof(UserEntity.UserId))!;
            string idCol = EntityMetaHelper.GetColumnName(idProp);

            // 2. クエリの構築
            string query = $@"
                SELECT *
                FROM {tableName}
                WHERE {idCol} = @Id";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;

                // 3. パラメータ設定（ここでもヘルパーを活用）
                cmd.Parameters.Add("@Id", EntityMetaHelper.GetSqlType(idProp)).Value = pkeys[0];

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.MapToEntity<UserEntity>();
                    }
                }
            }
            return null;
        }

        public override int Insert(UserEntity entity) => throw new NotImplementedException();
        public override int Update(UserEntity entity) => throw new NotImplementedException();
        public override int Delete(UserEntity entity) => throw new NotImplementedException();
    }
}
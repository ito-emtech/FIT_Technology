using DynamicDll.Db;
using FIT_Technology.Models.Entities;
using FIT_Technology.Models.Helpers; // 拡張メソッド(MapToEntity)の利用に必要
using Microsoft.Data.SqlClient;
using System.Data;

namespace FIT_Technology.Models.Daos
{
    /// <summary>
    /// 性別マスタ (m_gender) 用のデータアクセスオブジェクトです。
    /// </summary>
    public class GenderDao : BaseDao<GenderEntity>
    {
        public GenderDao() { }

        /// <summary>
        /// 主キー（性別コード）を指定してレコードを取得します。
        /// </summary>
        public override GenderEntity Find(params object[] pkeys)
        {
            if (pkeys == null || pkeys.Length == 0 || pkeys[0] == null) return null;

            // 1. EntityMetaHelperを使用してメタデータを取得
            string tableName = EntityMetaHelper.GetTableName<GenderEntity>();
            var propCd = typeof(GenderEntity).GetProperty(nameof(GenderEntity.GenderCd))!;
            string colCd = EntityMetaHelper.GetColumnName(propCd);

            // 2. クエリの構築
            // SELECT * を使用することで、将来的なカラム増減に対応
            string query = $@"
                SELECT *
                FROM {tableName}
                WHERE {colCd} = @PKey";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;

                // 3. パラメータ設定 (GenderCdがintなら自動でSqlDbType.Intが設定される)
                cmd.Parameters.Add("@PKey", EntityMetaHelper.GetSqlType(propCd)).Value = pkeys[0];

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // 4. ★拡張メソッドによる自動マッピング
                        return reader.MapToEntity<GenderEntity>();
                    }
                }
            }
            return null;
        }

        public override int Insert(GenderEntity entity) => throw new NotImplementedException();
        public override int Update(GenderEntity entity) => throw new NotImplementedException();
        public override int Delete(GenderEntity entity) => throw new NotImplementedException();
    }
}
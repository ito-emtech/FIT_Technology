using DynamicDll.Db;
using FIT_Technology.Models.Entities;
using FIT_Technology.Models.Helpers;
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

            // 型情報とテーブル名の取得
            var type = typeof(GenderEntity);
            string tableName = EntityHelpers.GetTableName<GenderEntity>();

            // プロパティ情報の取得
            var propCd = type.GetProperty(nameof(GenderEntity.GenderCd))!;
            var propNm = type.GetProperty(nameof(GenderEntity.GenderNm))!;

            // カラム名とSQLデータ型の取得
            string colCd = EntityHelpers.GetColumnName(propCd);
            string colNm = EntityHelpers.GetColumnName(propNm);
            SqlDbType sqlTyp = EntityHelpers.GetSqlType(propCd); // int型として取得される

            // クエリ構築
            string query = $@"
                SELECT {colCd}, {colNm}
                FROM {tableName}
                WHERE {colCd} = @PKey";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;
                // GenderCd は int なので、EntityHelpers.GetSqlType が SqlDbType.Int を返します
                cmd.Parameters.Add("@PKey", sqlTyp).Value = pkeys[0];

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return EntityHelpers.MapEntity<GenderEntity>(reader);
                    }
                }
            }
            return null;
        }

        public override int Insert(GenderEntity entity)
        {
            // 必要に応じて共通のInsertロジック、または個別SQLを実装
            throw new NotImplementedException();
        }

        public override int Update(GenderEntity entity)
        {
            // 必要に応じて共通のUpdateロジック、または個別SQLを実装
            throw new NotImplementedException();
        }

        public override int Delete(GenderEntity entity)
        {
            // 必要に応じて共通のDeleteロジック、または個別SQLを実装
            throw new NotImplementedException();
        }
    }
}
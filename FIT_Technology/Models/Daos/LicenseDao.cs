using DynamicDll.Db;
using FIT_Technology.Models.Entities;
using FIT_Technology.Models.Helpers; // 拡張メソッド(MapToEntity)の利用に必要
using Microsoft.Data.SqlClient;
using System.Data;

namespace FIT_Technology.Models.Daos
{
    /// <summary>
    /// ライセンス情報に関するデータアクセスオブジェクト（DAO）です。
    /// </summary>
    public class LicenseDao : BaseDao<LicenseEntity>
    {
        public LicenseDao() { }

        /// <summary>
        /// 主キーを使用して、特定のライセンスエンティティを検索します。
        /// </summary>
        public override LicenseEntity Find(params object[] pkeys)
        {
            if (pkeys == null || pkeys.Length == 0 || pkeys[0] == null) return null;

            // 1. EntityMetaHelperを使用してメタデータを取得
            string tableName = EntityMetaHelper.GetTableName<LicenseEntity>();
            var propCd = typeof(LicenseEntity).GetProperty(nameof(LicenseEntity.LicenseCd))!;
            string colCd = EntityMetaHelper.GetColumnName(propCd);

            // 2. クエリの構築
            // カラム指定を SELECT * にすることで、エンティティの変更に強いコードにします
            string query = $@"
                SELECT *
                FROM {tableName}
                WHERE {colCd} = @PKey";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;

                // 3. パラメータ設定 (型もヘルパーから動的に取得)
                cmd.Parameters.Add("@PKey", EntityMetaHelper.GetSqlType(propCd)).Value = pkeys[0];

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // 4. ★拡張メソッドを適用：自動マッピング
                        return reader.MapToEntity<LicenseEntity>();
                    }
                }
            }
            return null;
        }

        public override int Insert(LicenseEntity entity) => throw new NotImplementedException();
        public override int Update(LicenseEntity entity) => throw new NotImplementedException();
        public override int Delete(LicenseEntity entity) => throw new NotImplementedException();
    }
}
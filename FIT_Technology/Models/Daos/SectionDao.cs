using DynamicDll.Db;
using FIT_Technology.Models.Entities;
using FIT_Technology.Models.Helpers; // 拡張メソッド(MapToEntity)を使うために必須
using Microsoft.Data.SqlClient;
using System.Data;

namespace FIT_Technology.Models.Daos
{
    /// <summary>
    /// 部署情報に関するデータアクセスオブジェクト（DAO）です。
    /// </summary>
    public class SectionDao : BaseDao<SectionEntity>
    {
        public SectionDao() { }

        /// <summary>
        /// 主キーを使用して、特定の部署エンティティを検索します。
        /// </summary>
        public override SectionEntity Find(params object[] pkeys)
        {
            if (pkeys == null || pkeys.Length == 0 || pkeys[0] == null) return null;

            // 1. EntityMetaHelperを使用してメタデータを取得
            string tableName = EntityMetaHelper.GetTableName<SectionEntity>();
            var propCd = typeof(SectionEntity).GetProperty(nameof(SectionEntity.SectionCd))!;
            string colCd = EntityMetaHelper.GetColumnName(propCd);

            // 2. クエリ構築 (SELECT * でマッパーに任せるのが保守上おすすめ)
            string query = $@"
                SELECT *
                FROM {tableName}
                WHERE {colCd} = @PKey";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;

                // 3. パラメータ設定
                cmd.Parameters.Add("@PKey", EntityMetaHelper.GetSqlType(propCd)).Value = pkeys[0];

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // 4. ★拡張メソッドで自動マッピング
                        return reader.MapToEntity<SectionEntity>();
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// すべての部署マスタ情報を取得します。
        /// </summary>
        /// <returns>部署マスタエンティティのリスト</returns>
        public List<SectionEntity> FindAll()
        {
            var list = new List<SectionEntity>();

            // 1. EntityMetaHelperを使用してテーブル名と主キーのカラム名を取得
            string tableName = EntityMetaHelper.GetTableName<SectionEntity>();
            var propCd = typeof(SectionEntity).GetProperty(nameof(SectionEntity.SectionCd))!;
            string colCd = EntityMetaHelper.GetColumnName(propCd);

            // 2. クエリの構築（部署コード順にソートして全件取得）
            string query = $@"
                SELECT * FROM {tableName} 
                ORDER BY {colCd}";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // 3. ループ内で拡張メソッドを使って自動マッピングし、リストに追加
                    while (reader.Read())
                    {
                        list.Add(reader.MapToEntity<SectionEntity>());
                    }
                    reader.Close();
                }
            }
            return list;
        }

        public override int Insert(SectionEntity entity) => throw new NotImplementedException();
        public override int Update(SectionEntity entity) => throw new NotImplementedException();
        public override int Delete(SectionEntity entity) => throw new NotImplementedException();
    }
}
using DynamicDll.Db;
using FIT_Technology.Models.Entities;
using FIT_Technology.Models.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FIT_Technology.Models.Daos
{
    // 1. 継承する型を SectionEntity に変更
    public class SectionDao : BaseDao<SectionEntity>
    {
        public SectionDao() { }

        public override SectionEntity Find(params object[] pkeys)
        {
            if (pkeys == null || pkeys.Length == 0 || pkeys[0] == null) return null;

            // 2. SectionEntity の型情報を取得
            var type = typeof(SectionEntity);
            string tableName = EntityHelpers.GetTableName<SectionEntity>();

            // 3. SectionEntity のプロパティ情報を取得
            var propCd = type.GetProperty(nameof(SectionEntity.SectionCd))!;
            var propNm = type.GetProperty(nameof(SectionEntity.SectionNm))!;

            // 4. カラム名とSQL型を取得
            string colCd = EntityHelpers.GetColumnName(propCd);
            string colNm = EntityHelpers.GetColumnName(propNm);
            SqlDbType sqlTyp = EntityHelpers.GetSqlType(propCd);

            // 5. クエリ構築 (SectionEntity のカラムを使用)
            string query = $@"
                SELECT {colCd}, {colNm}
                FROM {tableName}
                WHERE {colCd} = @PKey";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;
                cmd.Parameters.Add("@PKey", sqlTyp).Value = pkeys[0];

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Helperを使用してマッピング
                        return EntityHelpers.MapEntity<SectionEntity>(reader);
                    }
                }
            }
            return null;
        }

        public override int Insert(SectionEntity entity)
        {
            // 必要に応じて実装
            throw new NotImplementedException();
        }

        public override int Update(SectionEntity entity)
        {
            // 必要に応じて実装
            throw new NotImplementedException();
        }

        public override int Delete(SectionEntity entity)
        {
            // 必要に応じて実装
            throw new NotImplementedException();
        }
    }
}
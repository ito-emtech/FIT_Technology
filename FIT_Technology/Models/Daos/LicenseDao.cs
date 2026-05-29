using DynamicDll.Db;
using FIT_Technology.Models.Entities;
using FIT_Technology.Models.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FIT_Technology.Models.Daos
{
    public class LicenseDao : BaseDao<LicenseEntity>
    {
        public LicenseDao() { }

        public override LicenseEntity Find(params object[] pkeys)
        {
            if (pkeys == null || pkeys.Length == 0 || pkeys[0] == null) return null;

            // 型情報からテーブル名とプロパティを取得
            var type = typeof(LicenseEntity);
            string tableName = EntityHelpers.GetTableName<LicenseEntity>();

            // 特定のプロパティ（UserId）のカラム名を取得
            var prop1 = type.GetProperty(nameof(LicenseEntity.LicenseCd))!;
            var prop2 = type.GetProperty(nameof(LicenseEntity.LicenseNm))!;

            string col1 = EntityHelpers.GetColumnName(prop1);
            string col2 = EntityHelpers.GetColumnName(prop2);

            // 文字列補完を使ってクエリを構築（リテラルを排除）
            string query = $@"
                SELECT {col1}, {col2}
                FROM {tableName}
                WHERE {col1} = @PKey";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;
                // パラメータ名もプロパティ名から生成可能
                cmd.Parameters.Add("@PKey", SqlDbType.VarChar).Value = pkeys[0];

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new LicenseEntity
                        {
                            LicenseCd = reader[col1].ToString()?.Trim() ?? string.Empty,
                            LicenseNm = reader[col2].ToString()?.Trim() ?? string.Empty
                        };
                    }
                }
            }
            return null;
        }

        public override int Insert(LicenseEntity entity)
        {
            throw new NotImplementedException();
        }

        public override int Update(LicenseEntity entity)
        {
            throw new NotImplementedException();
        }

        public override int Delete(LicenseEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}


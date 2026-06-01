using Microsoft.Data.SqlClient;
using System.Data;

namespace FIT_Technology.Models.Helpers
{
    public static class SqlReaderExtensions
    {
        /// <summary>
        /// SqlDataReaderからエンティティにマッピングします。
        /// </summary>
        public static T MapToEntity<T>(this SqlDataReader reader) where T : new()
        {
            T entity = new T();
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                // EntityMetaHelperからカラム名を取得
                string columnName = EntityMetaHelper.GetColumnName(prop);

                if (!reader.HasColumn(columnName)) continue;

                object value = reader[columnName];
                if (value != DBNull.Value)
                {
                    if (prop.PropertyType == typeof(string))
                        prop.SetValue(entity, value.ToString()?.Trim());
                    else
                        prop.SetValue(entity, value);
                }
            }
            return entity;
        }

        /// <summary>
        /// 指定したカラム名が存在するかチェックします。
        /// </summary>
        public static bool HasColumn(this SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace FIT_Technology.Models.Helpers
{
    public static class EntityMetaHelper
    {
        public static string GetTableName<T>() =>
            typeof(T).GetCustomAttribute<TableAttribute>()?.Name ?? typeof(T).Name;

        public static string GetColumnName(PropertyInfo prop) =>
            prop.GetCustomAttribute<ColumnAttribute>()?.Name ?? prop.Name;

        public static SqlDbType GetSqlType(PropertyInfo prop)
        {
            var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            return type switch
            {
                var t when t == typeof(string) => SqlDbType.VarChar,
                var t when t == typeof(int) => SqlDbType.Int,
                var t when t == typeof(DateTime) => SqlDbType.DateTime,
                _ => SqlDbType.VarChar
            };
        }
    }
}
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace FIT_Technology.Models.Helpers
{
    /// <summary>
    /// エンティティモデルに関するメタデータ取得を補助するユーティリティクラスです。
    /// </summary>
    /// <remarks>
    /// DataAnnotations（Table属性やColumn属性）を利用して、データベースの物理名を取得します。
    /// </remarks>
    public static class EntityHelpers
    {
        /// <summary>
        /// 指定された型に関連付けられたデータベースのテーブル名を取得します。
        /// </summary>
        /// <typeparam name="T">対象となるエンティティのクラス型</typeparam>
        /// <returns>
        /// [Table]属性で指定された名前を返します。
        /// 属性が設定されていない場合は、クラス名をそのまま返します。
        /// </returns>
        public static string GetTableName<T>() =>
            // TableAttributeが存在すればそのNameを、なければ型名を返す
            typeof(T).GetCustomAttribute<TableAttribute>()?.Name ?? typeof(T).Name;

        /// <summary>
        /// 指定されたプロパティに関連付けられたデータベースのカラム名を取得します。
        /// </summary>
        /// <param name="prop">対象となるプロパティの情報（PropertyInfo）</param>
        /// <returns>
        /// [Column]属性で指定された名前を返します。
        /// 属性が設定されていない場合は、プロパティ名をそのまま返します。
        /// </returns>
        public static string GetColumnName(PropertyInfo prop) =>
            // ColumnAttributeが存在すればそのNameを、なければプロパティ名を返す
            prop.GetCustomAttribute<ColumnAttribute>()?.Name ?? prop.Name;
        
        /// <summary>
        /// プロパティの型から対応する SqlDbType を取得します。
        /// </summary>
        public static SqlDbType GetSqlType(PropertyInfo prop)
        {
            var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            return type switch
            {
                var t when t == typeof(string) => SqlDbType.VarChar,
                var t when t == typeof(int) => SqlDbType.Int,
                var t when t == typeof(long) => SqlDbType.BigInt,
                var t when t == typeof(DateTime) => SqlDbType.DateTime,
                var t when t == typeof(bool) => SqlDbType.Bit,
                var t when t == typeof(decimal) => SqlDbType.Decimal,
                _ => SqlDbType.VarChar // デフォルト
            };
        }

        /// <summary>
        /// SqlDataReader の現在の行から、エンティティのインスタンスを生成し値を格納します。
        /// </summary>
        public static T MapEntity<T>(SqlDataReader reader) where T : new()
        {
            T entity = new T();
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                // プロパティからカラム名を取得
                string columnName = GetColumnName(prop);

                // Readerに該当するカラム名が存在するかチェック
                // (SELECT句に含まれていないプロパティはスキップ)
                if (!HasColumn(reader, columnName)) continue;

                object value = reader[columnName];

                if (value != DBNull.Value)
                {
                    // 型変換を考慮して値をセット
                    // 文字列の場合は .Trim() をかけるなどの調整もここで行える
                    if (prop.PropertyType == typeof(string))
                    {
                        prop.SetValue(entity, value.ToString()?.Trim());
                    }
                    else
                    {
                        prop.SetValue(entity, value);
                    }
                }
            }
            return entity;
        }

        /// <summary>
        /// Readerの中に指定したカラム名が存在するか判定します。
        /// </summary>
        private static bool HasColumn(SqlDataReader reader, string columnName)
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

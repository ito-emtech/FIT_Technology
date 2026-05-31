using DynamicDll.Db;
using FIT_Technology.Models.Entities;
using FIT_Technology.Models.Helpers;
using FIT_Technology.Models.ViewModels;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FIT_Technology.Models.Daos
{
    public class DemoGetLicenseDao : BaseDao<GetLicenseEntity>
    {
        public DemoGetLicenseDao() { }

        public override GetLicenseEntity Find(params object[] pkeys)
            => throw new NotImplementedException();
        public override int Insert(GetLicenseEntity entity) => throw new NotImplementedException();
        public override int Update(GetLicenseEntity entity) => throw new NotImplementedException();
        public override int Delete(GetLicenseEntity entity) => throw new NotImplementedException();
    }

    public class DemoEmployeeDao : BaseDao<EmployeeEntity>
    {
        public DemoEmployeeDao() { }

        /// <summary>
        /// 主キー（従業員コード）を使用して、特定の従業員エンティティを検索します。
        /// </summary>
        public override EmployeeEntity Find(params object[] pkeys)
        {
            if (pkeys == null || pkeys.Length == 0 || pkeys[0] == null) return null;

            string tableName = EntityMetaHelper.GetTableName<EmployeeEntity>();
            var idProp = typeof(EmployeeEntity).GetProperty(nameof(EmployeeEntity.EmpCd))!;
            string idCol = EntityMetaHelper.GetColumnName(idProp);

            string query = $@"
                SELECT *
                FROM {tableName}
                WHERE {idCol} = @Id";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;
                // char(4) なので、明示的に SqlDbType.Char を指定して検索速度と正確性を担保
                cmd.Parameters.Add("@Id", SqlDbType.Char, 4).Value = pkeys[0];

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.MapToEntity<EmployeeEntity>();
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 全ての従業員レコードを取得します。（LicenseMenuでの一覧表示用）
        /// </summary>
        public List<EmployeeEntity> FindAll()
        {
            var list = new List<EmployeeEntity>();
            string tableName = EntityMetaHelper.GetTableName<EmployeeEntity>();

            string query = $"SELECT * FROM {tableName}";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(reader.MapToEntity<EmployeeEntity>());
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 保有資格管理メニュー一覧用のデータを取得します（部署名・性別名付き）
        /// </summary>
        public List<LicenseMenuRowViewModel> FindAllForLicenseMenu()
        {
            var list = new List<LicenseMenuRowViewModel>();

            // 必要なカラムを明示的に指定してINNER JOINします
            string query = @"
                SELECT 
                    e.emp_cd,
                    e.last_nm,
                    e.first_nm,
                    e.last_nm_kana,
                    e.first_nm_kana,
                    e.gender_cd,
                    g.gender_nm,
                    e.birth_date,
                    e.section_cd,
                    s.section_nm,
                    e.emp_date
                FROM m_employee e
                INNER JOIN m_section s ON e.section_cd = s.section_cd
                INNER JOIN m_gender g ON e.gender_cd = g.gender_cd
                ORDER BY e.emp_cd";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(reader.MapToEntity<LicenseMenuRowViewModel>());
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 従業員マスタに新規レコードを登録します。
        /// </summary>
        public override int Insert(EmployeeEntity entity)
        {
            if (entity == null) return 0;

            string tableName = EntityMetaHelper.GetTableName<EmployeeEntity>();

            string query = $@"
                INSERT INTO {tableName} (
                    emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, 
                    gender_cd, birth_date, section_cd, emp_date
                ) VALUES (
                    @EmpCd, @LastNm, @FirstNm, @LastNmKana, @FirstNmKana, 
                    @GenderCd, @BirthDate, @SectionCd, @EmpDate
                )";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;

                // Entityの特性（char, nvarchar, date）に合わせて安全にバインド
                cmd.Parameters.Add("@EmpCd", SqlDbType.Char, 4).Value = entity.EmpCd;
                cmd.Parameters.Add("@LastNm", SqlDbType.NVarChar, 16).Value = entity.LastNm;
                cmd.Parameters.Add("@FirstNm", SqlDbType.NVarChar, 16).Value = entity.FirstNm;
                cmd.Parameters.Add("@LastNmKana", SqlDbType.NVarChar, 24).Value = entity.LastNmKana;
                cmd.Parameters.Add("@FirstNmKana", SqlDbType.NVarChar, 24).Value = entity.FirstNmKana;
                cmd.Parameters.Add("@GenderCd", SqlDbType.Int).Value = entity.GenderCd;
                cmd.Parameters.Add("@BirthDate", SqlDbType.Date).Value = entity.BirthDate;
                cmd.Parameters.Add("@SectionCd", SqlDbType.Char, 2).Value = entity.SectionCd;
                cmd.Parameters.Add("@EmpDate", SqlDbType.Date).Value = entity.EmpDate;

                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 従業員マスタの既存レコードを更新します。
        /// </summary>
        public override int Update(EmployeeEntity entity)
        {
            if (entity == null) return 0;

            string tableName = EntityMetaHelper.GetTableName<EmployeeEntity>();

            string query = $@"
                UPDATE {tableName}
                SET 
                    last_nm = @LastNm,
                    first_nm = @FirstNm,
                    last_nm_kana = @LastNmKana,
                    first_nm_kana = @FirstNmKana,
                    gender_cd = @GenderCd,
                    birth_date = @BirthDate,
                    section_cd = @SectionCd,
                    emp_date = @EmpDate
                WHERE emp_cd = @EmpCd";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;

                cmd.Parameters.Add("@EmpCd", SqlDbType.Char, 4).Value = entity.EmpCd;
                cmd.Parameters.Add("@LastNm", SqlDbType.NVarChar, 16).Value = entity.LastNm;
                cmd.Parameters.Add("@FirstNm", SqlDbType.NVarChar, 16).Value = entity.FirstNm;
                cmd.Parameters.Add("@LastNmKana", SqlDbType.NVarChar, 24).Value = entity.LastNmKana;
                cmd.Parameters.Add("@FirstNmKana", SqlDbType.NVarChar, 24).Value = entity.FirstNmKana;
                cmd.Parameters.Add("@GenderCd", SqlDbType.Int).Value = entity.GenderCd;
                cmd.Parameters.Add("@BirthDate", SqlDbType.Date).Value = entity.BirthDate;
                cmd.Parameters.Add("@SectionCd", SqlDbType.Char, 2).Value = entity.SectionCd;
                cmd.Parameters.Add("@EmpDate", SqlDbType.Date).Value = entity.EmpDate;

                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 主キーを基に従業員レコードを削除します。
        /// </summary>
        public override int Delete(EmployeeEntity entity)
        {
            if (entity == null) return 0;

            string tableName = EntityMetaHelper.GetTableName<EmployeeEntity>();

            string query = $@"
                DELETE FROM {tableName}
                WHERE emp_cd = @EmpCd";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;
                cmd.Parameters.Add("@EmpCd", SqlDbType.Char, 4).Value = entity.EmpCd;

                return cmd.ExecuteNonQuery();
            }
        }
    }
}
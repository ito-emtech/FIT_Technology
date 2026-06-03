using DynamicDll.Db;
using FIT_Technology.Models.Entities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FIT_Technology.Models.Daos
{
    public class EmployeeDao : BaseDao<EmployeeEntity>
    {
        public EmployeeDao() { }

        public override EmployeeEntity Find(params object[] pkeys)
        {
            string code = pkeys[0].ToString();

            EmployeeEntity result = null;

            string query = @"SELECT * FROM m_employee WHERE emp_cd = @EmpCd";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;
                cmd.Parameters.Add("@EmpCd", SqlDbType.Char).Value = code;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result = new EmployeeEntity()
                        {
                            EmpCd = GetString(reader["emp_cd"]),
                            LastNm = GetString(reader["last_nm"]),
                            FirstNm = GetString(reader["first_nm"]),
                            LastNmKana = GetString(reader["last_nm_kana"]),
                            FirstNmKana = GetString(reader["first_nm_kana"]),
                            GenderCd = (int)GetInt(reader["gender_cd"]),
                            BirthDate = (DateTime)reader["birth_date"],
                            SectionCd = GetString(reader["section_cd"]),
                            EmpDate = (DateTime)reader["emp_date"],
                            Updated = (DateTime)reader["updated"]
                        };
                    }
                    reader.Close();

                }
            }
            return result;
        }

        public override int Insert(EmployeeEntity entity)
        {
            int count = 0;
            string query = @"INSERT INTO m_employee(emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES(@EmpCd, @LastNm, @FirstNm, @LastNmKana, @FirstNmKana, @GenderCd, @BirthDate, @SectionCd, @EmpDate)";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;
                cmd.Parameters.Add("@EmpCd", SqlDbType.Char).Value = entity.EmpCd;
                cmd.Parameters.Add("@LastNm", SqlDbType.NVarChar).Value = entity.LastNm;
                cmd.Parameters.Add("@FirstNm", SqlDbType.NVarChar).Value = entity.FirstNm;
                cmd.Parameters.Add("@LastNmKana", SqlDbType.NVarChar).Value = entity.LastNmKana;
                cmd.Parameters.Add("@FirstNmKana", SqlDbType.NVarChar).Value = entity.FirstNmKana;
                cmd.Parameters.Add("@GenderCd", SqlDbType.Int).Value = entity.GenderCd;
                cmd.Parameters.Add("@BirthDate", SqlDbType.Date).Value = entity.BirthDate;
                cmd.Parameters.Add("@SectionCd", SqlDbType.Char).Value = entity.SectionCd;
                cmd.Parameters.Add("@EmpDate", SqlDbType.Date).Value = entity.EmpDate;

                try
                {
                    count = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return count;
            //return 0;
        }

        public override int Update(EmployeeEntity entity)
        {

            string query = @"UPDATE m_employee SET last_nm = @LastNm, first_nm = @FirstNm, last_nm_kana = @LastNmKana, first_nm_kana = @FirstNmKana, gender_cd = @GenderCd, birth_date = @BirthDate, section_cd = @SectionCd, emp_date = @EmpDate WHERE emp_cd = @EmpCd AND updated = @Updated";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;
                cmd.Parameters.Add("@EmpCd", SqlDbType.Char).Value = entity.EmpCd;
                cmd.Parameters.Add("@LastNm", SqlDbType.NVarChar).Value = entity.LastNm;
                cmd.Parameters.Add("@FirstNm", SqlDbType.NVarChar).Value = entity.FirstNm;
                cmd.Parameters.Add("@LastNmKana", SqlDbType.NVarChar).Value = entity.LastNmKana;
                cmd.Parameters.Add("@FirstNmKana", SqlDbType.NVarChar).Value = entity.FirstNmKana;
                cmd.Parameters.Add("@GenderCd", SqlDbType.Int).Value = entity.GenderCd;
                cmd.Parameters.Add("@BirthDate", SqlDbType.Date).Value = entity.BirthDate;
                cmd.Parameters.Add("@SectionCd", SqlDbType.Char).Value = entity.SectionCd;
                cmd.Parameters.Add("@EmpDate", SqlDbType.Date).Value = entity.EmpDate;
                // ★追加：画面から送られてきた「古い更新日時」をパラメーターにセット
                cmd.Parameters.Add("@Updated", SqlDbType.DateTime2).Value = entity.Updated;

                try
                {
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public override int Delete(EmployeeEntity entity)
        {
            int count = 0;
            string query = @"DELETE FROM m_employee Where emp_cd = @EmpCd";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;
                cmd.Parameters.Add("@EmpCd", SqlDbType.Char).Value = entity.EmpCd;

                try
                {
                    count = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return count;
        }

        public List<EmployeeEntity> FindAll()
        {
            List<EmployeeEntity> result = new List<EmployeeEntity>();

            string query = @"SELECT * FROM m_employee";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        EmployeeEntity entity = new EmployeeEntity()
                        {
                            EmpCd = GetString(reader["emp_cd"]),
                            LastNm = GetString(reader["last_nm"]),
                            FirstNm = GetString(reader["first_nm"]),
                            LastNmKana = GetString(reader["last_nm_kana"]),
                            FirstNmKana = GetString(reader["first_nm_kana"]),
                            GenderCd = (int)GetInt(reader["gender_cd"]),
                            BirthDate = (DateTime)reader["birth_date"],
                            SectionCd = GetString(reader["section_cd"]),
                            EmpDate = (DateTime)reader["emp_date"],
                            //,Created = (DateTime)reader["created"],
                            Updated = (DateTime)reader["updated"]
                        };
                        result.Add(entity);
                    }
                }
                return result;
            }
        }

        public bool Exists(string empcd)
        {
            int count = 0;
            string query = "SELECT COUNT(*) FROM m_employee WHERE emp_cd = @EmpCd";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                // 親クラス（BaseDao）が持っているトランザクションオブジェクト（trn）をコマンドにセットします
                cmd.Transaction = trn;

                cmd.Parameters.Add("@EmpCd", SqlDbType.Char).Value = empcd;

                try
                {
                    count = (int)cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            // 1件以上見つかったら true (重複している)
            return count > 0;
        }
    }
}

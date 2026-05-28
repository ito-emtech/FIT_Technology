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
                            LastNmKana = GetString(reader["last_name_kana"]),
                            FirstNmKana = GetString(reader["first_name_kana"]),
                            GenderCd = (int)GetInt(reader["gender_cd"]),
                            BirthDate = (DateTime)reader["birth_date"],
                            SectionCd = GetString(reader["section_cd"]),
                            EmpDate = (DateTime)reader["emp_date"]
                        };
                    }
                    reader.Close();

                }
            }
            return result;
        }

        public override int Insert(EmployeeEntity entity)
        {
            //int count = 0;
            //string query = @"INSERT INTO m_employee(emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_code, birth_date, section_cd, emp_date) VALUES(@EmpCd, @LastNm, @FirstNm, @LastNmKana, @FirstNmKana, @GenderCd, @BirthDate, @SectionCd, @EmpDate)";
            //using (SqlCommand cmd = new SqlCommand(query, con))
            //{
            //    cmd.Transaction = trn;
            //    cmd.Parameters.Add("@EmdCd", SqlDbType.Char).Value = entity.EmpCd;
            //    cmd.Parameters.Add("@LastNm", SqlDbType.NChar).Value = entity.;
            //    cmd.Parameters.Add("@FirstNm", SqlDbType.NChar).Value = entity.;
            //    cmd.Parameters.Add("@LastNmKana", SqlDbType.NChar).Value = entity.;
            //    cmd.Parameters.Add("@FirstNmKana", SqlDbType.NChar).Value = entity.;
            //    cmd.Parameters.Add("@GenderCd", SqlDbType.NChar).Value = entity.;
            //    cmd.Parameters.Add("@BirthDate", SqlDbType.NChar).Value = entity.;
            //    cmd.Parameters.Add("@SectionCd", SqlDbType.NChar).Value = entity.;
            //    cmd.Parameters.Add("@EmpDate", SqlDbType.NChar).Value = entity.;


            //    cmd.Parameters.Add("@", SqlDbType.NChar).Value = entity.;
            //    cmd.Parameters.Add("@Name", SqlDbType.NChar).Value = entity.Name;
            //    cmd.Parameters.Add("@Age", SqlDbType.NChar).Value = entity.Age;
            //    cmd.Parameters.Add("@Section", SqlDbType.NChar).Value = entity.Section;

            //    try
            //    {
            //        count = cmd.ExecuteNonQuery();
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}
            return 0;
        }

        public override int Update(EmployeeEntity entity)
        {
            throw new NotImplementedException();
        }

        public override int Delete(EmployeeEntity entity)
        {
            throw new NotImplementedException();
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
                            LastNmKana = GetString(reader["last_name_kana"]),
                            FirstNmKana = GetString(reader["first_name_kana"]),
                            GenderCd = (int)GetInt(reader["gender_cd"]),
                            BirthDate = (DateTime)reader["birth_date"],
                            SectionCd = GetString(reader["section_cd"]),
                            EmpDate = (DateTime)reader["emp_date"]
                            //,Created = (DateTime)reader["created"],
                            //Updated = (DateTime)reader["updated"]
                        };
                        result.Add(entity);
                    }
                }
                return result;
            }
        }
    }
}

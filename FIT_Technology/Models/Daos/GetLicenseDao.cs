using DynamicDll.Db;
using FIT_Technology.Models.Entities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FIT_Technology.Models.Daos
{
    public class GetLicenseDao : BaseDao<UserEntity>
    {
        public GetLicenseDao() { }

        public override UserEntity Find(params object[] pkeys)
        {
            // 実行したいSQL文
            return null;
        }

        public List<GetLicenseEntity> FindAll()
        {
            // 実行したいSQL文
            string query = @"SELECT * FROM t_get_license";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {   //トランザクション（一連の処理をひとまとめにする仕組み）
                cmd.Transaction = trn;

                // コマンドの実行とレコードの取得
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    List<GetLicenseEntity> list = new List<GetLicenseEntity>();

                    // レコードの操作

                    while (reader.Read())
                    {
                        GetLicenseEntity emp = new GetLicenseEntity();
                        emp.EmpCd = reader["emp_cd"].ToString() ?? string.Empty;
                        emp.LicenseCd = reader["license_cd"].ToString() ?? string.Empty;
                        emp.GetLicenseDate = (DateTime)reader["get_license_date"];
                        list.Add(emp);

                    }
                    return list;
                }
            }

        }
        //FindWhereを呼び出す
        public List<GetLicenseEntity> FindWhere(string emp_cd)
        {
            // 実行したいSQL文
            string query = @"SELECT emp_cd,license_cd,get_license_date FROM t_get_license WHERE emp_cd = @emp_cd";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {   //トランザクション（一連の処理をひとまとめにする仕組み）
                cmd.Transaction = trn;

                // パラメータ登録
                cmd.Parameters.Add("@emp_cd", SqlDbType.Char);


                // パラメータ入力
                cmd.Parameters["@emp_cd"].Value = emp_cd;


                // コマンドの実行とレコードの取得
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    List<GetLicenseEntity> list = new List<GetLicenseEntity>();

                    // レコードの操作

                    while (reader.Read())
                    {
                        GetLicenseEntity emp = new GetLicenseEntity();
                        emp.EmpCd = reader["emp_cd"].ToString() ?? string.Empty;
                        emp.LicenseCd = reader["license_cd"].ToString() ?? string.Empty;
                        emp.GetLicenseDate = (DateTime)reader["get_license_date"];
                        list.Add(emp);

                    }
                    return list;
                }
            }

        }

        public override int Insert(UserEntity entity)
        {
            throw new NotImplementedException();
        }

        public override int Update(UserEntity entity)
        {
            throw new NotImplementedException();
        }

        public override int Delete(UserEntity entity)
        {
            throw new NotImplementedException();
        }

        ///FindAllを作る
        //public override int (UserEntity entity)
        //{
        //    throw new NotImplementedException();
    }
}


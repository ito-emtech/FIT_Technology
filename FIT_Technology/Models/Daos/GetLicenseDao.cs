using DynamicDll.Db;
using FIT_Technology.Models.Entities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FIT_Technology.Models.Daos
{
    public class GetLicenseDao : BaseDao<GetLicenseEntity>
    {
        public GetLicenseDao() { }

        public override GetLicenseEntity Find(params object[] pkeys)
        {
            string emp_code = pkeys[0].ToString();
            string license_code = pkeys[1].ToString();

            GetLicenseEntity result = null;

            // ★「@EmpCd」と「AND」の間にあった全角スペースを半角スペースに修正しました
            string query = @"SELECT * FROM t_get_license WHERE emp_cd = @EmpCd AND license_cd = @LicenseCd;";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;
                cmd.Parameters.Add("@EmpCd", SqlDbType.Char).Value = emp_code;
                cmd.Parameters.Add("@LicenseCd", SqlDbType.Char).Value = license_code;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result = new GetLicenseEntity()
                        {
                            EmpCd = GetString(reader["emp_cd"]),
                            // ★「license_code」から「license_cd」に修正しました
                            LicenseCd = GetString(reader["license_cd"]),
                            GetLicenseDate = (DateTime)reader["get_license_date"],
                        };
                    }
                    // usingを使っているので本来不要ですが、あっても問題ありません
                    reader.Close();
                }
            }
            // これで正しく1件分のデータを返せるようになります！
            return result;
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
                cmd.Parameters.Add("@emp_cd", SqlDbType.Char).Value = emp_cd;


                //// パラメータ入力
                //cmd.Parameters["@emp_cd"].Value = emp_cd;


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

        public override int Insert(GetLicenseEntity entity)
        {
            string query = @"INSERT INTO t_get_license (emp_cd,license_cd, get_license_date) 
                     VALUES (@EmpCd, @LicenseCd, @GetLicenseDate);";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;

                // パラメータの登録
                cmd.Parameters.Add("@EmpCd", SqlDbType.Char).Value = entity.EmpCd;
                cmd.Parameters.Add("@LicenseCd", SqlDbType.Char).Value = entity.LicenseCd;
                cmd.Parameters.Add("@GetLicenseDate", SqlDbType.Date).Value = entity.GetLicenseDate;

                // クエリを実行し、影響を受けた行数（通常は1）を返します
                return cmd.ExecuteNonQuery();
            }
        }

        public override int Update(GetLicenseEntity entity)
        {
            throw new NotImplementedException();
        }

        public override int Delete(GetLicenseEntity entity)
        {

            // 💡 主キーである emp_cd と license_cd の両方を条件に指定して削除します
            string query = @"DELETE FROM t_get_license 
                     WHERE emp_cd = @EmpCd AND license_cd = @LicenseCd;";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Transaction = trn;

                // パラメータの登録
                cmd.Parameters.Add("@EmpCd", SqlDbType.Char).Value = entity.EmpCd;
                cmd.Parameters.Add("@LicenseCd", SqlDbType.Char).Value = entity.LicenseCd;

                // クエリを実行し、削除された行数を返します
                return cmd.ExecuteNonQuery();
            }
        }
    }



}


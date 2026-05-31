using DynamicDll.Db;
using FIT_Technology.Models.Constants;
using FIT_Technology.Models.Daos;
using FIT_Technology.Models.Entities;

namespace FIT_Technology.Models.Services
{
    /// <summary>
    /// 従業員情報に関する業務ロジック（一覧取得・登録など）を提供するサービスクラスです。
    /// </summary>
    public class DemoEmployeeService
    {
        /// <summary>
        /// 全ての従業員レコードを取得します。
        /// </summary>
        /// <returns>従業員エンティティのリスト。例外発生時は空のリストを返します。</returns>
        public List<EmployeeEntity> GetEmployees()
        {
            try
            {
                // 読み取り専用であっても、既存のアーキテクチャ方針に従いTranMngを生成
                using (TranMng tm = TranMng.BeginTransaction(DbConstants.EmpDbConnection))
                {
                    var dao = new DemoEmployeeDao();
                    return dao.FindAll() ?? new List<EmployeeEntity>();
                }
            }
            catch
            {
                // ロギング処理などが必要な場合はここに記述
                return new List<EmployeeEntity>();
            }
        }

        /// <summary>
        /// 指定された従業員コードを基に、特定の従業員情報を取得します。
        /// </summary>
        /// <param name="empCd">従業員コード</param>
        /// <returns>合致する従業員エンティティ。見つからない、または例外発生時は null を返します。</returns>
        public EmployeeEntity GetEmployee(string empCd)
        {
            if (string.IsNullOrEmpty(empCd)) { return null; }

            try
            {
                using (TranMng tm = TranMng.BeginTransaction(DbConstants.EmpDbConnection))
                {
                    var dao = new DemoEmployeeDao();
                    return dao.Find(empCd);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 新しい従業員情報をマスタに登録します。
        /// </summary>
        /// <param name="entity">登録する従業員エンティティ</param>
        /// <returns>登録に成功した場合は true、失敗または例外発生時は false を返します。</returns>
        public bool RegisterEmployee(EmployeeEntity entity)
        {
            if (entity == null) { return false; }

            try
            {
                using (TranMng tm = TranMng.BeginTransaction(DbConstants.EmpDbConnection))
                {
                    var dao = new DemoEmployeeDao();

                    // 事前チェック: 同一キーの従業員が既に存在しないか確認
                    var existing = dao.Find(entity.EmpCd);
                    if (existing != null)
                    {
                        // 既に存在する場合は一意制約違反を避けるため登録処理を中断
                        return false;
                    }

                    // 登録実行
                    int result = dao.Insert(entity);

                    if (result > 0)
                    {
                        tm.Commit(); // 影響行数が1行以上あればコミット
                        return true;
                    }

                    return false;
                }
            }
            catch
            {
                // 例外発生時は TranMng の Dispose により自動でロールバックされます
                return false;
            }
        }
    }
}

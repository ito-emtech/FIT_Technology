using DynamicDll.Db;
using FIT_Technology.Models.Constants;
using FIT_Technology.Models.Daos;
using FIT_Technology.Models.Entities;
using FIT_Technology.Models.ViewModels; // ★ViewModelのネームスペースを追加

namespace FIT_Technology.Models.Services
{
    /// <summary>
    /// 従業員の資格保有情報に関する業務ロジック（取得・登録・削除など）を提供するサービスクラスです。
    /// </summary>
    public class DemoGetLicenseService
    {
        /// <summary>
        /// 指定された従業員コードに紐づく、保有資格レコードの一覧を取得します。
        /// </summary>
        /// <param name="empCd">従業員コード</param>
        /// <returns>資格取得エンティティのリスト。例外発生時または該当なしの場合は空のリストを返します。</returns>
        public List<GetLicenseEntity> GetLicensesByEmpCd(string empCd)
        {
            if (string.IsNullOrEmpty(empCd)) { return new List<GetLicenseEntity>(); }

            try
            {
                using (TranMng tm = TranMng.BeginTransaction(DbConstants.EmpDbConnection))
                {
                    var dao = new DemoGetLicenseDao();
                    return dao.FindByEmpCd(empCd) ?? new List<GetLicenseEntity>();
                }
            }
            catch
            {
                // ロギング処理などが必要な場合はここに記述
                return new List<GetLicenseEntity>();
            }
        }

        /// <summary>
        /// 新しい資格取得情報を登録します。
        /// </summary>
        /// <param name="entity">登録する資格取得エンティティ</param>
        /// <returns>登録に成功した場合は true、重複エラーや例外発生時は false を返します。</returns>
        public bool RegisterLicense(GetLicenseEntity entity)
        {
            if (entity == null) { return false; }

            try
            {
                using (TranMng tm = TranMng.BeginTransaction(DbConstants.EmpDbConnection))
                {
                    var dao = new DemoGetLicenseDao();

                    // 事前チェック: 同一の 従業員×資格 が既に登録されていないか確認（一意制約違反の回避）
                    var existing = dao.Find(entity.EmpCd, entity.LicenseCd);
                    if (existing != null)
                    {
                        return false;
                    }

                    // 登録実行
                    int result = dao.Insert(entity);

                    if (result > 0)
                    {
                        tm.Commit(); // 影響行数があれば確実にコミット
                        return true;
                    }

                    return false;
                }
            }
            catch
            {
                // 例外発生時は TranMng の Dispose により自動でロールバック
                return false;
            }
        }

        /// <summary>
        /// 対象の資格取得情報をマスタから削除します。
        /// </summary>
        /// <param name="entity">削除するキー情報を含む資格取得エンティティ</param>
        /// <returns>削除に成功した場合は true、失敗または例外発生時は false を返します。</returns>
        public bool RemoveLicense(GetLicenseEntity entity)
        {
            if (entity == null) { return false; }

            try
            {
                using (TranMng tm = TranMng.BeginTransaction(DbConstants.EmpDbConnection))
                {
                    var dao = new DemoGetLicenseDao();

                    // 削除実行
                    int result = dao.Delete(entity);

                    if (result > 0)
                    {
                        tm.Commit(); // 正常に削除されたらコミット
                        return true;
                    }

                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 複数の資格取得情報をまとめて削除します（一括削除用）。
        /// </summary>
        /// <param name="entities">削除する資格取得エンティティのリスト</param>
        /// <returns>すべての削除が正常に完了した場合は true、1件でも失敗または例外が発生した場合は false を返します。</returns>
        public bool RemoveLicenses(List<GetLicenseEntity> entities)
        {
            if (entities == null || entities.Count == 0) { return false; }

            try
            {
                using (TranMng tm = TranMng.BeginTransaction(DbConstants.EmpDbConnection))
                {
                    var dao = new DemoGetLicenseDao();

                    foreach (var entity in entities)
                    {
                        int result = dao.Delete(entity);
                        if (result == 0)
                        {
                            // 1件でも削除に失敗（対象データが他者によって既に消されていた場合など）したら
                            // 安全のため一連の処理全体を失敗とみなす（コミットせず終了＝自動ロールバック）
                            return false;
                        }
                    }

                    tm.Commit(); // すべての削除が成功したら一括コミット
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 従業員情報に関する業務ロジック（一覧取得・登録など）を提供するサービスクラスです。
    /// </summary>
    public class DemoEmployeeService
    {
        /// <summary>
        /// 保有資格管理メニュー一覧用のデータを取得します（部署名・性別名付き）
        /// </summary>
        /// <returns>資格メニュー用表示データのリスト。例外発生時は空のリストを返します。</returns>
        public List<LicenseMenuRowViewModel> GetLicenseMenuRows()
        {
            try
            {
                // 既存のアーキテクチャ方針（トランザクション管理）に従いTranMngを生成
                using (TranMng tm = TranMng.BeginTransaction(DbConstants.EmpDbConnection))
                {
                    var dao = new DemoEmployeeDao();

                    // 先ほどDAOに作成した、結合クエリ＆MapToEntityを実行するメソッドを呼び出す
                    return dao.FindAllForLicenseMenu() ?? new List<LicenseMenuRowViewModel>();
                }
            }
            catch
            {
                // 必要に応じてロギング処理をここに記述
                return new List<LicenseMenuRowViewModel>();
            }
        }

        /// <summary>
        /// 全ての従業員レコードを取得します。
        /// </summary>
        /// <returns>従業員エンティティのリスト。例外発生時は空のリストを返します。</returns>
        public List<EmployeeEntity> GetEmployees()
        {
            try
            {
                using (TranMng tm = TranMng.BeginTransaction(DbConstants.EmpDbConnection))
                {
                    var dao = new DemoEmployeeDao();
                    return dao.FindAll() ?? new List<EmployeeEntity>();
                }
            }
            catch
            {
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
                        return false;
                    }

                    // 登録実行
                    int result = dao.Insert(entity);

                    if (result > 0)
                    {
                        tm.Commit();
                        return true;
                    }

                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 全ての部署マスタ情報を取得します。
        /// </summary>
        public List<SectionEntity> GetSections()
        {
            try
            {
                using (TranMng tm = TranMng.BeginTransaction(DbConstants.EmpDbConnection))
                {
                    var dao = new SectionDao();
                    return dao.FindAll() ?? new List<SectionEntity>();
                }
            }
            catch
            {
                return new List<SectionEntity>();
            }
        }

        /// <summary>
        /// すべての性別マスタ情報を取得します。
        /// </summary>
        public List<GenderEntity> GetGenders()
        {
            try
            {
                using (TranMng tm = TranMng.BeginTransaction(DbConstants.EmpDbConnection))
                {
                    var dao = new GenderDao();
                    return dao.FindAll() ?? new List<GenderEntity>();
                }
            }
            catch
            {
                return new List<GenderEntity>();
            }
        }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIT_Technology.Models.Entities
{
    /// <summary>
    /// 資格取得テーブル (t_get_license)
    /// 従業員の資格保有情報を管理するテーブル
    /// </summary>
    [Serializable]
    [Table("t_get_license")]
    public class GetLicenseEntity
    {
        /// <summary>
        /// 従業員コード
        /// 参照テーブル: m_employee
        /// </summary>
        [Key]
        [Column("emp_cd", Order = 0, TypeName = "char")]
        [Display(Name = "従業員コード")]
        [Required(ErrorMessage = "{0}は必須項目です。")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "{0}は{1}文字で入力してください。")]
        public string EmpCd { get; set; } = string.Empty;

        /// <summary>
        /// 資格コード
        /// 参照テーブル: m_license
        /// </summary>
        [Key]
        [Column("license_cd", Order = 1, TypeName = "char")]
        [Display(Name = "資格コード")]
        [Required(ErrorMessage = "{0}は必須項目です。")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "{0}は{1}文字で入力してください。")]
        public string LicenseCd { get; set; } = string.Empty;

        /// <summary>
        /// 取得日 (YYYY-MM-DD)
        /// </summary>
        [Column("get_license_date", TypeName = "date")]
        [Display(Name = "取得日")]
        [Required(ErrorMessage = "{0}を選択してください。")]
        [DataType(DataType.Date)]
        public DateTime GetLicenseDate { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        [Column("created", TypeName = "datetime2")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Created { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        [Column("updated", TypeName = "datetime2")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Updated { get; set; }

        // 既存の GetLicenseEntity クラス内に追記
        [Column("license_nm")] // SQLで結合して取得した列名とマッピング
        public string LicenseNm { get; set; } = string.Empty;

        /// <summary>
        /// 画面のテーブル表示用（例：［L0001］ITパスポート）
        /// </summary>
        [NotMapped]
        public string DisplayLicenseText => string.IsNullOrEmpty(LicenseCd)
            ? "（資格未選択）"
            : $"［{LicenseCd.Trim()}］{LicenseNm}";
    }
}
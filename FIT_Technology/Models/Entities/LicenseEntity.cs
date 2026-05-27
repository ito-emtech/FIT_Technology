using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIT_Technology.Models.Entities
{
    /// <summary>
    /// 資格マスタ (m_license)
    /// 資格情報を管理するテーブル
    /// </summary>
    [Serializable]
    [Table("m_license")]
    public class LicenseEntity
    {
        /// <summary>
        /// 資格コード
        /// </summary>
        [Key]
        [Column("license_cd", TypeName = "char")]
        [Display(Name = "資格コード")]
        [Required(ErrorMessage = "{0}は必須項目です。")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "{0}は{1}文字で入力してください。")]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "{0}は半角英数字で入力してください。")]
        public string LicenseCd { get; set; } = string.Empty;

        /// <summary>
        /// 資格名
        /// </summary>
        [Column("license_nm", TypeName = "nvarchar")]
        [Display(Name = "資格名")]
        [Required(ErrorMessage = "{0}は必須項目です。")]
        [StringLength(100, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
        public string LicenseNm { get; set; } = string.Empty;
    }
}
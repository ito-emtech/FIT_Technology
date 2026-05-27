using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIT_Technology.Models.Entities
{
    /// <summary>
    /// 所属マスタ (m_section)
    /// 所属部署情報を管理するテーブル
    /// </summary>
    [Serializable]
    [Table("m_section")]
    public class SectionEntity
    {
        /// <summary>
        /// 所属コード
        /// </summary>
        [Key]
        [Column("section_cd", TypeName = "char")]
        [Display(Name = "所属コード")]
        [Required(ErrorMessage = "{0}は必須項目です。")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "{0}は{1}文字で入力してください。")]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "{0}は半角英数字で入力してください。")]
        public string SectionCd { get; set; } = string.Empty;

        /// <summary>
        /// 所属部署名
        /// </summary>
        [Column("section_nm", TypeName = "nvarchar")]
        [Display(Name = "所属部署名")]
        [Required(ErrorMessage = "{0}は必須項目です。")]
        [StringLength(24, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
        public string SectionNm { get; set; } = string.Empty;
    }
}
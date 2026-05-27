using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIT_Technology.Models.Entities
{
    /// <summary>
    /// 性別マスタ (m_gender)
    /// 性別を管理するテーブル
    /// </summary>
    [Serializable]
    [Table("m_gender")]
    public class GenderEntity
    {
        /// <summary>
        /// 性別コード
        /// </summary>
        [Key]
        [Column("gender_cd", TypeName = "int")]
        [Display(Name = "性別コード")]
        [Required(ErrorMessage = "{0}は必須項目です。")]
        [Range(0, 9, ErrorMessage = "{0}は{1}から{2}の間で入力してください。")]
        public int GenderCd { get; set; }

        /// <summary>
        /// 性別名
        /// </summary>
        [Column("gender_nm", TypeName = "nvarchar")]
        [Display(Name = "性別名")]
        [Required(ErrorMessage = "{0}は必須項目です。")]
        [StringLength(4, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
        public string GenderNm { get; set; } = string.Empty;
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIT_Technology.Models.Entities
{
    /// <summary>
    /// 従業員マスタ (m_employee)
    /// </summary>
    [Serializable]
    [Table("m_employee")]
    public class EmployeeEntity
    {
        /// <summary>
        /// 従業員コード
        /// </summary>
        [Key]
        [Column("emp_cd", TypeName = "char")]
        [Display(Name = "従業員コード")]
        [Required(ErrorMessage = "{0}は必須項目です。")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "{0}は{1}文字で設定してください。")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "{0}は半角英数字で入力してください。")]
        public string EmpCd { get; set; } = string.Empty;

        /// <summary>
        /// 氏
        /// </summary>
        [Column("last_nm", TypeName = "nvarchar")]
        [Display(Name = "氏")]
        [Required(ErrorMessage = "{0}を入力してください。")]
        [StringLength(16, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
        public string LastNm { get; set; } = string.Empty;

        /// <summary>
        /// 名
        /// </summary>
        [Column("first_nm", TypeName = "nvarchar")]
        [Display(Name = "名")]
        [Required(ErrorMessage = "{0}を入力してください。")]
        [StringLength(16, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
        public string FirstNm { get; set; } = string.Empty;

        /// <summary>
        /// 氏（フリガナ）
        /// </summary>
        [Column("last_nm_kana", TypeName = "nvarchar")]
        [Display(Name = "氏（フリガナ）")]
        [Required(ErrorMessage = "{0}を入力してください。")]
        [StringLength(24, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
        [RegularExpression(@"^[ァ-ヶー]+$", ErrorMessage = "{0}は全角カタカナで入力してください。")]
        public string LastNmKana { get; set; } = string.Empty;

        /// <summary>
        /// 名（フリガナ）
        /// </summary>
        [Column("first_nm_kana", TypeName = "nvarchar")]
        [Display(Name = "名（フリガナ）")]
        [Required(ErrorMessage = "{0}を入力してください。")]
        [StringLength(24, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
        [RegularExpression(@"^[ァ-ヶー]+$", ErrorMessage = "{0}は全角カタカナで入力してください。")]
        public string FirstNmKana { get; set; } = string.Empty;

        /// <summary>
        /// 性別コード
        /// </summary>
        [Column("gender_cd", TypeName = "int")]
        [Display(Name = "性別")]
        [Required(ErrorMessage = "{0}を選択してください。")]
        public int GenderCd { get; set; }

        /// <summary>
        /// 生年月日
        /// </summary>
        [Column("birth_date", TypeName = "date")]
        [Display(Name = "生年月日")]
        [Required(ErrorMessage = "{0}を入力してください。")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// 所属コード
        /// </summary>
        [Column("section_cd", TypeName = "char")]
        [Display(Name = "所属コード")]
        [Required(ErrorMessage = "{0}を選択してください。")]
        [StringLength(2, ErrorMessage = "{0}は{1}文字で入力してください。")]
        public string SectionCd { get; set; } = string.Empty;

        /// <summary>
        /// 入社日
        /// </summary>
        [Column("emp_date", TypeName = "date")]
        [Display(Name = "入社日")]
        [Required(ErrorMessage = "{0}を入力してください。")]
        [DataType(DataType.Date)]
        public DateTime EmpDate { get; set; }

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
    }
}
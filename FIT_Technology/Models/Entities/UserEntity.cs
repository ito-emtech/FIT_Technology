using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIT_Technology.Models.Entities
{
    /// <summary>
    /// ユーザマスタ (m_user)
    /// システムを利用できるユーザ情報を管理するテーブル
    /// </summary>
    [Serializable]
    [Table("m_user")]
    public class UserEntity
    {
        /// <summary>
        /// ユーザ ID
        /// </summary>
        [Key]
        [Column("user_id", TypeName = "varchar")]
        [Display(Name = "ユーザID")]
        [Required(ErrorMessage = "{0}は必須項目です。")]
        [StringLength(24, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// パスワード
        /// </summary>
        [Column("password", TypeName = "varchar")]
        [Display(Name = "パスワード")]
        [Required(ErrorMessage = "{0}を入力してください。")]
        [StringLength(32, MinimumLength = 4, ErrorMessage = "{0}は{2}文字以上{1}文字以内で設定してください。")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

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
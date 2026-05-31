using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIT_Technology.Models.ViewModels
{
    /// <summary>
    /// 保有資格管理メニューの1行分のデータを表すビューモデルです。
    /// 既存の EntityMetaHelper と MapToEntity 拡張メソッドに完全対応しています。
    /// </summary>
    public class LicenseMenuRowViewModel
    {
        /// <summary>
        /// 従業員コード
        /// </summary>
        [Column("emp_cd")]
        public string EmpCd { get; set; } = string.Empty;

        /// <summary>
        /// 氏
        /// </summary>
        [Column("last_nm")]
        public string LastNm { get; set; } = string.Empty;

        /// <summary>
        /// 名
        /// </summary>
        [Column("first_nm")]
        public string FirstNm { get; set; } = string.Empty;

        /// <summary>
        /// 氏（フリガナ）
        /// </summary>
        [Column("last_nm_kana")]
        public string LastNmKana { get; set; } = string.Empty;

        /// <summary>
        /// 名（フリガナ）
        /// </summary>
        [Column("first_nm_kana")]
        public string FirstNmKana { get; set; } = string.Empty;

        /// <summary>
        /// 性別コード
        /// </summary>
        [Column("gender_cd")]
        public int GenderCd { get; set; }

        /// <summary>
        /// 性別名（m_gender から結合取得）
        /// </summary>
        [Column("gender_nm")]
        public string GenderNm { get; set; } = string.Empty;

        /// <summary>
        /// 生年月日
        /// </summary>
        [Column("birth_date")]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// 所属コード
        /// </summary>
        [Column("section_cd")]
        public string SectionCd { get; set; } = string.Empty;

        /// <summary>
        /// 部署名（m_section から結合取得）
        /// </summary>
        [Column("section_nm")]
        public string SectionNm { get; set; } = string.Empty;

        /// <summary>
        /// 入社日
        /// </summary>
        [Column("emp_date")]
        public DateTime EmpDate { get; set; }

        /// <summary>
        /// 画面表示用ショートカット：「［A3］人事部」の形式で取得します。
        /// </summary>
        public string DisplaySection => $"［{SectionCd}］{SectionNm}";
    }
}
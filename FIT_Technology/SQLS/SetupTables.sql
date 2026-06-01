/*C:\Users\user\source\repos\FIT_Technology\FIT_Technology\SQLS\SetupTables.sql*/

/* DB作成 */
use [master];
GO

/* 処理件数の表示を無効 */
SET NOCOUNT ON;

if (EXISTS(SELECT * FROM sysdatabases WHERE name IN('[emp_db]','emp_db')))
BEGIN
	ALTER DATABASE [emp_db] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	ALTER DATABASE [emp_db] SET MULTI_USER WITH NO_WAIT;
	DROP DATABASE emp_db;
END
GO

CREATE DATABASE [emp_db] COLLATE Japanese_CI_AS;
GO

/* データベースユーザを作成して権限を付与 */
USE [emp_db];
GO
IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'embexU')
	DROP USER [embexU];
GO
CREATE USER [embexU] FOR LOGIN [embexU];
GRANT CONTROL TO [embexU];
GO

/* 暗黙のトランザクションを有効 */
--SET IMPLICIT_TRANSACTIONS ON;




/*ユーザマスタ作成*/
CREATE TABLE m_user (
    user_id   VARCHAR(24) PRIMARY KEY,
    [password]    VARCHAR(32) NOT NULL,
-- ① インサート時にシステム日時を自動挿入
    created   DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
-- ② インサート時に初期値を入れ、更新は下記のトリガーで制御
    updated   DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);
GO
-- ③ ユーザマスタ更新日時自動入力設定
CREATE TRIGGER TR_m_user_Update
ON m_user
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE m_user
    SET updated = SYSDATETIME()
    FROM m_user t
	--主キーで内部結合
    INNER JOIN inserted i ON t.user_id = i.user_id;
END;
GO

/*所属マスタ作成*/
CREATE TABLE m_section (
    section_cd   CHAR(2) PRIMARY KEY,
    section_nm    NVARCHAR(24) NOT NULL
);
GO

/*性別マスタ*/
CREATE TABLE m_gender (
    gender_cd   INT PRIMARY KEY,
    gender_nm    NVARCHAR(4) NOT NULL
);
GO

/*資格マスタ*/
CREATE TABLE m_license (
    license_cd   CHAR(5) PRIMARY KEY,
    license_nm    NVARCHAR(100) NOT NULL
);
GO

/*従業員マスタ作成*/
CREATE TABLE m_employee (
    emp_cd   CHAR(4) PRIMARY KEY,
    last_nm    NVARCHAR(16) NOT NULL,
    first_nm   NVARCHAR(16) NOT NULL,
	last_nm_kana    NVARCHAR(24) NOT NULL,
    first_nm_kana   NVARCHAR(24) NOT NULL,
	-- m_gender参照
	gender_cd   INT NOT NULL,
	birth_date DATE NOT NULL,
	-- m_section参照
	section_cd CHAR(2) NOT NULL,
	emp_date DATE NOT NULL,
-- ① インサート時にシステム日時を自動挿入
    created   DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
-- ② インサート時に初期値を入れ、更新は下記のトリガーで制御
    updated   DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

	CONSTRAINT FK_m_employee_m_gender  FOREIGN KEY (gender_cd) 
        REFERENCES m_gender (gender_cd),
	CONSTRAINT FK_m_employee_m_section  FOREIGN KEY (section_cd) 
        REFERENCES m_section (section_cd)
);
GO

-- ③ 従業員マスタ更新日時自動入力設定
CREATE TRIGGER TR_m_employee_Update
ON m_employee
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE m_employee
    SET updated = SYSDATETIME()
    FROM m_employee t
	--主キーで内部結合
    INNER JOIN inserted i ON t.emp_cd = i.emp_cd;
END;
GO

/*資格取得テーブル*/
CREATE TABLE t_get_license (
	--m_employee参照
    emp_cd   CHAR(4) NOT NULL,
    --m_license参照
	license_cd    CHAR(5) NOT NULL,
    get_license_date DATE NOT NULL,
-- ① インサート時にシステム日時を自動挿入
    created   DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
-- ② インサート時に初期値を入れ、更新は下記のトリガーで制御
    updated   DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
	CONSTRAINT PK_t_get_license PRIMARY KEY (emp_cd, license_cd),

    CONSTRAINT FK_t_get_license_m_employee  FOREIGN KEY (emp_cd) 
        REFERENCES m_employee (emp_cd),
	CONSTRAINT FK_t_get_license_m_license  FOREIGN KEY (license_cd) 
        REFERENCES m_license (license_cd)
);
GO
-- ③ 資格取得テーブル更新日時自動入力設定
CREATE TRIGGER TR_t_get_license_Update
ON t_get_license
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE t_get_license
    SET updated = SYSDATETIME()
    FROM t_get_license t
	--主キーで内部結合
    INNER JOIN inserted i ON t.emp_cd = i.emp_cd 
       AND t.license_cd = i.license_cd;
END;
GO

USE [emp_db];

BEGIN TRANSACTION;

/* ユーザマスタINSERT */
INSERT INTO m_user (user_id, [password]) VALUES('root', 'pass');
INSERT INTO m_user (user_id, [password]) VALUES('t-yamada', 'taro');
INSERT INTO m_user (user_id, [password]) VALUES('h-suzuki', 'hanako');
INSERT INTO m_user (user_id, [password]) VALUES('i-sato', 'ichiro');

/* 所属マスタINSERT */
INSERT INTO m_section (section_cd, section_nm) VALUES('A1', '管理部');
INSERT INTO m_section (section_cd, section_nm) VALUES('A2', '総務部');
INSERT INTO m_section (section_cd, section_nm) VALUES('B1', '開発部');
INSERT INTO m_section (section_cd, section_nm) VALUES('C1', '営業部');
INSERT INTO m_section (section_cd, section_nm) VALUES('A3', '人事部');
INSERT INTO m_section (section_cd, section_nm) VALUES('A4', '経理部');
INSERT INTO m_section (section_cd, section_nm) VALUES('D1', '企画情報部');
INSERT INTO m_section (section_cd, section_nm) VALUES('E1', '法務・コンプライアンス部');

/* 性別マスタINSERT */
INSERT INTO m_gender (gender_cd, gender_nm) VALUES(0, '不明');
INSERT INTO m_gender (gender_cd, gender_nm) VALUES(1, '男性');
INSERT INTO m_gender (gender_cd, gender_nm) VALUES(2, '女性');
INSERT INTO m_gender (gender_cd, gender_nm) VALUES(3, 'その他');
INSERT INTO m_gender (gender_cd, gender_nm) VALUES(4, '回答拒否');
INSERT INTO m_gender (gender_cd, gender_nm) VALUES(5, '未回答');
INSERT INTO m_gender (gender_cd, gender_nm) VALUES(6, '非開示');
INSERT INTO m_gender (gender_cd, gender_nm) VALUES(9, '適用不能');

/* 資格マスタINSERT */
INSERT INTO m_license (license_cd, license_nm) VALUES('L0001', 'ITパスポート');
INSERT INTO m_license (license_cd, license_nm) VALUES('L0101', '基本情報技術者');
INSERT INTO m_license (license_cd, license_nm) VALUES('L0102', '応用情報技術者');
INSERT INTO m_license (license_cd, license_nm) VALUES('L0103', 'MCP');
INSERT INTO m_license (license_cd, license_nm) VALUES('L0201', 'MOS');
INSERT INTO m_license (license_cd, license_nm) VALUES('L0301', 'LPIC');
INSERT INTO m_license (license_cd, license_nm) VALUES('L0401', 'OCJ-P');

/* =================================================================
   従業員マスタINSERT（計20名）
   ※ emp_cd の先頭2桁と section_cd を総務部が手動で揃えて入力した想定
==================================================================== */
-- A1: 管理部 (2名)
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('A101', '佐藤', '一郎', 'サトウ', 'イチロウ', 1, '2007-08-25', 'A1', '2026-05-28');
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('A102', '田中', '裕子', 'タナカ', 'ユウコ', 2, '1991-07-22', 'A1', '2026-06-01');

-- A2: 総務部 (3名)
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('A201', '山田', '太郎', 'ヤマダ', 'タロウ', 1, '1998-04-30', 'A2', '2026-05-27');
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('A202', '山本', '美穂', 'ヤマモト', 'ミホ', 2, '2001-05-19', 'A2', '2026-06-01');
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('A203', '吉田', '真央', 'ヨシダ', 'マオ', 2, '2000-10-12', 'A2', '2026-06-01');

-- A3: 人事部 (2名)
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('A301', '山本', '美穂', 'ヤマモト', 'ミホ', 2, '2001-05-19', 'A3', '2026-06-01');
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('A302', '林', '修平', 'ハヤシ', 'シュウヘイ', 1, '1989-09-30', 'A3', '2026-06-01');

-- A4: 経理部 (2名)
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('A401', '中村', '大輔', 'ナカムラ', 'ダイスケ', 4, '1994-02-28', 'A4', '2026-06-01');
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('A402', '松本', '香織', 'マツモト', 'カオリ', 2, '1993-11-23', 'A4', '2026-06-01');

-- B1: 開発部 (6名)
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('B101', '鈴木', '花子', 'スズキ', 'ハナコ', 2, '2002-03-04', 'B1', '2026-05-28');
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('B102', '高橋', '健太', 'タカハシ', 'ケンタ', 1, '1995-12-15', 'B1', '2026-06-01');
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('B103', '渡辺', '直樹', 'ワタナベ', 'ナオキ', 1, '1988-11-05', 'B1', '2026-06-01');
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('B104', '山口', '結衣', 'ヤマグチ', 'ユイ', 2, '1997-02-14', 'B1', '2026-06-01');
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('B105', '清水', '美咲', 'シミズ', 'ミサキ', 9, '1996-07-07', 'B1', '2026-06-01');

-- C1: 営業部 (3名)
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('C101', '伊藤', '純', 'イトウ', 'ジュン', 3, '1999-03-11', 'C1', '2026-06-01');
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('C102', '佐々木', '翔', 'ササキ', 'ショウ', 1, '1992-06-25', 'C1', '2026-06-01');
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('C103', '井上', '翼', 'イノウエ', 'ツバサ', 1, '2004-05-05', 'C1', '2026-06-01');

-- D1: 企画情報部 (2名)
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('D101', '小林', '沙織', 'コバヤシ', 'サオリ', 2, '1996-09-04', 'D1', '2026-06-01');
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('D102', '斎藤', '大樹', 'サイトウ', 'ダイキ', 5, '1985-08-09', 'D1', '2026-06-01');

-- E1: 法務・コンプライアンス部 (2名)
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('E101', '加藤', '拓海', 'カトウ', 'タクミ', 1, '2003-01-30', 'E1', '2026-06-01');
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('E102', '木村', '愛', 'キムラ', 'アイ', 6, '1990-01-18', 'E1', '2026-06-01');


/* =================================================================
   資格取得テーブルINSERT（保有上限5個 / 未取得者2名設定）
   ※ 変更された新しい emp_cd に追随させています
==================================================================== */
-- 1. A101 (1個)
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('A101', 'L0103', '2026-05-28');

-- 2. A102 (2個)
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('A102', 'L0001', '2021-05-12');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('A102', 'L0201', '2022-09-01');

-- 3. A201 (3個)
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('A201', 'L0001', '2024-04-15');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('A201', 'L0101', '2025-05-27');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('A201', 'L0401', '2026-05-28');

-- 4. A202 (1個)
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('A202', 'L0201', '2024-05-19');

-- 5. A203 (2個)
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('A203', 'L0001', '2025-08-01');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('A203', 'L0201', '2026-02-15');

-- 6. A301 (★資格未取得者 1人目：レコードなし)

-- 7. A302 (4個)
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('A302', 'L0001', '2019-05-10');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('A302', 'L0101', '2021-11-12');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('A302', 'L0102', '2023-10-05');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('A302', 'L0301', '2024-04-15');

-- 8. A401 (2個)
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('A401', 'L0001', '2026-01-15');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('A401', 'L0201', '2026-04-20');

-- 9. A402 (2個)
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('A402', 'L0201', '2021-06-11');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('A402', 'L0301', '2023-05-14');

-- 10. B101 (4個)
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('B101', 'L0001', '2023-06-10');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('B101', 'L0101', '2024-11-15');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('B101', 'L0102', '2026-05-28');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('B101', 'L0301', '2025-02-20');

-- 11. B102 (★上限マックス 5個)
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('B102', 'L0001', '2022-04-10');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('B102', 'L0101', '2023-10-15');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('B102', 'L0102', '2024-12-20');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('B102', 'L0301', '2025-06-18');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('B102', 'L0401', '2026-03-05');

-- 12. B103 (3個)
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('B103', 'L0101', '2020-11-01');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('B103', 'L0102', '2023-12-01');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('B103', 'L0301', '2025-02-18');

-- 13. B104 (★上限マックス 5個)
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('B104', 'L0001', '2021-04-20');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('B104', 'L0101', '2022-10-18');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('B104', 'L0102', '2023-12-15');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('B104', 'L0103', '2024-08-22');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('B104', 'L0301', '2025-11-30');

-- 14. B105 (2個)
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('B105', 'L0101', '2024-09-09');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('B105', 'L0401', '2025-12-25');

-- 15. C101 (2個)
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('C101', 'L0001', '2024-08-11');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('C101', 'L0201', '2025-09-15');

-- 16. C102 (3個)
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('C102', 'L0101', '2022-06-01');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('C102', 'L0102', '2024-07-10');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('C102', 'L0201', '2023-11-05');

-- 17. C103 (★資格未取得者 2人目：レコードなし)

-- 18. D101 (4個)
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('D101', 'L0001', '2023-01-10');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('D101', 'L0101', '2024-05-25');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('D101', 'L0103', '2024-10-30');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('D101', 'L0401', '2025-07-14');

-- 19. D102 (1個)
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('D102', 'L0001', '2020-09-15');

-- 20. E101 (1個)
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('E101', 'L0103', '2026-03-10');

-- 21. E102 (3個)
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('E102', 'L0001', '2022-02-18');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('E102', 'L0101', '2023-07-21');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('E102', 'L0103', '2025-01-12');

/* トランザクションの確定 */
COMMIT;
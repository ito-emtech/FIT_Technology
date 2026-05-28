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

/* 性別マスタINSERT */
INSERT INTO m_gender (gender_cd, gender_nm) VALUES(0, '不明');
INSERT INTO m_gender (gender_cd, gender_nm) VALUES(1, '男性');
INSERT INTO m_gender (gender_cd, gender_nm) VALUES(2, '女性');
INSERT INTO m_gender (gender_cd, gender_nm) VALUES(3, 'その他');

/* 資格マスタINSERT */
INSERT INTO m_license (license_cd, license_nm) VALUES('L0001', 'ITパスポート');
INSERT INTO m_license (license_cd, license_nm) VALUES('L0101', '基本情報技術者');
INSERT INTO m_license (license_cd, license_nm) VALUES('L0102', '応用情報技術者');
INSERT INTO m_license (license_cd, license_nm) VALUES('L0103', 'MCP');
INSERT INTO m_license (license_cd, license_nm) VALUES('L0201', 'MOS');
INSERT INTO m_license (license_cd, license_nm) VALUES('L0301', 'LPIC');
INSERT INTO m_license (license_cd, license_nm) VALUES('L0401', 'OCJ-P');

/* 従業員マスタINSERT */
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('A123', '山田', '太郎', 'ヤマダ', 'タロウ', 1, '1998-04-30', 'A2', '2026-05-27');
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('B321', '鈴木', '花子', 'スズキ', 'ハナコ', 2, '2002-03-04', 'B1', '2026-05-28');
INSERT INTO m_employee (emp_cd, last_nm, first_nm, last_nm_kana, first_nm_kana, gender_cd, birth_date, section_cd, emp_date) VALUES('C987', '佐藤', '一郎', 'サトウ', 'イチロウ', 1, '2007-08-25', 'A1', '2026-05-28');

/* 資格取得テーブルINSERT */
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('A123', 'L0101', '2026-05-27');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('A123', 'L0401', '2026-05-28');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('B321', 'L0102', '2026-05-28');
INSERT INTO t_get_license (emp_cd, license_cd, get_license_date) VALUES('C987', 'L0103', '2026-05-28');

/* トランザクションの確定 */
COMMIT;
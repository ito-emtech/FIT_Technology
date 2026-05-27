/* 処理件数の表示を無効 */
SET NOCOUNT ON;


/* 暗黙のトランザクションを無効 */
SET IMPLICIT_TRANSACTIONS OFF;


/* DB作成 */
use [master];
if (EXISTS(SELECT * FROM sysdatabases WHERE name IN('[user_db]','user_db')))
begin
	ALTER DATABASE [user_db] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	ALTER DATABASE [user_db] SET MULTI_USER WITH NO_WAIT;
	DROP DATABASE user_db;
end
CREATE DATABASE [user_db] COLLATE Japanese_CI_AS;
GO

/* 暗黙のトランザクションを有効 */
SET IMPLICIT_TRANSACTIONS ON;


/* ログインを作成 -（作成が必要な場合のみ実行）*/
--if exists (select name from sys.server_principals where name = N'embexU')
--	DROP LOGIN [embexU];
--CREATE LOGIN [embexU] WITH PASSWORD=N'embexP', DEFAULT_DATABASE=[user_db], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF;


/* データベースユーザを作成して権限を付与 */
USE [user_db];
IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'embexU')
	DROP USER [embexU];
CREATE USER [embexU] FOR LOGIN [embexU];
GRANT CONTROL TO [embexU];


/*ユーザマスタ作成*/
CREATE TABLE m_user (
    user_id     VARCHAR(16)     NOT NULL,
    password    VARCHAR(16)     NOT NULL,
    name        NVARCHAR(40)    NOT NULL,
    age         INT             NULL
    PRIMARY KEY (user_id)
);

/* ユーザマスタINSERT */
INSERT INTO m_user VALUES('t-yamada', 'taro', '山田太郎', 25);
INSERT INTO m_user VALUES('h-suzuki', 'hanako', '鈴木花子', 32);
INSERT INTO m_user VALUES('i-sato', 'ichiro', '佐藤一郎', 43);


/* トランザクションの確定 */
COMMIT;

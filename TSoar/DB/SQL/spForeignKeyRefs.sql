SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('spForeignKeyRefs') IS NOT NULL) DROP PROCEDURE spForeignKeyRefs
GO
IF (TYPE_ID('tvp_FKRefs') IS NOT NULL) DROP TYPE tvp_FKRefs
CREATE TYPE tvp_FKRefs AS TABLE
(
	iNumFKRefs int,
	sPrimaryTable nvarchar(30),
	iPrimaryKey int,
	sFKTable nvarchar(30)
)
-- iNumFKRefs = number of foreign key references found by this stored procedure
-- sPrimaryTable = the name of the primary table for which we are looking for foreign key references
-- iPrimaryKey = the value of the primary key that is referenced by foreign key tables
-- sFKTable = the name of the Foreign Key Table in which iNumFKRefs references were found
GO
CREATE PROCEDURE spForeignKeyRefs
	@sinPrimaryTable nvarchar(30), @inPrimaryKey int
	-- @sinPrimaryTable = name of primary table for which we are seeking the number of foreign key references
	-- @inPrimaryKey = value of the primary key in @sinPrimaryTable for which we are counting the number of rows
		-- in foreign tables that refer to this primary key value; it must be an integer.
AS
BEGIN
	DECLARE @NUMFKREFS tvp_FKRefs
	DECLARE @sMsg nvarchar(2048)
	DECLARE @iCount int = 0
	DECLARE @SQL nvarchar(MAX)

	IF @sinPrimaryTable IS NULL
	BEGIN
		SET @sMsg = 'Stored procedure spForeignKeyRefs: input parameter PrimaryTable cannot be null.';
		THROW 72456, @sMsg, 7; -- 72456 and 7 are random rather meaningless choices
		RETURN
	END

	IF @inPrimaryKey IS NULL OR @inPrimaryKey < 1
	BEGIN;
		SET @sMsg = 'Stored procedure spForeignKeyRefs: input parameter PrimaryKey cannot be null or less than 1. Primary table is '
			+ @sinPrimaryTable + '.';
		THROW 72459, @sMsg, 7; -- 72459 and 7 are random rather meaningless choices
		RETURN
	END

	DECLARE @lNumFKRefs int
	DECLARE @sConstraintName nvarchar(2048)
	DECLARE @sPrimaryTable nvarchar(30)
	DECLARE @sPrimaryKey nvarchar(30)
	DECLARE @sFKTable nvarchar(30)
	DECLARE @sFKField nvarchar(30)
	DECLARE @sDelRefAction nvarchar(12)
	DECLARE @sUpdRefAction nvarchar(12)

	DECLARE FKTables CURSOR FOR
		-- from: https://www.mssqltips.com/sqlservertip/4753/list-dependencies-for-sql-server-foreign-keys/
		SELECT 
			C.CONSTRAINT_NAME [constraint_name] 
		   ,C.TABLE_NAME [referencing_table_name] 
		   ,KCU.COLUMN_NAME [referencing_column_name] 
		   ,C2.TABLE_NAME [referenced_table_name] 
		   ,KCU2.COLUMN_NAME [referenced_column_name]
		   ,RC.DELETE_RULE delete_referential_action_desc 
		   ,RC.UPDATE_RULE update_referential_action_desc
		FROM   INFORMATION_SCHEMA.TABLE_CONSTRAINTS C 
			   INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU 
				 ON C.CONSTRAINT_SCHEMA = KCU.CONSTRAINT_SCHEMA 
					AND C.CONSTRAINT_NAME = KCU.CONSTRAINT_NAME 
			   INNER JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC 
				 ON C.CONSTRAINT_SCHEMA = RC.CONSTRAINT_SCHEMA 
					AND C.CONSTRAINT_NAME = RC.CONSTRAINT_NAME 
			   INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS C2 
				 ON RC.UNIQUE_CONSTRAINT_SCHEMA = C2.CONSTRAINT_SCHEMA 
					AND RC.UNIQUE_CONSTRAINT_NAME = C2.CONSTRAINT_NAME 
			   INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU2 
				 ON C2.CONSTRAINT_SCHEMA = KCU2.CONSTRAINT_SCHEMA 
					AND C2.CONSTRAINT_NAME = KCU2.CONSTRAINT_NAME 
					AND KCU.ORDINAL_POSITION = KCU2.ORDINAL_POSITION 
		WHERE  C.CONSTRAINT_TYPE = 'FOREIGN KEY'
		AND  C2.TABLE_NAME = @sinPrimaryTable AND RC.DELETE_RULE='CASCADE'

	OPEN FKTables
	FETCH NEXT FROM FKTables INTO @sConstraintName, @sFKTable, @sFKField, @sPrimaryTable, @sPrimaryKey,
		@sDelRefAction, @sUpdRefAction
	WHILE @@FETCH_STATUS=0
	BEGIN
		SET @iCount = @iCount + 1
		SET @lNumFKRefs = 0
		IF (OBJECT_ID('INTERNALAUXTEMP') IS NOT NULL) DROP TABLE INTERNALAUXTEMP
		SET @SQL = N'SELECT * INTO INTERNALAUXTEMP FROM (SELECT F.ID FROM ' + @sFKTable + N' AS F INNER JOIN ' + @sPrimaryTable + N' AS P ON F.'
			+ @sFKField + N' = P.' + @sPrimaryKey + ' WHERE (P.' + @sPrimaryKey + N'= ' + CONVERT(nvarchar(12),@inPrimaryKey) + N')) AS X'

		EXEC sp_executesql @SQL
		SELECT @lNumFKRefs = COUNT(*) FROM INTERNALAUXTEMP
		INSERT INTO @NUMFKREFS VALUES (@lNumFKRefs, @sinPrimaryTable, @inPrimaryKey, @sFKTable)

		FETCH NEXT FROM FKTables INTO @sConstraintName, @sFKTable, @sFKField, @sPrimaryTable, @sPrimaryKey,
			@sDelRefAction, @sUpdRefAction
	END
	CLOSE FKTables
	DEALLOCATE FKTables
	IF @iCount < 1
	BEGIN
		SET @sMsg = 'Stored procedure spForeignKeyRefs: input parameter PrimaryTable `' + @sinPrimaryTable + 
			'` either: does not exist, or has no `CASCADE` delete rules, or is not the parent in any foreign key relationships to other tables.';
		THROW 72457, @sMsg, 7; -- 72457 and 7 are random rather meaningless choices
		RETURN
	END
	SELECT * FROM @NUMFKREFS
END
GO

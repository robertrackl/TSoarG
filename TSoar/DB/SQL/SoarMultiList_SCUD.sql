SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE IntSoarDB
GO
IF (OBJECT_ID('SoarMultiList_SCUD') IS NOT NULL) DROP PROCEDURE SoarMultiList_SCUD
GO
IF (TYPE_ID('tvp_SCUD') IS NOT NULL) DROP TYPE tvp_SCUD
CREATE TYPE tvp_SCUD AS TABLE (ix int, sPar nvarchar(1024));
-- ix is an index; sPar is a parameter value
GO
-- ===============================================
-- Author:		Robert Rackl
-- Create date: 2017 Sep 25
-- Description:	For tables with multiple fields, perform SCUD ops
-- ===============================================
CREATE PROCEDURE SoarMultiList_SCUD
	@sStatus nvarchar(2048) OUTPUT,
	@sAction nvarchar(10),
	@sInfoType nvarchar(25), -- What kind of data?
	@sKey nvarchar(1024) = NULL,
	@saInput tvp_SCUD READONLY
	-- Table @saInput provides input strings for a row in table @sTable (defined further below)
	-- The ix field in table @saInput needs to start counting with 2 and increment by 1 for each input value row
	-- For INSERT and UPDATE operations, the number of rows in table @saInput must equal one less than the number of columns in table @sTable
AS
BEGIN
	SET NOCOUNT ON;
	SET @sStatus = 'OK' -- optimistic
	DECLARE @sPrefix nvarchar(64)
	SET @sPrefix = 'Stored Procedure SoarMultiList_SCUD reports: '
	-- DEBUG
	DECLARE @sDebug varchar(6)
	DECLARE @lDebug int
	IF LEN(@sKey) > 6
	BEGIN
		SET @sDebug = SUBSTRING(@sKey,1,6)
		IF @sDebug = 'DEBUG_'
		BEGIN
			SET @sDebug = SUBSTRING(@sKey,7,20)
			SET @lDebug = CONVERT(int, @sDebug)
			SET @sKey = SUBSTRING(@sKey,21,1004)
		END
	END
	IF @lDebug = 10
	BEGIN
		SET @sStatus = @sPrefix + 'Debug: Entry'
		RETURN
	END
	
	DECLARE @SQL nvarchar(MAX)
	DECLARE @SQLflds nvarchar(MAX)
	DECLARE @SQLvals nvarchar(MAX)
	DECLARE @sIn nvarchar(1024)
	DECLARE @sTable nvarchar(20)
	DECLARE @sOrderBy nvarchar(MAX) -- SQL snippet for sorting
	DECLARE @terr int
	DECLARE @iCount int
	DECLARE @iSubCount int
	DECLARE @sView nvarchar(20)
	SET @SQL = ' '
	DECLARE @sCol varchar(25)
	DECLARE @iMax_length int
	DECLARE @sInput nvarchar(1024)
	DECLARE @sPunctuation nchar(2)  -- for creating comma-separated lists that must not start with a comma
	DECLARE @sKeyFld varchar(25)
	DECLARE @sPunctu char(1) -- is defined in table INTERNALAUXMULTI for surrounding sKeyFld data with appropriate punctuation
	DECLARE @sQPunctu char(1) -- is defined in table INTERNALAUXMULTI for surrounding sUniqueFld data with appropriate punctuation
	DECLARE @cPunct nchar(1) -- is set further below depending on the data type
	DECLARE @system_data_type nvarchar(25)
	DECLARE @sUniqueFld nvarchar(25)
	DECLARE @sFld varchar(100)
	DECLARE @sCompOp varchar(5)
	DECLARE @sFPunct char(1)
	DECLARE @sFValue varchar(MAX)
	DECLARE @ix int
	SET @sPunctuation = '  ' --Used in the UPDATE and INSERT portions below
	SELECT @sTable=sTable, @sView=sView, @sKeyFld=sKeyFld, @sPunctu=sPunctu, @sOrderBy=sOrderBy, @sUniqueFld=sUniqueFld, @sQPunctu=sQPunctu
		FROM INTERNALAUXMULTI WHERE sMFF=@sInfoType
	-- Table @sTable is presumed to have an ID column as its first column
	SET @iCount = @@ROWCOUNT
	IF @iCount <> 1
	BEGIN
		SELECT @sStatus = STUFF((SELECT ', "' + sMFF + '"' FROM INTERNALAUXMULTI FOR XML PATH('')), 1, 1, '')
		SET @sStatus = @sPrefix + 'Parameter sMFF is "' + @sInfoType + '", but should be one of: ' + @sStatus
		-- "Member Data Basic", "People Offices", "Membership FromTo",' +
		--	' "SSA Member FromTo", "People Qualifs", "Equity Shares", "Equipment", "Equipment Components", "Equipment Parameters", "Equipment OpHours",
		--	 "Operations", "OpDetails", "Aviators", "FlyingCharges", or "Contacts"'
		RETURN
	END

	BEGIN TRANSACTION
		BEGIN TRY
			--SELECT
			IF @sAction = 'SELECT'
				BEGIN
					SET @SQL = 'SELECT * FROM ' + @sView + ' ' + @sOrderBy
				END

			--SELECTONE
			ELSE IF @sAction = 'SELECTONE'
				BEGIN
					SET @SQL = 'SELECT * FROM ' + @sView + ' WHERE ' + @sKeyFld + ' = ' + @sPunctu + @sKey + @sPunctu
				END

			--EXISTS
			ELSE IF @sAction = 'EXISTS'
				BEGIN
					-- Does a record exist which has the contents of saInput[0].sPar in its @sUniqueFld?
					--DECLARE @eCount int
					DECLARE @sPar nvarchar(MAX)
					DECLARE @iID int
					SELECT TOP 1 @sPAR = sPar FROM @saInput
					EXECUTE tnp_ID_from_sUniqueFld @sTable, @sPar, @iID=@iID OUTPUT
					SET @sStatus = 'OK' + CONVERT(varchar(12),@iID)
					ROLLBACK TRANSACTION
					RETURN
				END

			--UPDATE or INSERT
			ELSE IF @sAction = 'UPDATE' OR @sAction = 'INSERT'
			  BEGIN
				IF @lDebug = 40
				BEGIN
					SET @sStatus = @sPrefix + 'Debug: Entered UPDATE or INSERT block for table `' + @sTable + '`'
					ROLLBACK TRANSACTION
					RETURN
				END
				SET @SQLflds = ''
				SET @SQLvals = ''
				SET @SQL = 'UPDATE ' + @sTable + ' SET '

				IF OBJECT_ID('TCOLS', 'U') IS NOT NULL DROP TABLE TCOLS; 
				DECLARE @bIsNullable bit
				DECLARE @lLensInput int
				DECLARE @SQLcols nvarchar(MAX)
				DECLARE @eCount int
				-- What are the columns of table @sTable ?
				SET @SQLcols = 'SELECT ROW_NUMBER() OVER (ORDER BY AC.[column_id]) as iRow, AC.[name] AS [column_name], TY.[name] AS system_data_type,' +
						'AC.[max_length], AC.[is_nullable] ' +
					'INTO TCOLS	FROM sys.[tables] AS S INNER JOIN sys.[all_columns] AC ON S.[object_id] = AC.[object_id] ' +
					'INNER JOIN sys.[types] TY ON AC.[system_type_id] = TY.[system_type_id] AND AC.[user_type_id] = TY.[user_type_id] ' +
					'WHERE ((S.[is_ms_shipped] = 0) AND (S.name=' + char(39) + @sTable + char(39) + ')) ORDER BY AC.[column_id]'
				EXEC sp_executesql @SQLcols
				SET @eCount = @@ROWCOUNT

				IF @lDebug = 43
				BEGIN
					SET @sStatus = @sPrefix + 'Debug: just after SELECT ... INTO TCOLS, @eCount=' + CONVERT(varchar(12),@eCount)
					ROLLBACK TRANSACTION
					RETURN
				END

				SELECT @iCount=Count(*) FROM @saInput
				IF @eCount <> @iCount + 1
				BEGIN
					SET @sStatus = @sPrefix + 'Input table row count ' +
						CONVERT(varchar(12),@iCount) + ' does not equal one less than the number of columns ' + CONVERT(varchar(12),@eCount) +
						' in table ' + @sTable
					ROLLBACK TRANSACTION
					RETURN
				END

				IF @lDebug = 46
				BEGIN
					SET @sStatus = @sPrefix + 'Debug: about to declare cursor'
					ROLLBACK TRANSACTION
					RETURN
				END

				DECLARE @lFetchCount int = 0
				DECLARE CC CURSOR FOR SELECT E.column_name, COLUMNPROPERTY(OBJECT_ID(@sTable),E.column_name,'PRECISION'),
					E.is_nullable, E.system_data_type, I.sPar FROM TCOLS AS E INNER JOIN @saInput AS I ON I.ix = E.iRow
				OPEN CC
				FETCH NEXT FROM CC INTO @sCol, @iMax_length, @bIsNullable, @system_data_type, @sInput
				WHILE @@FETCH_STATUS = 0
				BEGIN
					SET @lFetchCount = @lFetchCount + 1
					IF @lDebug = 47
					BEGIN
						SET @sStatus = @sPrefix + 'Debug: after first cursor fetch, @sCol=' + @sCol
						ROLLBACK TRANSACTION
						RETURN
					END

					SET @cPunct = dbo.GetPunct4DType(@system_data_type)
					SET @lLensInput = LEN(@sInput)
					IF @sInput='^_^'
					  BEGIN
						SET @lLensInput = 0
					  END
					IF @lLensInput < 1 AND @bIsNullable = 0
					BEGIN
						SET @sStatus = @sPrefix + 'Column ' + @sCol + ' has no input text, but the column is not nullable ' +
							', i.e., some meaningful input is required'
						ROLLBACK TRANSACTION
						RETURN
					END
					IF @system_data_type = 'smalldatetime' -- make an exception
					BEGIN
						SET @iMax_length = 20
					END
					IF @system_data_type = 'datetime2' -- make an exception
					BEGIN
						SET @iMax_length = 27
					END
					if @system_data_type = 'datetimeoffset' -- make another exception
					BEGIN
						set @iMax_length = 34
					END
					IF @iMax_Length > -1 AND @lLensInput > @iMax_length
					BEGIN
						SET @sStatus = @sPrefix + 'Input text length for ' + @sCol + ' must be at most ' +
							CONVERT(varchar(12),@iMax_Length) + ' characters long, not ' + CONVERT(varchar(12),@lLensInput)
						ROLLBACK TRANSACTION
						RETURN
					END
					IF @lLensInput < 1
					BEGIN
						SET @sInput = 'NULL'
					END ELSE
					BEGIN
						SET @sInput = @cPunct + @sInput + @cPunct
					END
					SET @SQLflds = @SQLflds + @sPunctuation + @sCol -- for INSERTing
					SET @SQLvals = @SQLvals + @sPunctuation + @sInput -- for INSERTing
					SET @SQL = @SQL + @sPunctuation + @sCol + ' = ' + @sInput -- for UPDATEing
					SET @sPunctuation = ', '

					IF @lDebug = 49 AND @lFetchCount > 2
					BEGIN
						SET @sStatus = @sPrefix + 'Debug: about to do second cursor fetch, @SQLflds=`' + @SQLflds + '`, @SQLvals=`' + @SQLvals + '`'
						ROLLBACK TRANSACTION
						RETURN
					END

					FETCH NEXT FROM CC INTO @sCol, @iMax_length, @bIsNullable, @system_data_type, @sInput
				END
				DROP TABLE TCOLS

				IF @lDebug = 50
				BEGIN
					SET @sStatus = @sPrefix + 'Debug: Just dropped table TCOLS, @SQLflds=`' + @SQLflds + '`, @SQLvals=`' + @SQLvals + '`'
					ROLLBACK TRANSACTION
					RETURN
				END

				IF @sAction = 'UPDATE'
				  BEGIN
					SET @SQL = @SQL + ' WHERE ' + @sKeyFld + ' = ' + @sPunctu + @sKey + @sPunctu
				  END
				ELSE
				  BEGIN
					SET @SQL = 'INSERT INTO ' + @sTable + ' (' + @SQLflds + ') VALUES (' + @SQLvals + ')'
				  END
				IF @lDebug = 60
				  BEGIN
					SET @sStatus = @sPrefix + 'Debug: at end of UPDATE or INSERT block, @SQL=`' + @SQL + '`'
					ROLLBACK TRANSACTION
					RETURN
				  END
			  END
 
			--DELETE
			ELSE IF @sAction = 'DELETE'
				BEGIN
					SET @SQL = 'DELETE FROM ' + @sTable + ' WHERE ' + @sKeyFld + ' = ' + @sPunctu + @sKey + @sPunctu
				END

			--BAD CHOICE
			ELSE
				BEGIN
					SET @sStatus = @sPrefix + 'Parameter sAction is not SELECT, SELECTONE, EXISTS, INSERT, UPDATE, or DELETE, but ' +
						@sAction
					ROLLBACK TRANSACTION
					RETURN
				END

--			IF @sAction='UPDATE' -- for debugging only
--			BEGIN
--				SET @sStatus=@sPrefix + '@SQL=`' + @SQL + '`'
--				ROLLBACK TRANSACTION
--				RETURN
--			END

			IF @lDebug = 90
			BEGIN
				SET @sStatus = @sPrefix + 'Debug 90: @SQL=`' + @SQL + '`'
				ROLLBACK TRANSACTION
				RETURN
			END
			IF @lDebug = 99
			BEGIN
				INSERT INTO DIAGNOSTICS (PiT,sKind,sDescription) VALUES (GETUTCDATE(),'Debug 99','SoarMultiList_SCUD about to execute @SQL=`' + @SQL + '`')
			END
			--Hopefully, everything is ok, so ...
			EXEC sp_executesql @SQL

			IF @sAction = 'INSERT'
			  BEGIN
				SET @sStatus = 'OK' + CONVERT(varchar(12),@@IDENTITY)
			  END
		END TRY
		BEGIN CATCH
			SET @terr = @@ERROR
			SET @sStatus = @sPrefix + 'Error ' + CONVERT(varchar,@terr) + ', @SQL=`' + @SQL + '`'
			ROLLBACK TRANSACTION
			RETURN
		END CATCH
	COMMIT TRANSACTION

	RETURN
END
GO

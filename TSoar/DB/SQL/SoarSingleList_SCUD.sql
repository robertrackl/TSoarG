SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('SoarSingleList_SCUD') IS NOT NULL) DROP PROCEDURE SoarSingleList_SCUD
GO
-- ===============================================
-- Author:		Robert Rackl
-- Create date: 2017 Sep 25
-- Description:	For tables with just two fields (ID and a string field), perform SCUD ops
-- ===============================================
CREATE PROCEDURE SoarSingleList_SCUD
	@sStatus nvarchar(MAX) OUTPUT,
	@sAction nvarchar(10), -- SELECT, INSERT, UPDATE, NUMFKREFS, DELETE
	@sMFF nvarchar(25), -- Which table
	@sInput1 nvarchar(1024) = NULL, -- data for INSERT, UPDATE, NUMFKREFS, DELETE
	@sInput2 nvarchar(1024) = NULL -- data for UPDATE
AS
BEGIN
	SET NOCOUNT ON;
	SET @sStatus = N'OK' -- optimistic
	DECLARE @sPrefix nvarchar(64)
	SET @sPrefix = N'Stored Procedure SoarSingleList_SCUD reports: '
	DECLARE @SQL nvarchar(1024)
	DECLARE @sIn nvarchar(1024)
	DECLARE @sIn2 nvarchar(1024)
	DECLARE @sTable nvarchar(30)
	DECLARE @sField nvarchar(MAX)
	DECLARE @sFKTable nvarchar(30) -- foreign key table
	DECLARE @sFKField nvarchar(30) -- foreign key field
	DECLARE @terr int
	DECLARE @iCount int
	DECLARE @lColLength int
	DECLARE @lLensInput int
	DECLARE @lLensInput2 int
	-- Note: 'FK' means Foreign Key
	DECLARE @Tmff table(ID int, sMFF nvarchar(25),        sTable nvarchar(30),     sField nvarchar(MAX),   sFKTable nvarchar(30), sFKField nvarchar(30))
	INSERT INTO @Tmff VALUES( 1, N'Qualifications',       N'QUALIFICATIONS',       N'sQualification',      N'PEOPLEQUALIFICS',    N'iQualification')
	INSERT INTO @Tmff VALUES( 2, N'Ratings',              N'RATINGS',              N'sRating',             N'PEOPLERATINGS',      N'iRating')
	INSERT INTO @Tmff VALUES( 3, N'Certifications',       N'CERTIFICATIONS',       N'sCertification',      N'PEOPLECERTIFICS',    N'iCertification')
	INSERT INTO @Tmff VALUES( 4, N'Membership Categories',N'MEMBERSHIPCATEGORIES', N'sMembershipCategory', N'MEMBERFROMTO',       N'iMemberCategory')
	INSERT INTO @Tmff VALUES( 5, N'SSA Member Categories',N'SSA_MEMBERCATEGORIES', N'sSSA_MemberCategory', N'SSA_MEMBERFROMTO',   N'iSSA_MemberCategory') -- SSA = Soaring Society of America
	INSERT INTO @Tmff VALUES( 6, N'Aviator Roles',        N'AVIATORROLES',         N'sAviatorRole',        N'AVIATORS',           N'iAviatorRole')
	INSERT INTO @Tmff VALUES( 7, N'Equipment Roles',      N'EQUIPMENTROLES',       N'sEquipmentRole',      N'OPDETAILS',          N'iEquipmentRole')
	INSERT INTO @Tmff VALUES( 8, N'Special Ops Types',    N'SPECIALOPTYPES',       N'sSpecialOpType',      N'SPECIALOPS',         N'iSpecialOpType')
	INSERT INTO @Tmff VALUES( 9, N'Launch Methods',       N'LAUNCHMETHODS',        N'sLaunchMethod',       N'OPERATIONS',         N'iLaunchMethod')
	INSERT INTO @Tmff VALUES(10, N'Equipment Types',      N'EQUIPTYPES',           N'sEquipmentType',      N'EQUIPMENT',          N'iEquipType')
	INSERT INTO @Tmff VALUES(11, N'Equipment Action Types',N'EQUIPACTIONTYPES',    N'sEquipActionType',    N'EQUIPAGINGPARS',     N'iEquipActionType')
	INSERT INTO @Tmff VALUES(12, N'Contact Types',        N'CONTACTTYPES',         N'sPeopleContactType',  N'PEOPLECONTACTS',     N'iContactType')
	INSERT INTO @Tmff VALUES(13, N'Board Offices',        N'BOARDOFFICES',         N'sBoardOffice',        N'PEOPLEOFFICES',      N'iBoardOffice')
	INSERT INTO @Tmff VALUES(14, N'Locations',            N'LOCATIONS',            N'sLocation',           N'OPERATIONS',         N'iTakeoffLoc')
	INSERT INTO @Tmff VALUES(15, N'ChargeCodes',          N'CHARGECODES',          N'sChargeCode',         N'OPERATIONS',         N'iChargeCode')
	INSERT INTO @Tmff VALUES(16, N'FA Accounting Items',  N'FA_ITEMS',             N'sFA_ItemCode',        N'RATES',              N'iFA_Item')
	INSERT INTO @Tmff VALUES(16, N'FA Payment Terms',     N'FA_PMTTERMS',          N'ID',                  N'RATES',              N'iFA_PmtTerm')
	INSERT INTO @Tmff VALUES(17, N'QBO Accounting Items', N'QBO_ITEMNAMES',        N'sQBO_ItemName',       N'RATES',              N'iQBO_ItemName')
	INSERT INTO @Tmff VALUES(18, N'Invoice Sources',      N'INVOICESOURCES',       N'sInvoiceSource',      N'INVOICES',           N'iInvoiceSource')
	INSERT INTO @Tmff VALUES(19, N'FlightOpsSchedCategs', N'FSCATEGS',             N'sCateg',              N'FSSIGNUPS',          N'iCateg')
	
	DECLARE @iMFF int
	-- Mnemonic MFF or mff refers to one of the single list tables like RATINGS, EQUIPTYPES, etc.
	SELECT @iMFF=ID, @sTable=sTable, @sField=sField, @sFKTable=sFKTable, @sFKField=sFKField FROM @Tmff WHERE sMFF=@sMFF
	SET @iCount = @@ROWCOUNT
	IF @iCount <> 1
	BEGIN
		DECLARE @MFFList nvarchar(max);
		SET @MFFList = N'';
		SELECT @MFFList += N'"' + sMFF + N'", ' FROM @Tmff;
		SELECT LEFT(@MFFList,LEN(@MFFList)-2);
		SET @sStatus = @sPrefix + N'Parameter sMFF is "' + @sMFF + N'", but should be one of: ' + @MFFList
		RETURN
	END
	SET @SQL = N'[empty]'
	SET @sIn = RTRIM(LTRIM(@sInput1))
	SET @sIn2 = RTRIM(LTRIM(@sInput2))
	SET @lLensInput = LEN(@sIn)
	SET @lLensInput2 = LEN(@sIn2)
	BEGIN TRANSACTION
		BEGIN TRY
			  --SELECT
			IF @sAction = N'SELECT'
				BEGIN
					SET @SQL = N'SELECT * FROM ' + @sTable + N' ORDER BY ' + @sField
					--SET @sStatus = N'Debug from SELECT: @SQL=' + @SQL
				END

			  --SELECTONE
			ELSE IF @sAction = N'SELECTONE'
				BEGIN
					SET @SQL = N'SELECT ' + @sField + N' FROM ' + @sTable + N' WHERE ID=' + @sInput1
				END

			  --EXISTS
			ELSE IF @sAction = N'EXISTS'
				BEGIN
					SET @SQL = N'SELECT ID FROM ' + @sTable + N' WHERE ' + @sField + ' = ' + nchar(39) + @sInput1 + nchar(39)
				END
 
			  --CREATE/INSERT
			ELSE IF @sAction = N'INSERT'
			  BEGIN
				IF @lLensInput < 1
				BEGIN
					SET @sIn = (SELECT sMFF FROM @Tmff WHERE ID=@iMFF)
					-- Form singular from plural
					IF RIGHT(@sIn, 3) = 'ies'
					  BEGIN
						SET @sIn = SUBSTRING(@sIn, 1, LEN(@sIn) - 3) + 'y'
					  END
					IF RIGHT(@sIn, 1) = 's'
					  BEGIN
						SET @sIn = SUBSTRING(@sIn, 1, LEN(@sIn) - 1)
					  END
					SET @sStatus = @sPrefix + N'Attempt at inserting an empty ' + @sIn
					ROLLBACK TRANSACTION
					RETURN
				END
				SET @lColLength = COLUMNPROPERTY(OBJECT_ID(@sTable),@sField, N'PRECISION')
				IF @lColLength <> -1 --Is the field MAX length?
				BEGIN
					IF @lLensInput > @lColLength
					BEGIN
						SET @sStatus = @sPrefix + N'Input text length must be at most ' + CONVERT(nvarchar(12),@lColLength) +
							N' characters long, not ' + CONVERT(nvarchar(12),@lLensInput)
						ROLLBACK TRANSACTION
						RETURN
					END
				END
			    SET @SQL = N'SELECT ID FROM ' + @sTable + N' WHERE ' + @sField + N'=' + nchar(39) + @sIn + nchar(39)
				EXEC sp_executesql @SQL
				SET @iCount = @@ROWCOUNT
				IF @iCount > 0
				BEGIN
					SET @sStatus = @sPrefix + N'Attempt at Duplication: `' + @sIn + N'` exists already'
					ROLLBACK TRANSACTION
					RETURN
				END
				SET @SQL = N'INSERT INTO ' + @sTable + N'(' + @sField + N')' +
						N' VALUES (' + nchar(39) + @sIn + nchar(39) + N')'
			  END

			  --UPDATE
			ELSE IF @sAction = N'UPDATE'
			  BEGIN
				SET @lColLength = COLUMNPROPERTY(OBJECT_ID(@sTable),@sField, N'PRECISION')
				IF ((@lLensInput < 1) OR (@lLensInput > @lColLength))
				BEGIN
					SET @sStatus = @sPrefix + N'Input text length 1 must not be empty and at most ' + CONVERT(varchar(12),@lColLength) +
						N' characters long, not ' + CONVERT(varchar(12),@lLensInput)
					ROLLBACK TRANSACTION
					RETURN
				END
				IF ((@lLensInput2 < 1) OR (@lLensInput2 > @lColLength))
				BEGIN
					SET @sStatus = @sPrefix + N'Input text length must not be empty and at most ' + CONVERT(varchar(12),@lColLength) +
						N' characters long, not ' + CONVERT(varchar(12),@lLensInput2)
					ROLLBACK TRANSACTION
					RETURN
				END
				SET @SQL = N'UPDATE ' + @sTable + N' SET ' + @sField + N' = ' + nchar(39) +
					@sIn2 + nchar(39) + N' WHERE ' + @sField + N' = ' + nchar(39) + @sIn + nchar(39)
			  END

			  --NUMFKREFS - number of foreign key references
			ELSE IF @sAction = N'NUMFKREFS'
			  BEGIN
				DECLARE @lNumFKRefs int = 0
				IF OBJECT_ID('MYTEMP') IS NOT NULL
				BEGIN
					DROP TABLE MYTEMP -- Note: cannot use temporary table in conjunction with dynamic SQL
				END
				-- Set up the dynamic sql statement:
				SET @SQL = N'SELECT * INTO MYTEMP FROM (SELECT F.ID FROM ' + @sFKTable + N' AS F INNER JOIN ' + @sTable + N' AS P ON F.'
					+ @sFKField + N' = P.ID WHERE (P.' + @sField + N'= ' + nchar(39) + @sInput1 + nchar(39) + N')) AS X'
				--SET @sStatus = @SQL
				--ROLLBACK TRANSACTION
				--RETURN
				-- Execute the dynamic sql statement
				EXEC sp_executesql @SQL
				SELECT @lNumFKRefs = COUNT(*) FROM MYTEMP
				SET @sStatus = N'OK' + CONVERT(varchar(12),@lNumFKRefs)
				COMMIT TRANSACTION
				RETURN
			  END

			  --DELETE
			ELSE IF @sAction = N'DELETE'
			  BEGIN
					SET @SQL = N'DELETE FROM ' + @sTable + N' WHERE ' + @sField + N'=' + nchar(39) + @sIn + nchar(39) 
			  END

			  --That's an error
			ELSE
				BEGIN
					SET @sStatus = @sPrefix + N'Parameter sAction is not SELECT, SELECTONE, INSERT, UPDATE, NUMFKREFS, or DELETE, but ' + @sAction
					ROLLBACK TRANSACTION
					RETURN
				END

			  -- Hopefully, everything is OK, so let's execute the command we built above:
			EXEC sp_executesql @SQL
			SET @iCount = @@ROWCOUNT

			IF @sAction = N'EXISTS'
			BEGIN
				IF @iCount < 1
				BEGIN
					SELECT -1	-- entry does not exist
				END
			END

			IF @sAction <> N'SELECT'
			BEGIN
			IF @iCount <> 1
				BEGIN
					SET @sStatus  = @sPrefix + N'The ' + @sAction + N' operation did not affect just one row, but ' + CONVERT(varchar(12),@iCount) + N'. SQL command was: ' + @SQL
					ROLLBACK TRANSACTION
					RETURN
				END
			END

		END TRY
		BEGIN CATCH
			SET @terr = @@ERROR
			SET @sStatus = @sPrefix + N'Error ' + CONVERT(varchar,@terr) + N': ' + ERROR_MESSAGE() + N', @SQL=`' + @SQL + N'`'
			ROLLBACK TRANSACTION
			RETURN
		END CATCH
	COMMIT TRANSACTION

	RETURN
END
GO

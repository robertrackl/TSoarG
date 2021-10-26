SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE SoarDB
GO
IF (OBJECT_ID('sf_ExpenseJournal') IS NOT NULL) DROP PROCEDURE sf_ExpenseJournal
GO
IF (TYPE_ID('sf_TFilter') IS NOT NULL) DROP TYPE sf_TFilter
CREATE TYPE sf_TFilter AS TABLE 
(
	sFilterName nvarchar(50) NOT NULL UNIQUE
		CHECK(sFilterName=N'FilterVersion' OR sFilterName=N'EnableFilteringOverall' OR sFilterName=N'TransactionType' OR
			sFilterName=N'Vendor' OR sFilterName=N'TransactionStatus' OR sFilterName=N'AttachmentCateg' OR sFilterName=N'AttachmentType'
			OR sFilterName=N'PaymentMethod' OR sFilterName=N'ExpenseAccount' OR sFilterName=N'PaymentAccount' OR sFilterName=N'XactDate'
			OR sFilterName=N'XactAmount' OR sFilterName=N'NumAttFiles'),
	sFilterType nvarchar(12)
		CHECK(sFilterType = N'List' OR sFilterType = N'Boolean' OR sFilterType = N'DateList' OR sFilterType = N'Range'),
	bEnabled bit,
	bINorEX bit,
	sList nvarchar(MAX),
	dLow money,
	dHigh money,
	sPunctuationMark nchar,
	sField nvarchar(MAX)
);
GO
IF (TYPE_ID('sf_TSort') IS NOT NULL) DROP TYPE sf_TSort
CREATE TYPE sf_TSort AS TABLE
(
	OrderBy nvarchar(25) NOT NULL UNIQUE 
		CHECK(OrderBy =	N'Date' OR OrderBy = N'sVendorName' OR OrderBy = N'cStatus' OR OrderBy = N'XT'),
	SortPriority int NOT NULL UNIQUE
		CHECK(SortPriority > 0 AND SortPriority < 5),
	SortOrder nvarchar(4) NOT NULL CHECK(SortOrder = N'asc' OR SortOrder = N'desc')
);
GO
CREATE PROCEDURE sf_ExpenseJournal
	@sStatus nvarchar(MAX) OUTPUT,
	@bShowXactTime bit,
	@taFilter sf_TFilter READONLY,
	@taSort sf_TSort READONLY
AS
	DECLARE @sCRLF nchar(2) -- carriage return / line feed combination
	SET @sCRLF = CHAR(0x0D) + CHAR(0x0A)
	DECLARE @sErr nchar(45)
	SET @sErr = N'Error in stored procedure sf_ExpenseJournal: '
	DECLARE @sSEL0 nvarchar(MAX)
	DECLARE @sFROM1 nvarchar(MAX)
	DECLARE @sWHERE1 nvarchar(MAX)
	DECLARE @sWHERE2 nvarchar(MAX)
	DECLARE @sORDERBY nvarchar(MAX)
	DECLARE @sAllOrderBy nvarchar(MAX)
	DECLARE @sQueryText nvarchar(MAX)
	DECLARE @mVersion money
	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT ON;
	SET @sStatus = 'Stored Procedure sf_ExpenseJournal did not run to completion' -- pessimistic
	BEGIN TRY

	SELECT @mVersion = dLow FROM @taFilter WHERE sFilterName=N'FilterVersion'
	IF @mVersion <> 1.1
	BEGIN
		SET @sStatus = @sErr + 'Filter Version should be 1.1, but is ' + CONVERT(nvarchar(12), @mVersion)
		RETURN
	END
	-- LEGEND for field names below:
	--XT = numerical total amount of the transaction for filtering and sorting purposes
	--XactTotal = Formatted total amount of the transaction
	--ExpAccount = The first expense account encountered among the expenditure entries
	--NX = Number of expenditure entries
	--PmtAccount = The first payment account encountered among the payment entries
	--NP = Number of payment entries
	--NumAtt = Number of attached files

	SET @sSEL0 =
		'SELECT DISTINCT ID, ' + 
		IIF(@bShowXactTime = 1, 'Date', 'CONVERT(nchar(4),DATEPART(yyyy, Date))+''/''+CONVERT(nchar(2),DATEPART(mm, Date))+''/''+CONVERT(nchar(2),DATEPART(dd, Date))') +
		 ' AS [Date], ' +
		    'sVendorName, cStatus, cStatusPrev, XT, XactTotal, ExpAccount AS [Expense Account],
			Description, NX, PmtAccount AS [Payment Account], NP, NumAtt, sMemo
		 FROM 
			(SELECT	XO.ID, XO.D AS Date, V.sVendorName, XO.cStatus, XO.cStatusPrev,

				(SELECT SUM(mAmount) / 2
				 FROM   SF_ENTRIES AS SF_ENTRIES_1
				 WHERE  (iXactId = XO.ID))											 AS XT,

				(SELECT FORMAT(SUM(mAmount) / 2, ''N2'')
				 FROM   SF_ENTRIES
				 WHERE  (iXactId = XO.ID))											 AS XactTotal,

				(SELECT TOP (1) A.sCode + '' '' + A.sName + '', ...''
				 FROM   SF_ENTRIES AS E
						INNER JOIN SF_PAYMENTMETHODS AS P ON E.iPaymentMethod = P.ID
						INNER JOIN SF_ACCOUNTS AS A ON E.iAccountId = A.ID
				 WHERE  (E.iXactId = XO.ID) AND (P.sPaymentMethod = N''(none)''))	 AS ExpAccount,

				(SELECT TOP (1) E.sDescription + '', ...''
				 FROM   SF_ENTRIES AS E INNER JOIN
						SF_PAYMENTMETHODS AS P ON E.iPaymentMethod = P.ID
				 WHERE  (E.iXactId = XO.ID) AND (P.sPaymentMethod = N''(none)''))	 AS Description,

				(SELECT COUNT(*)
				 FROM   SF_ENTRIES AS E INNER JOIN
						SF_PAYMENTMETHODS AS P ON E.iPaymentMethod = P.ID
				 WHERE  (E.iXactId = XO.ID) AND (P.sPaymentMethod = N''(none)''))	 AS NX,

				(SELECT TOP (1) A.sCode + '' '' + A.sName + '', ...''
				 FROM   SF_ENTRIES AS E
						INNER JOIN SF_PAYMENTMETHODS AS P ON E.iPaymentMethod = P.ID
						INNER JOIN SF_ACCOUNTS AS A ON E.iAccountId = A.ID
				 WHERE  (E.iXactId = XO.ID) AND (P.sPaymentMethod <> N''(none)''))	 AS PmtAccount,

				(SELECT COUNT(*)
				 FROM   SF_ENTRIES AS E INNER JOIN
						SF_PAYMENTMETHODS AS P ON E.iPaymentMethod = P.ID
				 WHERE  (E.iXactId = XO.ID) AND (P.sPaymentMethod <> N''(none)''))	 AS NP,

																					 XO.sMemo,
				(SELECT COUNT(*)
				 FROM   SF_XACT_DOCS AS XD INNER JOIN
						SF_XACTS AS X ON X.ID = XD.iXactId
				 WHERE  (XO.ID = XD.iXactId))										 AS NumAtt
		'
		SET @sFROM1 =
			'FROM SF_XACTS AS XO
			 INNER JOIN SF_XACTTYPES AS XY ON XO.iType = XY.ID
			 INNER JOIN SF_VENDORS AS V ON XO.iVendor = V.ID'

		SET @sWHERE1 = 'WHERE (XY.sTransactionType = N''Expense'')'
		SET @sWHERE2 = ''

		DECLARE @bEnableFilteringOverall bit
		SELECT @bEnableFilteringOverall = bEnabled FROM @taFilter											WHERE sFilterName=N'EnableFilteringOverall' --
		IF @bEnableFilteringOverall = 1
		BEGIN
			DECLARE @sCSList nvarchar(MAX) -- Comma-separated list
			DECLARE @bFilterEnabled bit
			DECLARE @bLEFT_OUTER_JOIN_SF_XACT_DOCS bit
			SET @bLEFT_OUTER_JOIN_SF_XACT_DOCS = 0
			SELECT @bFilterEnabled = bEnabled, @sCSList = sList FROM @taFilter								WHERE sFilterName = N'TransactionType' --
			IF (@bFilterEnabled = 1) AND (@sCSList <> N'All')
			BEGIN
				SET @sWHERE1 = 'WHERE (XY.sTransactionType IN (' + @sCSList + '))'
			END
			SELECT @bFilterEnabled = bEnabled, @sCSList = sList FROM @taFilter								WHERE sFilterName = N'Vendor' --
			IF @bFilterEnabled = 1 AND (@sCSList <> N'All')
			BEGIN
				SET @sWHERE1 = @sWHERE1 + @sCRLF + ' AND (V.sVendorName IN (' + @sCSList + '))'
			END
			SELECT @bFilterEnabled = bEnabled, @sCSList = sList FROM @taFilter								WHERE sFilterName = N'TransactionStatus' --
			IF @bFilterEnabled = 1 AND (@sCSList <> N'All')
			BEGIN
				SET @sWHERE1 = @sWHERE1 + @sCRLF +  ' AND (XO.cStatus IN (' + @sCSList + '))'
			END
			SELECT @bFilterEnabled = bEnabled, @sCSList = sList FROM @taFilter								WHERE sFilterName = N'AttachmentCateg' --
			IF @bFilterEnabled = 1 AND (@sCSList <> N'All')
			BEGIN
				SET @sFROM1 = @sFROM1 + @sCRLF + ' LEFT OUTER JOIN SF_XACT_DOCS AS XD ON XO.ID = XD.iXactId'
				SET @bLEFT_OUTER_JOIN_SF_XACT_DOCS = 1
				SET @sFROM1 = @sFROM1 + @sCRLF + ' INNER JOIN SF_ATTACHMENTCATEGS AS TC ON TC.ID = XD.iAttachmentCateg'
				SET @sWHERE1 = @sWHERE1 + @sCRLF + ' AND (TC.sAttachmentCateg IN (' + @sCSList + '))'
			END
			SELECT @bFilterEnabled = bEnabled, @sCSList = sList FROM @taFilter								WHERE sFilterName = N'AttachmentType' --
			IF @bFilterEnabled = 1 AND (@sCSList <> N'All')
			BEGIN
				IF @bLEFT_OUTER_JOIN_SF_XACT_DOCS = 0
				BEGIN
					SET @sFROM1 = @sFROM1 + @sCRLF + ' LEFT OUTER JOIN SF_XACT_DOCS AS XD ON XO.ID = XD.iXactId'
				END
				SET @bLEFT_OUTER_JOIN_SF_XACT_DOCS = 1
				SET @sFROM1 = @sFROM1 + @sCRLF + ' INNER JOIN SF_DOCS AS SD ON SD.ID = XD.iDocsId'
				SET @sFROM1 = @sFROM1 + @sCRLF + ' INNER JOIN SF_ALLOWEDATTACHTYPES AS AL ON AL.ID = SD.iFileType'
				SET @sWHERE1 = @sWHERE1 + @sCRLF + ' AND (AL.sAllowedFileType IN (' + @sCSList + '))'
			END
			SELECT @bFilterEnabled = bEnabled, @sCSList = sList FROM @taFilter								WHERE sFilterName = N'XactDate' --
			IF @bFilterEnabled = 1
			BEGIN
				DECLARE @iComma bigint
				DECLARE @sDLo nvarchar(50)
				DECLARE @sDHi nvarchar(50)
				SET @iComma = CHARINDEX(N',', @sCSList)
				SET @sDLo = RTRIM(LTRIM(SUBSTRING(@sCSList, 1, @iComma - 1)))
				SET @sDHi = RTRIM(LTRIM(SUBSTRING(@sCSList, @iComma + 1, 24)))
				SET @sWHERE1 = @sWHERE1 + @sCRLF + ' AND (XO.D >= ''' + @sDLo + ''') AND (XO.D <= ''' + @sDHi + ''')'
			END
			SELECT @bFilterEnabled = bEnabled, @sCSList = sList FROM @taFilter								WHERE sFilterName = N'PaymentMethod' --
			IF @bFilterEnabled = 1 AND (@sCSList <> N'All')
			BEGIN
				SET @sFROM1 = @sFROM1 + ' INNER JOIN SF_ENTRIES AS EN ON EN.iXactId = XO.ID INNER JOIN SF_PAYMENTMETHODS AS PM ON PM.ID = EN.iPaymentMethod'
				SET @sWHERE1 = @sWHERE1 + @sCRLF + ' AND (PM.sPaymentMethod IN (' + @sCSList + '))'
			END
			DECLARE @mLo money
			DECLARE @mHi money
			SELECT @bFilterEnabled = bEnabled, @mLo = dLow, @mHi = dHigh FROM @taFilter						WHERE sFilterName = N'XactAmount' --
			IF @bFilterEnabled = 1
			BEGIN
				SET @sWHERE2 = ' WHERE (XT BETWEEN ' + CONVERT(nvarchar(25),@mLo) + ' AND ' + CONVERT(nvarchar(25),@mHi) + ')'
			END
			SELECT @bFilterEnabled = bEnabled, @mLo = dLow, @mHi = dHigh FROM @taFilter						WHERE sFilterName = N'NumAttFiles' --
			IF @bFilterEnabled = 1
			BEGIN
				SET @sWHERE2 = @sWHERE2 + @sCRLF + ' AND (NumAtt BETWEEN ' + CONVERT(nvarchar(12),CONVERT(int,@mLo)) + ' AND ' + CONVERT(nvarchar(12),CONVERT(int,@mHi)) + ')'
			END
			-- Only those transactions where there is at least one Expense account in
			--  common among the expense accounts used in the transaction and the accounts in the given filter list for expense accounts:
			SELECT @bFilterEnabled = bEnabled, @sCSList = sList FROM @taFilter								WHERE sFilterName = N'ExpenseAccount' --
			IF @bFilterEnabled = 1 AND (@sCSList <> N'All')
			BEGIN
				SET @sWHERE1 = @sWHERE1 + @sCRLF +
				'AND (	(SELECT COUNT(*) FROM
							(SELECT sCode
								FROM SF_ACCOUNTS AC_0
									INNER JOIN SF_ENTRIES EN_0 ON EN_0.iAccountId=AC_0.ID
									INNER JOIN SF_XACTS XA_0 ON EN_0.iXactId=XA_0.ID
									INNER JOIN SF_PAYMENTMETHODS PM_0 ON PM_0.ID=EN_0.iPaymentMethod
								WHERE (XA_0.ID=XO.ID) AND (PM_0.sPaymentMethod = N''(none)'')
							 INTERSECT
							 SELECT sCode FROM SF_ACCOUNTS AS AC_1 WHERE sCode IN (' + @sCSList + ')
							) AS D1
						) > 0
					)'
			END
			-- Only those transactions where there is at least one Payment account in
			--  common among the payment accounts used in the transaction, and the accounts in the given filter list for payment accounts:
			SELECT @bFilterEnabled = bEnabled, @sCSList = sList FROM @taFilter								WHERE sFilterName = N'PaymentAccount' --
			IF @bFilterEnabled = 1 AND (@sCSList <> N'All')
			BEGIN
				SET @sWHERE1 = @sWHERE1 + @sCRLF +
				'AND (	(SELECT COUNT(*) FROM
							(SELECT sCode
								FROM SF_ACCOUNTS AC_0
									INNER JOIN SF_ENTRIES EN_0 ON EN_0.iAccountId=AC_0.ID
									INNER JOIN SF_XACTS XA_0 ON EN_0.iXactId=XA_0.ID
									INNER JOIN SF_PAYMENTMETHODS PM_0 ON PM_0.ID=EN_0.iPaymentMethod
								WHERE (XA_0.ID=XO.ID) AND (PM_0.sPaymentMethod <> N''(none)'')
							 INTERSECT
							 SELECT sCode FROM SF_ACCOUNTS AS AC_1 WHERE sCode IN (' + @sCSList + ')
							) AS D1
						) > 0
					)'
			END
		END

		SELECT @sAllOrderBy = COALESCE(@sAllOrderBy + ',' + OrderBy + ' ' + SortOrder, OrderBy + ' ' + SortOrder) FROM @taSort ORDER BY SortPriority
		SET @sORDERBY = 'ORDER BY ' + @sAllOrderBy

		SET @sQueryText = @sSEL0 + @sCRLF + @sFROM1 + @sCRLF + @sWHERE1 + @sCRLF + ' ) AS DQ ' + @sCRLF + @sWHERE2 + @sCRLF + @sORDERBY
		--SET @sStatus = @sQueryText
		--RETURN
		EXEC sp_executesql @sQueryText
		SET @sStatus = 'OK'
	END TRY
	BEGIN CATCH
        SET @sStatus = N'Err Num = ' + CONVERT(nvarchar(12), ERROR_NUMBER())
		SET @sStatus = @sStatus + N', ' + N'Err Severity = ' + CONVERT(nvarchar(12), ERROR_SEVERITY())
		SET @sStatus = @sStatus + N', ' + N'Err State = ' + CONVERT(nvarchar(12), ERROR_STATE())
		SET @sStatus = @sStatus + N', ' + N'in ' + ERROR_PROCEDURE()
		SET @sStatus = @sStatus + N', ' + N'Line Num = ' + CONVERT(nvarchar(12), ERROR_LINE())
		SET @sStatus = @sStatus + N', ' + N'Msg: ' + ERROR_MESSAGE()
		RETURN
	END CATCH
GO

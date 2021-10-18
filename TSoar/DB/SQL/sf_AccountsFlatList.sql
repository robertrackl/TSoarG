SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('sf_AccountsFlatList') IS NOT NULL) DROP FUNCTION sf_AccountsFlatList
GO
CREATE FUNCTION sf_AccountsFlatList ()
RETURNS 
@tblAccounts TABLE 
(
	sCode nvarchar(25), sSortCode nvarchar(40), sName nvarchar(128), sAccountType nvarchar(25), ID int, iSF_ParentAcct int, sNotes nvarchar(MAX)
)
AS
BEGIN
	DECLARE @idRoot int
	SELECT @idRoot=S.ID FROM SF_ACCOUNTS S WHERE S.sName='Root';

	-- Fill the table variable with the rows for your result set
	WITH CTE_DownLine AS
	(
		 ( -- Anchor
			  SELECT
					S.sCode, S.sSortCode, S.sName, A.sAccountType, S.ID, S.iSF_ParentAcct, S.sNotes
			  FROM
				   SF_ACCOUNTS AS S
				   INNER JOIN SF_ACCTTYPES AS A ON S.iSF_Type = A.ID
			  WHERE
				   S.ID = @idRoot
		 )
		 UNION ALL
		 ( -- Recursive Part
			  SELECT
				   This.sCode, This.sSortCode, This.sName, A.sAccountType, This.ID, This.iSF_ParentAcct, This.sNotes
			  FROM
				   SF_ACCOUNTS AS This
				   INNER JOIN CTE_DownLine AS Prior ON Prior.ID = This.iSF_ParentAcct
				   INNER JOIN SF_ACCTTYPES AS A ON This.iSF_Type = A.ID
		 )
	)
	INSERT INTO @tblAccounts (sCode, sSortCode, sName, sAccountType, ID, iSF_ParentAcct, sNotes)
	SELECT
		 sCode, sSortCode, sName, sAccountType, ID, iSF_ParentAcct, sNotes
	FROM
		 CTE_DownLine
	ORDER BY sSortCode;
	RETURN 
END
GO
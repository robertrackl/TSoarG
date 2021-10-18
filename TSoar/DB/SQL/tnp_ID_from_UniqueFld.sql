SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('tnp_ID_from_sUniqueFld') IS NOT NULL) DROP PROCEDURE tnp_ID_from_sUniqueFld
GO
CREATE PROCEDURE tnp_ID_from_sUniqueFld 
(
	@sTable nvarchar(20),
	@sUniqueValue nvarchar(MAX),
	@iID int OUTPUT
)
AS
BEGIN
	-- Find the ID of a record in table @sTable that has value @sUniqueValue in its sUniqueFld (defined in table INTERNALAUXMULTI)

	DECLARE @SQL nvarchar(MAX)

	SET @SQL = N'SELECT @sUniqueFld = sUniqueFld FROM INTERNALAUXMULTI WHERE sTable = ' + nchar(39) + @sTable + nchar(39)
	-- What is the name of the unique field for this table?
	DECLARE @sLocUF nvarchar(25)
	EXECUTE sp_executesql @SQL, N'@sUniqueFld nvarchar(25) OUTPUT ', @sLocUF OUTPUT

	-- If the desired record does not exist, return zero:
	SET @iID = 0
	-- What is the ID of the record with the given unique value?
	SET @SQL = N'SELECT @iID = ID FROM ' + @sTable + ' WHERE ' + @sLocUF + ' = ' + nchar(39) + @sUniqueValue + nchar(39)
	EXECUTE sp_executesql @SQL, N'@iID int OUTPUT', @iID OUTPUT
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE SoarDB
GO
IF (OBJECT_ID('GetPunct4DType') IS NOT NULL) DROP FUNCTION GetPunct4DType
GO
CREATE FUNCTION GetPunct4DType 
(
	@sDataType as nvarchar(25)
)
RETURNS nchar(1)
AS
BEGIN
	-- Declare the return variable
	DECLARE @cP nchar(1)
	SELECT @cP = D.cPunct
		FROM sys.types T INNER JOIN
			DYNPUNCTS D ON T.user_type_id = D.user_type_id
		WHERE (T.name = @sDataType)
	-- Return the result of the function
	RETURN @cP
END
GO


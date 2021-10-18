SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('spSettingsCheck') IS NOT NULL) DROP PROCEDURE spSettingsCheck
GO
CREATE PROCEDURE spSettingsCheck (
	@sTable nvarchar(30),
	@sField nvarchar(80),
	@sRequiredValue nvarchar(MAX),
	@iHits int output,
	@sMsg nvarchar(MAX) output)
AS
BEGIN
	DECLARE @sSQL nvarchar(MAX)

	SET @iHits = -1 -- pessimistic
	SET @sMsg = 'OK' -- optimistic
	SET @sSQL = 'SELECT ID FROM ' + @sTable + ' WHERE ' + @sField + ' = ''' + @sRequiredValue + ''''

	BEGIN TRY
		EXEC sp_executesql @sSQL
		SET @iHits = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SET @sMsg = 'For sField = "' + @sField + '" and sRequiredValue = "' + @sRequiredValue + '": ' +  ERROR_MESSAGE()
		RETURN
	END CATCH
END
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE IntSoarDB
GO
IF (OBJECT_ID('TNPF_GetSetting') IS NOT NULL) DROP FUNCTION TNPF_GetSetting
GO
CREATE FUNCTION TNPF_GetSetting -- TABLE VALUED
(
	@sGetSettingName as nvarchar(MAX)
)
RETURNS @Setting TABLE (
	ID int PRIMARY KEY NOT NULL,
	sSettingName nvarchar(MAX) NOT NULL,
	sSettingValue nvarchar(MAX) NOT NULL,
	sComments nvarchar(MAX) NULL,
	sStatus nvarchar(MAX) NOT NULL
)
AS
BEGIN
	-- Declare the return variable
	DECLARE
		@ID int = -1,
		@sSettingName nvarchar(MAX) = 'ERROR',
		@sSettingValue nvarchar(MAX) = 'error message',
		@sComments nvarchar(MAX) = '',
		@sStatus nvarchar(MAX) = 'OK', --optimistic
		@lCount int = 0
	-- Get the data
	IF @sGetSettingName = '*'
	BEGIN
		DECLARE CC CURSOR FOR SELECT ID, sSettingName, sSettingValue, sComments, 'OK' FROM TNP_SETTINGS
		OPEN CC
		FETCH NEXT FROM CC INTO @ID, @sSettingName, @sSettingValue, @sComments, @sStatus
		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @lCount = @lCount + 1
			INSERT @Setting
			SELECT @ID, @sSettingName, @sSettingValue, @sComments, @sStatus
			FETCH NEXT FROM CC INTO @ID, @sSettingName, @sSettingValue, @sComments, @sStatus
		END
	END ELSE
	BEGIN
		SELECT
			@ID = ID,
			@sSettingName = sSettingName,
			@sSettingValue = sSettingValue,
			@sComments = sComments
		FROM TNP_SETTINGS
		WHERE @sGetSettingName = sSettingName
		SET @lCount = @@ROWCOUNT
		IF @lCount > 0
		BEGIN
			INSERT @Setting
			SELECT @ID, @sSettingName, @sSettingValue, @sComments, 'OK'
		END
	END
	IF @lCount < 1
	BEGIN
		INSERT @Setting
		SELECT -1,@sGetSettingName, 'Nothing Found', 'ERROR', 'ERROR'
	END
	RETURN
END
GO

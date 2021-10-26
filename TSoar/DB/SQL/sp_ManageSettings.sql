SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('sp_ManageSettings') IS NOT NULL) DROP PROCEDURE sp_ManageSettings
GO
CREATE PROCEDURE sp_ManageSettings
	@sUser nvarchar(256)
AS
	-- Return a list of Settings to which this ASP.NET website user has access
	DECLARE @NRoles int
	SELECT @NRoles = COUNT(*)
		FROM aspnet_UsersInRoles
			INNER JOIN aspnet_Roles ON dbo.aspnet_UsersInRoles.RoleId = dbo.aspnet_Roles.RoleId
			INNER JOIN aspnet_Users ON dbo.aspnet_UsersInRoles.UserId = dbo.aspnet_Users.UserId
		WHERE (aspnet_Roles.RoleName = 'Admin') AND (aspnet_Users.UserName = @sUser)
	IF @NRoles > 0
	BEGIN
		-- User in @sUser has Admin role -=> give that user access to all settings
		SELECT S.ID, S.sSettingName, S.sExplanation, S.sSettingValue, S.cSettingType, S.sSelectStmnt, S.dLow, S.dHigh, S.sDataValueField, S.sDataTextField
			FROM SETTINGS AS S
			WHERE S.bUserSelectable = 1
			ORDER BY S.sSettingName
	END
	ELSE
	BEGIN
		-- Give @sUser access to Settings as specified in SETTINGSROLESBRIDGE
		SELECT S.ID, S.sSettingName, S.sExplanation, S.sSettingValue, S.cSettingType, S.sSelectStmnt, S.dLow, S.dHigh, S.sDataValueField, S.sDataTextField
			FROM SETTINGS AS S
			WHERE (S.ID IN
					(SELECT iSetting
						FROM SETTINGSROLESBRIDGE
							INNER JOIN aspnet_Roles ON SETTINGSROLESBRIDGE.uiRole = aspnet_Roles.RoleId
						WHERE
							  (uiRole IN
								(SELECT RoleId
									FROM aspnet_UsersInRoles AS aspnet_UsersInRoles_1
										INNER JOIN aspnet_Users ON aspnet_Users.UserId = aspnet_UsersInRoles_1.UserId
									WHERE (aspnet_Users.UserName = @sUser)
								 )
							   )
					 )
				   )
			ORDER BY S.sSettingName
	END
GO

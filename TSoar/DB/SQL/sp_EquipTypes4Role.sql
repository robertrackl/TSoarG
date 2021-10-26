SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('sp_EquipTypes4Role') IS NOT NULL) DROP PROCEDURE sp_EquipTypes4Role
GO
CREATE PROCEDURE sp_EquipTypes4Role
	@sRole nvarchar(25) -- A string that occurs in one or more equipment roles, such as 'Tow'.
	-- This stored procedure returns a list of equipment types which can perform a role
	--  that contains in its name the string given in parameter @sRole.
AS
BEGIN
SELECT E.ID, E.iEquipType, E.sShortEquipName
	FROM EQUIPMENT E
		INNER JOIN EQUIPTYPES ON E.iEquipType = EQUIPTYPES.ID
		INNER JOIN EQUIPROLESTYPES ON EQUIPTYPES.ID = EQUIPROLESTYPES.iEquipType
		INNER JOIN EQUIPMENTROLES ON EQUIPROLESTYPES.iEquipRole = EQUIPMENTROLES.ID
	WHERE (CHARINDEX(@sRole, EQUIPMENTROLES.sEquipmentRole) > 0)
END
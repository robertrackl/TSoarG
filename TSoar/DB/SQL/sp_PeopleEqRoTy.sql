SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('sp_PeopleEqRoTy') IS NOT NULL) DROP PROCEDURE sp_PeopleEqRoTy
GO
CREATE PROCEDURE sp_PeopleEqRoTy
AS
BEGIN
	SELECT
		F.ID,
		P.ID AS iPerson,
		P.sDisplayName,
		F.iAviatorRole,
		A.sAviatorRole,
		E.ID AS iEqRoTyId,
		R.sEquipmentRole + ' / ' + T.sEquipmentType + ' [' + E.sComments + ']' AS sEqRoTy,
		F.sComments
	FROM EQUIPTYPES AS T
		INNER JOIN EQUIPROLESTYPES AS E ON T.ID = E.iEquipType
		INNER JOIN EQUIPMENTROLES AS R ON E.iEquipRole = R.ID
		INNER JOIN PEOPLEEQUIPROLESTYPES AS F ON E.ID = F.iRoleType
		INNER JOIN PEOPLE AS P ON F.iPerson = P.ID
		INNER JOIN AVIATORROLES AS A ON F.iAviatorRole = A.ID
	ORDER BY P.sDisplayName, A.sAviatorRole, sEqRoTy
END
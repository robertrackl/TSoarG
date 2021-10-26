SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('sp_FlightLogRows') IS NOT NULL) DROP PROCEDURE sp_FlightLogRows
GO
CREATE PROCEDURE sp_FlightLogRows
	@iWhichFliteLog int
AS
BEGIN
	SELECT  R.ID,
			R.iFliteLog,
			R.cStatus,
			R.iTowEquip,
				EQUIPMENT.sShortEquipName AS sTowEquipName,
			R.iTowOperator,
				PEOPLE.sDisplayName AS sTowOperName, 
            R.iGlider,
				EQUIPMENT_1.sShortEquipName AS sGliderName,
			R.iLaunchMethod,
				LAUNCHMETHODS.sLaunchMethod,
			R.iPilot1,
				PEOPLE_1.sDisplayName AS sPilot1,
				R.iAviatorRole1,
				AVIATORROLES.sAviatorRole AS sAviatorRole1, 
				R.dPctCharge1,
			R.iPilot2,
				PEOPLE_2.sDisplayName AS sPilot2,
				R.iAviatorRole2,
				AVIATORROLES_1.sAviatorRole AS sAviatorRole2,
				R.dPctCharge2, 
            R.dReleaseAltitude,
			R.dMaxAltitude,
			R.iLocTakeOff,
				LOCATIONS.sLocation AS sLocTakeOff,
				R.DTakeOff,
			R.iLocLanding, 
				LOCATIONS_1.sLocation AS sLocLanding,
				R.DLanding,
			R.iChargeCode,CHARGECODES.cChargeCode,
				CHARGECODES.sChargeCode,
			R.mAmtCollected,
			R.sComments
	FROM   FLIGHTLOGROWS R
				INNER JOIN CHARGECODES ON R.iChargeCode = CHARGECODES.ID
				INNER JOIN LAUNCHMETHODS ON R.iLaunchMethod = LAUNCHMETHODS.ID
				INNER JOIN AVIATORROLES ON R.iAviatorRole1 = AVIATORROLES.ID
				INNER JOIN AVIATORROLES AS AVIATORROLES_1 ON R.iAviatorRole2 = AVIATORROLES_1.ID
				LEFT OUTER JOIN EQUIPMENT ON R.iTowEquip = EQUIPMENT.ID
				LEFT OUTER JOIN EQUIPMENT AS EQUIPMENT_1 ON R.iGlider = EQUIPMENT_1.ID
				INNER JOIN PEOPLE ON R.iTowOperator = PEOPLE.ID
				INNER JOIN PEOPLE AS PEOPLE_1 ON R.iPilot1 = PEOPLE_1.ID
				INNER JOIN PEOPLE AS PEOPLE_2 ON R.iPilot2 = PEOPLE_2.ID
				INNER JOIN LOCATIONS ON R.iLocTakeOff = LOCATIONS.ID
				INNER JOIN LOCATIONS AS LOCATIONS_1 ON R.iLocLanding = LOCATIONS_1.ID
	WHERE  R.iFliteLog = @iWhichFliteLog
END
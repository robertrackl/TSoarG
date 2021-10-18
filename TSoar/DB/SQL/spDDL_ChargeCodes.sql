SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('spDDL_ChargeCodes') IS NOT NULL) DROP PROCEDURE spDDL_ChargeCodes
GO
CREATE PROCEDURE spDDL_ChargeCodes (@iRate int)
AS
BEGIN
	SELECT ID, cChargeCode + ' - ' + sChargeCode + ', ' + IIF(bCharge4Launch=1, 't', 'f') + ', ' + IIF(bCharge4Rental=1, 't', 'f') AS sCode
		FROM CHARGECODES
		WHERE (ID NOT IN
			(SELECT iChargeCode	FROM CHARGECODESINRATES WHERE (iRate = @iRate)))
END
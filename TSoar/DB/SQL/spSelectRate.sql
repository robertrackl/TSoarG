SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('spSelectRate') IS NOT NULL) DROP PROCEDURE spSelectRate
GO
CREATE PROCEDURE spSelectRate (
	@iOpsID int,
	@sLaunchOrGlider nvarchar(6))
AS
BEGIN
	DECLARE @bCharge bit
	IF @sLaunchOrGlider <> N'Launch'
	BEGIN -- Glider Rental Rate
		SELECT @bCharge = CC.bCharge4Rental -- does the charge code require a charge for glider rental?
			FROM CHARGECODES AS CC
				INNER JOIN OPERATIONS AS OP ON CC.ID = OP.iChargeCode
			WHERE (OP.ID = @iOpsID)
		IF @bCharge = 1
		BEGIN
			SELECT RA.*
				FROM EQUIPTYPES ET
					INNER JOIN RATES RA ON ET.ID = RA.iEquipType
					INNER JOIN LAUNCHMETHODS AS LM ON RA.iLaunchMethod = LM.ID
					INNER JOIN EQUIPMENT EQ ON ET.ID = EQ.iEquipType
					INNER JOIN OPDETAILS OD ON EQ.ID = OD.iEquip
					INNER JOIN EQUIPMENTROLES OL ON OD.iEquipmentRole = OL.ID
					INNER JOIN OPERATIONS OP ON OD.iOperation = OP.ID AND RA.Dfrom <= OP.DBegin AND RA.DTo >= OP.DBegin
					INNER JOIN CHARGECODES CC ON OP.iChargeCode = CC.ID
					INNER JOIN CHARGECODESINRATES CI ON RA.ID = CI.iRate AND CC.ID = CI.iChargeCode
					INNER JOIN QBO_ITEMNAMES QI ON RA.iQBO_ItemName = QI.ID
				WHERE (LM.sLaunchMethod = N'Any') AND (OL.sEquipmentRole = N'Glider') AND (OP.ID = @iOpsID)
			UNION
			SELECT RA.*
				FROM EQUIPMENTROLES ER
					INNER JOIN OPDETAILS OD
					INNER JOIN EQUIPTYPES ET
					INNER JOIN EQUIPMENT EQ ON ET.ID = EQ.iEquipType
					INNER JOIN RATES RA ON ET.ID = RA.iEquipType
						ON OD.iEquip = EQ.ID
						ON ER.ID = OD.iEquipmentRole
					INNER JOIN OPERATIONS OP
					INNER JOIN LAUNCHMETHODS LM ON OP.iLaunchMethod = LM.ID
						ON OD.iOperation = OP.ID
						AND RA.iLaunchMethod = OP.iLaunchMethod
						AND RA.Dfrom <= OP.DBegin
						AND RA.DTo >= OP.DBegin
					INNER JOIN CHARGECODES CC ON OP.iChargeCode = CC.ID
					INNER JOIN CHARGECODESINRATES CI ON RA.ID = CI.iRate AND CC.ID = CI.iChargeCode
					INNER JOIN QBO_ITEMNAMES QI ON RA.iQBO_ItemName = QI.ID
				WHERE (LM.sLaunchMethod <> N'Any')AND (OP.ID = @iOpsID) AND (ER.sEquipmentRole = N'Glider')
		END
		ELSE
		BEGIN
			SELECT RA.*
				FROM RATES RA
					INNER JOIN QBO_ITEMNAMES QI ON RA.iQBO_ItemName = QI.ID
				WHERE RA.sShortName = N'Zero'
		END
	END -- Glider Rental Rate
	ELSE
	BEGIN -- Glider Launch Rate
		DECLARE @TblRates TABLE (ID int not null, sShortName nvarchar(30) not null, DFrom datetimeoffset(0) not null, DTo datetimeoffset(0) not null,
			iEquipType int not null, iLaunchMethod int not null, sChargeCodes varchar(26) not null, mSingleDpUse money not null, iNoChrg1stFt int not null, mAltDiffDpFt money not null,
			iNoChrg1stMin int not null, mDurationDpMin money not null, iDurCapMin int not null, sComment nvarchar(MAX) null, iQBO_ItemName int not null, iFA_Item int not null)
		SELECT @bCharge = CC.bCharge4Launch -- does the charge code require a charge for glider launch?
			FROM CHARGECODES AS CC
				INNER JOIN OPERATIONS AS OP ON CC.ID = OP.iChargeCode
			WHERE (OP.ID = @iOpsID)
		IF @bCharge = 1
		BEGIN
			INSERT INTO @TblRates (ID, sShortName, DFrom, DTo, iEquipType, iLaunchMethod, sChargeCodes, mSingleDpUse, iNoChrg1stFt, mAltDiffDpFt,
				iNoChrg1stMin, mDurationDpMin, iDurCapMin, sComment, iQBO_ItemName, iFA_Item)
			SELECT RA.ID, RA.sShortName, RA.DFrom, RA.DTo, RA.iEquipType, RA.iLaunchMethod, RA.sChargeCodes, RA.mSingleDpUse, RA.iNoChrg1stFt, RA.mAltDiffDpFt,
				RA.iNoChrg1stMin, RA.mDurationDpMin, RA.iDurCapMin, RA.sComment, RA.iQBO_ItemName, RA.iFA_Item
			FROM EQUIPMENTROLES ER
				INNER JOIN OPDETAILS OD
				INNER JOIN EQUIPTYPES ET
				INNER JOIN EQUIPMENT EQ ON ET.ID = EQ.iEquipType
				INNER JOIN RATES RA ON ET.ID = RA.iEquipType
					ON OD.iEquip = EQ.ID
					ON ER.ID = OD.iEquipmentRole
				INNER JOIN OPERATIONS OP
				INNER JOIN LAUNCHMETHODS LM ON OP.iLaunchMethod = LM.ID
					ON OD.iOperation = OP.ID
					AND RA.iLaunchMethod = OP.iLaunchMethod
					AND RA.Dfrom <= OP.DBegin
					AND RA.DTo >= OP.DBegin
				INNER JOIN CHARGECODES CC ON OP.iChargeCode = CC.ID
				INNER JOIN CHARGECODESINRATES CI ON RA.ID = CI.iRate AND CC.ID = CI.iChargeCode
			WHERE (OP.ID = @iOpsID) AND (ER.sEquipmentRole <> N'Glider')
			IF @@ROWCOUNT > 1
			BEGIN -- Let's see whether altitude difference can help decide which rate record to use
				DECLARE @iAltitDiff int
				SELECT DISTINCT @iAltitDiff = OD.dReleaseAltitude - LO.dRunwayAltitude
					FROM OPERATIONS OP
						INNER JOIN LOCATIONS LO ON OP.iTakeoffLoc = LO.ID
						INNER JOIN OPDETAILS OD ON OP.ID = OD.iOperation
					WHERE (OP.ID = @iOpsID)
				-- A negative value for iNoChrg1stFt indicates that the tow charge is mSingleDpUse only, up to an altitude difference of ABS(iNoChrg1stFt)
				DECLARE @iNoChrg1stFt int
				SELECT @iNoChrg1stFt = -iNoChrg1stFt FROM @TblRates WHERE (iNoChrg1stFt < 0) 
				IF @@ROWCOUNT = 1
				BEGIN
					IF @iAltitDiff <= @iNoChrg1stFt
					BEGIN
						DELETE FROM @TblRates WHERE iNoChrg1stFt >= 0
					END
					ELSE
					BEGIN
						DELETE FROM @TblRates WHERE iNoChrg1stFt < 0
					END
				END
			END
			SELECT * FROM @TblRates
		END
		ELSE
		BEGIN
			SELECT RA.*
				FROM RATES RA
					INNER JOIN QBO_ITEMNAMES QI ON RA.iQBO_ItemName = QI.ID
				WHERE RA.sShortName = N'Zero'
		END
	END  -- Glider Launch Rate
END
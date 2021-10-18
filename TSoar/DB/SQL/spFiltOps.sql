SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('spFiltOps') IS NOT NULL) DROP PROCEDURE spFiltOps
GO
-- Create a set of filtered flight operations for the purpose of creating invoices
CREATE PROCEDURE spFiltOps (
	@DFrom datetimeoffset(0),
	@DTo datetimeoffset(0),
	@sMemberDisplayName nvarchar(55),
	@bFO2BInvoicedOnly bit)
	-- where @bFO2BInvoicedOnly means a bit/boolean parameter specifying whether or not to display only flight operations that are to be invoiced.
	--       I.e., a value of true or 1 says to not display any flight operations for which an invoice has already been generated.
AS
BEGIN
	DECLARE @Tinv2go TABLE (i int, b bit)
	INSERT INTO @Tinv2go VALUES (-1, 1)
	INSERT INTO @Tinv2go VALUES ( 0, 1 - @bFO2BInvoicedOnly)
	INSERT INTO @Tinv2go VALUES ( 1, 1)
	INSERT INTO @Tinv2go VALUES ( 2, 1)

	IF @sMemberDisplayName = N' ALL'
	BEGIN
		SELECT OP.ID, OP.DBegin AS [Date], LM.sLaunchMethod, OD.dReleaseAltitude AS [Rel_Alt_ft], DATEDIFF(minute, OP.DBegin, OP.DEnd) AS [Dur_min],
				PE.sDisplayName AS [Aviator], AR.sAviatorRole AS [Role], AV.dPercentCharge AS [Perc], CC.cChargeCode AS [CC], OP.iInvoices2go AS [Inv2go]
			FROM AVIATORS AS AV
				INNER JOIN OPDETAILS AS OD ON AV.iOpDetail = OD.ID
				INNER JOIN OPERATIONS AS OP ON OD.iOperation = OP.ID
				INNER JOIN PEOPLE AS PE ON AV.iPerson = PE.ID
				INNER JOIN LAUNCHMETHODS AS LM ON OP.iLaunchMethod = LM.ID
				INNER JOIN CHARGECODES AS CC ON OP.iChargeCode = CC.ID
				INNER JOIN AVIATORROLES AS AR ON AV.iAviatorRole = AR.ID
			WHERE (AR.sAviatorRole <> 'Tow Pilot')
				AND (AR.sAviatorRole <> 'Instructor')
				AND (OP.DBegin >= @DFrom)
				AND (OP.DBegin <= @DTo)
				AND (OP.iInvoices2go IN (SELECT i FROM @Tinv2go WHERE b = 1))
			ORDER BY OP.DBegin
	END
	ELSE
	BEGIN
		SELECT OP.ID, OP.DBegin AS [Date], LM.sLaunchMethod, OD.dReleaseAltitude AS [Rel_Alt_ft], DATEDIFF(minute, OP.DBegin, OP.DEnd) AS [Dur_min],
				PE.sDisplayName AS [Aviator], AR.sAviatorRole AS [Role], AV.dPercentCharge AS [Perc], CC.cChargeCode AS [CC], OP.iInvoices2go AS [Inv2go]
			FROM AVIATORS AS AV
				INNER JOIN OPDETAILS AS OD ON AV.iOpDetail = OD.ID
				INNER JOIN OPERATIONS AS OP ON OD.iOperation = OP.ID
				INNER JOIN PEOPLE AS PE ON AV.iPerson = PE.ID
				INNER JOIN LAUNCHMETHODS AS LM ON OP.iLaunchMethod = LM.ID
				INNER JOIN CHARGECODES AS CC ON OP.iChargeCode = CC.ID
				INNER JOIN AVIATORROLES AS AR ON AV.iAviatorRole = AR.ID
			WHERE (AR.sAviatorRole <> 'Tow Pilot')
				AND (AR.sAviatorRole <> 'Instructor')
				AND (OP.DBegin >= @DFrom)
				AND (OP.DBegin <= @DTo)
				AND (OP.iInvoices2go IN (SELECT i FROM @Tinv2go WHERE b = 1))
				AND (@sMemberDisplayName = PE.sDisplayName)
			ORDER BY OP.DBegin
	END
END
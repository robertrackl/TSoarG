SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('spFiltOpsDistinct') IS NOT NULL) DROP PROCEDURE spFiltOpsDistinct
GO
-- Create a set of filtered flight operations for the purpose of creating invoices
CREATE PROCEDURE spFiltOpsDistinct (
	@DFrom datetimeoffset(0),
	@DTo datetimeoffset(0),
	@sMemberDisplayName nvarchar(55))
AS
BEGIN
	DECLARE @Tinv2go TABLE (i int, b bit)
	INSERT INTO @Tinv2go VALUES (-1, 1)
	INSERT INTO @Tinv2go VALUES ( 0, 0) -- Return only flight operations that have not yet been invoiced
	INSERT INTO @Tinv2go VALUES ( 1, 1)
	INSERT INTO @Tinv2go VALUES ( 2, 1)

	IF @sMemberDisplayName = N' ALL'
	BEGIN
		SELECT DISTINCT OP.ID
			FROM OPERATIONS AS OP
			WHERE (OP.DBegin >= @DFrom)
				AND (OP.DBegin <= @DTo)
				AND (OP.iInvoices2go IN (SELECT i FROM @Tinv2go WHERE b = 1))
	END
	ELSE
	BEGIN
		SELECT DISTINCT OP.ID
			FROM AVIATORS AS AV
				INNER JOIN OPDETAILS AS OD ON AV.iOpDetail = OD.ID
				INNER JOIN OPERATIONS AS OP ON OD.iOperation = OP.ID
				INNER JOIN PEOPLE AS PE ON AV.iPerson = PE.ID
				INNER JOIN AVIATORROLES AS AR ON AV.iAviatorRole = AR.ID
			WHERE (OP.DBegin >= @DFrom)
				AND (OP.DBegin <= @DTo)
				AND (OP.iInvoices2go IN (SELECT i FROM @Tinv2go WHERE b = 1))
				AND (@sMemberDisplayName = PE.sDisplayName)
	END
END
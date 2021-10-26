SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('spInvoiceDetails') IS NOT NULL) DROP PROCEDURE spInvoiceDetails
GO
-- Create a list of members to be shown in a dropdownlist for filtering the display of invoices
CREATE PROCEDURE spInvoiceDetails (
--	@DFromInv datetimeoffset(0),
--	@DToInv datetimeoffset(0),
	@DFromInvFO datetimeoffset(0),
	@DToInvFO datetimeoffset(0),
	@sMemberDisplayNameInv nvarchar(55),
	@bClosed bit)
AS
BEGIN
IF @sMemberDisplayNameInv = N' ALL'
	BEGIN
		SELECT I.ID, I.PiTRecordEntered, P2.sDisplayName AS GendBy, I.DInvoice, P.sDisplayName, I.DFrom, I.DTo,
				I.bClosed, I.bSelected, IL.sDescription, IL.mUnitPrice, 
				SUM(mUnitPrice) OVER (PARTITION BY I.ID) AS InvTotal, I.iRefNum, I.sXferFileName
			FROM INVLINES IL
				INNER JOIN INVOICES I ON IL.iInvoice = I.ID
				INNER JOIN PEOPLE P ON I.iPerson = P.ID
				INNER JOIN PEOPLE P2 ON I.iRecordEnteredBy = P2.ID
			WHERE ((I.bClosed = @bClosed) OR (I.bClosed = 0))
				AND (I.DInvoice >= @DFromInvFO) AND (I.DInvoice <= @DToInvFO)
--				AND (I.DFrom >= @DFromInvFO) AND (I.Dto <= @DToInvFO)
			ORDER BY I.ID
	END
ELSE
	BEGIN
		SELECT I.ID, I.PiTRecordEntered, P2.sDisplayName AS GendBy, I.DInvoice, P.sDisplayName, I.DFrom, I.DTo,
				I.bClosed, I.bSelected, IL.sDescription, IL.mUnitPrice, 
				SUM(mUnitPrice) OVER (PARTITION BY I.ID) AS InvTotal, I.iRefNum, I.sXferFileName
			FROM INVLINES IL
				INNER JOIN INVOICES I ON IL.iInvoice = I.ID
				INNER JOIN PEOPLE P ON I.iPerson = P.ID
				INNER JOIN PEOPLE P2 ON I.iRecordEnteredBy = P2.ID
			WHERE ((I.bClosed = @bClosed) OR (I.bClosed = 0))
				AND (I.DInvoice >= @DFromInvFO) AND (I.DInvoice <= @DToInvFO)
--				AND (I.DFrom >= @DFromInvFO) AND (I.Dto <= @DToInvFO)
				AND (P.sDisplayName = @sMemberDisplayNameInv)
			ORDER BY I.ID
	END
END

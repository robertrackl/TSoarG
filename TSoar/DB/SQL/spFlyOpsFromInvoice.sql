SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('spFlyOpsFromInvoice') IS NOT NULL) DROP PROCEDURE spFlyOpsFromInvoice
GO
CREATE PROCEDURE spFlyOpsFromInvoice -- Given an invoice ID, find the flight operations that this invoice is due to
(
	@iInvoiceID int	
)
AS
BEGIN
	SELECT DISTINCT INVLINES.iOperation
		FROM INVOICES
			INNER JOIN INVLINES ON INVOICES.ID = INVLINES.iInvoice
		WHERE (INVOICES.ID = @iInvoiceID)
END
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('spInvoiceTotal') IS NOT NULL) DROP PROCEDURE spInvoiceTotal
GO
-- Calculate and store the invoice total from the lines associated with the invoice
CREATE PROCEDURE spInvoiceTotal (@iuInvoice int)
AS
BEGIN
	DECLARE @Total money
	SELECT @Total = SUM(dbo.INVLINES.dQuantity * dbo.INVLINES.mUnitPrice)
		FROM INVLINES
		WHERE (INVLINES.iInvoice = @iuInvoice)
	UPDATE INVOICES SET mTotalAmt = @Total WHERE ID = @iuInvoice
END
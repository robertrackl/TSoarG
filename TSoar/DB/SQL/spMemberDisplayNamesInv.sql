SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('spMemberDisplayNamesInv') IS NOT NULL) DROP PROCEDURE spMemberDisplayNamesInv
GO
-- Create a list of members to be shown in a dropdownlist for filtering the display of invoices
CREATE PROCEDURE spMemberDisplayNamesInv (
	@DFrom datetimeoffset(0),
	@DTo datetimeoffset(0))
AS
BEGIN
	SELECT PEOPLE.ID, PEOPLE.sDisplayName
		FROM INVOICES
			INNER JOIN PEOPLE ON INVOICES.iPerson = PEOPLE.ID
		WHERE (INVOICES.DInvoice >= @DFrom)
			AND (INVOICES.DInvoice <= @DTo)
	UNION
	SELECT 0 AS ID, N' ALL' AS sDisplayName
		ORDER BY sDisplayName
END
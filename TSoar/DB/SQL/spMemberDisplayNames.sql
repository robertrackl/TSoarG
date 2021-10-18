SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('spMemberDisplayNames') IS NOT NULL) DROP PROCEDURE spMemberDisplayNames
GO
-- Create a set of filtered flight operations for the purpose of creating invoices
CREATE PROCEDURE spMemberDisplayNames (
	@DFrom datetimeoffset(0),
	@DTo datetimeoffset(0))
AS
BEGIN
	SELECT P0.ID, P0.sDisplayName
		FROM PEOPLE P0
		WHERE ID IN (
			SELECT P1.ID
			FROM AVIATORS
				INNER JOIN PEOPLE P1 ON AVIATORS.iPerson = P1.ID
				INNER JOIN OPDETAILS ON AVIATORS.iOpDetail = OPDETAILS.ID
				INNER JOIN OPERATIONS OP ON OPDETAILS.iOperation = OP.ID
				INNER JOIN AVIATORROLES AR ON AVIATORS.iAviatorRole = AR.ID
			WHERE (OP.DBegin >= @DFrom) AND (OP.DBegin <= @DTo)
				AND(AR.sAviatorRole <> 'Tow Pilot') AND (AR.sAviatorRole <> 'Instructor')
		)
	UNION
	SELECT 0 AS ID, N' ALL' AS sDisplayName
		ORDER BY sDisplayName
END
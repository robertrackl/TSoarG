SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('spCheck4MFCOverlap') IS NOT NULL) DROP PROCEDURE spCheck4MFCOverlap
GO
-- Check on whether there are any MFCs that overlap each other in time for the same membership category
CREATE PROCEDURE spCheck4MFCOverlap
AS
BEGIN
	-- Code ideas taken from: https://www.sqlservercentral.com/forums/topic/check-if-date-ranges-overlap (Hugo Kornelis's solution)
	SELECT C.sMembershipCategory AS [Membership Category], A.DFrom AS From_1, A.Dto AS To_1, B.DFrom AS From_2, B. DTo AS To_2
		FROM MINFLYCHGPARS A
		JOIN MINFLYCHGPARS B
			ON B.iMembershipCategory = A.iMembershipCategory
			AND B.DFrom < A.DTo
			AND A.DFrom < B.DTo
			AND A.DFrom < B.DFrom
		INNER JOIN MEMBERSHIPCATEGORIES C ON C.ID = A.iMembershipCategory
END
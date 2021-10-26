SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('spGetMFCPars') IS NOT NULL) DROP PROCEDURE spGetMFCPars
GO
-- Get the list of Minimum Flying Charges Parameters
CREATE PROCEDURE spGetMFCPars
AS
BEGIN
	SELECT M.ID, M.DFrom, M.DTo, M.iMembershipCategory, C.sMembershipCategory, CONVERT(nvarchar(15), M.mMinMonthlyFlyChrg) AS sMinFlyChrg, M.sComments
		FROM MINFLYCHGPARS M
			INNER JOIN  MEMBERSHIPCATEGORIES C ON M.iMembershipCategory = C.ID
		ORDER BY C.sMembershipCategory, M.DFrom
END
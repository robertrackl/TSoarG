SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('spMembCatFromFlyChg') IS NOT NULL) DROP PROCEDURE spMembCatFromFlyChg
GO
-- Get the membership category of the member associated with a flying charge; 
--	get the membership category that the member had at the time of the flying charge
CREATE PROCEDURE spMembCatFromFlyChg (
	@iuFlyChg int,
	@suStatus nvarchar(MAX) output)
AS
BEGIN
	SET @suStatus = 'unknown'
	SELECT C.sMembershipCategory
		FROM MEMBERSHIPCATEGORIES AS C
			INNER JOIN MEMBERFROMTO AS F ON C.ID = F.iMemberCategory
			INNER JOIN PEOPLE AS P ON F.iPerson = P.ID
			INNER JOIN FLYINGCHARGES AS Y ON P.ID = Y.iPerson AND F.DMembershipEnd >= Y.DateOfAmount AND F.DMembershipBegin <= Y.DateOfAmount
		WHERE (Y.ID = @iuFlyChg)
	SET @suStatus = 'OK' + CONVERT(nvarchar(12), @@ROWCOUNT)
END
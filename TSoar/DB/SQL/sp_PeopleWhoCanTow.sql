SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('sp_PeopleWhoCanTow') IS NOT NULL) DROP PROCEDURE sp_PeopleWhoCanTow
GO
CREATE PROCEDURE sp_PeopleWhoCanTow
	@Date DateTime
AS
BEGIN
	-- Who can tow by membership category
	SELECT PEOPLE.ID, PEOPLE.sDisplayName
		FROM MEMBERFROMTO
			INNER JOIN MEMBERSHIPCATEGORIES ON MEMBERFROMTO.iMemberCategory = MEMBERSHIPCATEGORIES.ID
			INNER JOIN PEOPLE ON MEMBERFROMTO.iPerson = PEOPLE.ID
		WHERE	(MEMBERFROMTO.DMembershipBegin <= @Date)
				AND	(MEMBERFROMTO.DMembershipEnd >= @Date)
				AND (MEMBERSHIPCATEGORIES.sMembershipCategory LIKE '%Tow%')
	UNION
	-- Who can tow by qualification
	SELECT PEOPLE.ID, PEOPLE.sDisplayName
		FROM PEOPLE
			INNER JOIN PEOPLEQUALIFICS ON PEOPLE.ID = PEOPLEQUALIFICS.iPerson
			INNER JOIN QUALIFICATIONS ON PEOPLEQUALIFICS.iQualification = QUALIFICATIONS.ID
		WHERE	(QUALIFICATIONS.sQualification LIKE '%Tow%')
				AND (QUALIFICATIONS.sQualification NOT LIKE '%Tow')
				AND (PEOPLEQUALIFICS.DSince <= @Date)
				AND (PEOPLEQUALIFICS.DExpiry >= @Date)
END
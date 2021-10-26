SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE IntSoarDB
GO
IF (OBJECT_ID('TNPF_EligibleRewardMembers') IS NOT NULL) DROP FUNCTION TNPF_EligibleRewardMembers
GO
CREATE FUNCTION TNPF_EligibleRewardMembers (@DAsOf DateTimeOffset(0), @bAsOf bit)
-- @DAsOf: Get list of members who were eligible as of this date
-- @bAsOf: 0 = ignore the date in @DAsOf (get list of members who were eligigble at any one time)
--         1 = Get list of members who were eligible as of the date given in @DAsOf
RETURNS 
@ELREWMBRS TABLE 
(
	ID int,
	sDisplayName nvarchar(55)
)
AS
BEGIN
	DECLARE @ID int, @sDisplayName nvarchar(55)
	IF @bAsOf = 0
	Begin
		DECLARE cursor_Rewards CURSOR LOCAL FAST_FORWARD FOR
		SELECT P.ID, P.sDisplayName
			FROM PEOPLE AS P INNER JOIN
				PEOPLEQUALIFICS AS PQ ON P.ID = PQ.iPerson INNER JOIN
				QUALIFICATIONS AS Q ON PQ.iQualification = Q.ID
			WHERE (Q.sQualification = N'PSSA Tow Pilot')
		UNION
		SELECT P.ID, P.sDisplayName
			FROM PEOPLE AS P INNER JOIN
				PEOPLECERTIFICS AS PC ON P.ID = PC.iPerson INNER JOIN
				CERTIFICATIONS AS C ON PC.iCertification = C.ID INNER JOIN
				PEOPLERATINGS AS PR ON P.ID = PR.iPerson INNER JOIN
				RATINGS AS R ON PR.iRatings = R.ID
			WHERE ((R.sRating = N'Glider') AND (C.sCertification = 'Instructor'))
		ORDER BY P.sDisplayName
	END
	ELSE
	BEGIN
		DECLARE cursor_Rewards CURSOR LOCAL FAST_FORWARD FOR
		SELECT P.ID, P.sDisplayName
			FROM PEOPLE AS P INNER JOIN
				PEOPLEQUALIFICS AS PQ ON P.ID = PQ.iPerson INNER JOIN
				QUALIFICATIONS AS Q ON PQ.iQualification = Q.ID
			WHERE (Q.sQualification = N'PSSA Tow Pilot') AND (PQ.DSince <= @DAsOf) AND (PQ.DExpiry >= @DAsOf)
		UNION
		SELECT P.ID, P.sDisplayName
			FROM PEOPLE AS P INNER JOIN
				PEOPLECERTIFICS AS PC ON P.ID = PC.iPerson INNER JOIN
				CERTIFICATIONS AS C ON PC.iCertification = C.ID INNER JOIN
				PEOPLERATINGS AS PR ON P.ID = PR.iPerson INNER JOIN
				RATINGS AS R ON PR.iRatings = R.ID
			WHERE ((R.sRating = N'Glider') AND (C.sCertification = 'Instructor')
				AND (PC.DSince <= @DAsOf) AND (PC.DExpiry >= @DAsOf)
				AND (PR.DSince <= @DAsOf) AND (PR.DExpiry >= @DAsOf))
		ORDER BY P.sDisplayName
	END
	OPEN cursor_Rewards;

	FETCH NEXT FROM cursor_Rewards INTO @ID, @sDisplayName
	WHILE @@FETCH_STATUS = 0
	BEGIN
		INSERT INTO @ELREWMBRS VALUES (@ID, @sDisplayName)	
		FETCH NEXT FROM cursor_Rewards INTO @ID, @sDisplayName
	END

	CLOSE cursor_Rewards
	DEALLOCATE cursor_Rewards
	
	RETURN 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('spCheck4PeopleFromToOverlaps') IS NOT NULL) DROP PROCEDURE spCheck4PeopleFromToOverlaps
GO
-- Check on whether there are any MFCs that overlap each other in time for the same membership category
CREATE PROCEDURE spCheck4PeopleFromToOverlaps (@suStatus nvarchar(700) output)
AS
BEGIN
	-- Code ideas taken from: https://www.sqlservercentral.com/forums/topic/check-if-date-ranges-overlap (Hugo Kornelis's solution)
	SET @suStatus = 'unknown'
	-- Loop over all People
	DECLARE @PEOPLE_ID int
	DECLARE @sDisplayName nvarchar(55)
	DECLARE @sCat nvarchar(55)
	DECLARE @From_1 DateTimeOffset(0)
	DECLARE @To_1 DateTimeOffset(0)
	DECLARE @From_2 DateTimeOffset(0)
	DECLARE @To_2 DateTimeOffset(0)

	DECLARE @TMemberC TABLE (sS varchar(25) not null, DBegin_1 DateTimeOffset(0) not null, DEnd_1 DateTimeOffset(0) not null,
								DBegin_2 DateTimeOffset(0) not null, DEnd_2 DateTimeOffset(0), sDisplayName nvarchar(55) not null)

	DECLARE people_cursor CURSOR FOR
	SELECT ID FROM PEOPLE
	OPEN people_cursor
	FETCH NEXT FROM people_cursor INTO @PEOPLE_ID
	WHILE @@FETCH_STATUS = 0
	BEGIN
		-- Club Membership
		INSERT INTO @TMemberC
		SELECT C.sMembershipCategory, A.DMembershipBegin AS From_1, A.DMembershipEnd AS To_1, B.DMembershipBegin AS From_2, B.DMembershipEnd AS To_2, P.sDisplayName
			FROM MEMBERFROMTO A
			JOIN MEMBERFROMTO B
				ON B.iMemberCategory = A.iMemberCategory
				AND B.DMembershipBegin < A.DMembershipEnd
				AND A.DMembershipBegin < B.DMembershipEnd
				AND A.DMembershipBegin < B.DMembershipBegin
			INNER JOIN MEMBERSHIPCATEGORIES C ON C.ID = A.iMemberCategory
			INNER JOIN PEOPLE P on P.ID = A.iPerson
			WHERE A.iPerson=@PEOPLE_ID AND B.iPerson=@PEOPLE_ID
		IF @@ROWCOUNT>0
		BEGIN
			SELECT @sDisplayName = T.sDisplayName, @sCat=sS, @From_1=DBegin_1, @To_1=DEnd_1, @From_2=DBegin_2, @To_2=DEnd_2 FROM @TMemberC AS T
			SET @suStatus = 'Club Membership overlap found: Member "' + @sDisplayName + '", membership category "' + @sCat + '": ' + 
				CONVERT(nvarchar(35), @From_1) + ' - ' + CONVERT(nvarchar(35), @To_1) + ' overlaps ' + CONVERT(nvarchar(35),@From_2) + ' - ' + CONVERT(nvarchar(35), @To_2)
			CLOSE people_cursor
			DEALLOCATE people_cursor
			RETURN
		END
		DELETE FROM @TMemberC

		-- SSA Membership
		INSERT INTO @TMemberC
		SELECT C.sSSA_MemberCategory, A.DMembershipBegin, A.DMembershipEnd, B.DMembershipBegin, B.DMembershipEnd, P.sDisplayName
			FROM SSA_MEMBERFROMTO A
			JOIN SSA_MEMBERFROMTO B
				ON B.iSSA_MemberCategory = A.iSSA_MemberCategory
				AND B.DMembershipBegin < A.DMembershipEnd
				AND A.DMembershipBegin < B.DMembershipEnd
				AND A.DMembershipBegin < B.DMembershipBegin
			INNER JOIN SSA_MEMBERCATEGORIES C ON C.ID = A.iSSA_MemberCategory
			INNER JOIN PEOPLE P on P.ID = A.iPerson
			WHERE A.iPerson=@PEOPLE_ID AND B.iPerson=@PEOPLE_ID
		IF @@ROWCOUNT>0
		BEGIN
			SELECT @sDisplayName = T.sDisplayName, @sCat=sS, @From_1=DBegin_1, @To_1=DEnd_1, @From_2=DBegin_2, @To_2=DEnd_2 FROM @TMemberC AS T
			SET @suStatus = 'SSA Membership overlap found: Member "' + @sDisplayName + '", membership category "' + @sCat + '": ' + 
				CONVERT(nvarchar(35), @From_1) + ' - ' + CONVERT(nvarchar(35), @To_1) + ' overlaps ' + CONVERT(nvarchar(35),@From_2) + ' - ' + CONVERT(nvarchar(35), @To_2)
			CLOSE people_cursor
			DEALLOCATE people_cursor
			RETURN
		END
		DELETE FROM @TMemberC

		-- Qualifications
		INSERT INTO @TMemberC
		SELECT C.sQualification, A.DSince, A.DExpiry, B.DSince, B.DExpiry, P.sDisplayName
			FROM PEOPLEQUALIFICS A
			JOIN PEOPLEQUALIFICS B
				ON B.iQualification = A.iQualification
				AND B.DSince < A.DExpiry
				AND A.DSince < B.DExpiry
				AND A.DSince < B.DSince
			INNER JOIN QUALIFICATIONS C ON C.ID = A.iQualification
			INNER JOIN PEOPLE P on P.ID = A.iPerson
			WHERE A.iPerson=@PEOPLE_ID AND B.iPerson=@PEOPLE_ID
		IF @@ROWCOUNT>0
		BEGIN
			SELECT TOP 1 @sDisplayName = T.sDisplayName, @sCat=sS, @From_1=DBegin_1, @To_1=DEnd_1, @From_2=DBegin_2, @To_2=DEnd_2 FROM @TMemberC AS T
			IF @sCat != 'None'
			BEGIN
				SET @suStatus = 'Qualifications overlap found: Member "' + @sDisplayName + '", qualification "' + @sCat + '": ' + 
					CONVERT(nvarchar(35), @From_1) + ' - ' + CONVERT(nvarchar(35), @To_1) + ' overlaps ' + CONVERT(nvarchar(35),@From_2) + ' - ' + CONVERT(nvarchar(35), @To_2)
				CLOSE people_cursor
				DEALLOCATE people_cursor
				RETURN
			END
		END
		DELETE FROM @TMemberC

		-- Certifications
		INSERT INTO @TMemberC
		SELECT C.sCertification, A.DSince, A.DExpiry, B.DSince, B.DExpiry, P.sDisplayName
			FROM PEOPLECERTIFICS A
			JOIN PEOPLECERTIFICS B
				ON B.iCertification = A.iCertification
				AND B.DSince < A.DExpiry
				AND A.DSince < B.DExpiry
				AND A.DSince < B.DSince
			INNER JOIN CERTIFICATIONS C ON C.ID = A.iCertification
			INNER JOIN PEOPLE P on P.ID = A.iPerson
			WHERE A.iPerson=@PEOPLE_ID AND B.iPerson=@PEOPLE_ID
		IF @@ROWCOUNT>0
		BEGIN
			SELECT TOP 1 @sDisplayName = T.sDisplayName, @sCat=sS, @From_1=DBegin_1, @To_1=DEnd_1, @From_2=DBegin_2, @To_2=DEnd_2 FROM @TMemberC AS T
			IF @sCat != 'None'
			BEGIN
				SET @suStatus = 'Certifications overlap found: Member "' + @sDisplayName + '", certification "' + @sCat + '": ' + 
					CONVERT(nvarchar(35), @From_1) + ' - ' + CONVERT(nvarchar(35), @To_1) + ' overlaps ' + CONVERT(nvarchar(35),@From_2) + ' - ' + CONVERT(nvarchar(35), @To_2)
				CLOSE people_cursor
				DEALLOCATE people_cursor
				RETURN
			END
		END
		DELETE FROM @TMemberC

		-- Ratings
		INSERT INTO @TMemberC
		SELECT C.sRating, A.DSince, A.DExpiry, B.DSince, B.DExpiry , P.sDisplayName
			FROM PEOPLERATINGS A
			JOIN PEOPLERATINGS B
				ON B.iRatings = A.iRatings
				AND B.DSince < A.DExpiry
				AND A.DSince < B.DExpiry
				AND A.DSince < B.DSince
			INNER JOIN RATINGS C ON C.ID = A.iRatings
			INNER JOIN PEOPLE P on P.ID = A.iPerson
			WHERE A.iPerson=@PEOPLE_ID AND B.iPerson=@PEOPLE_ID
		IF @@ROWCOUNT>0
		BEGIN
			SELECT TOP 1 @sDisplayName = T.sDisplayName, @sCat=sS, @From_1=DBegin_1, @To_1=DEnd_1, @From_2=DBegin_2, @To_2=DEnd_2 FROM @TMemberC AS T
			IF @sCat != 'None'
			BEGIN
				SET @suStatus = 'Ratings overlap found: Member "' + @sDisplayName + '", rating "' + @sCat + '": ' + 
					CONVERT(nvarchar(35), @From_1) + ' - ' + CONVERT(nvarchar(35), @To_1) + ' overlaps ' + CONVERT(nvarchar(35),@From_2) + ' - ' + CONVERT(nvarchar(35), @To_2)
				CLOSE people_cursor
				DEALLOCATE people_cursor
				RETURN
			END
		END
		DELETE FROM @TMemberC

		-- Board Offices
		INSERT INTO @TMemberC
		SELECT C.sBoardOffice, A.DOfficeBegin AS From_1, A.DOfficeEnd AS To_1, B.DOfficeBegin AS From_2, B.DOfficeEnd AS To_2, P.sDisplayName
			FROM PEOPLEOFFICES A
			JOIN PEOPLEOFFICES B
				ON B.iBoardOffice = A.iBoardOffice
				AND B.DOfficeBegin < A.DOfficeEnd
				AND A.DOfficeBegin < B.DOfficeEnd
				AND A.DOfficeBegin < B.DOfficeBegin
			INNER JOIN BOARDOFFICES C ON C.ID = A.iBoardOffice
			INNER JOIN PEOPLE P on P.ID = A.iPerson
			WHERE A.iPerson=@PEOPLE_ID AND B.iPerson=@PEOPLE_ID
		IF @@ROWCOUNT>0
		BEGIN
			SELECT @sDisplayName = T.sDisplayName, @sCat=sS, @From_1=DBegin_1, @To_1=DEnd_1, @From_2=DBegin_2, @To_2=DEnd_2 FROM @TMemberC AS T
			SET @suStatus = 'Board Office overlap found: Member "' + @sDisplayName + '", board office "' + @sCat + '": ' + 
				CONVERT(nvarchar(35), @From_1) + ' - ' + CONVERT(nvarchar(35), @To_1) + ' overlaps ' + CONVERT(nvarchar(35),@From_2) + ' - ' + CONVERT(nvarchar(35), @To_2)
			CLOSE people_cursor
			DEALLOCATE people_cursor
			RETURN
		END
		DELETE FROM @TMemberC

		FETCH NEXT FROM people_cursor INTO @PEOPLE_ID
	END
	CLOSE people_cursor
	DEALLOCATE people_cursor

	SET @suStatus = 'OK'
END
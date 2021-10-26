SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('spGenMFCs') IS NOT NULL) DROP PROCEDURE spGenMFCs
GO
-- Generate minimum flying charges using the parameters pointed at by @iuMFCPID in table MINFLYCHGPARS
CREATE PROCEDURE spGenMFCs (
	@iuMFCPID int, -- points to row in table MINFLYCHGPARS
	@iuUser int, -- ID of current web site user
	@sStatus nvarchar(MAX) output)
AS
BEGIN
	SET @sStatus = 'Unknown' -- pessimistic
	DECLARE @DFrom DateTimeOffset(0)
	DECLARE @DTo DateTimeOffset(0)
	DECLARE @iMembCat int
	DECLARE @mMFC money
	SELECT @DFrom = DFrom, @DTo = DTo, @iMembCat = iMembershipCategory, @mMFC = mMinMonthlyFlyChrg FROM MINFLYCHGPARS WHERE ID = @iuMFCPID
	IF @@ROWCOUNT < 1
	BEGIN
		SET @sStatus = 'No rows selected from table MINFLYCHGPARS'
		RETURN
	END

	BEGIN TRANSACTION
		BEGIN TRY
			-- Create a table of dates for which monthly minimum charges are to be generated
			DECLARE @TMonths TABLE (DM DateTimeOffset(0)) -- dates are always on the last day of the month
			DECLARE @MCounter DateTimeOffset(0)
			DECLARE @DEOM DateTimeOffset(0)
			SET @MCounter = DATEADD(month, DATEDIFF(month, 0, @DFrom), 0) -- gets first day of month
			WHILE @MCounter <= @DTo
			BEGIN
				SET @DEOM = EOMONTH(@MCounter) -- gets last day of month
				IF @DEOM > @DTo BREAK
				INSERT INTO @TMonths VALUES (@DEOM) 
				SET @MCounter = DATEADD(month, 1, @MCounter)
			END
		END TRY
		BEGIN CATCH
			SET @sStatus = 'Error ' + CONVERT(nvarchar(12),ERROR_NUMBER()) + ' in stored procedure spGenMFCs: `' + ERROR_MESSAGE() + '` in line ' + CONVERT(nvarchar(12),ERROR_LINE())
			ROLLBACK
			RETURN
		END CATCH
	COMMIT
	--SELECT * FROM @TMonths
	--RETURN
	DECLARE @iRecCount int
	-- Don't allow more than 3 years worth of MFCs
	DECLARE @iCount int
	SELECT @iCount = COUNT(*) FROM @TMonths
	IF @iCount > 36
	BEGIN
		SET @sStatus = 'Attempt at generating ' + CONVERT(nvarchar(12),@iCount) + ' months of minimum flying charges; limit is 36'
		RETURN
	END
	DECLARE @iMembCount int = 0
	BEGIN TRANSACTION
		BEGIN TRY
			-- Loop over all club members in the chosen membership category
			DECLARE @iMember int
			DECLARE Members_cursor CURSOR FOR
				SELECT P.ID
					FROM PEOPLE P
						INNER JOIN MEMBERFROMTO M ON P.ID = M.iPerson
					WHERE (M.iMemberCategory = @iMembCat) AND (M.DMembershipEnd >= @DFrom) AND (M.DMembershipBegin <= @DTo); -- last two conditions check for time range overlap
			OPEN Members_cursor
			FETCH NEXT FROM Members_cursor INTO @iMember
			WHILE @@FETCH_STATUS = 0
			BEGIN
				SET @iMembCount = @iMembCount + 1
				DECLARE @DM DateTimeOffset(0)
				DECLARE Months_cursor CURSOR FOR
					SELECT DM FROM @TMonths
				OPEN Months_cursor
				FETCH NEXT FROM Months_cursor INTO @DM
				WHILE @@FETCH_STATUS = 0
				BEGIN
					-- First, check that the desired flying charges don't exist already
					SELECT @iRecCount = COUNT(*) FROM FLYINGCHARGES WHERE iPerson = @iMember AND DateOfAmount = @DM AND cTypeOfAmount = 'M'
					IF @iRecCount > 1
					BEGIN
						SET @sStatus = 'More than 1 records exist already in table FLYINGCHARGES for member ID ' + CONVERT(nvarchar(12), @iMember) +
							' on ' + CONVERT(nvarchar(10),@DM) + ' cTypeOfAmount M'
						ROLLBACK
						RETURN
					END
					IF @iRecCount = 1
					BEGIN
						DELETE FROM FLYINGCHARGES WHERE iPerson = @iMember AND DateOfAmount = @DM AND cTypeOfAmount = 'M'
					END
					-- @iRecCount = 0
					INSERT INTO FLYINGCHARGES (PiTRecordEntered, iRecordEnteredBy, iPerson, mAmount, DateOfAmount, cTypeOfAmount) VALUES
						(GETDATE(), @iuUser, @iMember, @mMFC, @DM, 'M')
					FETCH NEXT FROM Months_cursor INTO @DM
				END
				CLOSE Months_cursor
				DEALLOCATE Months_cursor

				FETCH NEXT FROM Members_cursor INTO @iMember
			END
			CLOSE Members_cursor
			DEALLOCATE Members_cursor
		END TRY
		BEGIN CATCH
			SET @sStatus = 'Error ' + CONVERT(nvarchar(12),ERROR_NUMBER()) + ' in stored procedure spGenMFCs: `' + ERROR_MESSAGE() + '` in line ' + CONVERT(nvarchar(12),ERROR_LINE())
			ROLLBACK
			RETURN
		END CATCH
	COMMIT
	SET @sStatus = 'OK' + CONVERT(nvarchar(12), @iMembCount)
END
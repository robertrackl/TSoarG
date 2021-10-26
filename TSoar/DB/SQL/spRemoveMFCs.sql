SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('spRemoveMFCs') IS NOT NULL) DROP PROCEDURE spRemoveMFCs
GO
-- Generate minimum flying charges using the parameters pointed at by @iuMFCPID in table MINFLYCHGPARS
CREATE PROCEDURE spRemoveMFCs (
	@iuMFCPID int, -- points to row in table MINFLYCHGPARS
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
			-- Create a table of dates for which monthly minimum charges are to be removed
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
	DECLARE @iMembCount int = 0
	DECLARE @iRowCount int = 0
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
					DELETE FROM FLYINGCHARGES WHERE iPerson = @iMember AND DateOfAmount = @DM AND cTypeOfAmount = 'M'
					SET @iRowCount = @iRowCount + @@ROWCOUNT
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
	SET @sStatus = 'OK' + CONVERT(nVarchar(12),@iRowCount) + ',' + CONVERT(nvarchar(12), @iMembCount)
END
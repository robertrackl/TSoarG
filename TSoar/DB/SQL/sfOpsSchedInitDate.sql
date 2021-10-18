SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('sfOpsSchedInitDate') IS NOT NULL) DROP FUNCTION sfOpsSchedInitDate
GO
-- In table FSDATES, find the date which is in the future and closest to today:
CREATE FUNCTION sfOpsSchedInitDate ()
RETURNS @DDates TABLE (iDate int, Dresult Date)
AS
BEGIN
	DECLARE @Dtoday date = GETDATE()
	DECLARE @Dresult date = @Dtoday -- default if everything below fails
	DECLARE @iDate int = 0
	SELECT @iDate = ID, @Dresult = Date FROM FSDATES
		WHERE Date = (SELECT MIN(Date) FROM FSDATES WHERE Date >= @Dtoday)
	DECLARE @iCnt int = @@ROWCOUNT
	IF @iCnt < 1
	BEGIN
		-- Did not find a date greater than or equal to today, so:
		SELECT @DResult = MAX(Date) FROM FSDATES
		SELECT @iDate = ID FROM FSDATES WHERE Date = @DResult
	END
	INSERT INTO @DDates (iDate, Dresult) VALUES (@iDate, @DResult)
	RETURN
END
GO
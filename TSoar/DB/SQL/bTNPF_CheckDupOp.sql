SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('bTNPF_CheckDupOp') IS NOT NULL) DROP FUNCTION bTNPF_CheckDupOp
GO
-- Check whether a flight operation exists with given launch method, takeoff location, and takeoff time
CREATE FUNCTION bTNPF_CheckDupOp (@iLaunchMethod int, @iTakeoffLoc int, @DBegin smalldatetime, @DTakeoff smalldatetime)
RETURNS bit
BEGIN
	DECLARE @bRet bit
	DECLARE @iCount int
	SELECT @iCount=COUNT(*) FROM OPERATIONS WHERE iLaunchMethod = @iLaunchMethod AND iTakeoffLoc = @iTakeoffLoc AND @DBegin=DBegin AND ABS(DATEDIFF(minute, DBegin, @DTakeoff)) < 2
	SET @bRet = 0
	IF @iCount > 0 SET @bRet = 1
	RETURN @bRet
END
GO
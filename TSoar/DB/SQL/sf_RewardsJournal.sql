SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('sf_RewardsJournal') IS NOT NULL) DROP PROCEDURE sf_RewardsJournal
GO
IF (TYPE_ID('sf_TRewardsFilter') IS NOT NULL) DROP TYPE sf_TRewardsFilter
CREATE TYPE sf_TRewardsFilter AS TABLE 
(
	sFilterName nvarchar(50) NOT NULL UNIQUE
		CHECK(sFilterName=N'FilterVersion' OR sFilterName=N'EnableFilteringOverall' OR sFilterName=N'Member' OR
			sFilterName=N'EarnClaimDate' OR sFilterName=N'ShowExpired' OR sFilterName=N'EarnClaimCode' OR sFilterName=N'LimitAtTopBottom'
			OR sFilterName=N'LimitRowCount'),
	sFilterType nvarchar(12)
		CHECK(sFilterType = N'List' OR sFilterType = N'Boolean' OR sFilterType = N'DateList' OR sFilterType = N'Integ32'),
	bEnabled bit,
	sList nvarchar(MAX),
	YesNo bit,
	Integ32 int
);
GO
CREATE PROCEDURE sf_RewardsJournal
	@iDebug int,
	@sStatus nvarchar(MAX) OUTPUT,
	@taFilter sf_TRewardsFilter READONLY
AS
	DECLARE @sCRLF nchar(2) -- carriage return / line feed combination
	SET @sCRLF = CHAR(0x0D) + CHAR(0x0A)
	DECLARE @sErr nchar(45)
	SET @sErr = N'Error in stored procedure sf_RewardsJournal: '
	DECLARE @sSEL0 nvarchar(MAX)
	DECLARE @iTopLines int
	DECLARE @sSEL_TopLines nvarchar(MAX)
	DECLARE @bEnabled bit
	DECLARE @bEnabled_TopLines bit
	DECLARE @iTopBottom int
	DECLARE @sAscDesc nvarchar(4)
	DECLARE @sWHERE0 nvarchar(MAX)
	DECLARE @sConjunction nvarchar(5)
	DECLARE @iMember int
	DECLARE @sWhere_Member nvarchar(MAX)
	DECLARE @sWHERE_EarnClaimDate nvarchar(MAX)
	DECLARE @sDateList nvarchar(MAX)
	DECLARE @iComma int
	DECLARE @sDateLo nvarchar(MAX)
	DECLARE @sDateHi nvarchar(MAX)
	DECLARE @sWHERE_EarnClaimCode nvarchar(MAX)
	DECLARE @sList nvarchar(MAX)
	DECLARE @sWHERE_ShowExpired nvarchar(MAX)
	DECLARE @iVersion int
	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT ON;
	SET @sStatus = 'Stored Procedure sf_RewardsJournal did not run to completion' -- pessimistic
	BEGIN TRY

	SELECT @iVersion = Integ32 FROM @taFilter WHERE sFilterName=N'FilterVersion'
	IF @iVersion <> 1
	BEGIN
		SET @sStatus = @sErr + 'Filter Version should be 1, but is ' + CONVERT(nvarchar(12), @iVersion)
		RETURN
	END

	SELECT @bEnabled_TopLines = bEnabled FROM @taFilter WHERE sFilterName = N'LimitAtTopBottom'
	SET @sSEL_TopLines = ' '
	IF @bEnabled_TopLines = 1
	BEGIN
		SELECT @iTopLines = Integ32 FROM @taFilter WHERE sFilterName = N'LimitRowCount'
		SET @sSEL_TopLines = 'TOP (' + CONVERT(nvarchar(3),@iTopLines) + ')'
	END
	SELECT @iTopBottom = Integ32 FROM @taFilter WHERE sFilterName = N'LimitAtTopBottom'
	SET @sAscDesc = CASE @iTopBottom WHEN 1 THEN 'ASC' ELSE 'DESC' END

	SELECT @bEnabled = bEnabled FROM @taFilter WHERE sFilterName = N'EnableFilteringOverall'
	SET @sWHERE0 = ' WHERE (T.DExpiry > DateAdd(Day, DateDiff(Day, 0, GetDate()), 0)) ' -- Where expiry date is later than this early morning
	SET @sConjunction = ' '
	IF @bEnabled = 1
	BEGIN
		SET @sWHERE0 = ''

		SELECT @bEnabled = bEnabled FROM @taFilter WHERE sFilterName = N'Member'
		SET @sWhere_Member = ' '
		IF @bEnabled = 1
		BEGIN
			SELECT @iMember = Integ32 FROM @taFilter WHERE sFilterName = N'Member'
			SET @sWhere_Member = '(T.iPerson = ' + CONVERT(nvarchar(12),@iMember) + ')'
			SET @sWHERE0 = @sWHERE0 + @sWhere_Member
			SET @sConjunction = ' AND '
		END

		SELECT @bEnabled = bEnabled FROM @taFilter WHERE sFilterName = N'EarnClaimDate'
		SET @sWHERE_EarnClaimDate = ' '
		IF @bEnabled = 1
		BEGIN
			SELECT @sDateList = sList FROM @taFilter WHERE sFilterName = N'EarnClaimDate'
			-- Split the string on a comma since lower and upper limit are separated by a comma
			SET @iComma = CHARINDEX(',', @sDateList)
			IF @iComma < 1
			BEGIN
				SET @sStatus = @sErr + 'No comma in EarnClaimDate DateList'
				RETURN
			END
			SET @sDateLo = SUBSTRING(@sDateList, 1, @iComma - 1)
			SET @sDateHi = SUBSTRING(@sDateList, @iComma + 1, 10)
			SET @sWHERE_EarnClaimDate = '(CASE cECCode WHEN N''C'' THEN DClaim WHEN N''G'' THEN DClaim ELSE DEarn END >= ''' + @sDateLo + 
								''') AND (CASE cECCode WHEN N''C'' THEN DClaim WHEN N''G'' THEN DClaim ELSE DEarn END <= ''' + @sDateHi + ''')'
			SET @sWHERE0 = @sWHERE0 + @sConjunction + @sWHERE_EarnClaimDate
			SET @sConjunction = ' AND '
		END

		SELECT @bEnabled = bEnabled FROM @taFilter WHERE sFilterName = N'EarnClaimCode'
		SET @sWHERE_EarnClaimCode = ' '
		IF @bEnabled = 1
		BEGIN
			SELECT @sList = sList FROM @taFilter WHERE sFilterName = N'EarnClaimCode'
			SET @sWHERE_EarnClaimCode = '(T.cECCode = ''' + @sList + ''')'
			SET @sWHERE0 = @sWHERE0 + @sConjunction + @sWHERE_EarnClaimCode
			SET @sConjunction = ' AND '
		END

		SELECT @bEnabled = bEnabled FROM @taFilter WHERE sFilterName = N'ShowExpired'
		SET @sWHERE_ShowExpired = ' '
		IF @bEnabled = 0
		BEGIN
			--SET @sWHERE_ShowExpired = '(T.DExpiry > DateAdd(Day, DateDiff(Day, 0, GetDate()), 0))'
			SET @sWHERE_ShowExpired = '(T.DExpiry > DATEADD(day, -90, GetDate()))'
			SET @sWHERE0 = @sWHERE0 + @sConjunction + @sWHERE_ShowExpired
		END

		SET @sWHERE0 = LTRIM(RTRIM(@sWHERE0))
		IF LEN(@sWHERE0) > 0
		BEGIN
			SET @sWHERE0 = ' WHERE ' + @sWHERE0
		END
	END
	 -- SCR 223 start (added T.bForward)
	SET @sSEL0 = 'SELECT ' + @sSEL_TopLines + ' T.ID, CASE cECCode WHEN N''C'' THEN 0 WHEN N''G'' THEN 0 ELSE 1 END AS bEC, T.DEarn, T.DExpiry, T.DClaim,
			T.iPerson, P.sDisplayName, T.iServicePts, T.cECCode, T.bForward, T.sComments
		FROM PEOPLE P INNER JOIN TIREWARDS T ON P.ID = T.iPerson ' + @sWHERE0 +
		' ORDER BY CASE cECCode WHEN N''C'' THEN DClaim WHEN N''G'' THEN DClaim ELSE DEarn END ' + @sAscDesc
	-- SCR 223 end
		IF @iDebug > 0
		BEGIN
			SET @sStatus = @sSEL0
			RETURN
		END
		EXEC sp_executesql @sSEL0
		SET @sStatus = 'OK'
	END TRY
	BEGIN CATCH
        SET @sStatus = N'Err Num = ' + CONVERT(nvarchar(12), ERROR_NUMBER())
		SET @sStatus = @sStatus + N', ' + N'Err Severity = ' + CONVERT(nvarchar(12), ERROR_SEVERITY())
		SET @sStatus = @sStatus + N', ' + N'Err State = ' + CONVERT(nvarchar(12), ERROR_STATE())
		SET @sStatus = @sStatus + N', ' + N'in ' + ERROR_PROCEDURE()
		SET @sStatus = @sStatus + N', ' + N'Line Num = ' + CONVERT(nvarchar(12), ERROR_LINE())
		SET @sStatus = @sStatus + N', ' + N'Msg: ' + ERROR_MESSAGE()
		RETURN
	END CATCH
GO

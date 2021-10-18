SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('TNPF_MRewards2') IS NOT NULL) DROP FUNCTION TNPF_MRewards2
GO
-- SCR 223 -- lots of changes
CREATE FUNCTION TNPF_MRewards2 (@sUser nvarchar(256), @bIsDisplayName bit, @DExpiryRef datetimeoffset(0))
-- When @bIsDisplayName is false (0) then @sUser is the sUserName in table PEOPLE
-- When @bIsDisplayName is true (1) then @sUser is the sDisplayName in table PEOPLE
RETURNS 
@REW TABLE 
(
		EarnD datetimeoffset(0),
		ExpiryD datetimeoffset(0),
		ClaimD datetimeoffset(0),
		OrderByD datetimeoffset(0),
		ServicePtsi int,
		Expiredb bit,
		ECCodec nchar(1),
		Cumuli int,
		CumForwarded int,
		Claim1yri int,
		Claim1yrG int,
		Commentss nvarchar(MAX)
)
AS
BEGIN
	DECLARE @iCumul int, @iClaim1yr int,  @iClaim1yrG int, @iClaimYr int, @iPrevClaimYr int, @DOrder datetimeoffset(0), @bExpired bit
	SET @iCumul = 0 -- cumulative account value
	SET @iClaim1yr = 0 -- service points claimed during one calendar year
	SET @iClaim1yrG = 0 -- service points gift-claimed during one calendar year

	DECLARE @DEarn datetimeoffset(0), @iOrder int, @DExpiry datetimeoffset(0), @DClaim datetimeoffset(0), @iServicePts int
	DECLARE @cECCode nchar(1), @bForward bit, @iCumForwarded int = 0, @sComments nvarchar(MAX)

--	IF @bIsDisplayName = 0
--	BEGIN
		DECLARE cursor_Rewards CURSOR LOCAL FAST_FORWARD FOR
			SELECT DEarn, DExpiry, DClaim, iServicePts, cECCode, bForward, sComments,
				CASE
					WHEN cECCode = N'C' OR cECCode = N'G'
						THEN DClaim
					ELSE DEarn
					END
				AS DOrder,
				CASE
					cECCode
					WHEN N'S' THEN 1
					WHEN N'T' THEN 2
					WHEN N'I' THEN 3
					WHEN N'C' THEN 41
					WHEN N'G' THEN 42
					WHEN N'X' THEN 51
					WHEN N'R' THEN 52
				END
				AS iOrder
			FROM TIREWARDS  INNER JOIN
				PEOPLE ON TIREWARDS.iPerson = PEOPLE.ID
			WHERE 
				(cECCode = N'R' OR cECCode = N'X') OR
				(PEOPLE.sUserName = @sUser AND @bIsDisplayName = 0) OR (PEOPLE.sDisplayName = @sUser AND @bIsDisplayName = 1)
			ORDER BY DOrder, iOrder
--	END
--	ELSE
--	BEGIN
--		DECLARE cursor_Rewards CURSOR LOCAL FAST_FORWARD FOR
--			SELECT DEarn, DExpiry, DClaim, iServicePts, cECCode, bForward, sComments, CASE cECCode WHEN N'C' THEN DClaim WHEN N'G' THEN DClaim ELSE DEarn END AS DOrder
--			FROM TIREWARDS  INNER JOIN
--				PEOPLE ON TIREWARDS.iPerson = PEOPLE.ID
--			WHERE (PEOPLE.sDisplayName = @sUser)
--			ORDER BY DOrder;
--	END

	OPEN cursor_Rewards;

	FETCH NEXT FROM cursor_Rewards INTO @DEarn, @DExpiry, @DClaim,  @iServicePts, @cECCode, @bForward, @sComments, @DOrder, @iOrder
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @bExpired = CASE WHEN @DExpiry < @DExpiryRef AND @cECCode <> N'C' AND @cECCode <> N'G' THEN 1 ELSE 0 END
		IF @cECCode <> N'X' AND @cECCode <> N'R'
		BEGIN
			SET @iCumul = @iCumul + @iServicePts
		END
		IF @bForward = 1
		BEGIN
			SET @iCumForwarded = @iCumForwarded + @iServicePts
		END
		IF @cECCode = N'R'
		BEGIN
			SET @iClaim1yr = 0
			SET @iClaim1yrG = 0
		END
		IF @cECCode = N'C'
		BEGIN
			SET @iClaim1yr = @iClaim1Yr - @iServicePts
		END
		IF @cECCode = N'G'
		BEGIN
			SET @iClaim1yr = @iClaim1Yr - @iServicePts
			SET @iClaim1yrG = @iClaim1YrG - @iServicePts
		END
		IF @cECCode = N'X'
		BEGIN
			-- The next statement causes expired and unused service points to be forgotten.
			-- But should someone have an overdraft in their account, that is not forgotten:
			SET @iCumul = @iCumForwarded + (CASE WHEN @iCumul < 0 THEN @iCumul ELSE 0 END)
			SET @iCumForwarded = 0
		END

		INSERT INTO @REW VALUES (@DEarn, @DExpiry, @DClaim, @DOrder, @iServicePts, @bExpired, @cECCode, @iCumul, @iCumForwarded, @iClaim1yr, @iClaim1yrG, @sComments)	

		FETCH NEXT FROM cursor_Rewards INTO @DEarn, @DExpiry, @DClaim, @iServicePts, @cECCode, @bForward, @sComments, @DOrder, @iOrder
	END

	CLOSE cursor_Rewards
	DEALLOCATE cursor_Rewards
	
	RETURN 
END
GO
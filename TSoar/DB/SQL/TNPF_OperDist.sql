SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('TNPF_OperDist') IS NOT NULL) DROP FUNCTION TNPF_OperDist
GO
CREATE FUNCTION TNPF_OperDist (@iComponent int, @DUpTo DateTimeOffset(0))
RETURNS decimal(12,2)
-- @iComponent = pointer to record in table EQUIPCOMPONENTS
-- @DUpTo = point in time up to which distance traveled desired for component @iComponent
BEGIN
	DECLARE @dM decimal(12,4) = 0.0 -- for accumulating distance traveled
	DECLARE @dm2 decimal(12,4) = 0.0
	DECLARE @iComp int = @iComponent
	DECLARE @iParent int = 100
	DECLARE @iLevel int = 0
	DECLARE @Dt_AR datetimeoffset(0)
	DECLARE @DEarlyRef datetimeoffset(0) = '1900-01-01 00:00:00 +00:00'
	DECLARE @DLateRef datetimeoffset(0) = '2999-12-31 23:59:00 +00:00'
	DECLARE @Dt_A_max datetimeoffset(0) = @DEarlyRef
	DECLARE @Dt_R_max datetimeoffset(0) = @DLateRef
	DECLARE @DLinkBegin datetimeoffset(0)
	DECLARE @DlinkEnd datetimeoffset(0)
	DECLARE @DLinkBeginParent datetimeoffset(0)
	DECLARE @DLinkEndParent datetimeoffset(0)
	DECLARE @bOper bit = 0
	DECLARE @bAction bit = 0

	-- What is the hierarchical level of the component of interest @iComponent? And, do we need to replace out-of-bounds DLinkBegin/End values?
	SELECT @DLinkBegin = DLinkBegin, @DlinkEnd = DLinkEnd, @iParent = iParentComponent
		FROM EQUIPCOMPONENTS
		WHERE ID = @iComponent
	SET @iComp = @iParent
	WHILE @iParent > 0
	BEGIN
		SELECT @iParent = iParentComponent, @DLinkBeginParent = DLinkBegin, @DLinkEndParent = DLinkEnd
			FROM EQUIPCOMPONENTS
			WHERE ID = @iComp
		SET @iLevel += 1
		IF @DLinkBeginParent >= @DEarlyRef AND @DLinkBegin < @DEarlyRef
		BEGIN
			SET @DLinkBegin = @DLinkBeginParent
		END
		IF @DLinkEndParent <= @DLateRef AND @DlinkEnd > @DLateRef
		BEGIN
			SET @DlinkEnd = @DLinkEndParent
		END
		SET @iComp = @iParent
	END

	-- Perhaps there is no overlap in the time intervals of interest?
		--
		--                 @DUpTo         DLinkBegin           DLinkEnd        
		--  ---------+------|--+---------+-|-------+---------+--|----------
		--                  |              +--------------------+
		--   ---------------+
	-- Or perhaps the desired end point of the calculation is beyond the end of the component's association with its parent?
		--
		--       DLinkBegin           DLinkEnd             @DUpTo
		-- -----+-|-------+---------+--|------+---------+----|-----
		--        +--------------------+                     |
		-- --------------------------------------------------+
	IF ((@DLinkBegin > @DUpTo) OR (@DUpTo > @DlinkEnd))
	BEGIN
		RETURN @dM -- still zero at this point
	END

	-- Find the latest reset record but earlier than and up to @DUpto
	SET @Dt_AR = @DLinkBegin
	SELECT TOP 1 @dm2 = O.dDistance, @Dt_R_max = O.DFrom
		FROM EQUIPOPERDATA O
		WHERE O.iEquipComponent = @iComponent AND O.DFrom <= @DUpTo AND cSource = 'R'
		ORDER BY DFrom DESC
	IF @@ROWCOUNT > 0
	BEGIN
		IF @Dt_R_max > @Dt_AR SET @Dt_AR = @Dt_R_max
		SET @bOper = 1
	END
	
	-- Find the latest action item complete record but earlier than and up to @DUpTo
	SELECT TOP 1 @dM = A.dAtCompletionDist, @Dt_A_max = A.DComplete
		FROM EQUIPACTIONITEMS A
			INNER JOIN EQUIPAGINGITEMS G ON G.ID = A.iEquipAgingItem
		WHERE G.iEquipComponent = @iComponent AND A.DComplete <= @DUpTo
			AND A.iPercentComplete > 0 AND A.dAtCompletionDist >= 0.0
		ORDER BY A.DComplete DESC
	IF @@ROWCOUNT > 0
	BEGIN
		IF @Dt_A_max > @Dt_AR SET @Dt_AR = @Dt_A_max
		SET @bAction = 1
	END

	-- Use the later of the two 
	IF @bOper = 1 AND @bAction = 1
	BEGIN
		IF @Dt_A_max > @Dt_R_max
		BEGIN
			SET @bOper = 0
		END
	END
	IF @bOper = 1
	BEGIN
		SET @dM = @dm2
	END
	-- If @bAction = 1 and @bOper = 0 then @dM already contains the distance we want

	SET @iComp = @iComponent
	WHILE @iLevel > -1
	BEGIN
		-- (1) Add distance traveled for manually entered data where time intervals are entirely within @Dt_AR and @DUpTo
		--
		--       @Dt_AR         O.DFrom              O.DTo         @DUpTo
		--  ------|---+--------+-|-------+---------+--|------+------|--+---
		--        |              +--------------------+             |
		--        +-------------------------------------------------+
		SELECT @dm2 = SUM(O.dDistance)
			FROM EQUIPOPERDATA O
			WHERE O.iEquipComponent = @iComp AND O.DFrom >= @Dt_AR AND O.DTo <= @DUpTo AND cSource = 'M'
		SET @dM += ISNULL(@dm2,0.0)

		-- When the manually entered time interval is partially within @Dt_AR and @DUpTo:
		--		we estimate applicable distance traveled by proportional fraction
		-- (2) @Dt_AR < DFrom < @DUpTo < DTo
		--
		--                       +--------------------------------------------+
		--       @Dt_AR         O.DFrom                            @DUpTo    O.DTo
		--  ------|---+--------+-|-------+---------+---------+------|--+------|
		--        +-------------------------------------------------+
		SELECT @dm2 = SUM(O.dDistance * DATEDIFF(second, O.DFrom, @DUpTo) / DATEDIFF(second, O.DFrom, O.DTo))
			FROM EQUIPOPERDATA O
			WHERE O.iEquipComponent = @iComp AND O.DFrom > @Dt_AR AND O.DTo >= @DUpTo AND O.DFrom < @DUpTo AND cSource = 'M'
		SET @dM += ISNULL(@dm2, 0.0)

		-- (3) DFrom <= @Dt_AR < O.DTo < @DUpTo
		--
		--                       +-------------------------+
		--                      O.DFrom    @Dt_AR         O.DTo    @DUpTo    
		--  ------|---+--------+-|-------+--|------+-------|-+------|--+------
		--                                  +-----------------------+
		SELECT @dm2 = SUM(O.dDistance * DATEDIFF(second, @Dt_AR, O.DTo) / DATEDIFF(second, O.DFrom, O.DTo))
			FROM EQUIPOPERDATA O
			WHERE O.iEquipComponent = @iComp AND O.DFrom <= @Dt_AR AND O.DTo < @DUpTo AND @Dt_AR < O.DTo AND cSource = 'M'
		SET @dM += ISNULL(@dm2, 0.0)

		-- (4) DFrom < @Dt_AR AND  O.DTo > @DUpTo
		--
		--       O.DFrom        @Dt_AR               @DUpTo        O.DTo
		--  ------|---+--------+-|-------+---------+--|------+------|--+---
		--        |              +--------------------+             |
		--        +-------------------------------------------------+
		SELECT @dm2 = SUM(O.dDistance * DATEDIFF(second, @Dt_AR, @DUpTo) / DATEDIFF(second, O.DFrom, O.DTo))
			FROM EQUIPOPERDATA O
			WHERE O.iEquipComponent = @iComp AND O.DFrom <= @Dt_AR AND O.DTo > @DUpTo AND cSource = 'M'
		SET @dM += ISNULL(@dm2, 0.0)

		IF @iLevel = 0
		BEGIN
			BREAK -- Jump out of WHILE loop
		END
		ELSE
		BEGIN
			SELECT @iParent = iParentComponent
				FROM EQUIPCOMPONENTS
				WHERE ID = @iComp
			SET @iComp = @iParent
			SET @iLevel -= 1
			-- Do the next WHILE loop
		END
	END

	RETURN @dM
END
GO

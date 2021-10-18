SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('spOperHours') IS NOT NULL) DROP PROCEDURE spOperHours
GO
-- Calculate the number of hours of operation from tables EQUIPOPERHOURS and OPERATIONS
CREATE PROCEDURE spOperHours (@iComponent int, @DUpTo datetimeoffset(0), @iDbgCtrl int, @dHours decimal(12,4) OUT, @sDbg nvarchar(MAX) OUT)
-- @iComponent = pointer to record in table EQUIPCOMPONENTS
-- @DUpTo = point in time up to which operational hours are desired for component @iComponent
-- @iDbgCtrl = debug control, output goes into @sDbg; 0 = no debug
-- @dHours = resulting hours of operation
AS
BEGIN
	DECLARE @dH decimal(12,4) = 0.0 -- for accumulating ops hours
	DECLARE @dh2 decimal(12,4) = 0.0
	DECLARE @iComp int = @iComponent
	DECLARE @iParent int = 100
	DECLARE @iLevel int = 0
	DECLARE @Dt_AR datetimeoffset(0)
	-- @DEarlyRef: if a component has a DLinkBegin < DEarlyRef then its actual DLinkBegin is that of its parent
	DECLARE @DEarlyRef datetimeoffset(0) = '1900-01-01 00:00:00 +00:00'
	-- @DLateRef: if a component has a DLinkEnd > DEarlyRef then its actual DLinkEnd is that of its parent
	DECLARE @DLateRef datetimeoffset(0) = '2999-12-31 23:59:00 +00:00'
	DECLARE @Dt_A_max datetimeoffset(0) = @DEarlyRef
	DECLARE @Dt_R_max datetimeoffset(0) = @DLateRef
	DECLARE @DLinkBegin datetimeoffset(0)
	DECLARE @DlinkEnd datetimeoffset(0)
	DECLARE @DLinkBeginParent datetimeoffset(0)
	DECLARE @DLinkEndParent datetimeoffset(0)
	DECLARE @bOper bit = 0
	DECLARE @bAction bit = 0

	SET @sDBG = '';
	IF @iDbgCtrl > 0
	BEGIN
		SET @sDBG += 'Entry with @iComponent = ' + CONVERT(nvarchar(12), @iComponent) + ', @DUpTo = ' + CONVERT(nvarchar(34), @DUpTo) + ', @iDbgCtrl = ' + CONVERT(nvarchar(12), @iDbgCtrl)
	END

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
		IF @DLinkBegin < @DEarlyRef
		BEGIN
			SET @DLinkBegin = @DLinkBeginParent
		END
		IF @DlinkEnd > @DLateRef
		BEGIN
			SET @DlinkEnd = @DLinkEndParent
		END
		SET @iComp = @iParent
	END
	IF @iDbgCtrl > 10
	BEGIN
		SET @sDbg += ';^^' + 'DLinkBegin = ' + CONVERT(nvarchar(34), @DLinkBegin) + ' and DLinkEnd = ' + CONVERT(nvarchar(34), @DLinkEnd)
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
	IF ((@DLinkBegin >= @DUpTo) OR (@DUpTo > @DlinkEnd))
	BEGIN
		IF @iDbgCtrl > 10
		BEGIN
			SET @sDbg += ';^^' + 'Exit: @DUpTo is not within DLinkBegin = ' + CONVERT(nvarchar(34), @DLinkBegin) + ' and DLinkEnd = ' + CONVERT(nvarchar(34), @DLinkEnd)
		END
		SET @dHours = @dH -- still zero at this point
	END
	ELSE
	BEGIN
		-- @Dt_AR is the point in time earlier than @DUpTo but the later of:
		--		Latest Reset record of component @iComponent
		--		Latest action item

		-- Find the latest reset record but earlier than and up to @DUpto
		SET @Dt_AR = @DLinkBegin
		SELECT TOP 1 @dh2 = O.dHours, @Dt_R_max = O.DFrom
			FROM EQUIPOPERDATA O
			WHERE O.iEquipComponent = @iComponent AND O.DFrom <= @DUpTo AND cSource = 'R'
			ORDER BY DFrom DESC
		IF @@ROWCOUNT > 0
		BEGIN
			IF @Dt_R_max > @Dt_AR SET @Dt_AR = @Dt_R_max
			SET @bOper = 1
		END
	
		-- Find the latest action item complete record but earlier than and up to @DUpTo
		SELECT TOP 1 @dH = A.dAtCompletionHrs, @Dt_A_max = A.DComplete
			FROM EQUIPACTIONITEMS A
				INNER JOIN EQUIPAGINGITEMS G ON G.ID = A.iEquipAgingItem
			WHERE G.iEquipComponent = @iComponent AND A.DComplete <= @DUpTo
				AND A.iPercentComplete > 0 AND A.dAtCompletionHrs >= 0.0
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
			SET @dH = @dh2
		END
		-- If @bAction = 1 and @bOper = 0 then @dH already contains the hours we want

		IF @iDbgCtrl > 10
		BEGIN
			SET @sDbg += ';^^' + '@Dt_R_max = ' + CONVERT(nvarchar(34), @Dt_R_max) + ', @Dt_A_max = ' + CONVERT(nvarchar(34), @Dt_A_max) + ', @Dt_AR = ' + CONVERT(nvarchar(34), @Dt_AR)
		END

		SET @iComp = @iComponent
		WHILE @iLevel > -1
		BEGIN

			IF @iDbgCtrl > 20
			BEGIN
				SET @sDbg += ';^^' + '  Starting WHILE: @iLevel = ' + CONVERT(nvarchar(12), @iLevel)
			END

			-- (1) Add ops hours for manually entered data where time intervals are entirely within @Dt_AR and @DUpTo
			--
			--       @Dt_AR         O.DFrom              O.DTo         @DUpTo
			--  ------|---+--------+-|-------+---------+--|------+------|--+---
			--        |              +--------------------+             |
			--        +-------------------------------------------------+
			SET @dh2 = 0.0
			SELECT @dh2 = SUM(O.dHours)
				FROM EQUIPOPERDATA O
				WHERE O.iEquipComponent = @iComp AND O.DFrom >= @Dt_AR AND O.DTo <= @DUpTo AND cSource = 'M' -- We include the equal-signs here
			SET @dh2 = ISNULL(@dh2,0.0)
			SET @dH += @dh2

			IF @iDbgCtrl > 20
			BEGIN
				SET @sDbg += ';^^' + '    WHILE (1): @dh2 = ' + CONVERT(nvarchar(14), ISNULL(@dh2,0.0)) + ', @dH = ' + CONVERT(nvarchar(14), @dH)
			END

			-- When the manually entered time interval is partially within @Dt_AR and @DUpTo:
			--		we estimate applicable flight times by proportional fraction
			-- (2) @Dt_AR < DFrom < @DUpTo < DTo
			--
			--                       +--------------------------------------------+
			--       @Dt_AR         O.DFrom                            @DUpTo    O.DTo
			--  ------|---+--------+-|-------+---------+---------+------|--+------|
			--        +-------------------------------------------------+
			SET @dh2 = 0.0
			SELECT @dh2 = SUM(O.dHours * DATEDIFF(second, O.DFrom, @DUpTo) / DATEDIFF(second, O.DFrom, O.DTo))
				FROM EQUIPOPERDATA O
				WHERE O.iEquipComponent = @iComp AND O.DFrom > @Dt_AR AND O.DTo > @DUpTo AND O.DFrom < @DUpTo AND cSource = 'M' -- We do NOT include the equal-signs here
			SET @dh2 = ISNULL(@dh2,0.0)
			SET @dH += @dh2

			IF @iDbgCtrl > 20
			BEGIN
				SET @sDbg += ';^^' + '    WHILE (2): @dh2 = ' + CONVERT(nvarchar(14), ISNULL(@dh2,0.0)) + ', @dH = ' + CONVERT(nvarchar(14), @dH)
			END

			-- (3) DFrom <= @Dt_AR < O.DTo < @DUpTo
			--
			--                       +-------------------------+
			--                      O.DFrom    @Dt_AR         O.DTo    @DUpTo    
			--  ------|---+--------+-|-------+--|------+-------|-+------|--+------
			--                                  +-----------------------+
			SET @dh2 = 0.0
			SELECT @dh2 = SUM(O.dHours * DATEDIFF(second, @Dt_AR, O.DTo) / DATEDIFF(second, O.DFrom, O.DTo))
				FROM EQUIPOPERDATA O
				WHERE O.iEquipComponent = @iComp AND O.DFrom < @Dt_AR AND O.DTo < @DUpTo AND @Dt_AR < O.DTo AND cSource = 'M' -- We do NOT include the equal-signs here
			SET @dh2 = ISNULL(@dh2,0.0)
			SET @dH += @dh2

			IF @iDbgCtrl > 20
			BEGIN
				SET @sDbg += ';^^' + '    WHILE (3): @dh2 = ' + CONVERT(nvarchar(14), ISNULL(@dh2,0.0)) + ', @dH = ' + CONVERT(nvarchar(14), @dH)
			END

			-- (4) DFrom < @Dt_AR AND  O.DTo > @DUpTo
			--
			--       O.DFrom        @Dt_AR               @DUpTo        O.DTo
			--  ------|---+--------+-|-------+---------+--|------+------|--+---
			--        |              +--------------------+             |
			--        +-------------------------------------------------+
			SET @dh2 = 0.0
			SELECT @dh2 = SUM(O.dHours * CONVERT(decimal(12,4),DATEDIFF(second, @Dt_AR, @DUpTo)) / CONVERT(decimal(12,4),DATEDIFF(second, O.DFrom, O.DTo)))
				FROM EQUIPOPERDATA O
				WHERE O.iEquipComponent = @iComp AND ((O.DFrom <= @Dt_AR AND O.DTo > @DUpTo) OR (O.DFrom < @Dt_AR AND O.DTo >= @DUpTo)) AND cSource = 'M' -- Equal-signs included selectively; note the double parentheses around the OR expression!
			SET @dh2 = ISNULL(@dh2,0.0)
			SET @dH += @dh2

			IF @iDbgCtrl > 20
			BEGIN
				SET @sDbg += ';^^' + '    WHILE (4): @iComp = ' + CONVERT(nvarchar(12), @iComp) + ', @dh2 = ' + CONVERT(nvarchar(14), ISNULL(@dh2,0.0)) + ', @dH = ' + CONVERT(nvarchar(14), @dH)
			END

			IF @iLevel = 0
			BEGIN
				SET @dh2 = 0.0
				SELECT @dh2 = SUM(DATEDIFF(minute, O.DBegin, O.DEnd))/60.0
					FROM OPERATIONS O
						INNER JOIN OPDETAILS D ON O.ID = D.iOperation
						INNER JOIN EQUIPMENT E ON E.ID = D.iEquip
						INNER JOIN EQUIPCOMPONENTS C ON E.ID = C.iEquipment
					WHERE C.ID = @iComp AND O.DBegin >= @Dt_AR AND O.DBegin <= @DUpTo
				SET @dh2 = ISNULL(@dh2,0.0)
				SET @dH += @dh2

				IF @iDbgCtrl > 20
				BEGIN
					SET @sDbg += ';^^' + '    Jumping out of WHILE: @dh2 = ' + CONVERT(nvarchar(14), ISNULL(@dh2,0.0)) + ', @dH = ' + CONVERT(nvarchar(14), @dH)
				END
				BREAK -- Jump out of WHILE loop
			END
			ELSE
			BEGIN
				SELECT @iParent = iParentComponent
					FROM EQUIPCOMPONENTS
					WHERE ID = @iComp
				SET @iComp = @iParent
				SET @iLevel -= 1
			END
			-- Do the next WHILE loop
		END

		IF @iDbgCtrl > 0
		BEGIN
			SET @sDbg += ';^^' + 'Exit with @iDbgCtrl = ' + CONVERT(nvarchar(12), @iDbgCtrl)
		END
		SET @dHours = @dH
	END
END

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('tfEqCumData') IS NOT NULL) DROP FUNCTION tfEqCumData
GO
-- Equipment Aging Cumulative Data up to the present time for one equipment component
CREATE FUNCTION tfEqCumData(@iComponId int)
	RETURNS @tblCompCumDat
	TABLE(
		iLine int NOT NULL,
		iComponId int NOT NULL,
		sError nvarchar(MAX) NULL,
		DMaxUnderlLast datetime2(3) NOT NULL,
		dAccumHours decimal(12,4) NOT NULL,
		iAccumCycles int NOT NULL,
		dAccumDist decimal(12,2) NOT NULL,
		sDebug nvarchar(MAX) NULL,
		sActionItem nvarchar(300) NOT NULL,
		sDeadline nvarchar(64) NOT NULL,
		cPrevailSchedMeth nchar(1) NOT NULL,
		iPercentComplete int NOT NULL
	)
	-- Regarding field iLine: Contains a line number for sorting purposes. Different line number ranges have different meanings:
		-- 0 : exists only if no Action Item (AI) could be found for an Aging Item (error condition)
		-- 1 : the Action Item (AI) that resulted in the earlist absolute reference date (DAbsRef) among all Aging Items and Action Items for this component pointed to by @iComponId
		-- 10001 to 19999 : Completed AIs, i.e., iPercentComplete = 100
		-- 20001 to 29999 : Incomplete AIs with 1 <= iPercentComplete <= 99
		-- 30001 to 39999 : Incomplete AIs with iPercentComplete = 0
	-- @iComponId = ID in table EQUIPCOMPONENTS
AS
BEGIN
	DECLARE @sNewLine nchar(4) = CHAR(13) + CHAR(10) + N'# '
	DECLARE @sError nvarchar(MAX) = N'OK' -- optimistic
	DECLARE @sDebug nvarchar(MAX) = CHAR(13) + CHAR(10) + N'## Debug at ' + CONVERT(nvarchar(34), SYSDATETIMEOFFSET()) + N' for @iComponId = ' + CONVERT(nvarchar(34),@iComponId) + N':'
	DECLARE @DUnderlLast datetime2(3) = '0001-01-01 00:00:00' -- To become the PiT when underlying data for this component was last updated in various sections of this code; shown is sDebug
	DECLARE @DMaxUnderlLast datetime2(3) = '0001-01-01 00:00:00' -- To become the PiT when underlying data for this component was last updated, i.e. , max of all @DUnderlLast
	DECLARE @iDebugLevel int = 0
	SELECT @iDebugLevel = iDebugLevel FROM DEVDEBUGCONTROLS WHERE sDebugThread = N'EqOperStatus'
	IF @iDebugLevel < 1
	BEGIN
		SET @sDebug = N''
	END

	INSERT INTO @tblCompCumDat (iLine, iComponId, sError, DMaxUnderlLast, dAccumHours, iAccumCycles, dAccumDist, sDebug, sActionItem, sDeadline, cPrevailSchedMeth, iPercentComplete)
		VALUES (0, @iComponId, @sError, @DMaxUnderlLast, 0.0, 0, 0.0, @sDebug, N'[undefined]', N'[undefined]', N'N', 0)

	-- Define:
	--	H1 data = Associated with completion of the last Action Item in table EQUIPACTIONITEMS (equipment component specific). Does not cascade down to child components.
	--  H2R data = 'Reset' data in table EQUIPOPERDATA (equipment component specific). Does not cascade down to child components.
	--	H2M data = 'Manually Entered' data in table EQUIPOPERDATA (equipment component specific, cascades down to child components)
	--	H3 data = Flight ops data in table OPERATIONS (only specific to an entire piece of equipment; cascades down to child components)

	-- H1 and H2R provide absolute references, i.e., the total number of hours/cycles/distance of the equipment component up to a point in time (PiT)
	-- H2M and H3 provide incremental data that is to be added to H1 or H2R to track the total ops data up to the PiT of H2M or H3 data.
	--
	DECLARE @iRowCount int
	DECLARE @iUsedElapsed tinyint, @iUsedHours tinyint, @iUsedCycles tinyint, @iUsedDist tinyint, @iUsedUnique tinyint
	DECLARE @DeadLine datetimeoffset(0)
	DECLARE @dHrs decimal(12,4)
	DECLARE @iCycles int
	DECLARE @dDist decimal(12,2)
	DECLARE @DUpd datetime2(3) = '0001-01-01 00:00:00'
	DECLARE @DThresholdBegin datetimeoffset(0) = '1900/01/01 01:01:00 +00:00'
	DECLARE @dAccumHours decimal(12,4) = 0.0
	DECLARE @iAccumCycles int = 0
	DECLARE @dAccumDist decimal(12,2) = 0.0
	DECLARE @cPrevailSchedMeth nchar(1) = N'N'
	DECLARE @iLineCount int = 10000
	DECLARE @sAbsRefSource nchar(20) = N'EQUIPCOMPONENTS     ' -- Which table provided the value for @DAbsRef?

	-- *************
	-- SECTION - A - Determine the latest absolute reference time DAbsRef, and acquire H1 and H2R data
	-- *************
		DECLARE @DAbsRef datetimeoffset(0) = '0001-01-01 00:00:00 -00:00'
		DECLARE @DT datetimeoffset(0);
		IF @iDebugLevel > 5 SET @sDebug = @sDebug + @sNewLine + REPLICATE(N' ', 4) + N'SECTION - A'
		-- -------------
		-- Section - A.1 - Get DLinkBegin to be ready to possibly using it for DAbsRef
		-- -------------
		IF @iDebugLevel > 5
		BEGIN
			SET @sDebug = @sDebug + @sNewLine + REPLICATE(N' ', 8) + N'Section - A.1'
			UPDATE @tblCompCumDat SET sDebug = @sDebug WHERE iLine = 0
		END
		-- Find the DLinkBegin for this component possibly taking into account the parents'/upline's DLinkBegins
			;WITH CTE_DLinkBegin1 (DLinkBegin, iParentComponent, PiTRecordEntered) AS
			(
				( -- Anchor
					SELECT DLinkBegin, iParentComponent, PiTRecordEntered
						FROM EQUIPCOMPONENTS
						WHERE ID = @iComponId
				)
				UNION ALL
				( -- Recursive part
					SELECT This.DLinkBegin, This.iParentComponent, This.PiTRecordEntered
						FROM EQUIPCOMPONENTS as This
						INNER JOIN CTE_DLinkBegin1 AS Prior ON Prior.iParentComponent = This.ID
						WHERE Prior.DLinkBegin < @DThresholdBegin
				)
			)
			SELECT @iRowCount = COUNT(*) FROM CTE_DLinkBegin1 WHERE DLinkBegin = (SELECT MAX(DLinkBegin) FROM CTE_DLinkBegin1)
			IF @iRowCount < 1
			BEGIN
				SET @sError = N'For equipment component with ID = ' + CONVERT(nvarchar(34), @iComponId) + N' no component could be found in the child/parent list with a valid DLinkBegin.'
				UPDATE @tblCompCumDat SET sError = @sError WHERE iLine = 0
				return
			END
			IF @iRowCount > 1
			BEGIN
				SET @sError = N'For equipment component with ID = ' + CONVERT(nvarchar(34), @iComponId) + N' more than one component was found in the child/parent list with a valid DLinkBegin.'
				UPDATE @tblCompCumDat SET sError = @sError WHERE iLine = 0
				return
			END;
			WITH CTE_DLinkBegin2 (DLinkBegin, iParentComponent, PiTRecordEntered) AS
			(
				( -- Anchor
					SELECT DLinkBegin, iParentComponent, PiTRecordEntered
						FROM EQUIPCOMPONENTS
						WHERE ID = @iComponId
				)
				UNION ALL
				( -- Recursive part
					SELECT This.DLinkBegin, This.iParentComponent, This.PiTRecordEntered
						FROM EQUIPCOMPONENTS as This
						INNER JOIN CTE_DLinkBegin2 AS Prior ON Prior.iParentComponent = This.ID
						WHERE Prior.DLinkBegin < @DThresholdBegin
				)
			)
			SELECT @DUnderlLast = PiTRecordEntered, @DAbsRef = DLinkBegin FROM CTE_DLinkBegin2
					WHERE DLinkBegin = (SELECT MAX(DLinkBegin) FROM CTE_DLinkBegin2)
			IF @DUnderlLast > @DMaxUnderlLast SET @DMaxUnderlLast = @DUnderlLast
			IF @iDebugLevel > 5
			BEGIN
				SET @sDebug = @sDebug + @sNewLine + REPLICATE(N' ', 12) + N'after CTE_DLinkBegin2 - DAbsRef = ' + CONVERT(nvarchar(30),@DAbsRef) + N', @sAbsRefSource=' + @sAbsRefSource + 
					N', @DUnderlLast=' + CONVERT(nvarchar(34),@DUnderlLast) + N', @DMaxUnderlLast=' + CONVERT(nvarchar(34),@DMaxUnderlLast)
			END
			-- At this point, there is only one record in @tblCompCumDat with iLine = 0; let's set it to 1:
			UPDATE @tblCompCumDat SET iLine = 1, sDebug = @sDebug, DMaxUnderlLast = @DMaxUnderlLast
			-- Table EQUIPCOMPONENTS has no operating data; therefore, @dAccumHours / @iAccumCycles / @dAccumDist stay at 0.

		-- -------------
		-- Section - A.2 - Acquire H1 data (be ready to possibly use it for DAbsRef), and prepare to report on the status of each action item that goes with this equipment component
		-- -------------
		-- Remember that:
		--			## An equipment components (table EQUIPCOMPONENTS) can have any number of Aging Items (table EQUIPAGINGITEMS) associated with it.
		--			## An Aging Item can have any number of Action Items (table EQUIPACTIONITEMS) associated with it.
		IF @iDebugLevel > 5
		BEGIN
			SET @sDebug = @sDebug + @sNewLine + REPLICATE(N' ', 8) + N'Section - A.2'
			UPDATE @tblCompCumDat SET sDebug = @sDebug WHERE iLine = 1
		END
			-- Loop over Aging Items for component with ID @iComponId
			DECLARE @idAgingItem int
			DECLARE @sActionItem nvarchar(300)

			DECLARE AgingItems CURSOR FORWARD_ONLY READ_ONLY
				FOR SELECT G.ID, G.sName + N' - ' + P.sShortDescript AS sActionItem
				FROM EQUIPAGINGITEMS AS G
					INNER JOIN EQUIPAGINGPARS AS P ON P.ID = G.iParam
				WHERE iEquipComponent = @iComponId
			OPEN AgingItems
			FETCH NEXT FROM AgingItems INTO @idAgingItem, @sActionItem
			WHILE @@FETCH_STATUS = 0
			BEGIN
				IF @iDebugLevel > 5
				BEGIN
					SET @sDebug = @sDebug + @sNewLine + REPLICATE(N' ', 12) + N'@idAgingItem = ' + CONVERT(nvarchar(12), @idAgingItem)
				END
				UPDATE @tblCompCumDat SET sDebug = @sDebug, sActionItem = @sActionItem WHERE iLine = 1

				-- ...............
				-- Section - A.2.a - Latest Completed Action Item
				-- ...............
				IF @iDebugLevel > 5
				BEGIN
					SET @sDebug = @sDebug + @sNewLine + REPLICATE(N' ', 12) + N'Section - A.2.a'
				END
					-- IF iPercentComplete is 100: The latest of these becomes DAbsRef, and that sets H1 data. Store into line 1 of table @tblCompCumDat.
					--		Earlier completed action items are ignored here, but we keep them for historical documentation purposes.
						DECLARE @iActionItem int = -99999
						SELECT @iActionItem = ID, @DT = DComplete, @dHrs = dAtCompletionHrs, @iCycles = iAtCompletionCycles, @dDist = dAtCompletionDist, @DUpd = PiTRecordEntered, @cPrevailSchedMeth = cPrevailSchedMeth,
								@DeadLine = DeadLine, @iUsedElapsed = iUsedElapsed, @iUsedHours = iUsedHours, @iUsedCycles = iUsedCycles, @iUsedDist = iUsedDist, @iUsedUnique = iUsedUnique
							FROM EQUIPACTIONITEMS
							WHERE iEquipAgingItem = @idAgingItem AND iPercentComplete = 100
									AND DComplete = (SELECT MAX(DComplete) FROM EQUIPACTIONITEMS WHERE iEquipAgingItem = @idAgingItem AND iPercentComplete = 100)
						SET @iRowCount = @@ROWCOUNT -- This is a necessary step; if you leave it out and directly use  IF @@RouwCount > 0  it does not work properly.
						IF @iDebugLevel > 5
						BEGIN
							SET @sDebug = @sDebug + N', @iActionItem = ' + CONVERT(nvarchar(12), @iActionItem) + N', @iRowCount = ' + CONVERT(nvarchar(30),@iRowCount) + N', @cPrevailSchedMeth=' + @cPrevailSchedMeth; UPDATE @tblCompCumDat SET sDebug = @sDebug WHERE iLine = 1
							UPDATE @tblCompCumDat SET sDebug = @sDebug WHERE iLine = 1
						END
						IF @iRowCount > 1
						BEGIN
							SET @sError = N'For equipment component with ID = ' + CONVERT(nvarchar(34), @iComponId)
								+ N' more than one action item was found with iPercentComplete=100 and completion date ' + CONVERT(nvarchar(34), @DT)
								+ N'. The equipment manager must make sure those completion dates are at least 1 second different from each other.'
							UPDATE @tblCompCumDat SET sError = @sError WHERE iLine = 1
							return
						END
						IF @iRowCount > 0
						BEGIN
						-- Find the latest action item completion among all aging items for this equipment component
							IF @DT > @DAbsRef
							BEGIN
								SET @DAbsRef = @DT
								SET @sAbsRefSource = N'EQUIPACTIONITEMS    '
								SET @dAccumHours = @dHrs
								SET @iAccumCycles = @iCycles
								SET @dAccumDist = @dDist
								SET @DUnderlLast = @DUpd;
								IF @DUnderlLast > @DMaxUnderlLast SET @DMaxUnderlLast = @DUnderlLast
								IF @iDebugLevel > 5
								BEGIN
									SET @sDebug = @sDebug + @sNewLine + REPLICATE(N' ', 16) + N'DAbsRef = ' + CONVERT(nvarchar(30),@DAbsRef) + N', @sAbsRefSource=' + @sAbsRefSource
										+ N',  @dAccumHours = ' + CONVERT(nvarchar(30), @dAccumHours)+ N', @iAccumCycles = ' + CONVERT(nvarchar(30), @iAccumCycles)
										+ N', @dAccumDist = ' + CONVERT(nvarchar(30), @dAccumDist) + N', @DUnderlLast  = ' + CONVERT(nvarchar(34), @DUnderlLast)
										+ N', @DMaxUnderlLast = ' + CONVERT(nvarchar(34), @DMaxUnderlLast)
								END
								UPDATE @tblCompCumDat SET sDebug = @sDebug, DMaxUnderlLast = @DMaxUnderlLast,
									dAccumHours = @dAccumHours, iAccumCycles = @iAccumCycles, dAccumDist = @dAccumDist, iPercentComplete=100,
									sDeadline = CONVERT(nvarchar(34), @DeadLine), cPrevailSchedMeth = @cPrevailSchedMeth WHERE iLine = 1
								DECLARE @iRowCount2 int
								SELECT @iRowCount2 = COUNT(*) FROM @tblCompCumDat WHERE iLine = 10001
								IF @iRowCount2 < 1
								BEGIN
									INSERT INTO @tblCompCumDat (iLine, iComponId, sError, DMaxUnderlLast, dAccumHours, iAccumCycles, dAccumDist, sDebug, sActionItem, sDeadline, cPrevailSchedMeth, iPercentComplete)
										VALUES (10001, @iComponId, @sError, @DMaxUnderlLast, @dAccumHours, @iAccumCycles, @dAccumDist, N'<__', @sActionItem, CONVERT(nvarchar(34), @DeadLine), N'N', 100)
								END
								ELSE
								BEGIN
									UPDATE @tblCompCumDat SET sDebug = @sDebug, DMaxUnderlLast = @DMaxUnderlLast,
										dAccumHours = @dAccumHours, iAccumCycles = @iAccumCycles, dAccumDist = @dAccumDist, iPercentComplete=100,
										sDeadline = CONVERT(nvarchar(34), @DeadLine), cPrevailSchedMeth = @cPrevailSchedMeth WHERE iLine = 10001
								END
							END
						END
						ELSE
						BEGIN
							UPDATE @tblCompCumDat SET sDebug = @sDebug, sActionItem=N'[No completed Action Items found]', iPercentComplete=-1, sDeadline = N' ', cPrevailSchedMeth = N' ' WHERE iLine = 1
						END

				-- ...............
				-- Section - A.2.b - Incomplete Action Items (AIs)
				-- ...............
				IF @iDebugLevel > 5
				BEGIN
					SET @sDebug = @sDebug + @sNewLine + REPLICATE(N' ', 12) + N'Section - A.2.b'
				END
					-- Incomplete action items have iPercentComplete < 100.
					-- For one aging item there can be at most 1 incomplete AI with iPercentComplete = 0, and at most one AI with 1 <= iPercentComplete <= 99.
					-- We find them so we can report on upcoming (or missed) deadlines.
						DECLARE @dDeadLineHrs decimal(12,4)
						DECLARE @iDeadLineCycles int
						DECLARE @dDeadLineDist decimal(12,2)
						DECLARE @sDeadLine nvarchar(128) = N'-- UNDEFINED --'
						DECLARE @iPercentComplete int
						DECLARE @sExpl nvarchar(20) = N'undef'
						DECLARE @iRecCount int = 0
						DECLARE @iIncompleteCount int = 1
						WHILE @iIncompleteCount < 3
						BEGIN
							IF @iIncompleteCount = 1
							BEGIN
								SET @iLineCount = 20000
								SELECT @iRecCount = Count(*)
									FROM EQUIPACTIONITEMS AS AC
									WHERE iEquipAgingItem = @idAgingItem AND iPercentComplete > 0 AND iPercentComplete < 100
								SET @sExpl = N'between 1 and 99'
							END
							IF @iIncompleteCount = 2
							BEGIN
								SET @iLineCount = 30000
								SELECT @iRecCount = Count(*)
									FROM EQUIPACTIONITEMS AS AC
									WHERE iEquipAgingItem = @idAgingItem AND iPercentComplete < 1
								SET @sExpl = N'= 0'
							END
							IF @iDebugLevel > 5
							BEGIN
								SET @sDebug = @sDebug + N' @iIncompleteCount = ' + CONVERT(nvarchar(12), @iIncompleteCount) + N', @iRecCount = ' + CONVERT(nvarchar(12), @iRecCount)
								UPDATE @tblCompCumDat SET sDebug = @sDebug WHERE iLine = 1
							END
							IF @iRecCount > 1
							BEGIN
								SET @sError = N'For equipment component with ID = ' + CONVERT(nvarchar(34), @iComponId)
									+ N', Aging Item ID = ' + CONVERT(nvarchar(12), @idAgingItem)
									+ N', found more than 1 Action Item (' + CONVERT(nvarchar(12), @iRecCount) + N') with % Complete ' + @sExpl + N'.'
								UPDATE @tblCompCumDat SET sError = @sError WHERE iLine = 1
								return
							END
							IF @iRecCount > 0
							BEGIN
								SELECT @iActionItem=AC.ID, @DeadLine=AC.DeadLine, @dDeadLineHrs=AC.dDeadLineHrs, @iDeadLineCycles=AC.iDeadLineCycles, @dDeadLineDist=AC.dDeadLineDist,
										@iPercentComplete=AC.iPercentComplete, @iUsedElapsed=AC.iUsedElapsed, @iUsedHours=AC.iUsedHours, @iUsedCycles=AC.iUsedCycles,
										@iUsedDist=AC.iUsedDist, @iUsedUnique=AC.iUsedUnique, @cPrevailSchedMeth=AC.cPrevailSchedMeth
									FROM EQUIPACTIONITEMS AS AC
										INNER JOIN EQUIPAGINGITEMS AS G ON G.ID = AC.iEquipAgingItem
										INNER JOIN EQUIPAGINGPARS AS P ON P.ID = G.iParam
									WHERE iEquipAgingItem = @idAgingItem AND iPercentComplete < 100
								IF (@iUsedElapsed + @iUsedHours + @iUsedCycles + @iUsedDist + @iUsedUnique) < 1
								BEGIN
									SET @sError = N'For equipment component with ID = ' + CONVERT(nvarchar(34), @iComponId)
										+ N', Aging Item ID = ' + CONVERT(nvarchar(12), @idAgingItem)
										+ N', Action Item with ID = ' + CONVERT(nvarchar(12), @iActionItem) + N' does not use any scheduling method, but must use at least 1 of 5.'
									UPDATE @tblCompCumDat SET sError = @sError WHERE iLine = 1
									return
								END
								DECLARE @iSchedMeth int = 0 -- Which scheduling method
								DECLARE @bInsert bit = 0
								WHILE @iSchedMeth < 6
								BEGIN
									SET @bInsert = 0
									SET @iSchedMeth = @iSchedMeth + 1
									SET @iLineCount = @iLineCount + 1
									SET @sDeadLine = N'@iSchedMeth ' + CONVERT(nvarchar(12), @iSchedMeth) + N', @iLineCount = ' + CONVERT(nvarchar(12), @iLineCount) + N', @sDeadLine is UNDEFINED'
									IF (@iSchedMeth = 1 AND @iUsedElapsed > 0) OR (@iSchedMeth = 5 AND @iUsedUnique > 0)
									BEGIN
										SET @bInsert = 1
										SET @sDeadLine = CONVERT(nvarchar(34), @DeadLine)
									END
									ELSE IF @iSchedMeth = 2 AND @iUsedHours > 0
									BEGIN
										SET @bInsert = 1
										SET @sDeadLine = CONVERT(nvarchar(14), @dDeadLineHrs) + N' Hours'
									END
									ELSE IF @iSchedMeth = 3 AND @iUsedCycles > 0
									BEGIN
										SET @bInsert = 1
										SET @sDeadLine = CONVERT(nvarchar(14), @dDeadLineHrs) + N' Cycles'
									END
									ELSE IF @iSchedMeth = 4 AND @iUsedDist > 0
									BEGIN
										SET @bInsert = 1
										SET @sDeadLine = CONVERT(nvarchar(14), @dDeadLineHrs) + N' Miles or Kilometers'
									END
									SET @iPercentComplete = ISNULL(@iPerCentComplete, -999)
									IF @bInsert = 1
									BEGIN
										INSERT INTO @tblCompCumDat (iLine, iComponId, sError, DMaxUnderlLast, dAccumHours, iAccumCycles, dAccumDist, sDebug, sActionItem, sDeadline, cPrevailSchedMeth, iPercentComplete)
											VALUES (@iLineCount, @iComponId, N'OK', '0001/01/01', 0.0, 0, 0.0, N'#<<' , @sActionItem, @sDeadLine, @cPrevailSchedMeth, @iPercentComplete)
									END
									IF @iDebugLevel > 5
									BEGIN
										SET @sDebug = @sDebug + @sNewLine + REPLICATE(N' ', 16) + N'@iSchedMeth=' + CONVERT(nvarchar(12), @iSchedMeth) + N', @iLineCount=' + CONVERT(nvarchar(12), @iLineCount)
											+ N', @sDeadLine = ' + @sDeadLine
									END
								END -- WHILE @iSchedMeth < 6
							END -- IF @iRecCount > 0
							SET @iIncompleteCount = @iIncompleteCount + 1
						END -- WHILE @iIncompleteCount < 3
						IF @iDebugLevel > 5
						BEGIN
							UPDATE @tblCompCumDat SET sDebug = @sDebug WHERE iLine = 1
						END

					FETCH NEXT FROM AgingItems INTO @idAgingItem, @sActionItem
				END -- WHILE @@FETCH_STATUS = 0
				CLOSE AgingItems;
				DEALLOCATE AgingItems;
			IF @iDebugLevel > 5
			BEGIN
				SET @sDebug = @sDebug + @sNewLine + REPLICATE(N' ', 12) + N'after H1 - DAbsRef = ' + CONVERT(nvarchar(30),@DAbsRef) + N', @sAbsRefSource=' + @sAbsRefSource +
					N', @DUnderlLast=' + CONVERT(nvarchar(34),@DUnderlLast) + N', @DMaxUnderlLast=' + CONVERT(nvarchar(34),@DMaxUnderlLast)
				UPDATE @tblCompCumDat SET sDebug = @sDebug WHERE iLine = 1
			END

		-- -------------
		-- Section - A.3 - Acquire H2R data, and possibly use it for DAbsRef
		-- -------------
		IF @iDebugLevel > 5
		BEGIN
			SET @sDebug = @sDebug + @sNewLine + REPLICATE(N' ', 8) + N'Section - A.3 - H2R'
			UPDATE @tblCompCumDat SET sDebug = @sDebug WHERE iLine = 1
		END
		-- When did the latest H2R data occur in table EQUIPOPERDATA for this equipment component?
			SELECT @DT = DFrom, @dHrs = dHours, @iCycles = iCycles, @dDist = dDistance, @DUpd = PiTRecordEntered
				FROM EQUIPOPERDATA
				WHERE iEquipComponent = @iComponId AND cSource = 'R'
							AND DFrom = (SELECT MAX(Dfrom) FROM EQUIPOPERDATA WHERE iEquipComponent = @iComponId AND cSource = 'R')
			SET @iRowCount = @@ROWCOUNT
			IF @iDebugLevel > 5
			BEGIN
				SET @sDebug = @sDebug + N' @iRowCount = ' + CONVERT(nvarchar(30),@iRowCount); UPDATE @tblCompCumDat SET sDebug = @sDebug WHERE iLine = 1
			END
			IF @iRowCount > 0
			BEGIN
				IF @DT >= @DAbsRef
				BEGIN
					SET @DAbsRef = @DT
					SET @sAbsRefSource = N'EQUIPOPERDATA       '
					SET @dAccumHours = @dHrs
					SET @iAccumCycles = @iCycles
					SET @dAccumDist = @dDist
					SET @DUnderlLast = @DUpd
					IF @DUnderlLast > @DMaxUnderlLast SET @DMaxUnderlLast = @DUnderlLast
					IF @iDebugLevel > 5
					BEGIN
						SET @sDebug = @sDebug + @sNewLine + REPLICATE(N' ', 12) + N'@dAccumHours = ' + CONVERT(nvarchar(30),@dAccumHours) +
							N', @iAccumCycles = ' + CONVERT(nvarchar(34), @iAccumCycles) + N', @dAccumDist = ' + CONVERT(nvarchar(34), @dAccumDist) +
							N', @DUnderlLast=' + CONVERT(nvarchar(34),@DUnderlLast) + N', @DMaxUnderlLast=' + CONVERT(nvarchar(34),@DMaxUnderlLast)
					END
				END
			END
			IF @iDebugLevel > 5
			BEGIN
				SET @sDebug = @sDebug + @sNewLine + REPLICATE(N' ', 12) + N'after H2R - DAbsRef = ' + CONVERT(nvarchar(34), @DAbsRef) + N', @sAbsRefSource=' + @sAbsRefSource +
					N', @DUnderlLast=' + CONVERT(nvarchar(34),@DUnderlLast) + N', @DMaxUnderlLast=' + CONVERT(nvarchar(34),@DMaxUnderlLast)
					+ @sNewLine + N'_' + REPLICATE(N' ', 15) + N'@dAccumHours = ' + CONVERT(nvarchar(30),@dAccumHours) +
					N', @iAccumCycles = ' + CONVERT(nvarchar(34), @iAccumCycles) + N', @dAccumDist = ' + CONVERT(nvarchar(34), @dAccumDist)
			END
			UPDATE @tblCompCumDat SET sDebug = @sDebug, DMaxUnderlLast = @DMaxUnderlLast, dAccumHours = @dAccumHours, iAccumCycles = @iAccumCycles, dAccumDist = @dAccumDist WHERE iLine = 1

	-- *************
	-- SECTION - B - Acquire H2M data
	-- *************
		IF @iDebugLevel > 5
		BEGIN
			SET @sDebug = @sDebug + @sNewLine + REPLICATE(N' ', 4) + N'SECTION - B - H2M'
			UPDATE @tblCompCumDat SET sDebug = @sDebug WHERE iLine = 1
		END
		-- Look for H2M data for this equipment component after DAbsRef, and before the current time in case somebody entered future data
			DECLARE @DNow datetimeoffset(0) = SYSDATETIMEOFFSET()
			DECLARE @DFrom datetimeoffset(0)
			DECLARE @DTo datetimeoffset(0)
			DECLARE @TCIds TABLE (IDcomponent int, iEquipment int)
			DECLARE @iEquipmentComponent int
			DECLARE @iID_EquipOperData int
			DECLARE @iEquipment int -- ID in Table EQUIPMENT
			DECLARE @iCId int;
			-- Get H2M and H3 data for this component and all its upline (parents/grandparents/...)
			WITH CTE_H2M (IDcomponent, iParentComponent, iEquipment) AS
			(
				( -- Anchor
					SELECT C.ID AS IDcomponent, C.iParentComponent, C.iEquipment
						FROM EQUIPCOMPONENTS C 
						WHERE C.ID = @iComponId
				)
				UNION ALL
				( -- Recursive part
					SELECT This.ID AS IDcomponent, This.iParentComponent, This.iEquipment
						FROM EQUIPCOMPONENTS AS This
						INNER JOIN CTE_H2M AS Prior ON Prior.iParentComponent = This.ID -- This must say 'This.ID'; cannot say 'IDComponent'
				)
			)
			INSERT INTO @TCIds(IDcomponent, iEquipment) SELECT IDcomponent, iEquipment FROM CTE_H2M -- Table of IDs of components that may contribute to H2M data of component with ID @iComponId
			SELECT @iRowCount = count(*) FROM @TCIds
			IF @iDebugLevel > 5
			BEGIN
				SET @sDebug = @sDebug + @sNewLine + REPLICATE(N' ', 8) + N'H2M @iComponId = ' + CONVERT(nvarchar(30),@iComponId) + N', @iRowCount = ' + CONVERT(nvarchar(30),@iRowCount) + N' component(s) to examine'
				UPDATE @tblCompCumDat SET sDebug = @sDebug WHERE iLine = 1
			END
			DECLARE cursorH2M CURSOR FORWARD_ONLY READ_ONLY
				FOR SELECT T.IDcomponent, T.iEquipment, O.ID, O.dHours, O.iCycles, O.dDistance, O.DFrom, O.DTo, O.PiTRecordEntered FROM @TCIds T
					INNER JOIN EQUIPOPERDATA O ON O.iEquipComponent = T.IDcomponent
					WHERE O.cSource = N'M' AND (O.DTo >= @DAbsRef AND O.DFrom <= @DNow)
			OPEN cursorH2M
			FETCH NEXT FROM cursorH2M INTO @iEquipmentComponent, @iEquipment, @iID_EquipOperData, @dHrs, @iCycles, @dDist, @DFrom, @DTo, @DUnderlLast
			WHILE @@FETCH_STATUS = 0
			BEGIN
				-- H2M data from table EQUIPOPERDATA
					IF @iDebugLevel > 5
					BEGIN
						SET @sDebug = @sDebug +  @sNewLine + REPLICATE(N' ', 12) + N'cursorH2M: @iEquipmentComponent=' + CONVERT(nvarchar(30),@iEquipmentComponent) +
								', @iID_EquipOperData=' + CONVERT(nvarchar(30), @iID_EquipOperData) + ', @dHrs=' + CONVERT(nvarchar(30),@dHrs) +
								', @iCycles=' + CONVERT(nvarchar(30),@iCycles) + ', @dDist=' + CONVERT(nvarchar(30),@dDist)
						UPDATE @tblCompCumDat SET sDebug = @sDebug WHERE iLine = 1
					END
					SET @dHrs = ISNULL(@dHrs, -1999.99)
					IF @dHrs < 0.0
					BEGIN
						SET @sError = N'H2M @dHrs is null for @iEquipmentComponent = ' + CONVERT(nvarchar(12), @iEquipmentComponent) + N', @iEquipment = ' + CONVERT(nvarchar(12), @iEquipment)
						UPDATE @tblCompCumDat SET sError = @sError WHERE iLine = 1
						return
					END
					SET @iCycles = ISNULL(@iCycles, -1999)
					IF @iCycles < 0
					BEGIN
						SET @sError = N'H2M @iCycles is null for @iEquipmentComponent = ' + CONVERT(nvarchar(12), @iEquipmentComponent) + N', @iEquipment = ' + CONVERT(nvarchar(12), @iEquipment)
						UPDATE @tblCompCumDat SET sError = @sError WHERE iLine = 1
						return
					END
					SET @dDist = ISNULL(@dDist, -1999.99)
					IF @dDist < 0.0
					BEGIN
						SET @sError = N'H2M @dDist is null for @iEquipmentComponent = ' + CONVERT(nvarchar(12), @iEquipmentComponent) + N', @iEquipment = ' + CONVERT(nvarchar(12), @iEquipment)
						UPDATE @tblCompCumDat SET sError = @sError WHERE iLine = 1
						return
					END
				-- We make adjustments to the operating data if the time interval DFrom - DTo overlaps
					--    one or both of the boundaries @DAbsRef and @Dnow, assuming that the given data
					--    are evenly distributed over Dfrom - DTo so that the adjustments are simply linear.
					DECLARE @fDenom FLOAT = CONVERT(FLOAT, DATEDIFF(minute,@DFrom,@Dto)) -- length of interval DFrom - DTo used in denominators below
					IF @fDenom < 1.0
					BEGIN
						SET @sError = N'H2M @fDenom is < 1.0 minute for @iEquipmentComponent = ' + CONVERT(nvarchar(12), @iEquipmentComponent) + N', @iEquipment = ' + CONVERT(nvarchar(12), @iEquipment)
						UPDATE @tblCompCumDat SET sError = @sError WHERE iLine = 1
						return
					END
					DECLARE @sH2MAdjust nvarchar(MAX) = N''
					-- Five cases:
					-- Cases 1 and 2
						-- IF (@DFrom = @DTo) OR (@DFrom >= @DAbsRef AND @DTo <= @DNow) ... no adjustment of dHours / iCycles / dDist required
					-- Case 3
					DECLARE @fAdjFactor FLOAT = 1.0
					IF (@DFrom < @DAbsRef) AND (@DTo <= @DNow)
					BEGIN
						SET @fAdjFactor = CONVERT(FLOAT, DATEDIFF(minute, @DAbsRef, @DTo)) / @fDenom
						SET @sH2MAdjust = N'Case 3:'
					END
					-- Case 4
					ELSE IF @DFrom >= @DAbsRef AND @DTo > @DNow
					BEGIN
						SET @fAdjFactor = CONVERT(FLOAT, DATEDIFF(minute, @DFrom, @DNow)) / @fDenom
						SET @sH2MAdjust = N'Case 4:'
					END
					-- Case 5
					ELSE IF @DFrom < @DAbsRef AND @DTo > @DNow
					BEGIN
						SET @fAdjFactor = CONVERT(FLOAT, DATEDIFF(minute, @DAbsRef, @DNow)) / @fDenom
						SET @sH2MAdjust = N'Case 5:'
					END
					IF LEN(@sH2MAdjust) > 0
					BEGIN
						SET @sH2MAdjust = @sH2MAdjust + N' @fAdjFactor=' + CONVERT(nvarchar(34), @fAdjFactor) + N', DFrom=' + CONVERT(nvarchar(34), @DFrom) +
								N', DTo=' + CONVERT(nvarchar(34),@DTo) + N', @fDenom=' + CONVERT(nvarchar(34), @fDenom)
						IF @iDebugLevel > 5
						BEGIN
							SET @sDebug = @sDebug + @sNewLine + REPLICATE(N' ', 16) + N'H2M Adjustment: ' + @sH2MAdjust
						END
					END
					SET @dAccumHours = @dAccumHours + @dHrs * @fAdjFactor
					SET @iAccumCycles = @iAccumCycles + @iCycles * @fAdjFactor
					SET @dAccumDist = @dAccumDist + @dDist * @fAdjFactor
					IF @DUnderlLast > @DMaxUnderlLast SET @DMaxUnderlLast = @DUnderlLast
					IF @iDebugLevel > 5
					BEGIN
						SET @sDebug = @sDebug + @sNewLine + REPLICATE(N' ', 16) +
							N'@DUnderlLast=' + CONVERT(nvarchar(34),@DUnderlLast) + N', @DMaxUnderlLast=' + CONVERT(nvarchar(34),@DMaxUnderlLast)
							+ N', @dAccumHours = ' + CONVERT(nvarchar(30),@dAccumHours) +
							N', @iAccumCycles = ' + CONVERT(nvarchar(34), @iAccumCycles) + N', @dAccumDist = ' + CONVERT(nvarchar(34), @dAccumDist)
					END
					UPDATE @tblCompCumDat SET sDebug = @sDebug, dAccumHours = @dAccumHours, iAccumCycles = @iAccumCycles, dAccumDist = @dAccumDist, DMaxUnderlLast = @DMaxUnderlLast WHERE iLine = 1
				FETCH NEXT FROM cursorH2M INTO @iEquipmentComponent, @iEquipment, @iID_EquipOperData, @dHrs, @iCycles, @dDist, @DFrom, @DTo, @DUnderlLast
			END	--WHILE @@FETCH_STATUS = 0
			CLOSE cursorH2M;
			DEALLOCATE cursorH2M;

	-- *************
	-- SECTION - C - Acquire H3 data
	-- *************
		IF @iDebugLevel > 5
		BEGIN
			SET @sDebug = @sDebug + @sNewLine + REPLICATE(N' ', 4) + N'SECTION - C - H3'
			UPDATE @tblCompCumDat SET sDebug = @sDebug WHERE iLine = 1
		END
		-- H3 data from table OPERATIONS: hours and cycles of operations only; H3 data does not have data for distance traveled
			-- There is no need to thread through the hierarchy of components because table EQUIPCOMPONENTS contains field iEquipment
			--   which points to a row in table EQUIPMENT to which this component belongs. For determining the sum of flight times
			--   the depth in the hierarchy of this component does not matter.
			SELECT @iEquipment = C.iEquipment FROM EQUIPCOMPONENTS C WHERE C.ID = @iComponId
			SELECT @dHrs = SUM(DATEDIFF(minute,P.DBegin,P.DEnd)) / 60.0, @iCycles = COUNT(*), @DUnderlLast = MAX(P.PiTRecordEntered)
				FROM EQUIPMENT Q
					INNER JOIN EQUIPCOMPONENTS C ON C.iEquipment = Q.ID
					INNER JOIN OPDETAILS D ON D.iEquip = Q.ID
					INNER JOIN OPERATIONS P ON P.ID = D.iOperation
					WHERE C.ID = @iComponId AND (P.DBegin >= @DAbsRef AND P.DBegin <= @DNow) --Any operations possibly entered for a future date are not included

			SET @dHrs = ISNULL(@dHrs, 0.0)
			SET @iCycles = ISNULL(@iCycles, 0)
			SET @DUnderlLast = ISNULL(@DUnderlLast, '0001-01-01')
			IF @DUnderlLast > @DMaxUnderlLast SET @DMaxUnderlLast = @DUnderlLast

			DECLARE @sH3_Debug nvarchar(MAX) = @sNewLine + REPLICATE(N' ', 8)
			SET @sH3_Debug = @sH3_Debug + N'@dHrs=' + CONVERT(nvarchar(20), @dHrs) + N', @iCycles=' + CONVERT(nvarchar(34), @iCycles) + N', @iEquipment=' + 
				CONVERT(nvarchar(12), @iEquipment) + N', @iComponId=' + CONVERT(nvarchar(12), @iComponId) +
				N', @DUnderlLast=' + CONVERT(nvarchar(34),@DUnderlLast) + N', @DMaxUnderlLast=' + CONVERT(nvarchar(34),@DMaxUnderlLast)

			SET @dAccumHours = @dAccumHours + @dHrs
			SET @iAccumCycles = @iAccumCycles + @iCycles
			IF @iDebugLevel > 5
			BEGIN
				SET @sDebug = @sDebug + @sH3_Debug + @sNewLine + REPLICATE(N' ', 8)
					+ N'@dAccumHours = ' + CONVERT(nvarchar(30),@dAccumHours) + N', @iAccumCycles = ' + CONVERT(nvarchar(34), @iAccumCycles) + N', @dAccumDist = ' + CONVERT(nvarchar(34), @dAccumDist)
			END
			UPDATE @tblCompCumDat SET sDebug = @sDebug, DMaxUnderlLast = @DMaxUnderlLast, dAccumHours = @dAccumHours, iAccumCycles = @iAccumCycles WHERE iLine = 1
	RETURN
END
GO

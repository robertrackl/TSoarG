SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('tf_EqStatList') IS NOT NULL) DROP FUNCTION tf_EqStatList
GO
CREATE FUNCTION tf_EqStatList () -- table-valued function - Equipment Status List
RETURNS 
@tblEqStats TABLE 
(
	iLine int NOT NULL,
	ID_Compon int NOT NULL, sShortEquipName nvarchar(25) NOT NULL, sRegistrationId nvarchar(20) NULL, -- SCR 231
	sCName nvarchar(MAX) NOT NULL, sOperStatus nvarchar(MAX) NULL, sComment nvarchar(MAX) NULL, sSorter nvarchar(1024) NOT NULL,
	sError nvarchar(MAX) NOT NULL, DUnderlLast datetime2(3) NOT NULL, sDebug nvarchar(MAX) NULL, dAccumHours decimal(12,4) NOT NULL,
	iAccumCycles int NOT NULL, dAccumDist decimal(12,2) NOT NULL, sActionItem nvarchar(300) NOT NULL, sDeadline nvarchar(64) NOT NULL,
	cPrevailSchedMeth nchar(1) NOT NULL, iPercentComplete int NOT NULL
)
AS
BEGIN
	DECLARE @idRoot int
	DECLARE @iSubSort int = 1
	-- @tblDownLine receives a list of components in semi-hierarchical order, i.e., all the components that belong to the same hierarchy
	--		starting from one root are grouped together according to the sSorter field.
	DECLARE @tblDownLine TABLE(ID_Compon int, iComponentLevel int, sCName varchar(255), sSorter varchar(255))
	DECLARE Roots CURSOR FORWARD_ONLY READ_ONLY
		FOR SELECT ID FROM EQUIPCOMPONENTS WHERE bEntire = 1 AND bReportOperStatus = 1 ORDER BY sComponent
		-- bEntire=1 also implies that iParentComponent=0
	OPEN Roots
	FETCH NEXT FROM Roots INTO @idRoot
	WHILE @@FETCH_STATUS = 0 -- Loop over all the roots of hierarchy lists; some lists may only be one item long.
	BEGIN
		-- Create the downline set: a hierarchical list of equipment components. The components without a parent (bEntire=1) are the roots of each hierachy list.
			;WITH CTE_DownLine(ID_Compon, iComponentLevel, sCName, sSorter)
				AS
				(
					( -- Anchor: we start at the root component pointed to by @idRoot
						SELECT C.ID, 1, CONVERT(nvarchar(255), sComponent) As sCName, CONVERT(VARCHAR(255), Q.sShortEquipName) AS sSorter
							FROM EQUIPCOMPONENTS AS C
								INNER JOIN EQUIPMENT AS Q ON Q.ID = C.iEquipment
							WHERE C.ID = @idRoot
					)
					UNION ALL
					( -- Recursive Part: we find the children in the hierarchy
						SELECT This.ID, iComponentLevel + 1, CONVERT(nvarchar(255), IIF(iComponentLevel<2,'|-- ','+-- ') + REPLICATE('--- ', iComponentLevel-1) + This.sComponent) AS sCName,
								CONVERT(VARCHAR(255), RTRIM(Prior.sSorter) + ' | ' + This.sComponent) AS sSorter
							FROM EQUIPCOMPONENTS AS This
								INNER JOIN CTE_DownLine AS Prior ON Prior.ID_Compon = This.iParentComponent
					)
				)
			INSERT INTO @tblDownLine (ID_Compon, iComponentLevel, sCName, sSorter)
				SELECT ID_Compon, iComponentLevel, sCName, sSorter FROM CTE_DownLine ORDER BY sSorter

		FETCH NEXT FROM Roots INTO @idRoot
	END
	CLOSE Roots;
	DEALLOCATE Roots;
	-- Iterate over the downline set which contains each component only once, but arranged hierarchically: parents are followed by their children as determined by the sSorter field.
		DECLARE @ID_compon int
		DECLARE @sCName varchar(255)
		DECLARE @sSorter varchar(1024)
		DECLARE Components CURSOR FORWARD_ONLY READ_ONLY
			FOR SELECT ID_Compon, sCName, sSorter FROM @tblDownLine ORDER BY sSorter
		OPEN components
		FETCH NEXT FROM Components INTO @ID_compon, @sCName, @sSorter
		WHILE @@FETCH_STATUS = 0
		BEGIN
			DECLARE @iLine int
			DECLARE @sError nvarchar(MAX)
			DECLARE @DUnderlLast datetime2(3)
			DECLARE @sDebug nvarchar(MAX)
			DECLARE @dAccumHours decimal(12,4)
			DECLARE @iAccumCycles int
			DECLARE @dAccumDist decimal(12,2)
			DECLARE @sActionItem nvarchar(300)
			DECLARE @sDeadline nvarchar(64)
			DECLARE @cPrevailSchedMeth nchar(1)
			DECLARE @iPercentComplete int
			-- Get the cumulative operational data
			DECLARE CumOps CURSOR  FORWARD_ONLY READ_ONLY
				FOR	SELECT iLine, sError, DMaxUnderlLast, sDebug, dAccumHours, iAccumCycles, dAccumDist, sActionItem, sDeadline, cPrevailSchedMeth, iPercentComplete
					FROM dbo.tfEqCumData(@ID_compon)
			OPEN CumOps
			FETCH NEXT FROM CumOps INTO	@iLine, @sError, @DUnderlLast, @sDebug, @dAccumHours, @iAccumCycles, @dAccumDist, @sActionItem, @sDeadline, @cPrevailSchedMeth, @iPercentComplete
			WHILE @@FETCH_STATUS = 0
			BEGIN
				INSERT INTO @tblEqStats (iLine, ID_Compon, sShortEquipName, sRegistrationId, sCName, sOperStatus, sComment, sSorter, sError, DUnderlLast, sDebug, dAccumHours,
						iAccumCycles, dAccumDist, sActionItem, sDeadline, cPrevailSchedMeth, iPercentComplete)
					SELECT @iLine, C.ID, Q.sShortEquipName,Q.sRegistrationId, @sCName, C.sOperStatus, C.sComment, @sSorter, @sError, @DUnderlLast, @sDebug, @dAccumHours,
							@iAccumCycles, @dAccumDist, @sActionItem, @sDeadline, @cPrevailSchedMeth, @iPercentComplete
						FROM EQUIPCOMPONENTS AS C
							INNER JOIN EQUIPMENT AS Q on Q.ID = C.iEquipment
						WHERE C.ID = @ID_Compon
				FETCH NEXT FROM CumOps INTO	@iLine, @sError, @DUnderlLast, @sDebug, @dAccumHours, @iAccumCycles, @dAccumDist, @sActionItem, @sDeadline, @cPrevailSchedMeth, @iPercentComplete
			END
			CLOSE CumOps
			DEALLOCATE CumOps

			FETCH NEXT FROM Components INTO @ID_compon, @sCName, @sSorter
		END
		CLOSE Components;
		DEALLOCATE Components;
	RETURN
END

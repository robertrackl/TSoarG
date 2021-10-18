SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('sf_EqCompList') IS NOT NULL) DROP FUNCTION sf_EqCompList
GO
CREATE FUNCTION sf_EqCompList ()
RETURNS 
@tblEqComps TABLE 
(
	-- SCR 231 (many changes to do with bReportOperStatus, sOperStatus)
	ID int, iEquipment int, sShortEquipName nvarchar(25), sComponent nvarchar(50), sCName nvarchar(MAX), bEntire bit, DLinkBegin datetimeoffset(0), DLinkEnd datetimeoffset(0),
		bReportOperStatus bit, sOperStatus nvarchar(MAX), sComment nvarchar(MAX), iParentComponent int, Sort nvarchar(MAX)
)
AS
BEGIN
	DECLARE @idRoot int
	DECLARE Roots CURSOR FORWARD_ONLY READ_ONLY
		FOR SELECT ID FROM EQUIPCOMPONENTS WHERE bEntire = 1 ORDER BY sComponent
	OPEN Roots
	FETCH NEXT FROM Roots INTO @idRoot
	WHILE @@FETCH_STATUS = 0
	BEGIN
		-- Fill the table variable with the rows of your result set
		WITH CTE_DownLine(ID, iEquipment, sShortEquipName, sComponent, sCName, bEntire, DLinkBegin, DLinkEnd, bReportOperStatus, sOperStatus, sComment, iParentComponent, ComponentLevel, Sort) AS
		(
			 ( -- Anchor
				  SELECT
						C.ID, iEquipment, Q.sShortEquipName, sComponent, CONVERT(varchar(255), sComponent) As sCName, bEntire, DLinkBegin, DLinkEnd,
						bReportOperStatus, sOperStatus, C.sComment, iParentComponent, 1, CONVERT(VARCHAR(255), sComponent) AS Sort
				  FROM
						EQUIPCOMPONENTS AS C
						INNER JOIN EQUIPMENT AS Q ON C.iEquipment = Q.ID
				  WHERE
						C.ID = @idRoot
			 )
			 UNION ALL
			 ( -- Recursive Part
				  SELECT
						This.ID, This.iEquipment, Q.sShortEquipName, This.sComponent, CONVERT(varchar(255), IIF(ComponentLevel<2,'|-- ','+-- ') + REPLICATE('--- ', ComponentLevel-1) + This.sComponent) AS sCName,
							This.bEntire, This.DLinkBegin, This.DLinkEnd, This.bReportOperStatus, This.sOperStatus, This.sComment, This.iParentComponent, 
							ComponentLevel + 1, CONVERT(VARCHAR(255), RTRIM(Sort) + ' | ' + This.sComponent) AS Sort
				  FROM
						EQUIPCOMPONENTS AS This
						INNER JOIN CTE_DownLine AS Prior ON Prior.ID = This.iParentComponent
						INNER JOIN EQUIPMENT AS Q ON This.iEquipment = Q.ID
			 )
		)
		INSERT INTO @tblEqComps (ID, iEquipment, sShortEquipName, sComponent, sCName, bEntire, DLinkBegin, DLinkEnd, bReportOperStatus, sOperStatus, sComment, iParentComponent, Sort)
		SELECT
			 ID, iEquipment, sShortEquipName, sComponent, sCName, bEntire, DLinkBegin, DLinkEnd, bReportOperStatus, sOperStatus, sComment, iParentComponent, Sort
		FROM
			 CTE_DownLine

		FETCH NEXT FROM Roots INTO @idRoot
	END
	CLOSE Roots;
	DEALLOCATE Roots;
	RETURN
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('TEEq_Cleanup') IS NOT NULL) DROP PROCEDURE TEEq_Cleanup
GO
CREATE PROCEDURE TEEq_Cleanup (@sStatus nvarchar(2048) output)
AS
BEGIN
	--BEGIN TRY
	--	SET @sStatus = 'OK' -- Optimistic
	--	DELETE FROM OPSCALNAMES
	--	DELETE FROM EQUIPACTIONITEMS
	--	DELETE FROM EQUIPAGINGITEMS
	--	DELETE FROM EQUIPAGINGPARS
	--	DELETE FROM EQUIPOPERDATA
	--	DELETE FROM EQUIPCOMPONENTS
	--END TRY
	--BEGIN CATCH
	--	SET @sStatus = N'Err Number = ' + CONVERT(nvarchar(12),ERROR_NUMBER()) + N'; Err Severity = ' + CONVERT(nvarchar(12),ERROR_SEVERITY()) + N'; Err State = ' + CONVERT(nvarchar(12),ERROR_STATE()) +
	--		'; Err Msg = ' + ERROR_MESSAGE()
	--END CATCH
	SET @sStatus = 'Database cleanup disabled in production environment'
END
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('TEEq_DataSetup') IS NOT NULL) DROP PROCEDURE TEEq_DataSetup
GO
CREATE PROCEDURE TEEq_DataSetup (@sWhat nvarchar(12), @sStatus nvarchar(2048) output)
AS
BEGIN
	SET @sStatus = 'Database setup disabled in production environment'
	SELECT 1
END
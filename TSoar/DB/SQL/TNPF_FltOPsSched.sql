SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('TNPF_FltOpsSched') IS NOT NULL) DROP FUNCTION TNPF_FltOpsSched
GO
CREATE FUNCTION TNPF_FltOpsSched ()
RETURNS 
@FOS TABLE 
(
	FOSDateID int,
	FOSDate Date,
	FOS_DayOfWeek nvarchar(12),
	sNote nvarchar(50),
	sSignUp01 nvarchar(55),	sRemarks01 nvarchar(256),
	sSignUp02 nvarchar(55),	sRemarks02 nvarchar(256),
	sSignUp03 nvarchar(55),	sRemarks03 nvarchar(256),
	sSignUp04 nvarchar(55),	sRemarks04 nvarchar(256),
	sSignUp05 nvarchar(55),	sRemarks05 nvarchar(256),
	sSignUp06 nvarchar(55),	sRemarks06 nvarchar(256),
	sSignUp07 nvarchar(55),	sRemarks07 nvarchar(256),
	sSignUp08 nvarchar(55),	sRemarks08 nvarchar(256),
	sSignUp09 nvarchar(55),	sRemarks09 nvarchar(256),
	sSignUp10 nvarchar(55),	sRemarks10 nvarchar(256),
	sSignUp11 nvarchar(55),	sRemarks11 nvarchar(256),
	sSignUp12 nvarchar(55),	sRemarks12 nvarchar(256)
)
AS
BEGIN
	DECLARE @DDate date
	DECLARE @bEnabled bit
	DECLARE @sNote nvarchar(MAX)

	DECLARE cursor_Dates CURSOR LOCAL FAST_FORWARD FOR
		SELECT Date, bEnabled, sNote
			FROM FSDATES 
			ORDER BY Date;
	OPEN cursor_Dates

	FETCH NEXT FROM cursor_Dates INTO @DDate, @bEnabled, @sNote
	WHILE @@FETCH_STATUS = 0
	BEGIN
		if @bEnabled = 1
		BEGIN
			
		END
		FETCH NEXT FROM cursor_Dates INTO @DDate, @bEnabled, @sNote
	END

	RETURN
END
GO
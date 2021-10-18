SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('TNPF_LinkBeginEnd') IS NOT NULL) DROP FUNCTION TNPF_LinkBeginEnd
GO
CREATE FUNCTION TNPF_LinkBeginEnd (@iComponent int)
RETURNS @TBeginEnd Table ( DBegin DateTimeOffset(0), DEnd DateTimeOffset(0))
-- Determines values for DLinkBegin and DLinkEnd for a component.
-- If DLinkBegin in EQUIPCOMPONENTS is earlier than 1900/1/1 01:01 +00:00 then the DLinkBegin of the Parent component is used.
-- If DLinkEnd in EQUIPCOMPONENTS is later than 2999/12/31 22:59:00 +00:00 then the DLinkEnd of the parent component is used.
AS
BEGIN
	DECLARE @iComp int = @iComponent
	DECLARE @bDoneLow bit = 0  -- Are we done at the low end?
	DECLARE @bDoneHigh bit = 0 -- Are we done at the high end?
	DECLARE @iParent int
	DECLARE @DLinkBegin    DateTimeOffset(0)
	DECLARE @DLinkEnd      DateTimeOffset(0)
	DECLARE @DMin		   DateTimeOffset(0) = CONVERT(DateTimeOffset(0),'0001/01/01 00:00:00 +00:00')
	DECLARE @DMax		   DateTimeOffset(0) = CONVERT(DateTimeOffset(0),'9999/12/31 23:59:00 +00:00')
	DECLARE @DLinkBeginRes DateTimeOffset(0) = @DMin -- 'Result', initial value (earliest possible)
	DECLARE @DLinkEndRes   DateTimeOffset(0) = @DMax -- 'Result', initial value (latest possible)
	DECLARE @DLow          DateTimeOffset(0) = CONVERT(DateTimeOffset(0),'1900/01/01 01:01:00 +00:00')
	DECLARE @DHigh         DateTimeOffset(0) = CONVERT(DateTimeOffset(0),'2999/12/31 22:59:00 +00:00')
	DECLARE @iLoopCounter int = 0

	WHILE (@bDoneLow = 0) OR (@bDoneHigh = 0)
	-- An infinite loop can happen when:
	--		an 'Entire' component (bEntire = 1) does not have iParentComponent = 0 [but this is prevented by a check constraint on table EQUIPCOMPONENTS];
	--		a hierarchy does not have an 'Entire' component at the root [prevented by integrity check in DBIntegrity.aspx.cs in #region Table EQUIPCOMPONENTS:
	--           'Entire' entries must have 'real' DLInkBegin/End dates; and must refer to an existing piece of equipment];
	--		iParentComponent points to itself
	--		iParentComponent points to one of its children
	BEGIN
		SELECT @DLinkBegin=C.DLinkBegin, @DLinkEnd=C.DLinkEnd, @iParent=C.iParentComponent
			FROM EQUIPCOMPONENTS C
			WHERE C.ID=@iComp
		IF @bDoneLow = 0
		BEGIN
			IF @DLinkBegin >= @DLow
			BEGIN
				SET @DLinkBeginRes = @DLinkBegin
				SET @bDoneLow = 1
			END
		END
		IF @bDoneHigh = 0
		BEGIN
			IF @DLinkEnd <= @DHigh
			BEGIN
				SET @DLinkEndRes = @DLinkEnd
				SET @bDoneHigh = 1
			END
		END
		IF @bDoneLow = 1 AND @bDoneHigh = 1
		BEGIN
			BREAK -- out of the WHILE loop
		END
		SET @iComp = @iParent -- climb up the component hierarchy one level
		SET @iLoopCounter += 1
		IF @iLoopCounter > 100 -- We assume that an infinite loop is occurring
		BEGIN
			SET @DLinkBeginRes = @DMin -- signal an error
			SET @DLinkEndRes = @DMax -- signal an error
			BREAK
		END
	END
	INSERT INTO @TBeginEnd (DBegin, DEnd) VALUES (@DLinkBeginRes, @DLinkEndRes)
	RETURN
END
GO
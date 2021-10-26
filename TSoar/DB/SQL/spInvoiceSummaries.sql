SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('spInvoiceSummaries') IS NOT NULL) DROP PROCEDURE spInvoiceSummaries
GO
-- Summarize Invoices for all members over a given time interval
CREATE PROCEDURE spInvoiceSummaries (
	@iUser int,
	@DFrom datetimeoffset(0),
	@DTo datetimeoffset(0),
	@suStatus nvarchar(700) output)
AS
BEGIN
BEGIN TRY
	SET @suStatus = 'Unknown';
	DECLARE @iCountInv int = 0 -- Invoices to be counted as having contributed to actual flying charges
	DECLARE @iNotCountInv int = 0 -- Invoices not to be counted because they are duplicates of already existing ones
	-- Loop over all members
	DECLARE @iMember int
	DECLARE @iInvoice int
	DECLARE Members_cursor CURSOR FOR
		SELECT P.ID
			FROM PEOPLE P
				INNER JOIN MEMBERFROMTO M ON P.ID = M.iPerson
			WHERE (M.DMembershipEnd >= @DFrom) AND (M.DMembershipBegin <= @DTo); -- check for time range overlap
	OPEN Members_cursor
	FETCH NEXT FROM Members_cursor INTO @iMember
	WHILE @@FETCH_STATUS = 0
	BEGIN
		-- Loop over the Invoices for one member
		DECLARE @mSum money
		SET @mSum = 0
		DECLARE @mTotalAmt money
		DECLARE @bClosed bit
		DECLARE @DInvoice DateTimeOffset(0)
		DECLARE @sDisplayName nvarchar(55)
		DECLARE @iFlyingCharges int = 0
		DECLARE @iRowCount int
		DECLARE @bUpdate bit = 0
		DECLARE Invoices_cursor CURSOR FOR
			SELECT I.ID, I.mTotalAmt, I.bClosed, I.DInvoice, P.sDisplayName
				FROM INVOICES I 
					INNER JOIN PEOPLE P ON P.ID = I.iPerson
				WHERE (I.iPerson = @iMember) AND (I.DInvoice >= @DFrom) AND (I.DInvoice <= @DTo)
		OPEN Invoices_cursor
		FETCH NEXT FROM Invoices_cursor INTO @iInvoice, @mTotalAmt, @bClosed, @DInvoice, @sDisplayName
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF @bClosed = 0
			BEGIN
				SET @suStatus = 'An open invoice is not allowed during summarization; invoice ID = ' + CONVERT(nvarchar(12), @iInvoice) +
					', member = ' + @sDisplayName + ', invoice date = ' + CONVERT(nvarchar(12),@DInvoice)
			END
			DECLARE @bInsert bit = 1 -- To aid in checking for existing flying charge contributions
			-- Do we need to create a new record in table FLYINGCHARGES?
			SELECT @iFlyingCharges=ID FROM FLYINGCHARGES WHERE iPerson = @iMember AND DateOfAmount = @DTo
			SET @iRowCount = @@ROWCOUNT
			IF @iRowCount > 1
			BEGIN
				SET @suStatus = 'Duplicate record in table FLYINGCHARGES for iPerson = ' + CONVERT(nvarchar(12), @iMember) + ' and DateOfAmount = ' + CONVERT(nvarchar(12),@DTo)
				RETURN
			END
			IF @iRowCount < 1
			BEGIN
				INSERT INTO FLYINGCHARGES (PiTRecordEntered, iRecordEnteredBy, iPerson, mAmount, DateOfAmount, cTypeOfAmount)
					VALUES (GETUTCDATE(), @iUser, @iMember, 0, @DTo, 'A')
				SET @iFlyingCharges = @@IDENTITY
			END
			ELSE -- A flying charges record exists already
			BEGIN
				SELECT ID FROM INVSINFLYCHRGS WHERE iFlyingCharge = @iFlyingCharges AND iInvoice = @iInvoice
				IF @@ROWCOUNT > 0
				BEGIN
					SET @bInsert = 0
					SET @iNotCountInv = @iNotCountInv + 1
				END
			END

			IF @bInsert = 1
			BEGIN
				-- For each invoice, store pointers into table INVINFLYCHRGS, and save today's date there, too
				INSERT INTO INVSINFLYCHRGS (iFlyingCharge, iInvoice, dSummarized) VALUES (@iFlyingCharges, @iInvoice, GETUTCDATE())
				-- Sum the amounts of the Flying Activities Invoices for one member within the given time interval
				SET @mSum = @mSum + @mTotalAmt
				SET @bUpdate = 1
			END

			FETCH NEXT FROM Invoices_cursor INTO @iInvoice, @mTotalAmt, @bClosed, @DInvoice, @sDisplayName
		END
		CLOSE Invoices_cursor
		DEALLOCATE Invoices_cursor

		IF @bUpdate = 1
		BEGIN
			UPDATE FLYINGCHARGES SET mAmount = @mSum WHERE ID = @iFlyingCharges
			SET @iCountInv = @iCountInv + 1
		END
		SET @bUpdate = 0

		FETCH NEXT FROM Members_cursor INTO @iMember
	END
	CLOSE Members_cursor
	DEALLOCATE Members_cursor
	SET @suStatus = 'OK'
	DECLARE @sResults nvarchar(700) = 'Invoice count = ' + CONVERT(nvarchar(12),@iCountInv) + ', Duplicate count = ' + CONVERT(nvarchar(12),@iNotCountInv)
	SET @suStatus = 'OK' + @sResults
	SELECT 0
END TRY
BEGIN CATCH
	SET @suStatus = 'Error ' + CONVERT(nvarchar(12),ERROR_NUMBER()) + ' in spInvoiceSummaries line ' + CONVERT(nvarchar(12),ERROR_LINE()) + ': ' + ERROR_MESSAGE()
	RETURN
END CATCH
END

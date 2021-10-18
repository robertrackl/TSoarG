
SELECT DISTINCT ID, Date, sVendorName, cStatus, cStatusPrev, XT, XactTotal, ExpAccount AS [Expense Account(s)],
	Description, NX, PmtAccount AS [Payment Account(s)], NP, NumAtt, sMemo
FROM 
	(SELECT									 XO.ID, XO.D AS Date, V.sVendorName, XO.cStatus, XO.cStatusPrev,

        (SELECT SUM(mAmount) / 2
         FROM   SF_ENTRIES AS SF_ENTRIES_1
         WHERE  (iXactId = XO.ID))											 AS XT, -- numerical total amount of the transaction for sorting purposes

        (SELECT FORMAT(SUM(mAmount) / 2, 'N2')
         FROM   SF_ENTRIES
         WHERE  (iXactId = XO.ID))											 AS XactTotal, -- Formatted total amount of the transaction 

        (SELECT TOP (1) A.sCode + ' ' + A.sName + ', ...'
         FROM   SF_ENTRIES AS E
				INNER JOIN SF_PAYMENTMETHODS AS P ON E.iPaymentMethod = P.ID
				INNER JOIN SF_ACCOUNTS AS A ON E.iAccountId = A.ID
         WHERE  (E.iXactId = XO.ID) AND (P.sPaymentMethod = N'(none)'))		 AS ExpAccount, -- The first expense account encountered among the expenditure entries

        (SELECT TOP (1) E.sDescription + ', ...'
         FROM   SF_ENTRIES AS E INNER JOIN
                SF_PAYMENTMETHODS AS P ON E.iPaymentMethod = P.ID
         WHERE  (E.iXactId = XO.ID) AND (P.sPaymentMethod = N'(none)'))		 AS Description,

        (SELECT COUNT(*)
         FROM   SF_ENTRIES AS E INNER JOIN
                SF_PAYMENTMETHODS AS P ON E.iPaymentMethod = P.ID
         WHERE  (E.iXactId = XO.ID) AND (P.sPaymentMethod = N'(none)'))		 AS NX, -- Number of expenditure entries

        (SELECT TOP (1) A.sCode + ' ' + A.sName + ', ...'
         FROM   SF_ENTRIES AS E
				INNER JOIN SF_PAYMENTMETHODS AS P ON E.iPaymentMethod = P.ID
				INNER JOIN SF_ACCOUNTS AS A ON E.iAccountId = A.ID
         WHERE  (E.iXactId = XO.ID) AND (P.sPaymentMethod <> N'(none)'))	 AS PmtAccount, -- The first payment account encountered among the payment entries

        (SELECT COUNT(*)
         FROM   SF_ENTRIES AS E INNER JOIN
                SF_PAYMENTMETHODS AS P ON E.iPaymentMethod = P.ID
         WHERE  (E.iXactId = XO.ID) AND (P.sPaymentMethod <> N'(none)'))	 AS NP, -- Number of payment entries

																			 XO.sMemo,
        (SELECT COUNT(*)
         FROM   SF_XACT_DOCS AS XD INNER JOIN
                SF_XACTS AS X ON X.ID = XD.iXactId
         WHERE  (XO.ID = XD.iXactId))										 AS NumAtt -- Number of attached files

	FROM SF_XACTS AS XO
		INNER JOIN SF_XACTTYPES AS XY ON XO.iType = XY.ID
		INNER JOIN SF_VENDORS AS V ON XO.iVendor = V.ID
		INNER JOIN SF_ENTRIES AS EN ON EN.iXactId = XO.ID						-- This join is needed only when we are filtering on a list of payment methods
		INNER JOIN SF_PAYMENTMETHODS AS PM ON PM.ID = EN.iPaymentMethod			-- This join is needed only when we are filtering on a list of payment methods
		--LEFT OUTER JOIN SF_XACT_DOCS AS XD ON XO.ID = XD.iXactId				-- This join is needed when one or both of the next two joins are present
		--INNER JOIN SF_ATTACHMENTCATEGS AS TC ON TC.ID = XD.iAttachmentCateg	-- This join must only be specified when we are filtering on attachment categories
		--INNER JOIN SF_DOCS AS SD ON SD.ID = XD.iDocsId						-- This join must only be specified when we are filtering on allowed attachment file types
		--INNER JOIN SF_ALLOWEDATTACHTYPES AS AL ON AL.ID = SD.iFileType		-- This join must only be specified when we are filtering on allowed attachment file types
	WHERE (XY.sTransactionType = N'Expense')
		AND (V.sVendorName IN (N'Crest Airpark', N'(none)', N'Johnson`s Hardware'))
		AND (XO.cStatus IN (N'A', N'V', N'D'))
		AND (XO.D BETWEEN '2018/10/29 00:00 -08:00' AND '2018/11/11 23:59 -08:00')
		AND (PM.sPaymentMethod IN (N'Cash', N'Check'))
		-- Only those transactions where there is at least one Expense account in common among the accounts used in the transaction and the accounts in the filter list for expense accounts
		AND (	(SELECT COUNT(*) FROM
					(SELECT sCode
						FROM SF_ACCOUNTS AC_0
							INNER JOIN SF_ENTRIES EN_0 ON EN_0.iAccountId=AC_0.ID
							INNER JOIN SF_XACTS XA_0 ON EN_0.iXactId=XA_0.ID
							INNER JOIN SF_PAYMENTMETHODS PM_0 ON PM_0.ID=EN_0.iPaymentMethod
						WHERE (XA_0.ID=XO.ID) AND (PM_0.sPaymentMethod = N'(none)')
					 INTERSECT
					 SELECT sCode FROM SF_ACCOUNTS AS AC_1 WHERE sCode IN ('5.1.3.6','5.1.1.4.1','5.1.1.3') -- filter list
					) AS D1
				) > 0
			)
		-- Only those transactions where there is at least one Payment account in common among the accounts used in the transaction and the accounts in the filter list for payment accounts
		AND (	(SELECT COUNT(*) FROM
					(SELECT sCode
						FROM SF_ACCOUNTS AC_0
							INNER JOIN SF_ENTRIES EN_0 ON EN_0.iAccountId=AC_0.ID
							INNER JOIN SF_XACTS XA_0 ON EN_0.iXactId=XA_0.ID
							INNER JOIN SF_PAYMENTMETHODS PM_0 ON PM_0.ID=EN_0.iPaymentMethod
						WHERE (XA_0.ID=XO.ID) AND (PM_0.sPaymentMethod <> N'(none)')
					 INTERSECT
					 SELECT sCode FROM SF_ACCOUNTS AS AC_1 WHERE sCode IN ('1.1.1.1','1.1.1.2','1.1.1.3') -- filter list
					) AS D1
				) > 0
			)
		-- AND (TC.sAttachmentCateg IN (N'Receipt', N'Invoice', N'Bill'))		-- This condition must only be present when we are filtering on attachment categories
		-- AND (AL.sAllowedFileType IN (N'.jpg', N'.jpeg'))						-- This condition must only be present when we are filtering on allowed attachment file types
	) AS DQ
WHERE (XT BETWEEN 0 AND 999999999.99)
	AND (NumAtt BETWEEN 0 AND 100)
ORDER BY Date, sVendorName

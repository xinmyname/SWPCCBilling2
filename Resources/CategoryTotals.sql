SELECT		f.Category AS FeeCategory,
					SUM(il.Quantity * il.UnitPrice) AS Amount
FROM 			InvoiceLine il,
					Fee f
WHERE 		InvoiceId IN ({0})
AND				f.Code == il.FeeCode
AND				f.Category NOT IN ('Payment')
GROUP BY	f.Category
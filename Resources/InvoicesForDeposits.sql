SELECT      d.Id,
            group_concat(i.Id)
FROM        Invoice i,
            Payment p,
            Deposit d
WHERE       i.Opened=?
AND         d.Id = p.DepositId
AND         p.InvoiceId = i.Id
GROUP BY    d.Id
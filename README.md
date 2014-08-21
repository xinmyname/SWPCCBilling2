SWPCCBilling2
=============

Basic CLI invoicing

sqlite3 SWPCCBilling.sqlite "SELECT i.FamilyName,i.Opened,il.FeeCode,il.Quantity,il.UnitPrice FROM Invoice i, InvoiceLine il WHERE il.InvoiceId = i.Id"

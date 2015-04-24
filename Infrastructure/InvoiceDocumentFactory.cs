using System;
using SWPCCBilling2.Models;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Nancy;

namespace SWPCCBilling2.Infrastructure
{
	public class InvoiceDocumentFactory
	{
		
		public Stream CreateInvoiceHtmlStream(long invoiceId, string viewName = null)
		{
			Bootstrapper bootstrapper = Bootstrapper.DefaultBootstrapper;

			INancyEngine engine = bootstrapper.GetEngine();
			var request = new Request("GET", String.Format("/invoice/{0}/{1}", invoiceId, viewName ?? ""), "http");
			NancyContext context = engine.HandleRequest(request);
			Response response = context.Response;
			var stream = new MemoryStream();
			response.Contents(stream);		

			stream.Seek(0, SeekOrigin.Begin);

			return stream;
		}

		public string CreateInvoiceHtmlText(long invoiceId, string viewName = null)
		{
			string text;

			using (Stream stream = CreateInvoiceHtmlStream(invoiceId))
			using (var reader = new StreamReader(stream))
			{
				text = reader.ReadToEnd();
			}

			return text;
		}
	}
}

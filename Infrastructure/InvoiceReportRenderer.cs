using System;
using System.Collections.Generic;
using SWPCCBilling2.Modules;
using System.Reflection;
using System.IO;
using System.Text;

namespace SWPCCBilling2
{
	public class InvoiceReportRenderer
	{
		private readonly string _depositInvoiceDataTemplate;
		private readonly string _invoiceDataTemplate;
		private readonly string _paymentDataTemplate;
		private readonly string _feePaymentsTemplate;

		public InvoiceReportRenderer()
		{
			_depositInvoiceDataTemplate = LoadResource("DepositInvoiceData.html");
			_invoiceDataTemplate = LoadResource("InvoiceData.html");
			_paymentDataTemplate = LoadResource("PaymentData.html");
			_feePaymentsTemplate = LoadResource("FeePayments.html");
		}

		public string Render(IEnumerable<DepositInvoiceData> depositInvoiceData)
		{
			var builder = new StringBuilder();

			foreach (DepositInvoiceData did in depositInvoiceData)
				builder.Append(RenderDepositInvoiceData(did));

			return builder.ToString();
		}

		private string LoadResource(string name)
		{
			string resourceText = "";

			Assembly asm = GetType().Assembly;

			string fullResourceName = "SWPCCBilling2.Views.Report.Invoices." + name;

			using (Stream stream = asm.GetManifestResourceStream(fullResourceName))
			using (var reader = new StreamReader(stream))

			{
				resourceText = reader.ReadToEnd();
			}

			return resourceText;
		}

		private string RenderDepositInvoiceData(DepositInvoiceData depositInvoiceData)
		{
			var builder = new StringBuilder(_depositInvoiceDataTemplate);

			builder.Replace("{{DepositId}}", depositInvoiceData.DepositId.ToString());
			builder.Replace("{{DepositDateText}}", depositInvoiceData.DepositDateText);
			builder.Replace("{{DepositAmountText}}", depositInvoiceData.DepositAmountText);
			builder.Replace("{{InvoiceData}}", RenderInvoiceData(depositInvoiceData.InvoiceData));

			return builder.ToString();
		}

		private string RenderInvoiceData(IEnumerable<InvoiceData> invoiceData)
		{
			var builder = new StringBuilder();

			foreach (InvoiceData id in invoiceData)
				builder.Append(RenderInvoiceData(id));

			return builder.ToString();
		}

		private string RenderInvoiceData(InvoiceData invoiceData)
		{
			var builder = new StringBuilder(_invoiceDataTemplate);

			builder.Replace("{{InvoiceId}}", invoiceData.InvoiceId.ToString());
			builder.Replace("{{FamilyName}}", invoiceData.FamilyName);
			builder.Replace("{{AmountDueText}}", invoiceData.AmountDueText);
			builder.Replace("{{PaymentData}}", RenderPaymentData(invoiceData.PaymentData));
			builder.Replace("{{FeePayments}}", RenderFeePayments(invoiceData.FeePayments));

			return builder.ToString();
		}

		private string RenderPaymentData(IEnumerable<PaymentData> paymentData)
		{
			var builder = new StringBuilder();

			foreach (PaymentData pd in paymentData)
				builder.Append(RenderPaymentData(pd));

			return builder.ToString();
		}

		private string RenderPaymentData(PaymentData paymentData)
		{
			var builder = new StringBuilder(_paymentDataTemplate);

			builder.Replace("{{PaymentId}}", paymentData.PaymentId.ToString());
			builder.Replace("{{CheckNumber}}", paymentData.CheckNumber);
			builder.Replace("{{AmountText}}", paymentData.AmountText);
			builder.Replace("{{ReceivedDateText}}", paymentData.ReceivedDateText);

			return builder.ToString();
		}
	
		private string RenderFeePayments(IEnumerable<FeePayment> feePayments)
		{
			var builder = new StringBuilder();

			foreach (FeePayment fp in feePayments)
				builder.Append(RenderFeePayment(fp));

			return builder.ToString();
		}

		private string RenderFeePayment(FeePayment feePayment)
		{
			var builder = new StringBuilder(_feePaymentsTemplate);

			builder.Replace("{{FeeCategory}}", feePayment.FeeCategory);
			builder.Replace("{{TotalPaidText}}", feePayment.TotalPaidText);

			return builder.ToString();
		}
	}
}


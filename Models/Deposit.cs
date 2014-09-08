using System;
using System.ComponentModel.DataAnnotations;

namespace SWPCCBilling2.Models
{

	public class Deposit
	{
		[Key]
		public long Id { get; set; }
		public DateTime Date { get; set; }
		public double Amount { get; set; }

		public Deposit()
		{
		}

		public Deposit(DateTime date, double amount)
		{
			Date = date;
			Amount = amount;
		}
	}
}

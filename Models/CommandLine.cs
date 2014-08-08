using System;
using System.Collections.Generic;
using SWPCCBilling2.Infrastructure;
using System.Text;

namespace SWPCCBilling2.Models
{
	public class CommandLine
	{
		public string Text { get; private set; }

		public CommandLine(string text)
		{
			Text = text;
		}
	}
	
}

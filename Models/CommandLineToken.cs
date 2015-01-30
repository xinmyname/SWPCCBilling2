using System;
using System.Collections.Generic;
using System.Text;
using SWPCCBilling2.Models;
using System.Linq;

namespace SWPCCBilling2.Models
{

	public class CommandLineToken
	{
		public int Position { get; set; }
		public int Length { get; set; }
		public string Text { get; set; }
		public bool IsWhiteSpace { get; set; }

		public override string ToString()
		{
			return string.Format("[Token: Position={0}, Length={1}, Text=\"{2}\", IsWhiteSpace={3}]", Position, Length, Text, IsWhiteSpace);
		}
	}

	public class TextToken : CommandLineToken
	{
	}

	public class WhiteSpaceToken : CommandLineToken
	{
	}

	public class QuoteToken : CommandLineToken
	{
	}

	public class CommentToken : CommandLineToken
	{
	}
}

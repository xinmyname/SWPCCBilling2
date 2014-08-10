using System;
using System.Text;

namespace SWPCCBilling2.Infrastructure
{
	public class Span
	{
		public string Text { get; private set; }
		public bool IsWhiteSpace { get; private set; }

		private readonly ICompleteText _completion;

		public Span(string trunk, bool isWhiteSpace, ICompleteText completion)
		{
			Text = trunk;
			IsWhiteSpace = isWhiteSpace;
			_completion = completion;
			_completion.Trunk = trunk;
		}

		public Span(string trunk, bool isWhiteSpace)
		{
			Text = trunk;
			IsWhiteSpace = isWhiteSpace;
			_completion = NoCompletion.Default;
		}

		public int Length { get { return Text.Length; } }

		public void CompleteNext()
		{
			string completeWith = _completion.Next();

			if (completeWith == null)
				return;

			Text = completeWith;
		}
	}

	public class WhiteSpaceSpan : Span
	{
		public WhiteSpaceSpan(string trunk)
			: base(trunk, true)
		{
		}
	}
}

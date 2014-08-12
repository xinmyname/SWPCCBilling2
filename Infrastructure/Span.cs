using System;
using System.Text;
using SWPCCBilling2.Completions;

namespace SWPCCBilling2.Infrastructure
{
	public class Span
	{
		public string Text { get; private set; }
		public bool IsWhiteSpace { get; private set; }
		public bool IsParameter { get; private set; }

		private readonly ICompleteText _completion;

		public Span(string trunk, bool isWhiteSpace, bool isParameter, ICompleteText completion)
		{
			Text = trunk;
			IsWhiteSpace = isWhiteSpace;
			IsParameter = isParameter;
			_completion = completion;
			_completion.Trunk = trunk;
		}

		public Span(string trunk, bool isWhiteSpace, bool isParameter)
		{
			Text = trunk;
			IsWhiteSpace = isWhiteSpace;
			IsParameter = isParameter;
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
			: base(trunk, true, false)
		{
		}
	}

	public class ParameterSpan : Span
	{
		public ParameterSpan(string trunk)
			: base(trunk, false, true)
		{
		}

		public ParameterSpan(string trunk, ICompleteText completion)
			: base(trunk, false, true, completion)
		{
		}
	}
}

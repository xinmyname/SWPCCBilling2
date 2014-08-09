using System;
using System.Text;

namespace SWPCCBilling2.Infrastructure
{
	public class Span
	{
		public string Text { get; private set; }
		public int Length { get { return Text.Length; } }

		private readonly string _stem;
		private readonly ICompleteText _completion;

		public Span(string stem, ICompleteText completion)
		{

			_stem = stem;
			_completion = completion;
			//_completion.Trunk = _stem;
			Text = stem;
		}

		public void CompleteNext()
		{
			string completeWith = _completion.Next();

			if (completeWith == null)
				return;

			Text = completeWith;
		}
	}
}

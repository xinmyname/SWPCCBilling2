using System;
using System.Text;

namespace SWPCCBilling2.Infrastructure
{
	public class Span
	{
		public string Text { get; private set; }
		public int Length { get { return Text.Length; } }

		private readonly ICompleteText _completion;

		public Span(string trunk, ICompleteText completion)
		{
			_completion = completion;
			_completion.Trunk = trunk;
			Text = trunk;
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

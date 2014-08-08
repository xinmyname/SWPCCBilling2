using System;
using System.Text;

namespace SWPCCBilling2.Infrastructure
{
	public class Span
	{
		public StringBuilder Text { get; set; }
		public int Position { get; set; }
		public int Length { get { return Text.Length; } }

		private readonly ICompleteText _completion;

		public Span(int position, ICompleteText completion)
		{
			Text = new StringBuilder();
			Position = position;
			_completion = completion;
		}

		public Span(int position)
		{
			Text = new StringBuilder();
			Position = position;
			_completion = new NoCompletion();
		}

		public void Enter()
		{
			_completion.Preload();
		}

		public void Leave()
		{
		}

		public bool MoveLeft()
		{
			if (Position == 0)
				return false;

			Position--;
			return true;
		}

		public bool MoveRight()
		{
			if (Position == Length)
				return false;

			Position++;
			return true;
		}

		public bool InsertCharacter(char ch)
		{
			Text.Insert(Position, ch);

			if (Position <= _completion.TrunkLength)
				_completion.TrunkInsert(Position, ch);

			Position++;
			return true;
		}

		public bool DeletePreviousCharacter()
		{
			if (Position == 0)
				return false;

			Text.Remove(Position-1, 1);

			if (Position <= _completion.TrunkLength)
				_completion.TrunkRemove(Position - 1);

			Position--;
			return true;
		}

		public bool DeleteCurrentCharacter()
		{
			if (Position == Length)
				return false;

			Text.Remove(Position, 1);

			if (Position <= _completion.TrunkLength)
				_completion.TrunkRemove(Position);

			return true;
		}

		public void CompleteNext()
		{
			string completeWith = _completion.Next();

			if (completeWith == null)
				return;

			Text = new StringBuilder(completeWith);
			Position = Text.Length;
		}

		public override string ToString()
		{
			return string.Format("[Span: Text=\"{0}\", Trunk=\"{1}\"]", Text, _completion.Trunk);
		}
	}
	
}

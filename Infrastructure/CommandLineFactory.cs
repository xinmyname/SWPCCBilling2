using System;
using System.Collections.Generic;
using System.Text;
using SWPCCBilling2.Models;
using System.Linq;

namespace SWPCCBilling2.Infrastructure
{
	public class CommandLineFactory
	{
		private readonly ActionMetaData _actionMetaData;
		private readonly StringBuilder _line;
		private readonly IList<string> _history;
		private int _historyIndex;
		private int _left;
		private int _top;
		private int _pos;
		private int _prevSpanLength;
		private bool _recalled;
		private bool _done;
		private bool _reparse;
		private List<Span> _spans;
		private List<string> _errors;

		public CommandLineFactory(ActionMetaData actionMetaData)
		{
			_actionMetaData = actionMetaData;
			_line = new StringBuilder();
			_history = new List<string>();
			_historyIndex = 0;
			_done = false;
			_spans = new List<Span>();
			_errors = new List<string>();
		}

		public void Prompt()
		{
			Console.Write("> ");
			_left = Console.CursorLeft;
			_top = Console.CursorTop;
			_line.Clear();
			_pos = 0;
			_recalled = false;
			_prevSpanLength = 0;
			_spans.Clear();
			_errors.Clear();
		}

		public IEnumerable<CommandLine> Acquire()
		{
			Prompt();

			do
			{
				_reparse = false;

				ConsoleKeyInfo keyInfo = ConsoleEx.TranslateKey();

				switch (keyInfo.Key)
				{
					case ConsoleKey.Tab:
						break;

					case ConsoleKey.Enter:
						Console.WriteLine();
						yield return new CommandLine(null, null, _errors);
						SaveLineToHistory();
						Prompt();
						break;

					case ConsoleKey.Backspace:
						DeletePreviousCharacter();
						break;

					case ConsoleKey.Delete:
						DeleteCurrentCharacter();
						break;

					case ConsoleKey.LeftArrow:
						MoveLeft();
						break;

					case ConsoleKey.RightArrow:
						MoveRight();
						break;

					case ConsoleKey.UpArrow:
						RecallPreviousLineFromHistory();
						break;

					case ConsoleKey.DownArrow:
						RecallNextLineFromHistory();
						break;
					
					case ConsoleKey.Escape:
						_line.Append("credit-payment Sherwood/Odman 1235 123.45");
						_reparse = true;
						break;

					default:
						InsertKeyInfo(keyInfo);
						break;
				}

				if (_reparse)
					ParseLine();

				RenderLine();

			} while (!_done);
		}

		public void SaveLineToHistory()
		{
			if (!_recalled)
				_history.Add(_line.ToString());

			_historyIndex = _history.Count;
		}

		public void DeletePreviousCharacter()
		{
			if (_pos > 0)
			{
				_line.Remove(_pos-1, 1);
				_pos--;
				_reparse = true;
				_recalled = false;
			}
		}

		public void DeleteCurrentCharacter()
		{
			if (_pos < _line.Length)
			{
				_line.Remove(_pos, 1);
				_reparse = true;
				_recalled = false;
			}
		}

		public void MoveLeft()
		{
			if (_pos > 0)
				_pos--;
		}

		public void MoveRight()
		{
			if (_pos != _line.Length)
				_pos++;
		}

		public void RecallPreviousLineFromHistory()
		{
			if (_history.Count == 0)
				return;

			_historyIndex--;

			if (_historyIndex < 0)
				_historyIndex = _history.Count-1;

			_line.Clear();
			_line.Append(_history[_historyIndex]);
			_pos = _line.Length;
			_reparse = true;
			_recalled = true;
		}

		public void RecallNextLineFromHistory()
		{
			if (_history.Count == 0)
				return;

			_historyIndex++;

			if (_historyIndex >= _history.Count)
				_historyIndex = 0;

			_line.Clear();
			_line.Append(_history[_historyIndex]);
			_pos = _line.Length;
			_reparse = true;
			_recalled = true;
		}

		public void InsertKeyInfo(ConsoleKeyInfo keyInfo)
		{
			if (keyInfo.Modifiers == ConsoleModifiers.Alt ||
				keyInfo.Modifiers == ConsoleModifiers.Control ||
				keyInfo.KeyChar == '\0')
				return;

			_line.Insert(_pos, keyInfo.KeyChar);

			_pos++;
			_reparse = true;
			_recalled = false;
		}

		public void RenderLine()
		{
			Console.CursorVisible = false;
			Console.SetCursorPosition(_left, _top);

			int spanLength = 0;

			foreach (Span span in _spans)
			{
				Console.Write(span.Text);
				spanLength += span.Length;
			}

			int spanDelta = spanLength - _prevSpanLength;

			while (spanDelta < 0)
			{
				Console.Write(' ');
				spanDelta++;
			}

			_prevSpanLength = spanLength;

			Console.SetCursorPosition(_left + _pos, _top);
			Console.CursorVisible = true;
		}

		public void ParseLine()
		{
			bool actionPresent = false;
			ActionInfo actionInfo = null;

			_spans.Clear();
			_errors.Clear();

			foreach (Token token in TokenizeLine())
			{
				ICompleteText completion = new NoCompletion();

				if (!token.IsWhiteSpace && !actionPresent)
				{
					completion = new ActionCompletion();
					actionPresent = true;
					actionInfo = _actionMetaData.GetAction(token.Text);

					if (actionInfo == null)
						_errors.Add("Unknown action. Type 'help' to get a list of actions.");
				}

				_spans.Add(new Span(token.Text, completion));
			}
		}

		public IEnumerable<Token> TokenizeLine()
		{
 			char[] chars = _line.ToString().ToCharArray();

			for (int i = 0; i < chars.Length;) 
			{
				var token = new StringBuilder();
				int position = i;

				if (Char.IsWhiteSpace(chars[i]))
				{
					for (; i < chars.Length && Char.IsWhiteSpace(chars[i]); i++)
						token.Append(chars[i]);

					yield return new Token {
						Position = position,
						Length = i - position,
						Text = token.ToString(),
						IsWhiteSpace = true
					};
				}
				else
				{
					for (; i < chars.Length && !Char.IsWhiteSpace(chars[i]); i++)
						token.Append(chars[i]);

					yield return new Token {
						Position = position,
						Length = i - position,
						Text = token.ToString(),
						IsWhiteSpace = false
					};
				}
			}
		}
	}

	public class Token
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
}

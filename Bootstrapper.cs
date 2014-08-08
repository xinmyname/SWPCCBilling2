using System;
using System.Collections.Generic;
using SWPCCBilling2.Infrastructure;
using System.Text;

namespace SWPCCBilling2
{
	public class CommandLine
	{
		public string Text { get; private set; }

		public CommandLine(string text)
		{
			Text = text;
		}
	}

	public class CommandLineFactory
	{
		private readonly IList<string> _history;
		private int _historyIndex;
		private int _left;
		private int _top;
		private int _pos;
		private int _prevLineLength;
		private bool _recalled;
		private bool _done;
		private bool _reparse;
		private StringBuilder _line;

		public CommandLineFactory()
		{
			_history = new List<string>();
			_historyIndex = 0;
			_line = new StringBuilder();
			_done = false;
		}

		public void Prompt()
		{
			Console.WriteLine();
			Console.Write("> ");
			_left = Console.CursorLeft;
			_top = Console.CursorTop;
			_line.Clear();
			_pos = 0;
			_recalled = false;
		}

		public IEnumerable<CommandLine> Acquire()
		{
			Prompt();

			do
			{
				_prevLineLength = _line.Length;
				_reparse = false;

				ConsoleKeyInfo keyInfo = ConsoleEx.TranslateKey();

				switch (keyInfo.Key)
				{
				case ConsoleKey.Tab:
					break;

				case ConsoleKey.Enter:
					yield return new CommandLine(_line.ToString());
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

				default:
					InsertKeyInfo(keyInfo);
					break;
				}

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

			_line = new StringBuilder(_history[_historyIndex]);
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

			_line = new StringBuilder(_history[_historyIndex]);
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
			int lineLengthDelta = _line.Length - _prevLineLength;

			Console.CursorVisible = false;
			Console.SetCursorPosition(_left, _top);
			Console.Write(_line.ToString());

			while (lineLengthDelta < 0)
			{
				Console.Write(' ');
				lineLengthDelta++;
			}

			Console.SetCursorPosition(_left + _pos, _top);
			Console.CursorVisible = true;
		}
	}


	class Bootstrapper
	{
		public static void Main(string[] args)
		{
			var cmdLineFactory = new CommandLineFactory();

			foreach (CommandLine cmdLine in cmdLineFactory.Acquire())
			{
			}

			Console.WriteLine("Done!");
		}
	}
}

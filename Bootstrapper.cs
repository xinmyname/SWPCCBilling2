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

		public CommandLineFactory()
		{
			_history = new List<string>();
			_historyIndex = 0;
		}

		public IEnumerable<CommandLine> Acquire()
		{
			Console.Write("> ");
			int left = Console.CursorLeft;
			int top = Console.CursorTop;
			var line = new StringBuilder();
			int pos = 0;

			bool done = false;
			bool recalled = false;

			do
			{
				int prevLineLength = line.Length;
				bool reparse = false;

				ConsoleKeyInfo keyInfo = ConsoleEx.TranslateKey();

				switch (keyInfo.Key)
				{
				case ConsoleKey.Tab:
					break;

				case ConsoleKey.Enter:
					Console.WriteLine();
					Console.Write("> ");
					left = Console.CursorLeft;
					top = Console.CursorTop;
					pos = 0;
					yield return new CommandLine(line.ToString());
				
					if (!recalled)
						_history.Add(line.ToString());

					_historyIndex = _history.Count;
					line.Clear();

					break;

				case ConsoleKey.Backspace:
					if (pos > 0)
					{
						line.Remove(pos-1, 1);
						pos--;
						reparse = true;
						recalled = false;
					}
					break;

				case ConsoleKey.Delete:
					if (pos < line.Length)
					{
						line.Remove(pos, 1);
						reparse = true;
						recalled = false;
					}
					break;

				case ConsoleKey.LeftArrow:
					if (pos > 0)
						pos--;
					break;

				case ConsoleKey.RightArrow:
					if (pos != line.Length)
						pos++;
					break;

				case ConsoleKey.UpArrow:
					if (_history.Count == 0)
						break;
					_historyIndex--;
					if (_historyIndex < 0)
						_historyIndex = _history.Count-1;
					line = new StringBuilder(_history[_historyIndex]);
					pos = line.Length;
					reparse = true;

					recalled = true;
					break;

				case ConsoleKey.DownArrow:
					if (_history.Count == 0)
						break;
					_historyIndex++;
					if (_historyIndex >= _history.Count)
						_historyIndex = 0;
					line = new StringBuilder(_history[_historyIndex]);
					pos = line.Length;
					reparse = true;
					recalled = true;
					break;

				default:
					if (keyInfo.Modifiers == ConsoleModifiers.Alt ||
						keyInfo.Modifiers == ConsoleModifiers.Control ||
						keyInfo.KeyChar == '\0')
						break;

					line.Insert(pos, keyInfo.KeyChar);
					pos++;
					reparse = true;
					recalled = false;
					break;
				}

				int lineLengthDelta = line.Length - prevLineLength;

				Console.CursorVisible = false;
				Console.SetCursorPosition(left, top);
				Console.Write(line.ToString());

				while (lineLengthDelta < 0)
				{
					Console.Write(' ');
					lineLengthDelta++;
				}

				Console.SetCursorPosition(left + pos, top);
				Console.CursorVisible = true;

			} while (!done);
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

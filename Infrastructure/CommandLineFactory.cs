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
		private ActionInfo _actionInfo;

		public CommandLineFactory(ActionMetaData actionMetaData)
		{
			_actionMetaData = actionMetaData;
			_line = new StringBuilder();
			_history = new List<string>();
			_historyIndex = 0;
			_done = false;
			_spans = new List<Span>();
			_errors = new List<string>();
			_actionInfo = null;
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
						CompleteUnderCursor();
						break;

					case ConsoleKey.Enter:
						ParseLine();
						Console.WriteLine();
						object[] parameters = LoadParametersFromSpans();
						yield return new CommandLine(_actionInfo, parameters, _errors);
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

		public void CompleteUnderCursor()
		{
			int lenAccum = 0;

			Span curSpan = null;

			foreach (Span span in _spans)
			{
				lenAccum += span.Length;

				if (lenAccum <= _pos)
					curSpan = span;
				else
					break;
			}

			if (curSpan != null)
			{
				string lastText = curSpan.Text;
				curSpan.CompleteNext();
				int delta = curSpan.Text.Length - lastText.Length;
				_pos += delta;
			}
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
			if (_pos != _prevSpanLength)
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

			_line.Clear();
			int spanLength = 0;

			foreach (Span span in _spans)
			{
				_line.Append(span.Text);
				spanLength += span.Length;
			}

			Console.Write(_line);

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
			_actionInfo = null;
		
			_spans.Clear();
			_errors.Clear();

			int paramNum = 0;

			foreach (CommandLineToken token in TokenizeLine())
			{
				// If there was an error, don't do any else, just copy the text
				if (_errors.Any() || token.IsWhiteSpace) 
				{
					AddTokenToSpansWithoutCompletion(token);
					continue;
				}

				if (_actionInfo == null)
					_actionInfo = ConvertTokenToActionSpan(token);
				else 
				{
					ConvertTokenToParameterSpan(token, paramNum);
					paramNum++;
				}
			}
		}

		public IEnumerable<CommandLineToken> TokenizeLine()
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

					yield return new CommandLineToken {
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

					yield return new CommandLineToken {
						Position = position,
						Length = i - position,
						Text = token.ToString(),
						IsWhiteSpace = false
					};
				}
			}
		}

		private void AddTokenToSpansWithoutCompletion(CommandLineToken token)
		{
			_spans.Add(new Span(token.Text, token.IsWhiteSpace));
		}

		private ActionInfo ConvertTokenToActionSpan(CommandLineToken token)
		{
			ActionInfo actionInfo = _actionMetaData.GetAction(token.Text);

			if (actionInfo == null)
				_errors.Add("Unknown action. Type 'help' to get a list of actions.");

			_spans.Add(new Span(token.Text, false, new ActionCompletion()));

			return actionInfo;
		}

		private void ConvertTokenToParameterSpan(CommandLineToken token, int paramNum)
		{
			ICompleteText completion = null;

			if (paramNum < _actionInfo.Parameters.Count) 
			{
				ActionParam actionParam = _actionInfo.Parameters[paramNum];
				completion = (ICompleteText)Activator.CreateInstance(actionParam.CompletionType);
			}

			if (completion == null)
				completion = NoCompletion.Default;

			_spans.Add(new Span(token.Text, false, completion));
		}

		private object[] LoadParametersFromSpans()
		{

			return null;
		}
	}
}

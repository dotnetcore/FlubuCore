using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FlubuCore.Infrastructure.Terminal
{
    public class ConsoleHintedInput
    {
        private readonly IDictionary<char, IReadOnlyCollection<string>> _hintsSourceDictionary;
        private readonly List<string> _commandsHistory = new List<string>();
        private List<Suggestion> _suggestionsForUserInput;
        private int _suggestionPosition;
        private int _historyPosition;

        /// <summary>
        /// Creates new instance of <see cref="ConsoleHintedInput"/> class
        /// </summary>
        /// <param name="hintsSourceDictionary">Collection containing input hints</param>
        public ConsoleHintedInput(IReadOnlyCollection<string> defaultHints, IDictionary<char, IReadOnlyCollection<string>>  hintsSourceDictionary = null)
        {
            _hintsSourceDictionary = hintsSourceDictionary;

            if (_hintsSourceDictionary == null)
            {
                _hintsSourceDictionary = new Dictionary<char, IReadOnlyCollection<string>>();
            }

            _hintsSourceDictionary.Add('*', defaultHints);
        }

        /// <summary>
        /// Reads input from user using hints. Commands history is supported
        /// </summary>
        /// <param name="inputRegex"></param>
        /// <param name="hintColor"></param>
        /// <returns></returns>
        public string ReadHintedLine(string inputRegex = ".*", ConsoleColor hintColor = ConsoleColor.DarkGray)
        {
            ConsoleKeyInfo input;

            Suggestion suggestion = null;
            var userInput = string.Empty;
            var fullInput = string.Empty;
            var readLine = string.Empty;
            var wasUserInput = false;
            bool writeSugestionToConsole;
            var cursorPosition = new ConsoleCursorPosition(ConsoleUtils.Prompt.Length, Console.CursorTop, Console.WindowWidth);
            while ((input = Console.ReadKey()).Key != ConsoleKey.Enter)
            {
                writeSugestionToConsole = false;
                int positionToDelete;
                switch (input.Key)
                {
                    case ConsoleKey.Delete:
                        positionToDelete = cursorPosition.InputLength;
                        if (positionToDelete >= 0 && positionToDelete < userInput.Length)
                        {
                            cursorPosition--;
                            userInput = userInput.Any() ? userInput.Remove(positionToDelete, 1) : string.Empty;
                        }

                        wasUserInput = !string.IsNullOrWhiteSpace(userInput);
                        UpdateSuggestionsForUserInput(userInput);
                        suggestion = GetFirstSuggestion();
                        break;
                    case ConsoleKey.Backspace:
                        positionToDelete = cursorPosition.InputLength - 1;
                        if (positionToDelete >= 0 && positionToDelete < userInput.Length)
                        {
                            userInput = userInput.Any() ? userInput.Remove(positionToDelete, 1) : string.Empty;
                            cursorPosition--;
                        }

                        if (cursorPosition.InputLength < 0)
                        {
                            cursorPosition = cursorPosition.SetLength(0);
                        }

                        wasUserInput = !string.IsNullOrWhiteSpace(userInput);
                        UpdateSuggestionsForUserInput(userInput);
                        suggestion = GetFirstSuggestion();
                        break;
                    case ConsoleKey.Tab:
                        if (suggestion != null)
                        {
                            writeSugestionToConsole = true;
                            userInput = suggestion.Value + ' ';
                            UpdateSuggestionsForUserInput(userInput);
                            suggestion = GetFirstSuggestion();
                            var tmp = fullInput.LastIndexOf(" ");
                            if (tmp == -1)
                            {
                                cursorPosition = cursorPosition.SetLength(userInput.Length);
                            }
                            else
                            {
                                cursorPosition = cursorPosition.SetLength(tmp + userInput.Length);
                            }
                        }

                        break;
                    case ConsoleKey.Spacebar:
                        userInput = userInput + " ";
                        wasUserInput = true;
                        cursorPosition++;
                        ////if (suggestion != null)
                        ////{
                        ////    userInput = suggestion.Value + ' ';
                        ////}
                        ////else if (Regex.IsMatch(input.KeyChar.ToString(), inputRegex))
                        ////{
                        ////    cursorPosition++;
                        ////    userInput = userInput.Insert(cursorPosition.InputLength - 1, input.KeyChar.ToString());
                        ////    UpdateSuggestionsForUserInput(userInput);
                        ////    suggestion = GetFirstSuggestion();
                        ////    cursorPosition = cursorPosition.SetLength(userInput.Length);
                        ////}
                        break;
                    case ConsoleKey.UpArrow:
                        if (!wasUserInput)
                        {
                            userInput = GetPreviousCommandFromHistory();
                            cursorPosition = cursorPosition.SetLength(userInput.Length);
                        }
                        else
                        {
                            suggestion = GetPreviousSuggestion();
                        }

                        break;
                    case ConsoleKey.DownArrow:
                        if (!wasUserInput)
                        {
                            userInput = GetNextCommandFromHistory();
                            cursorPosition = cursorPosition.SetLength(userInput.Length);
                        }
                        else
                        {
                            suggestion = GetNextSuggestion();
                        }

                        break;
                    case ConsoleKey.LeftArrow:
                        if (cursorPosition.InputLength > 0)
                        {
                            cursorPosition--;
                        }

                        break;
                    case ConsoleKey.RightArrow:
                        if (cursorPosition.InputLength < userInput.Length)
                        {
                            cursorPosition++;
                        }

                        break;
                    case ConsoleKey.Home:
                        cursorPosition = cursorPosition.SetLength(0);
                        break;
                    case ConsoleKey.End:
                        cursorPosition = cursorPosition.SetLength(userInput.Length);
                        break;
                    case ConsoleKey.F1:
                    case ConsoleKey.F2:
                    case ConsoleKey.F3:
                    case ConsoleKey.F4:
                    case ConsoleKey.F5:
                    case ConsoleKey.F6:
                    case ConsoleKey.F7:
                    case ConsoleKey.F8:
                    case ConsoleKey.F9:
                    case ConsoleKey.F10:
                    case ConsoleKey.F11:
                    case ConsoleKey.F12:
                        break;
                    default:
                        if (Regex.IsMatch(input.KeyChar.ToString(), inputRegex))
                        {
                            cursorPosition++;
                            userInput = userInput.Insert(cursorPosition.InputLength - 1,
                                input.KeyChar.ToString());
                        }

                        UpdateSuggestionsForUserInput(userInput);
                        suggestion = GetFirstSuggestion();
                        break;
                }

                readLine = suggestion != null ? suggestion.Value : userInput.TrimEnd(' ');

                ClearConsoleLines(cursorPosition.StartTop, cursorPosition.Top);
                var li = fullInput.TrimEnd().LastIndexOf(" ");
                    if (li == -1)
                {
                    Console.Write(userInput);
                    fullInput = userInput;
                }
                else
                {
                    if (!writeSugestionToConsole)
                    {
                        Console.Write(userInput);
                        fullInput = userInput;
                    }
                    else
                    {
                        userInput = userInput.TrimEnd() + " ";
                        fullInput = $"{fullInput.Substring(0, li)} {userInput}";
                        userInput = fullInput;
                        Console.Write(fullInput);
                        cursorPosition = cursorPosition.SetLength(fullInput.Length);
                    }
                }

                if (userInput.Any())
                {
                    if (suggestion != null && suggestion.Value != userInput && writeSugestionToConsole == false)
                    {
                        WriteSuggestion(suggestion, hintColor);
                    }
                }

                Console.CursorLeft = cursorPosition.Left;
                Console.CursorTop = cursorPosition.Top;
            }

            //ClearConsoleLines();
            //Console.WriteLine(readLine);
            AddCommandToHistory(readLine);
            return readLine;
        }

        private static void WriteSuggestion(Suggestion suggestion, ConsoleColor hintColor)
        {
            if (suggestion.HighlightIndexes == null || !suggestion.HighlightIndexes.Any())
            {
                ConsoleUtils.Write($" ({suggestion.Value})", hintColor);
                return;
            }

            var orderedIndexes = suggestion.HighlightIndexes.OrderBy(v => v).ToArray();
            var idx = 0;

            ConsoleUtils.Write(" (", hintColor);
            for (var i = 0; i < suggestion.Value.Length; i++)
            {
                ConsoleColor color;
                if (idx < orderedIndexes.Length && i == orderedIndexes[idx])
                {
                    idx++;
                    color = Console.ForegroundColor;
                }
                else
                {
                    color = hintColor;
                }

                ConsoleUtils.Write(suggestion.Value[i].ToString(), color);
            }

            ConsoleUtils.Write(")", hintColor);
        }

        private static void ClearConsoleLines(int startline, int endline)
        {
            for (var i = startline; i <= endline; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(0, startline);
            ConsoleUtils.WritePrompt();
        }

        private void UpdateSuggestionsForUserInput(string userInput)
        {
            _suggestionPosition = 0;

            if (string.IsNullOrEmpty(userInput))
            {
                _suggestionsForUserInput = null;
                return;
            }

            var splitedUserInput = userInput.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x));
            var lastInput = splitedUserInput.Last();
            char hintSourceKey;
            if (_hintsSourceDictionary.ContainsKey(lastInput[0]))
            {
                hintSourceKey = lastInput[0];
                lastInput = lastInput.Substring(1);
            }
            else
            {
                hintSourceKey = '*';
            }

            var hintSource = _hintsSourceDictionary[hintSourceKey]; 
            if (hintSource.All(item => item.Length < lastInput.Length))
            {
                _suggestionsForUserInput = null;
                return;
            }

            //simple case then user's input is equal to start of hint
            var hints = hintSource
                .Where(item => item.Length > lastInput.Length && item.Substring(0, lastInput.Length) == lastInput)
                .Select(hint => new Suggestion
                {
                    Value = hint,
                    HighlightIndexes = Enumerable.Range(0, lastInput.Length).ToArray()
                })
                .ToList();

            //more complex case: tokenize hint and try to search user input from beginning of tokens
            foreach (var item in hintSource)
            {
                var parts = item.Split(new[] { ' ', ';', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

                var candidate = parts.FirstOrDefault(part => part.Length >= lastInput.Length && part.Substring(0, lastInput.Length) == lastInput);
                if (candidate != null)
                {
                    hints.Add(new Suggestion
                    {
                        Value = item,
                        HighlightIndexes = Enumerable.Range(item.IndexOf(candidate, StringComparison.Ordinal), lastInput.Length).ToArray()
                    });
                }
            }

            ////try to split user's input into separate char and find all of them into string

            foreach (var item in hintSource)
            {
                var highlightIndexes = new List<int>();
                var startIndex = 0;
                var found = true;
                for (var i = 0; i < lastInput.Length; i++)
                {
                    if (startIndex >= item.Length)
                    {
                        found = false;
                        break;
                    }

                    var substring = item.Substring(startIndex);
                    var idx = substring.IndexOf(lastInput[i]);
                    if (idx < 0)
                    {
                        //no such symbol in the hints source item
                        found = false;
                        break;
                    }

                    startIndex = startIndex + idx + 1;
                    highlightIndexes.Add(startIndex - 1);
                }

                if (found)
                {
                    hints.Add(new Suggestion
                    {
                        Value = item,
                        HighlightIndexes = highlightIndexes.ToArray()
                    });
                }
            }

            _suggestionsForUserInput = hints;
        }

        private void AddCommandToHistory(string readLine)
        {
            if (!string.IsNullOrWhiteSpace(readLine) && !_commandsHistory.Contains(readLine, StringComparer.OrdinalIgnoreCase))
            {
                _commandsHistory.Add(readLine);
            }

            _historyPosition = _commandsHistory.Count;
        }

        private string GetNextCommandFromHistory()
        {
            if (!_commandsHistory.Any())
                return string.Empty;
            _historyPosition++;
            if (_historyPosition >= _commandsHistory.Count)
            {
                _historyPosition = _commandsHistory.Count - 1;
            }

            return _commandsHistory[_historyPosition];
        }

        private string GetPreviousCommandFromHistory()
        {
            if (!_commandsHistory.Any())
                return string.Empty;

            _historyPosition--;
            if (_historyPosition >= _commandsHistory.Count)
            {
                _historyPosition = _commandsHistory.Count - 1;
            }

            if (_historyPosition < 0)
            {
                _historyPosition = 0;
            }

            return _commandsHistory[_historyPosition];
        }

        private Suggestion GetFirstSuggestion()
        {
            return _suggestionsForUserInput?.FirstOrDefault();
        }

        private Suggestion GetNextSuggestion()
        {
            if (_suggestionsForUserInput == null || !_suggestionsForUserInput.Any())
                return null;

            _suggestionPosition++;
            if (_suggestionPosition >= _suggestionsForUserInput.Count)
            {
                _suggestionPosition = 0;
            }

            return _suggestionsForUserInput[_suggestionPosition];
        }

        private Suggestion GetPreviousSuggestion()
        {
            if (_suggestionsForUserInput == null || !_suggestionsForUserInput.Any())
                return null;

            _suggestionPosition--;
            if (_suggestionPosition < 0)
            {
                _suggestionPosition = _suggestionsForUserInput.Count - 1;
            }

            return _suggestionsForUserInput[_suggestionPosition];
        }

        private class Suggestion
        {
            public string Value { get; set; }

            public int[] HighlightIndexes { get; set; }
        }
    }
}
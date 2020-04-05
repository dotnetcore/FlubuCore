using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using FlubuCore.Infrastructure.Terminal.Commands;
using FlubuCore.Targeting;
using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Git;
using FlubuCore.Tasks.MsSql;
using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Testing;
using FlubuCore.Tasks.Versioning;
using NuGet.Packaging;

namespace FlubuCore.Infrastructure.Terminal
{
    public class FlubuConsole
    {
        private static readonly IDictionary<string, IReadOnlyCollection<Hint>> _commandsHintsSourceDictionary = new Dictionary<string, IReadOnlyCollection<Hint>>(StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<string, Type> _allSupportedExternalProcessesForOptionHints = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        private readonly TargetTree _targetTree;
        private readonly List<string> _commandsHistory = new List<string>();
        private IDictionary<string, IReadOnlyCollection<Hint>> _hintsSourceDictionary;
        private List<Suggestion> _suggestionsForUserInput;
        private int _suggestionPosition;
        private int _historyPosition;
        private string _currentDirectory;
        private Suggestion _lastSuggestion;
        private ConsoleKey _previousPressedKey = ConsoleKey.Clear;

        /// <summary>
        /// Creates new instance of <see cref="FlubuConsole"/> class.
        /// </summary>
        /// <param name="hintsSourceDictionary">Collection containing input hints.</param>
        public FlubuConsole(TargetTree targetTree, IReadOnlyCollection<Hint> defaultHints, IDictionary<string, IReadOnlyCollection<Hint>> hintsSourceDictionary = null)
        {
            _targetTree = targetTree;
            InitializeHints(defaultHints, hintsSourceDictionary);
        }

        /// <summary>
        /// Execute flubu internal command.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>Return's true.</returns>
        public bool ExecuteInternalCommand(string commandLine)
        {
            if (commandLine.Trim().Equals(InternalCommands.Dir, StringComparison.OrdinalIgnoreCase))
            {
                DirectoryInfo objDirectoryInfo = new DirectoryInfo(@".");
                FileInfo[] allFiles = objDirectoryInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly);
                var directories = objDirectoryInfo.GetDirectories("*.*");
                foreach (var directory in directories)
                {
                    Console.WriteLine($"{directory.LastWriteTime}    <DIR>          {directory.Name}");
                }

                var nfi = new NumberFormatInfo { NumberGroupSeparator = "." };
                foreach (var entry in allFiles)
                {
                    Console.WriteLine($"{entry.LastWriteTime}            {entry.Length.ToString("#,##0", nfi)} {entry.Name}");
                }

                Console.WriteLine(string.Empty);
            }
            else if (commandLine.Equals(InternalCommands.CdBack, StringComparison.OrdinalIgnoreCase))
            {
                Directory.SetCurrentDirectory(Path.GetFullPath(".."));
            }
            else if (commandLine.Equals(InternalCommands.CdBackToDisk, StringComparison.OrdinalIgnoreCase))
            {
                Directory.SetCurrentDirectory(Path.GetPathRoot(Directory.GetCurrentDirectory()));
            }
            else if (commandLine.StartsWith(InternalCommands.Cd, StringComparison.OrdinalIgnoreCase))
            {
                var splitedLine = commandLine.Split(' ').ToList();
                if (splitedLine.Count > 1)
                {
                    var newPath = Path.GetFullPath(splitedLine[1]);
                    if (Directory.Exists(newPath))
                    {
                        Directory.SetCurrentDirectory(newPath);
                    }
                }
            }
            else if (commandLine.StartsWith(InternalCommands.Cls, StringComparison.OrdinalIgnoreCase))
            {
                Console.Clear();
                ConsoleUtils.WritePrompt(Directory.GetCurrentDirectory());
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reads input from user using hints. Commands history is supported.
        /// </summary>
        /// <param name="inputRegex"></param>
        /// <returns></returns>
        public string ReadLine(string currentDirectory, string inputRegex = ".*")
        {
            ConsoleKeyInfo input;
            _currentDirectory = currentDirectory;
            Suggestion suggestion = null;
            var userInput = string.Empty;
            var fullInput = string.Empty;
            var readLine = string.Empty;
            var wasUserInput = false;
            var cursorPosition = new ConsoleCursorPosition(currentDirectory.Length + ConsoleUtils.Prompt.Length - 1, Console.CursorTop, Console.WindowWidth);
            ClearConsoleLines(cursorPosition.StartTop, cursorPosition.Top);
            while ((input = Console.ReadKey()).Key != ConsoleKey.Enter)
            {
                var writeSugestionToConsole = false;
                int positionToDelete;
                if (_previousPressedKey == ConsoleKey.Tab && input.Key != ConsoleKey.Tab)
                {
                    _suggestionsForUserInput = null;
                    suggestion = null;
                }

                switch (input.Key)
                {
                    case ConsoleKey.Delete:
                        positionToDelete = cursorPosition.InputLength;
                        if (positionToDelete >= 0 && positionToDelete < userInput.Length)
                        {
                            if (_currentDirectory.Length > cursorPosition.Left - cursorPosition.InputLength)
                            {
                                cursorPosition--;
                            }

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
                        if (_previousPressedKey == ConsoleKey.Tab)
                        {
                            suggestion = GetNextSuggestion();
                            if (suggestion != null)
                            {
                                writeSugestionToConsole = true;
                                userInput = suggestion.Value + ' ';
                            }
                        }
                        else
                        {
                            if (suggestion != null)
                            {
                                writeSugestionToConsole = true;
                                userInput = suggestion.Value + ' ';
                                ////UpdateSuggestionsForUserInput(userInput);
                                suggestion = GetFirstSuggestion();
                                var tmp = fullInput.LastIndexOf(" ");

                                if (fullInput.EndsWith("="))
                                {
                                    cursorPosition = cursorPosition.SetLength(fullInput.Length);
                                }
                                else if (tmp == -1)
                                {
                                    cursorPosition = cursorPosition.SetLength(userInput.Length);
                                }
                                else
                                {
                                    cursorPosition = cursorPosition.SetLength(tmp + userInput.Length);
                                }
                            }
                        }

                        break;
                    case ConsoleKey.Spacebar:
                        userInput = userInput.Insert(cursorPosition.InputLength, " ");
                        wasUserInput = true;
                        cursorPosition++;

                        if (userInput.StartsWith(InternalCommands.Cd))
                        {
                            UpdateDirectorySuggestionsForUserInput(userInput);
                            suggestion = GetFirstSuggestion();
                        }
                        else
                        {
                            _suggestionsForUserInput = null;
                            suggestion = null;
                        }

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

                        wasUserInput = true;

                        if (userInput.StartsWith(InternalCommands.Cd))
                        {
                            UpdateDirectorySuggestionsForUserInput(userInput);
                        }
                        else
                        {
                            UpdateSuggestionsForUserInput(userInput);
                        }

                        suggestion = GetFirstSuggestion();
                        break;
                }

                readLine = suggestion != null ? suggestion.Value : userInput.TrimEnd(' ');

                ClearConsoleLines(cursorPosition.StartTop, cursorPosition.Top);
                var li = fullInput.TrimEnd().LastIndexOf(" ");
                if (li == -1 && !fullInput.StartsWith(InternalCommands.Cd, StringComparison.OrdinalIgnoreCase))
                {
                    if (input.Key == ConsoleKey.Tab && _previousPressedKey == ConsoleKey.Tab && suggestion != null)
                    {
                        fullInput = userInput;
                        cursorPosition = cursorPosition.SetLength(fullInput.Length);
                        ConsoleUtils.Write(fullInput, ConsoleColor.Green);
                    }
                    else
                    {
                        ConsoleUtils.Write(userInput, ConsoleColor.Green);
                        fullInput = userInput;
                    }
                }
                else
                {
                    if (!writeSugestionToConsole)
                    {
                        ConsoleUtils.Write(userInput, ConsoleColor.Green);
                        fullInput = userInput;
                    }
                    else
                    {
                        userInput = WriteSugestionAsUserInput(userInput, suggestion, li, ref fullInput, ref cursorPosition);
                    }
                }

                if (userInput.Any())
                {
                    if (suggestion != null && suggestion.Value != userInput && writeSugestionToConsole == false)
                    {
                        WriteSuggestion(suggestion);
                        WriteOnBottomLine(suggestion.Help);
                        _lastSuggestion = suggestion;
                    }
                    else
                    {
                        var splitedUserInput = userInput.Split(' ');
                        var toCheck = splitedUserInput.Last().Split('=').First();
                        if (_lastSuggestion != null && toCheck.Equals(_lastSuggestion.Value))
                        {
                            WriteOnBottomLine(_lastSuggestion.Help);
                        }
                        else
                        {
                            WriteOnBottomLine(string.Empty);
                        }
                    }
                }
                else
                {
                    WriteOnBottomLine(string.Empty);
                }

                Console.CursorLeft = cursorPosition.Left;
                Console.CursorTop = cursorPosition.Top;
                _previousPressedKey = input.Key;
            }

            Console.WriteLine(string.Empty);
            AddCommandToHistory(fullInput);
            return fullInput;
        }

        private static string WriteSugestionAsUserInput(string userInput, Suggestion suggestion, int li, ref string fullInput, ref ConsoleCursorPosition cursorPosition)
        {
            var suggestionValue = userInput;
            suggestionValue = suggestionValue.TrimEnd();
            if (li == -1)
            {
                li = fullInput.Length - 1;
            }

            if (!suggestionValue.StartsWith("-") && !fullInput.StartsWith(InternalCommands.Cd, StringComparison.OrdinalIgnoreCase))
            {
                suggestionValue = $" {suggestionValue} ";
            }
            else
            {
                if (fullInput.Contains("\\"))
                {
                    li = li + fullInput.Split(' ').Last().Split('\\').First().Length + 2;
                }
                else if (fullInput.Contains("/"))
                {
                    li = li + fullInput.Split(' ').Last().Split('/').First().Length + 2;
                }
                else
                {
                    suggestionValue = $" {suggestionValue}";
                }
            }

            if (suggestion.SuggestionType == HintType.Value)
            {
                suggestionValue = $" -{suggestion.Key}={suggestionValue.Trim()}";
            }

            fullInput = $"{fullInput.Substring(0, li)}{suggestionValue}";
            suggestionValue = fullInput;
            ConsoleUtils.Write(fullInput, ConsoleColor.Green);
            cursorPosition = cursorPosition.SetLength(fullInput.Length);
            return suggestionValue;
        }

        private static void WriteSuggestion(Suggestion suggestion)
        {
            if (suggestion.HighlightIndexes == null || !suggestion.HighlightIndexes.Any())
            {
                ConsoleUtils.Write($" ({suggestion.Value})", suggestion.SuggestionColor);
                return;
            }

            var orderedIndexes = suggestion.HighlightIndexes.OrderBy(v => v).ToArray();
            var idx = 0;

            ConsoleUtils.Write(" (", suggestion.SuggestionColor);
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
                    color = suggestion.SuggestionColor;
                }

                ConsoleUtils.Write(suggestion.Value[i].ToString(), color);
            }

            ConsoleUtils.Write(")", suggestion.SuggestionColor);
        }

        private static void WriteOnBottomLine(string text)
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            int adjustment;
            int window = Console.WindowTop + Console.WindowHeight;
            if (Console.CursorTop == (window - 1))
            {
                adjustment = 0;
            }
            else
            {
                if (Console.CursorTop != (window - 3) && Console.CursorTop != (window - 2))
                {
                    ClearLine(3);
                }

                if (Console.CursorTop != (window - 2))
                {
                    ClearLine(2);
                }

                adjustment = 1;
            }

            ClearLine(adjustment);
            Console.CursorLeft = 0;
            Console.Write(text);

            if (y != 0)
            {
                y = y - 1;
            }

            //// Restore previous position
            Console.SetCursorPosition(x, y);
        }

        private static void ClearLine(int fromBottom)
        {
            var lineToDelete = Console.WindowTop + Console.WindowHeight - fromBottom;

            if (lineToDelete >= Console.BufferHeight)
            {
                Console.BufferHeight = Console.BufferHeight + 1000;
            }

            Console.CursorTop = lineToDelete;
            Console.CursorLeft = 0;
            Console.Write(new string(' ', Console.WindowWidth - 1));
        }

        private void ClearConsoleLines(int startline, int endline)
        {
            for (var i = startline; i <= endline; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(0, startline);
            ConsoleUtils.WritePrompt(_currentDirectory);
            Console.SetCursorPosition(_currentDirectory.Length + 1, startline);
        }

        private void UpdateSuggestionsForUserInput(string userInput)
        {
            _suggestionPosition = 0;

            if (string.IsNullOrEmpty(userInput))
            {
                _suggestionsForUserInput = null;
                return;
            }

            var splitedUserInput = userInput.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            if (splitedUserInput.Count == 0)
            {
                _suggestionsForUserInput = null;
                return;
            }

            var rootCommand = splitedUserInput.First();
            var lastInput = splitedUserInput.Last();
            string prefix = lastInput[0].ToString();
            List<Hint> hintSource = new List<Hint>();
            if (userInput.EndsWith(" "))
            {
                lastInput = $"{lastInput} ";
            }

            string hintSourceKey = null;
            string[] splitedLastInput = null;
            if (lastInput.Contains("="))
            {
                splitedLastInput = lastInput.Split('=');
            }

            if (splitedUserInput.Count > 1 && splitedLastInput != null) //// value hints
            {
                hintSourceKey = splitedLastInput[0].TrimStart('-').ToLower();
                if (_hintsSourceDictionary.ContainsKey(hintSourceKey))
                {
                    hintSource.AddRange(_hintsSourceDictionary[hintSourceKey].ToList());
                    GetSuggestionFromHints(hintSource, splitedLastInput[1], hintSourceKey);
                }

                return;
            }
            else if (splitedUserInput.Count > 1 && _hintsSourceDictionary.ContainsKey(lastInput[0].ToString()))
            {
                hintSourceKey = lastInput[0].ToString();
                lastInput = lastInput.Substring(1);
            }
            else if (_commandsHintsSourceDictionary.ContainsKey(rootCommand) && splitedUserInput.Count < 3)
            {
                hintSource.AddRange(_commandsHintsSourceDictionary[rootCommand]);
            }
            else
            {
                hintSourceKey = "*";
            }

            if (hintSourceKey != null)
            {
                if (!_commandsHintsSourceDictionary.ContainsKey(rootCommand))
                {
                    hintSource.AddRange(_hintsSourceDictionary[hintSourceKey].ToList());
                }

                if (hintSourceKey == "*" && splitedUserInput.Count == 1)
                {
                    hintSource.Add(DotnetCommands.RootCommandHint);
                }
            }

            if (prefix == "-" || prefix == "/")
            {
                var targetHints = GetHintsFromTarget(rootCommand, prefix);
                hintSource.AddRange(targetHints);

                if (targetHints?.Count == 0)
                {
                    foreach (var externalProcessCommand in _allSupportedExternalProcessesForOptionHints)
                    {
                        if (userInput.StartsWith(externalProcessCommand.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            hintSource.AddRange(GetHintsFromTask(prefix, externalProcessCommand.Value));
                        }
                    }
                }
            }

            GetSuggestionFromHints(hintSource, lastInput, hintSourceKey);
        }

        private void UpdateDirectorySuggestionsForUserInput(string userInput)
        {
            var splitedUserInput = userInput.Split(' ').ToList();
            if (splitedUserInput.Count == 0 || splitedUserInput.Count > 2)
            {
                _suggestionsForUserInput = null;
                return;
            }

            var directoryInput = splitedUserInput.Last();
            var searchPath = ".";

            var splitedDirectories = directoryInput.Split('\\', '/');
            if (splitedDirectories.Length > 1)
            {
                string tmp = string.Empty;
                for (int i = 0; i < splitedDirectories.Length - 1; i++)
                {
                    tmp = $"{tmp}/{splitedDirectories[i]}";
                }

                searchPath = $"{searchPath}{tmp}";
            }

            if (!Directory.Exists(searchPath))
            {
                _suggestionsForUserInput = null;
            }

            DirectoryInfo objDirectoryInfo = new DirectoryInfo(searchPath);
            var directories = objDirectoryInfo.GetDirectories("*.*");
            List<Hint> hintSource = new List<Hint>();
            hintSource.AddRange(directories.Select(d => new Hint() { Name = d.Name, OnlySimpleSearh = true }));
            var lastInput = splitedDirectories.Last();
            GetSuggestionFromHints(hintSource, lastInput, "*");
        }

        private void GetSuggestionFromHints(List<Hint> hintSource, string lastInput, string hintSourceKey)
        {
            if (hintSource.All(item => item.Name.Length < lastInput.Length))
            {
                _suggestionsForUserInput = null;
                return;
            }
#if NETSTANDARD1_6
            //simple case then user's input is equal to start of hint
            var hints = hintSource
                .Where(item => item.Name.Length > lastInput.Length && item.Name.Substring(0, lastInput.Length) == lastInput)
                .Select(hint => new Suggestion
                {
                    Value = hint.Name,
                    Help = hint.Help,
                    Key = hintSourceKey,
                    SuggestionColor = hint.HintColor,
                    SuggestionType = hint.HintType,
                    HighlightIndexes = Enumerable.Range(0, lastInput.Length).ToArray()
                })
                .ToList();
#else
            //simple case then user's input is equal to start of hint
            var hints = hintSource
                .Where(item => item.Name.Length > lastInput.Length && item.Name.Substring(0, lastInput.Length)
                                   .Equals(lastInput, StringComparison.OrdinalIgnoreCase))
                .Select(hint => new Suggestion
                {
                    Value = hint.Name,
                    Help = hint.Help,
                    Key = hintSourceKey,
                    SuggestionColor = hint.HintColor,
                    SuggestionType = hint.HintType,
                    HighlightIndexes = Enumerable.Range(0, lastInput.Length).ToArray()
                })
                .ToList();
#endif

            //more complex case: tokenize hint and try to search user input from beginning of tokens
            foreach (var item in hintSource)
            {
                var parts = item.Name.Split(new[] { ' ', ';', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

                string candidate;
#if NETSTANDARD1_6
                candidate =
 parts.FirstOrDefault(part => part.Length >= lastInput.Length && part.Substring(0, lastInput.Length) == lastInput);
#else
                candidate = parts.FirstOrDefault(part => part.Length >= lastInput.Length && part.Substring(0, lastInput.Length)
                                                             .Equals(lastInput, StringComparison.InvariantCultureIgnoreCase));
#endif
                if (candidate != null)
                {
                    hints.Add(new Suggestion
                    {
                        Value = item.Name,
                        Help = item.Help,
                        Key = hintSourceKey,
                        SuggestionColor = item.HintColor,
                        SuggestionType = item.HintType,
                        HighlightIndexes = Enumerable
                            .Range(item.Name.IndexOf(candidate, StringComparison.Ordinal), lastInput.Length).ToArray()
                    });
                }
            }

            ////try to split user's input into separate char and find all of them into string

            foreach (var item in hintSource)
            {
                if (item.OnlySimpleSearh)
                {
                    continue;
                }

                var highlightIndexes = new List<int>();
                var startIndex = 0;
                var found = true;
                for (var i = 0; i < lastInput.Length; i++)
                {
                    if (startIndex >= item.Name.Length)
                    {
                        found = false;
                        break;
                    }

                    var substring = item.Name.Substring(startIndex);
#if NETSTANDARD1_6
                    var idx = substring.IndexOf(lastInput[i]);
#else
                    var idx = substring.IndexOf(lastInput[i].ToString(), StringComparison.OrdinalIgnoreCase);
#endif
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
                        Value = item.Name,
                        Help = item.Help,
                        SuggestionColor = item.HintColor,
                        SuggestionType = item.HintType,
                        HighlightIndexes = highlightIndexes.ToArray()
                    });
                }
            }

            if (hintSourceKey != null && hintSourceKey != "*")
            {
                foreach (var hint in hints)
                {
                    if (!hint.Value.StartsWith(hintSourceKey) && hint.SuggestionType != HintType.Value)
                    {
                        hint.Value = $"{hintSourceKey}{hint.Value}";
                    }
                }
            }

            _suggestionsForUserInput = hints;
        }

        private List<Hint> GetHintsFromTarget(string targetName, string prefix)
        {
            List<Hint> targetSpecificHints = new List<Hint>();
            if (_targetTree.HasTarget(targetName))
            {
                List<ITargetInternal> targets = new List<ITargetInternal>();
                var mainTarget = _targetTree.GetTarget(targetName);
                targets.Add(mainTarget);

                AddDependencies(mainTarget, targets);

                foreach (var target in targets)
                {
                    foreach (var taskGroup in target.TasksGroups)
                    {
                        foreach (var task in taskGroup.Tasks)
                        {
                            var type = task.task.GetType();
                            targetSpecificHints.AddRange(GetHintsFromTask(prefix, type));
                        }
                    }
                }
            }

            return targetSpecificHints.Distinct().ToList();
        }

        private List<Hint> GetHintsFromTask(string prefix, Type type)
        {
            List<Hint> taskHints = new List<Hint>();
            var methods = type.GetRuntimeMethods();
            methods = methods.Where(m => m.GetCustomAttributes(typeof(ArgKeyAttribute), false).ToList().Count > 0);

            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<ArgKeyAttribute>();

                if (attribute == null || attribute.Keys.Length == 0)
                {
                    continue;
                }

                if (attribute.Keys[0].StartsWith(prefix.ToString()))
                {
                    var help = method.GetSummary();
                    help = Regex.Replace(help, @"\s+", " ");
                    help = help.Replace(Environment.NewLine, string.Empty);
                    var hint = new Hint()
                    {
                        Name = attribute.Keys[0],
                        Help = help,
                    };

                    taskHints.Add(hint);
                }
            }

            return taskHints;
        }

        private void AddDependencies(ITargetInternal target, List<ITargetInternal> targets)
        {
            foreach (var dependencyName in target.Dependencies)
            {
                var dependantTarget = _targetTree.GetTarget(dependencyName.Key);
                targets.Add(dependantTarget);
                AddDependencies(dependantTarget, targets);
            }
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

        private void InitializeHints(IReadOnlyCollection<Hint> defaultHints, IDictionary<string, IReadOnlyCollection<Hint>> hintsSourceDictionary)
        {
            _hintsSourceDictionary = hintsSourceDictionary;

            if (_hintsSourceDictionary == null)
            {
                _hintsSourceDictionary = new Dictionary<string, IReadOnlyCollection<Hint>>();
            }

            _hintsSourceDictionary.Add("*", defaultHints);

            if (!_hintsSourceDictionary.ContainsKey("-"))
            {
                _hintsSourceDictionary.Add("-", new List<Hint>());
            }

            if (!_hintsSourceDictionary.ContainsKey("/"))
            {
                _hintsSourceDictionary.Add("/", new List<Hint>());
            }

            if (_commandsHintsSourceDictionary.Count == 0)
            {
                _commandsHintsSourceDictionary.Add(GitCommands.GitCommandHints);
                _commandsHintsSourceDictionary.Add(DotnetCommands.DotnetCommandHints);
                _commandsHintsSourceDictionary.Add(DockerCommands.GitCommandHints);
                _commandsHintsSourceDictionary.Add(ChocolateyCommands.ChocoCommandHints);
            }

            if (_allSupportedExternalProcessesForOptionHints.Count == 0)
            {
                _allSupportedExternalProcessesForOptionHints.AddRange(DotnetCommands.SupportedExternalProcesses);
                _allSupportedExternalProcessesForOptionHints.AddRange(DockerCommands.SupportedExternalProcesses);
                _allSupportedExternalProcessesForOptionHints.AddRange(GitCommands.SupportedExternalProcesses);
                _allSupportedExternalProcessesForOptionHints.AddRange(ChocolateyCommands.SupportedExternalProcesses);
            }
        }

        private class Suggestion
        {
            public string Value { get; set; }

            public string Help { get; set; }

            public string Key { get; set; }

            public HintType SuggestionType { get; set; }

            public ConsoleColor SuggestionColor { get; set; }

            public int[] HighlightIndexes { get; set; }
        }
    }
}

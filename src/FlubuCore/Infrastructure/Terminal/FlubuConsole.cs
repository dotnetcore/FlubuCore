using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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
        private static Dictionary<string, Type> _supportedExternalProcesses = new Dictionary<string, Type>()
        {
            { "dotnet build", typeof(DotnetBuildTask) },
            { "dotnet clean", typeof(DotnetCleanTask) },
            { "dotnet pack", typeof(DotnetPackTask) },
            { "dotnet publish", typeof(DotnetPublishTask) },
            { "dotnet test", typeof(DotnetTestTask) },
            { "dotnet restore", typeof(DotnetRestoreTask) },
            { "dotnet nuget push", typeof(DotnetNugetPushTask) },
            { "dotnet msbuild", typeof(DotnetMsBuildTask) },
            { "dotnet tool install", typeof(DotnetToolInstall) },
            { "dotnet tool uninstall", typeof(DotnetToolUninstall) },
            { "dotnet tool update", typeof(DotnetToolUpdate) },
            { "gitversion", typeof(GitVersionTask) },
            { "sqlcmd", typeof(SqlCmdTask) },
            { "coverlet", typeof(CoverletTask) },
            { "git add", typeof(GitAddTask) },
            { "git checkout", typeof(GitCheckoutTask) },
            { "git clone", typeof(GitCloneTask) },
            { "git commit", typeof(GitCommitTask) },
            { "git pull", typeof(GitPullTask) },
            { "git push", typeof(GitPushTask) },
            { "git tag", typeof(GitTagTask) },
            { "git submodule", typeof(GitSubmoduleTask) },
            { "git rm", typeof(GitRemoveFilesTask) }
        };

        private static List<Hint> _dotnetCommands = new List<Hint>
        {
            new Hint { Name = "dotnet build", Help = "The dotnet build command builds the project and its dependencies into a set of binaries.", OnlySimpleSearh = true },
            new Hint { Name = "dotnet test", Help = "The dotnet test command is used to execute unit tests in a given project.", OnlySimpleSearh = true },
            new Hint { Name = "dotnet pack", Help = "The dotnet pack command builds the project and creates NuGet packages.", OnlySimpleSearh = true },
            new Hint { Name = "dotnet publish", Help = "Packs the application and its dependencies into a folder for deployment to a hosting system.", OnlySimpleSearh = true },
            new Hint { Name = "dotnet restore", Help = "The dotnet restore command uses NuGet to restore dependencies as well as project-specific tools that are specified in the project file.", OnlySimpleSearh = true },
            new Hint { Name = "dotnet nuget push", Help = "The dotnet nuget push command pushes a package to the server and publishes it.", OnlySimpleSearh = true },
            new Hint { Name = "dotnet clean", Help = "The dotnet clean command cleans the output of the previous build.", OnlySimpleSearh = true },
            new Hint { Name = "dotnet msbuild", Help = "Builds the specified targets in the project file.", OnlySimpleSearh = true },
            new Hint { Name = "dotnet tool install", Help = "The dotnet tool install command provides a way for you to install .NET Core Global Tools on your machine. ", OnlySimpleSearh = true },
            new Hint { Name = "dotnet tool uninstall", Help = "Uninstalls the specified .NET Core Global Tool from your machine.", OnlySimpleSearh = true },
            new Hint { Name = "dotnet tool update", Help = "Updates the specified .NET Core Global Tool on your machine.", OnlySimpleSearh = true },
        };

        private readonly TargetTree _targetTree;
        private readonly IDictionary<char, IReadOnlyCollection<Hint>> _hintsSourceDictionary;
        private readonly List<string> _commandsHistory = new List<string>();
        private List<Suggestion> _suggestionsForUserInput;
        private int _suggestionPosition;
        private int _historyPosition;
        private string _currentDirectory;
        private Suggestion _lastSuggestion;

        /// <summary>
        /// Creates new instance of <see cref="FlubuConsole"/> class
        /// </summary>
        /// <param name="hintsSourceDictionary">Collection containing input hints</param>
        public FlubuConsole(TargetTree targetTree, IReadOnlyCollection<Hint> defaultHints, IDictionary<char, IReadOnlyCollection<Hint>> hintsSourceDictionary = null)
        {
            _targetTree = targetTree;
            _hintsSourceDictionary = hintsSourceDictionary;

            if (_hintsSourceDictionary == null)
            {
                _hintsSourceDictionary = new Dictionary<char, IReadOnlyCollection<Hint>>();
            }

            _hintsSourceDictionary.Add('*', defaultHints);

            if (!_hintsSourceDictionary.ContainsKey('-'))
            {
                _hintsSourceDictionary.Add('-', new List<Hint>());
            }

            if (!_hintsSourceDictionary.ContainsKey('/'))
            {
                _hintsSourceDictionary.Add('/', new List<Hint>());
            }
        }

        /// <summary>
        /// Execute flubu internal command.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>Return's true</returns>
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

                foreach (var entry in allFiles)
                {
                    Console.WriteLine($"{entry.LastWriteTime}            {entry.Length} {entry.Name}");
                }

                Console.WriteLine(string.Empty);
            }
            else if (commandLine.Equals("cd.."))
            {
                Directory.SetCurrentDirectory(Directory.GetCurrentDirectory() + "/..");
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
        /// <param name="hintColor"></param>
        /// <returns></returns>
        public string ReadLine(string currentDirectory, string inputRegex = ".*", ConsoleColor hintColor = ConsoleColor.DarkGray)
        {
            ConsoleKeyInfo input;
            _currentDirectory = currentDirectory;
            Suggestion suggestion = null;
            var userInput = string.Empty;
            var fullInput = string.Empty;
            var readLine = string.Empty;
            var wasUserInput = false;
            var target = string.Empty;
            var cursorPosition = new ConsoleCursorPosition(currentDirectory.Length + ConsoleUtils.Prompt.Length - 1, Console.CursorTop, Console.WindowWidth);
            ClearConsoleLines(cursorPosition.StartTop, cursorPosition.Top);
            while ((input = Console.ReadKey()).Key != ConsoleKey.Enter)
            {
                var writeSugestionToConsole = false;
                int positionToDelete;
                switch (input.Key)
                {
                    case ConsoleKey.Delete:
                        positionToDelete = currentDirectory.Length + cursorPosition.InputLength - 1;
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

                        if (userInput.StartsWith(InternalCommands.Cd))
                        {
                            UpdateDirectorySuggestionsForUserInput(userInput);
                            suggestion = GetFirstSuggestion();
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
                    ConsoleUtils.Write(userInput, ConsoleColor.Green);
                    fullInput = userInput;
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
                        userInput = WriteSugestionAsUserInput(userInput, li, ref fullInput, ref cursorPosition);
                    }
                }

                if (userInput.Any())
                {
                    if (suggestion != null && suggestion.Value != userInput && writeSugestionToConsole == false)
                    {
                        WriteSuggestion(suggestion, hintColor);
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
            }

            Console.WriteLine(string.Empty);
            AddCommandToHistory(readLine);
            return fullInput;
        }

        private static string WriteSugestionAsUserInput(string userInput, int li, ref string fullInput,
            ref ConsoleCursorPosition cursorPosition)
        {
            userInput = userInput.TrimEnd();
            if (li == -1)
            {
                li = fullInput.Length - 1;
            }

            if (!userInput.StartsWith("-") && !fullInput.StartsWith(InternalCommands.Cd, StringComparison.OrdinalIgnoreCase))
            {
                userInput = $" {userInput} ";
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
                    userInput = $" {userInput}";
                }
            }

            fullInput = $"{fullInput.Substring(0, li)}{userInput}";
            userInput = fullInput;
            ConsoleUtils.Write(fullInput, ConsoleColor.Green);
            cursorPosition = cursorPosition.SetLength(fullInput.Length);
            return userInput;
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

            //// Restore previous position
            Console.SetCursorPosition(x, y);
        }

        private static void ClearLine(int fromBottom)
        {
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - fromBottom;
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

            var targetName = splitedUserInput.First();
            var lastInput = splitedUserInput.Last();
            char prefix = lastInput[0];
            if (userInput.EndsWith(" "))
            {
                lastInput = $"{lastInput} ";
            }

            char? hintSourceKey = null;
            if (splitedUserInput.Count > 1 && _hintsSourceDictionary.ContainsKey(lastInput[0]))
            {
                hintSourceKey = lastInput[0];
                lastInput = lastInput.Substring(1);
            }
            else
            {
                if (!splitedUserInput[0].Equals("dotnet", StringComparison.OrdinalIgnoreCase))
                {
                    hintSourceKey = '*';
                }
            }

            List<Hint> hintSource = null;
            if (hintSourceKey.HasValue)
            {
                hintSource = _hintsSourceDictionary[hintSourceKey.Value].ToList();
            }

            if (hintSourceKey == '*')
            {
                if (splitedUserInput.Count == 1)
                {
                    hintSource.AddRange(_dotnetCommands);
                }

                if (splitedUserInput.Count == 2 && splitedUserInput[0].Equals("dotnet", StringComparison.OrdinalIgnoreCase))
                {
                    hintSource.AddRange(_dotnetCommands);
                }
            }

            if (hintSource == null)
            {
                _suggestionsForUserInput = null;
                return;
            }

            if (prefix == '-' || prefix == '/')
            {
                var targetHints = GetHintsFromTarget(targetName, prefix);
                hintSource.AddRange(targetHints);

                if (targetHints?.Count == 0)
                {
                    var allProcesses = new Dictionary<string, Type>();
                    allProcesses.AddRange(_supportedExternalProcesses);
                    allProcesses.AddRange(DockerCommands.SupportedExternalProcesses);
                    foreach (var externalProcessCommand in allProcesses)
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
            GetSuggestionFromHints(hintSource, lastInput, '*');
        }

        private void GetSuggestionFromHints(List<Hint> hintSource, string lastInput, char? hintSourceKey)
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
                        HighlightIndexes = highlightIndexes.ToArray()
                    });
                }
            }

            if (hintSourceKey != '*')
            {
                foreach (var hint in hints)
                {
                    if (!hint.Value.StartsWith(hintSourceKey.ToString()))
                    {
                        hint.Value = $"{hintSourceKey}{hint.Value}";
                    }
                }
            }

            _suggestionsForUserInput = hints;
        }

        private List<Hint> GetHintsFromTarget(string targetName, char prefix)
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

        private List<Hint> GetHintsFromTask(char prefix, Type type)
        {
            List<Hint> taskHints = new List<Hint>();
            var methods = type.GetRuntimeMethods();
            methods = methods.Where(m => m.GetCustomAttributes(typeof(ArgKey), false).ToList().Count > 0);

            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<ArgKey>();

                if (attribute == null || attribute.Keys.Length == 0)
                {
                    continue;
                }

                if (attribute.Keys[0].StartsWith(prefix.ToString()))
                {
                    var help = method.GetSummary();
                    foreach (var key in attribute.Keys)
                    {
                        var hint = new Hint()
                        {
                            Name = key,
                            Help = help,
                        };

                        taskHints.Add(hint);
                    }
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

        private class Suggestion
        {
            public string Value { get; set; }

            public string Help { get; set; }

            public int[] HighlightIndexes { get; set; }
        }
    }
}
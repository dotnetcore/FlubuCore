using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlubuCore.Context;
using FlubuCore.Infrastructure.Terminal;
using FlubuCore.Tasks;
using FlubuCore.Templating.Models;
using GlobExpressions;

namespace FlubuCore.Templating.Tasks
{
    public class TemplateReplacementTokenTask : IFlubuTemplateTask
    {
        private readonly IFlubuSession _flubuSession;

        private List<Tuple<string, string>> _replacementTokens;

        public TemplateReplacementTokenTask(IFlubuSession flubuSession)
        {
            _flubuSession = flubuSession;
        }

        public void BeforeFileProcessing(TemplateModel template, List<string> files)
        {
            _replacementTokens = GetReplacementTokens(template);
        }

        public void BeforeFileCopy(string sourceFilePath)
        {
            if (_replacementTokens.Any())
            {
                _flubuSession.Tasks().ReplaceTokensTask(sourceFilePath)
                    .Replace(_replacementTokens.ToArray())
                    .DoNotLogTaskExecutionInfo()
                    .WithLogLevel(LogLevel.None)
                    .Execute(_flubuSession);
            }
        }

        public void AfterFileCopy(string destinationFilePath)
        {
        }

        public void AfterFileProcessing(TemplateModel template)
        {
        }

        private List<Tuple<string, string>> GetReplacementTokens(TemplateModel templateData)
        {
            var replacementTokens = new List<Tuple<string, string>>();

            if (templateData?.Tokens != null && templateData.Tokens.Count > 0)
            {
                foreach (var token in templateData.Tokens)
                {
                    switch (token.InputType)
                    {
                        case InputType.Options:
                        {
                            Console.WriteLine(string.Empty);
                            for (int i = 0; i < token.Values.Count; i++)
                            {
                                Console.WriteLine($"{i} - {token.Values[i]}");
                            }

                            Console.WriteLine(string.Empty);
                            bool correctOption = false;
                            int chosenOption;

                            do
                            {
                                Console.Write($"Choose replacement value for token '{token.Token}':");
                                var key = Console.ReadLine().Trim();
                                if (int.TryParse(key, out chosenOption))
                                {
                                    if (chosenOption >= 0 && chosenOption < token.Values.Count)
                                    {
                                        correctOption = true;
                                    }
                                }
                            }
                            while (correctOption);

                            replacementTokens.Add(new Tuple<string, string>(token.Token, token.Values[chosenOption]));

                            break;
                        }

                        default:
                        {
                            var initialText = !string.IsNullOrEmpty(token.Description)
                                ? token.Description
                                : $"Enter replacement value for token {token.Token}";

                            bool directoryAndFilesSuggestions = token.InputType == InputType.Files;

                            string allowedFileExtensionGlobPattern = null;

                            if (token.Files?.AllowedFileExtension != null)
                            {
                                allowedFileExtensionGlobPattern = token.Files.AllowedFileExtension.StartsWith("*.")
                                    ? token.Files.AllowedFileExtension
                                    : $"*.{token.Files.AllowedFileExtension}";
                            }

                            var hints = new List<Hint>();

                            if (token.InputType == InputType.Hints)
                            {
                                hints.AddRange(token.Values.Select(value => new Hint()
                                {
                                    Name = value,
                                    HintColor = ConsoleColor.DarkGray
                                }));
                            }

                            var console = new FlubuConsole(hints, options: o =>
                            {
                                o.WritePrompt = false;
                                o.InitialText = initialText;
                                o.InitialHelp = token.Help;
                                o.OnlyDirectoriesSuggestions = directoryAndFilesSuggestions;
                                o.IncludeFileSuggestions = directoryAndFilesSuggestions;
                                o.FileSuggestionsSearchPattern = allowedFileExtensionGlobPattern;
                                o.DefaultSuggestion = token.DefaultValue;
                            });

                            bool isRightFileExtension = true;
                            string newValue;
                            do
                            {
                                newValue = console.ReadLine().Trim();
                                if (!string.IsNullOrEmpty(allowedFileExtensionGlobPattern))
                                {
                                    isRightFileExtension =
                                        CheckFileExtension(newValue, allowedFileExtensionGlobPattern);
                                }
                            }
                            while (!isRightFileExtension);

                            if (!string.IsNullOrEmpty(newValue))
                            {
                                replacementTokens.Add(new Tuple<string, string>(token.Token, newValue));
                            }

                            break;
                        }
                    }
                }
            }

            return replacementTokens;
        }

        private bool CheckFileExtension(string filePath, string allowedFileExtensionGLobPattern)
        {
            bool isRightFileExtension;
            if (Glob.IsMatch(filePath, allowedFileExtensionGLobPattern))
            {
                if (File.Exists(filePath))
                {
                    isRightFileExtension = true;
                }
                else
                {
                    _flubuSession.LogInfo("File not found");
                    isRightFileExtension = false;
                }
            }
            else
            {
                isRightFileExtension = false;
                _flubuSession.LogInfo("Not allowed file extension");
            }

            return isRightFileExtension;
        }
    }
}

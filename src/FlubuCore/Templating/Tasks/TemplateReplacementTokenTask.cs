using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Infrastructure.Terminal;
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

        public void BeforeFileCopy(string sourcefilePath)
        {
            if (_replacementTokens.Any())
            {
                _flubuSession.Tasks().ReplaceTokensTask(sourcefilePath)
                    .Replace(_replacementTokens.ToArray()).Execute(_flubuSession);
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
                    var initialText = !string.IsNullOrEmpty(token.Description)
                        ? token.Description
                        : $"Enter replacement value for {token.Token}";

                    bool directoryAndFilesSuggestions = token.InputType.HasValue && token.InputType.Value == InputType.Files;

                    string allowedFileExtensionGlobPattern = null;

                    if (token.Files?.AllowedFileExtension != null)
                    {
                        allowedFileExtensionGlobPattern = token.Files.AllowedFileExtension.StartsWith("*.")
                            ? token.Files.AllowedFileExtension
                            : $"*.{token.Files.AllowedFileExtension}";
                    }

                    var console = new FlubuConsole(new List<Hint>(), options: o =>
                    {
                        o.WritePrompt = false;
                        o.InitialText = initialText;
                        o.OnlyDirectoriesSuggestions = directoryAndFilesSuggestions;
                        o.IncludeFileSuggestions = directoryAndFilesSuggestions;
                        o.FileSuggestionsSearchPattern = allowedFileExtensionGlobPattern;
                    });

                    bool isRightFileExtension = true;
                    string newValue;
                    do
                    {
                        newValue = console.ReadLine();
                        if (!string.IsNullOrEmpty(allowedFileExtensionGlobPattern))
                        {
                            isRightFileExtension = CheckFileExtension(newValue, allowedFileExtensionGlobPattern);
                        }
                    }
                    while (!isRightFileExtension);

                    replacementTokens.Add(new Tuple<string, string>(token.Token, newValue));
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

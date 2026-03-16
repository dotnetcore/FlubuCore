using System;
using System.Collections.Generic;
using System.Linq;
using FlubuCore.Commanding.Internal;
using FlubuCore.Context;
using FlubuCore.Scripting;
using FlubuCore.Targeting;

namespace FlubuCore.Commanding
{
    public class ShellCompletionProvider
    {
        private static readonly string[] BuiltInOptions =
        {
            "--script",
            "--interactive",
            "--parallel",
            "--debug",
            "--dryRun",
            "--noColor",
            "--nodeps",
            "--noint",
            "--ci",
            "--help",
        };

        private static readonly string[] BuiltInCommands =
        {
            InternalFlubuCommands.Setup,
            InternalFlubuCommands.New,
        };

        /// <summary>
        /// Writes the shell completion registration script to stdout.
        /// </summary>
        public static void WriteSetupScript(string shell, string toolName = "flubu")
        {
            string script;
            switch (shell.ToLowerInvariant())
            {
                case "bash":
                    script = GetBashCompletionScript(toolName);
                    break;
                case "zsh":
                    script = GetZshCompletionScript(toolName);
                    break;
                case "pwsh":
                case "powershell":
                    script = GetPwshCompletionScript(toolName);
                    break;
                case "fish":
                    script = GetFishCompletionScript(toolName);
                    break;
                default:
                    Console.Error.WriteLine($"Unsupported shell: {shell}. Supported shells: bash, zsh, pwsh, fish.");
                    return;
            }

            Console.Write(script);
        }

        /// <summary>
        /// Writes completion candidates to stdout, one per line.
        /// </summary>
        public void WriteCompletions(
            string partialCommandLine,
            IFlubuSession flubuSession,
            IBuildScript script,
            IScriptProperties scriptProperties,
            ITargetCreator targetCreator)
        {
            var completions = GetCompletions(partialCommandLine, flubuSession, script, scriptProperties, targetCreator);
            foreach (var completion in completions)
            {
                Console.WriteLine(completion);
            }
        }

        public IReadOnlyList<string> GetCompletions(
            string partialCommandLine,
            IFlubuSession flubuSession,
            IBuildScript script,
            IScriptProperties scriptProperties,
            ITargetCreator targetCreator)
        {
            var tokens = TokenizeCommandLine(partialCommandLine);
            var wordToComplete = GetWordToComplete(partialCommandLine, tokens);

            if (wordToComplete.StartsWith("-"))
            {
                var scriptArgs = DiscoverScriptArgs(script, scriptProperties);
                var candidates = BuiltInOptions.Concat(scriptArgs).ToList();
                return FilterCandidates(candidates, wordToComplete);
            }

            var targetNames = DiscoverTargets(flubuSession, script, targetCreator);
            var targetCandidates = targetNames.Concat(BuiltInCommands).ToList();
            return FilterCandidates(targetCandidates, wordToComplete);
        }

        private static List<string> DiscoverTargets(
            IFlubuSession flubuSession,
            IBuildScript script,
            ITargetCreator targetCreator)
        {
            if (script == null || flubuSession == null)
            {
                return new List<string>();
            }

            try
            {
                if (script is DefaultBuildScript defaultBuildScript)
                {
                    defaultBuildScript.ConfigureDefaultProps(flubuSession);
                    defaultBuildScript.ConfigureBuildPropertiesInternal(flubuSession);
                    defaultBuildScript.ConfigureTargetsInternal(flubuSession);
                }

                targetCreator.CreateTargetFromMethodAttributes(script, flubuSession);

                return flubuSession.TargetTree.GetTargetNames().ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        private static List<string> DiscoverScriptArgs(
            IBuildScript script,
            IScriptProperties scriptProperties)
        {
            if (script == null || scriptProperties == null)
            {
                return new List<string>();
            }

            try
            {
                return scriptProperties.GetPropertiesHints(script)
                    .Select(h => h.Name)
                    .ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        private static List<string> TokenizeCommandLine(string commandLine)
        {
            if (string.IsNullOrWhiteSpace(commandLine))
            {
                return new List<string>();
            }

            var tokens = commandLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            // Strip leading "flubu" or "flubu.exe" command name
            if (tokens.Count > 0 &&
                (tokens[0].Equals("flubu", StringComparison.OrdinalIgnoreCase) ||
                 tokens[0].Equals("flubu.exe", StringComparison.OrdinalIgnoreCase)))
            {
                tokens.RemoveAt(0);
            }

            return tokens;
        }

        private static string GetWordToComplete(string commandLine, List<string> tokens)
        {
            if (string.IsNullOrEmpty(commandLine) || commandLine.EndsWith(" ") || tokens.Count == 0)
            {
                return string.Empty;
            }

            return tokens.Last();
        }

        private static IReadOnlyList<string> FilterCandidates(List<string> candidates, string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                return candidates.OrderBy(c => c, StringComparer.OrdinalIgnoreCase).ToList();
            }

            return candidates
                .Where(c => c.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .OrderBy(c => c, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string GetBashCompletionScript(string toolName)
        {
            return $@"_{toolName}_completions() {{
    local IFS=$'\n'
    local cur=""${{COMP_WORDS[COMP_CWORD]}}""
    COMPREPLY=($(compgen -W ""$({toolName} --completions ""${{COMP_LINE}}"" 2>/dev/null)"" -- ""$cur""))
}}
complete -o default -F _{toolName}_completions {toolName}
";
        }

        private static string GetZshCompletionScript(string toolName)
        {
            return $@"_{toolName}_completions() {{
    local completions
    completions=($({toolName} --completions ""${{words[*]}}"" 2>/dev/null))
    compadd -- $completions
}}
compdef _{toolName}_completions {toolName}
";
        }

        private static string GetPwshCompletionScript(string toolName)
        {
            return $@"Register-ArgumentCompleter -CommandName {toolName} -Native -ScriptBlock {{
    param($wordToComplete, $commandAst, $cursorPosition)
    $line = $commandAst.ToString()
    & {toolName} --completions ""$line"" 2>$null | ForEach-Object {{
        [System.Management.Automation.CompletionResult]::new($_, $_, 'ParameterValue', $_)
    }}
}}
";
        }

        private static string GetFishCompletionScript(string toolName)
        {
            return $@"complete -c {toolName} -f -a '({toolName} --completions (commandline -cp) 2>/dev/null)'
";
        }
    }
}

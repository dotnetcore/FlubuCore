using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
#if !NETSTANDARD1_6
using System.Drawing;
#endif
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DotNet.Cli.Flubu.Commanding;
using FlubuCore.Commanding;
using FlubuCore.Context;
using FlubuCore.Infrastructure.Terminal;
using FlubuCore.IO;
using FlubuCore.Targeting;
using FlubuCore.Tasks.NetCore;
using FlubuCore.WebApi.Client;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.DotNet.Cli.Utils;

namespace FlubuCore.Scripting
{
    public abstract class DefaultBuildScript : IBuildScript
    {
        public int Run(IFlubuSession flubuSession)
        {
            try
            {
                BeforeBuildExecution(flubuSession);
                RunBuild(flubuSession);
                flubuSession.Complete();
                AfterBuildExecution(flubuSession);
                return 0;
            }
            catch (TargetNotFoundException e)
            {
                flubuSession.ResetDepth();
                OnBuildFailed(flubuSession, e);
                AfterBuildExecution(flubuSession);
                if (flubuSession.Args.RethrowOnException)
                    throw;

                flubuSession.LogError(e.Message);
                return 3;
            }
            catch (WebApiException e)
            {
                flubuSession.ResetDepth();
                OnBuildFailed(flubuSession, e);
                AfterBuildExecution(flubuSession);
                if (flubuSession.Args.RethrowOnException)
                    throw;

                return 1;
            }
            catch (FlubuException e)
            {
                flubuSession.ResetDepth();
                OnBuildFailed(flubuSession, e);

                if (!flubuSession.Args.RethrowOnException)
                {
#if !NETSTANDARD1_6
                    flubuSession.LogError($"ERROR: {e.Message}", Color.Red);
#else
                    flubuSession.LogError($"error: {e.Message}");
#endif
                }

                AfterBuildExecution(flubuSession);
                if (flubuSession.Args.RethrowOnException)
                    throw;

                return 1;
            }
            catch (Exception e)
            {
                flubuSession.ResetDepth();
                OnBuildFailed(flubuSession, e);

                if (!flubuSession.Args.RethrowOnException)
                {
#if !NETSTANDARD1_6
                    flubuSession.LogError($"ERROR: {e}", Color.Red);
#else
                    flubuSession.LogError($"error: {e}");
#endif
                }

                AfterBuildExecution(flubuSession);
                if (flubuSession.Args.RethrowOnException)
                    throw;

                return 2;
            }
        }

        protected abstract void ConfigureBuildProperties(IBuildPropertiesContext context);

        protected abstract void ConfigureTargets(ITaskContext context);

        private static (List<string> targetsToRun, bool unknownTarget, List<string> notFoundTargets) ParseCmdLineArgs(
            List<string> mainCommands, TargetTree targetTree)
        {
            if (mainCommands == null || mainCommands.Count == 0) return (null, false, null);

            if (targetTree.HasAllTargets(mainCommands, out var notFoundTargets))
                return (mainCommands, false, null);

            return (new List<string> { "help" }, true, notFoundTargets);
        }

        private void RunBuild(IFlubuSession flubuSession)
        {
            bool resetTargetTree = false;

            ConfigureDefaultProps(flubuSession);

            ConfigureBuildProperties(flubuSession);

            ConfigureDefaultTargets(flubuSession);

            ScriptProperties.SetPropertiesFromScriptArg(this, flubuSession);

            TargetCreator.CreateTargetFromMethodAttributes(this, flubuSession);

            ConfigureTargets(flubuSession);

            var targetsInfo = default((List<string> targetsToRun, bool unknownTarget, List<string> notFoundTargets));

            if (!flubuSession.Args.InteractiveMode)
            {
                targetsInfo = ParseCmdLineArgs(flubuSession.Args.MainCommands, flubuSession.TargetTree);
                flubuSession.UnknownTarget = targetsInfo.unknownTarget;
                if (targetsInfo.targetsToRun == null || targetsInfo.targetsToRun.Count == 0)
                {
                    var defaultTargets = flubuSession.TargetTree.DefaultTargets;
                    targetsInfo.targetsToRun = new List<string>();
                    if (defaultTargets != null && defaultTargets.Count != 0)
                    {
                        foreach (var defaultTarget in defaultTargets)
                        {
                            targetsInfo.targetsToRun.Add(defaultTarget.TargetName);
                        }
                    }
                    else
                    {
                        targetsInfo.targetsToRun.Add("help");
                    }
                }

                ExecuteTarget(flubuSession, targetsInfo);
            }
            else
            {
                FlubuInteractiveMode(flubuSession, targetsInfo, resetTargetTree);
            }
        }

        private bool ExecuteTarget(IFlubuSession flubuSession, (List<string> targetsToRun, bool unknownTarget, List<string> notFoundTargets) targetsInfo)
        {
            flubuSession.Start();

            //// specific target help
            if (targetsInfo.targetsToRun.Count == 2 &&
                targetsInfo.targetsToRun[1].Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                flubuSession.TargetTree.RunTargetHelp(flubuSession, targetsInfo.targetsToRun[0]);
                return true;
            }

            if (targetsInfo.targetsToRun.Count == 1 || !flubuSession.Args.ExecuteTargetsInParallel)
            {
                if (targetsInfo.targetsToRun[0].Equals("help", StringComparison.OrdinalIgnoreCase))
                {
                    flubuSession.TargetTree.ScriptArgsHelp = ScriptProperties.GetPropertiesHelp(this);
                }

                BeforeTargetExecution(flubuSession);
                foreach (var targetToRun in targetsInfo.targetsToRun)
                {
                    flubuSession.TargetTree.RunTarget(flubuSession, targetToRun);
                }

                AfterTargetExecution(flubuSession);
            }
            else
            {
                flubuSession.LogInfo("Running target's in parallel.");
                var tasks = new List<Task>();
                BeforeTargetExecution(flubuSession);
                foreach (var targetToRun in targetsInfo.targetsToRun)
                {
                    tasks.Add(flubuSession.TargetTree.RunTargetAsync(flubuSession, targetToRun, true));
                }

                Task.WaitAll(tasks.ToArray());
                AfterTargetExecution(flubuSession);
            }

            if (targetsInfo.unknownTarget)
            {
                var targetNotFoundMsg = $"Target {string.Join(" and ", targetsInfo.notFoundTargets)} not found.";
                if (flubuSession.Args.InteractiveMode)
                {
                    flubuSession.LogInfo(targetNotFoundMsg);
                }
                else
                {
                    throw new TargetNotFoundException(targetNotFoundMsg);
                }
            }

            AssertAllTargetDependenciesWereExecuted(flubuSession);
            return false;
        }

        private void FlubuInteractiveMode(IFlubuSession flubuSession, (List<string> targetsToRun, bool unknownTarget, List<string> notFoundTargets) targetsInfo, bool resetTargetTree)
        {
            flubuSession.InteractiveMode = true;
            var source = new Dictionary<char, IReadOnlyCollection<Hint>>();
            var propertyKeys = ScriptProperties.GetPropertiesKeys(this, flubuSession);
            propertyKeys.Add(new Hint { Name = "-parallel" });
            propertyKeys.Add(new Hint { Name = "-dryrun" });
            propertyKeys.Add(new Hint { Name = "-noColor" });
            source.Add('-', propertyKeys);

            List<Hint> defaultHints = new List<Hint>();

            foreach (var targetName in flubuSession.TargetTree.GetTargetNames())
            {
                var target = flubuSession.TargetTree.GetTarget(targetName);
                defaultHints.Add(new Hint
                {
                    Name = target.TargetName,
                    Help = target.Description
                });
            }

            var flubuConsole = new FlubuConsole(flubuSession.TargetTree, defaultHints, source);
            flubuSession.TargetTree.RunTarget(flubuSession, "help.onlyTargets");
            flubuSession.LogInfo(" ");

            do
            {
                if (flubuSession.InteractiveMode)
                {
                    var commandLine = flubuConsole.ReadLine(Directory.GetCurrentDirectory());

                    if (string.IsNullOrEmpty(commandLine))
                    {
                        continue;
                    }

                    var app = new CommandLineApplication(false);
                    IFlubuCommandParser parser = new FlubuCommandParser(app, null);
                    var args = parser.Parse(commandLine.Split(' ')
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim()).ToArray());
                    targetsInfo = ParseCmdLineArgs(args.MainCommands, flubuSession.TargetTree);

                    flubuSession.InteractiveArgs = args;
                    flubuSession.ScriptArgs = args.ScriptArguments;
                    ScriptProperties.SetPropertiesFromScriptArg(this, flubuSession);

                    if (args.MainCommands.Count == 0)
                    {
                        continue;
                    }

                    if (CommandExecutor.InteractiveExitCommands.Contains(args.MainCommands[0], StringComparer.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    if (targetsInfo.unknownTarget)
                    {
                        var internalCommandExecuted = flubuConsole.ExecuteInternalCommand(commandLine);
                        if (internalCommandExecuted)
                        {
                            continue;
                        }

                        var splitedLine = commandLine.Split(' ').ToList();
                        var command = splitedLine.First();
                        var runProgram = flubuSession.Tasks().RunProgramTask(command).DoNotLogTaskExecutionInfo().WorkingFolder(".");
                        splitedLine.RemoveAt(0);
                        try
                        {
                            runProgram.WithArguments(splitedLine.ToArray()).Execute(flubuSession);
                        }
                        catch (CommandUnknownException)
                        {
                            flubuSession.LogError($"'{command}' is not recognized as a flubu target, internal or external command, operable program or batch file.");
                        }
                        catch (TaskExecutionException)
                        {
                        }

                        continue;
                    }

                    if (resetTargetTree)
                    {
                        flubuSession.TargetTree.ResetTargetTree();
                        ConfigureDefaultProps(flubuSession);
                        ConfigureBuildProperties(flubuSession);
                        ConfigureDefaultTargets(flubuSession);
                        ScriptProperties.SetPropertiesFromScriptArg(this, flubuSession);
                        TargetCreator.CreateTargetFromMethodAttributes(this, flubuSession);
                        ConfigureTargets(flubuSession);
                    }

                    resetTargetTree = true;
                }

                try
                {
                    if (ExecuteTarget(flubuSession, targetsInfo)) return;
                }
                catch (TaskExecutionException)
                {
                }
            }
            while (flubuSession.InteractiveMode);
        }

        protected virtual void BeforeTargetExecution(ITaskContext context)
        {
        }

        protected virtual void AfterTargetExecution(ITaskContext context)
        {
        }

        protected virtual void BeforeBuildExecution(ITaskContext context)
        {
        }

        protected virtual void AfterBuildExecution(IFlubuSession session)
        {
            session.TargetTree.LogBuildSummary(session);
        }

        protected virtual void OnBuildFailed(IFlubuSession session, Exception ex)
        {
        }

        private void ConfigureDefaultProps(IFlubuSession flubuSession)
        {
            flubuSession.SetBuildVersion(new Version(1, 0, 0, 0));
            flubuSession.SetDotnetExecutable(ExecuteDotnetTask.FindDotnetExecutable());

            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            OSPlatform platform;

            if (!isWindows)
            {
                var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
                platform = isLinux ? OSPlatform.Linux : OSPlatform.OSX;
            }
            else
            {
                platform = OSPlatform.Windows;
            }

            flubuSession.SetOSPlatform(platform);
            flubuSession.SetNodeExecutablePath(IOExtensions.GetNodePath());
            flubuSession.SetProfileFolder(IOExtensions.GetUserProfileFolder());
            flubuSession.SetNpmPath(IOExtensions.GetNpmPath());
            flubuSession.SetBuildDir("build");
            flubuSession.SetOutputDir("output");
            flubuSession.SetProductRootDir(".");

            if (isWindows)
            {
                // do windows specific tasks
            }
        }

        private void ConfigureDefaultTargets(IFlubuSession flubuSession)
        {
            var defaultTagets = flubuSession.Properties.GetDefaultTargets();

            switch (defaultTagets)
            {
                case DefaultTargets.Dotnet:
                {
                    ConfigureDefaultDotNetTargets(flubuSession);
                    break;
                }
            }
        }

        private void ConfigureDefaultDotNetTargets(IFlubuSession flubuSession)
        {
            var loadSolution = flubuSession.CreateTarget("load.solution")
                .SetDescription("Load & analyze VS solution")
                .AddTask(x => x.LoadSolutionTask())
                .SetAsHidden();

            var cleanOutput = flubuSession.CreateTarget("clean.output")
                .SetDescription("Clean solution outputs")
                .AddTask(x => x.CleanOutputTask())
                .DependsOn(loadSolution);

            var prepareBuildDir = flubuSession.CreateTarget("prepare.build.dir")
                .SetDescription("Prepare the build directory")
                .Do(TargetPrepareBuildDir)
                .SetAsHidden();

            var fetchBuildVersion = flubuSession.CreateTarget("fetch.build.version")
                .SetDescription("Fetch the build version")
                .SetAsHidden();

            var generateCommonAssInfo = flubuSession.CreateTarget("generate.commonassinfo")
                .SetDescription("Generate CommonAssemblyInfo.cs file")
                .DependsOn(fetchBuildVersion)
                .AddTask(x => x.GenerateCommonAssemblyInfoTask());

            flubuSession.CreateTarget("compile")
                .SetDescription("Compile the VS solution")
                .AddTask(x => x.CompileSolutionTask())
                .DependsOn(prepareBuildDir, cleanOutput, generateCommonAssInfo);
        }

        private void TargetPrepareBuildDir(ITaskContext context)
        {
            var buildDir = context.Properties.Get<string>(BuildProps.BuildDir);
            var createDirectoryTask = context.Tasks().CreateDirectoryTask(buildDir, true);
            createDirectoryTask.Execute(context);
        }

        private void AssertAllTargetDependenciesWereExecuted(IFlubuSession flubuSession)
        {
            if (flubuSession.Args.TargetsToExecute != null && flubuSession.Args.TargetsToExecute.Count > 1)
            {
                if (flubuSession.Args.TargetsToExecute.Count - 1 != flubuSession.TargetTree.DependenciesExecutedCount)
                {
                    throw new TaskExecutionException("Wrong number of target dependencies were runned.", 3);
                }
            }
        }
    }
}
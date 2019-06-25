using System;
using System.Collections.Generic;
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

namespace FlubuCore.Scripting
{
    public abstract class DefaultBuildScript : IBuildScript
    {
        public int Run(ITaskSession taskSession)
        {
            try
            {
                BeforeBuildExecution(taskSession);
                RunBuild(taskSession);
                taskSession.Complete();
                AfterBuildExecution(taskSession);
                return 0;
            }
            catch (TargetNotFoundException e)
            {
                taskSession.ResetDepth();
                OnBuildFailed(taskSession, e);
                AfterBuildExecution(taskSession);
                if (taskSession.Args.RethrowOnException)
                    throw;

                taskSession.LogError(e.Message);
                return 3;
            }
            catch (WebApiException e)
            {
                taskSession.ResetDepth();
                OnBuildFailed(taskSession, e);
                AfterBuildExecution(taskSession);
                if (taskSession.Args.RethrowOnException)
                    throw;

                return 1;
            }
            catch (FlubuException e)
            {
                taskSession.ResetDepth();
                OnBuildFailed(taskSession, e);

                if (!taskSession.Args.RethrowOnException)
                {
#if !NETSTANDARD1_6
                    taskSession.LogError($"ERROR: {e.Message}", Color.Red);
#else
                    taskSession.LogError($"error: {e.Message}");
#endif
                }

                AfterBuildExecution(taskSession);
                if (taskSession.Args.RethrowOnException)
                    throw;

                return 1;
            }
            catch (Exception e)
            {
                taskSession.ResetDepth();
                OnBuildFailed(taskSession, e);

                if (!taskSession.Args.RethrowOnException)
                {
#if !NETSTANDARD1_6
                    taskSession.LogError($"ERROR: {e}", Color.Red);
#else
                    taskSession.LogError($"error: {e}");
#endif
                }

                AfterBuildExecution(taskSession);
                if (taskSession.Args.RethrowOnException)
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

        private void RunBuild(ITaskSession taskSession)
        {
            ConfigureDefaultProps(taskSession);

            ConfigureBuildProperties(taskSession);

            ConfigureDefaultTargets(taskSession);

            ScriptProperties.SetPropertiesFromScriptArg(this, taskSession);

            TargetCreator.CreateTargetFromMethodAttributes(this, taskSession);

            ConfigureTargets(taskSession);

            (List<string> targetsToRun, bool unknownTarget, List<string> notFoundTargets) targetsInfo =
                default((List<string> targetsToRun, bool unknownTarget, List<string> notFoundTargets));
            bool runInTerminalMode;
            ConsoleHintedInput inputReader = null;

            if (!taskSession.Args.InteractiveMode)
            {
                runInTerminalMode = false;

                targetsInfo = ParseCmdLineArgs(taskSession.Args.MainCommands, taskSession.TargetTree);
                taskSession.UnknownTarget = targetsInfo.unknownTarget;
                if (targetsInfo.targetsToRun == null || targetsInfo.targetsToRun.Count == 0)
                {
                    var defaultTargets = taskSession.TargetTree.DefaultTargets;
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
            }
            else
            {
                runInTerminalMode = true;
                var source = new Dictionary<char, IReadOnlyCollection<string>>();
                var propertyKeys = ScriptProperties.GetPropertiesKeys(this, taskSession);
                source.Add('-', propertyKeys);

                inputReader = new ConsoleHintedInput(taskSession.TargetTree.GetTargetNames().ToList(), source);
                taskSession.TargetTree.RunTarget(taskSession, "help.onlyTargets");
                taskSession.LogInfo(" ");
            }

            do
            {
                if (runInTerminalMode)
                {
                    var commandLine = inputReader.ReadHintedLine();
                    var app = new CommandLineApplication(false);
                    IFlubuCommandParser parser = new FlubuCommandParser(app, null);
                    var args = parser.Parse(commandLine.Split(' ')
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim()).ToArray());
                    targetsInfo = ParseCmdLineArgs(args.MainCommands, taskSession.TargetTree);
                    taskSession.ScriptArgs = args.ScriptArguments;
                    ScriptProperties.SetPropertiesFromScriptArg(this, taskSession);

                    if (args.MainCommands[0].Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                        args.MainCommands[0].Equals("quit", StringComparison.OrdinalIgnoreCase) ||
                        args.MainCommands[0].Equals("x", StringComparison.OrdinalIgnoreCase) ||
                        args.MainCommands[0].Equals("q", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }

                taskSession.Start();

                //// specific target help
                if (targetsInfo.targetsToRun.Count == 2 &&
                    targetsInfo.targetsToRun[1].Equals("help", StringComparison.OrdinalIgnoreCase))
                {
                    taskSession.TargetTree.RunTargetHelp(taskSession, targetsInfo.targetsToRun[0]);
                    return;
                }

                if (targetsInfo.targetsToRun.Count == 1 || !taskSession.Args.ExecuteTargetsInParallel)
                {
                    if (targetsInfo.targetsToRun[0].Equals("help", StringComparison.OrdinalIgnoreCase))
                    {
                        taskSession.TargetTree.ScriptArgsHelp = ScriptProperties.GetPropertiesHelp(this);
                    }

                    BeforeTargetExecution(taskSession);
                    foreach (var targetToRun in targetsInfo.targetsToRun)
                    {
                        taskSession.TargetTree.RunTarget(taskSession, targetToRun);
                    }

                    AfterTargetExecution(taskSession);
                }
                else
                {
                    taskSession.LogInfo("Running target's in parallel.");
                    var tasks = new List<Task>();
                    BeforeTargetExecution(taskSession);
                    foreach (var targetToRun in targetsInfo.targetsToRun)
                    {
                        tasks.Add(taskSession.TargetTree.RunTargetAsync(taskSession, targetToRun, true));
                    }

                    Task.WaitAll(tasks.ToArray());
                    AfterTargetExecution(taskSession);
                }

                if (targetsInfo.unknownTarget)
                {
                    throw new TargetNotFoundException(
                        $"Target {string.Join(" and ", targetsInfo.notFoundTargets)} not found.");
                }

                AssertAllTargetDependenciesWereExecuted(taskSession);
            }
            while (runInTerminalMode);
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

        protected virtual void AfterBuildExecution(ITaskSession session)
        {
            session.TargetTree.LogBuildSummary(session);
        }

        protected virtual void OnBuildFailed(ITaskSession session, Exception ex)
        {
        }

        private void ConfigureDefaultProps(ITaskSession taskSession)
        {
            taskSession.SetBuildVersion(new Version(1, 0, 0, 0));
            taskSession.SetDotnetExecutable(ExecuteDotnetTask.FindDotnetExecutable());

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

            taskSession.SetOSPlatform(platform);
            taskSession.SetNodeExecutablePath(IOExtensions.GetNodePath());
            taskSession.SetProfileFolder(IOExtensions.GetUserProfileFolder());
            taskSession.SetNpmPath(IOExtensions.GetNpmPath());
            taskSession.SetBuildDir("build");
            taskSession.SetOutputDir("output");
            taskSession.SetProductRootDir(".");

            if (isWindows)
            {
                // do windows specific tasks
            }
        }

        private void ConfigureDefaultTargets(ITaskSession taskSession)
        {
            var defaultTagets = taskSession.Properties.GetDefaultTargets();

            switch (defaultTagets)
            {
                case DefaultTargets.Dotnet:
                {
                    ConfigureDefaultDotNetTargets(taskSession);
                    break;
                }
            }
        }

        private void ConfigureDefaultDotNetTargets(ITaskSession taskSession)
        {
            var loadSolution = taskSession.CreateTarget("load.solution")
                .SetDescription("Load & analyze VS solution")
                .AddTask(x => x.LoadSolutionTask())
                .SetAsHidden();

            var cleanOutput = taskSession.CreateTarget("clean.output")
                .SetDescription("Clean solution outputs")
                .AddTask(x => x.CleanOutputTask())
                .DependsOn(loadSolution);

            var prepareBuildDir = taskSession.CreateTarget("prepare.build.dir")
                .SetDescription("Prepare the build directory")
                .Do(TargetPrepareBuildDir)
                .SetAsHidden();

            var fetchBuildVersion = taskSession.CreateTarget("fetch.build.version")
                .SetDescription("Fetch the build version")
                .SetAsHidden();

            var generateCommonAssInfo = taskSession.CreateTarget("generate.commonassinfo")
                .SetDescription("Generate CommonAssemblyInfo.cs file")
                .DependsOn(fetchBuildVersion)
                .AddTask(x => x.GenerateCommonAssemblyInfoTask());

            taskSession.CreateTarget("compile")
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

        private void AssertAllTargetDependenciesWereExecuted(ITaskSession taskSession)
        {
            if (taskSession.Args.TargetsToExecute != null && taskSession.Args.TargetsToExecute.Count > 1)
            {
                if (taskSession.Args.TargetsToExecute.Count - 1 != taskSession.TargetTree.DependenciesExecutedCount)
                {
                    throw new TaskExecutionException("Wrong number of target dependencies were runned.", 3);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.IO;
using FlubuCore.Targeting;
using FlubuCore.Tasks.NetCore;

namespace FlubuCore.Scripting
{
    public abstract class DefaultBuildScript : IBuildScript
    {
        public int Run(ITaskSession taskSession)
        {
            try
            {
                RunBuild(taskSession);
                return 0;
            }
            catch (FlubuException e)
            {
                if (taskSession.Args.RethrowOnException)
                    throw;

                taskSession.LogInfo(e.Message);
                return 1;
            }
            catch (Exception ex)
            {
                if (taskSession.Args.RethrowOnException)
                    throw;

                taskSession.LogInfo(ex.ToString());
                return 2;
            }
        }

        protected abstract void ConfigureBuildProperties(IBuildPropertiesContext context);

        protected abstract void ConfigureTargets(ITaskContext session);

        private static List<string> ParseCmdLineArgs(ITaskContextInternal context, TargetTree targetTree)
        {
            if (context.Args.MainCommands == null || context.Args.MainCommands.Count == 0) return null;

            if (targetTree.HasAllTargets(context.Args.MainCommands, out var notFoundTargets))
                return context.Args.MainCommands;

            if (context.Args.TreatUnknownTargetAsException)
                throw new TargetNotFoundException($"Target {string.Join(" and ", notFoundTargets)} not found.");

            context.LogInfo($"ERROR: Target {string.Join(" and ", notFoundTargets)} not found.");
            return new List<string>
                { "help" };
        }

        private void RunBuild(ITaskSession taskSession)
        {
            ConfigureDefaultProps(taskSession);

            ConfigureBuildProperties(taskSession);

            ConfigureDefaultTargets(taskSession);

            ConfigureTargets(taskSession);

            var targetsToRun = ParseCmdLineArgs(taskSession, taskSession.TargetTree);

            if (targetsToRun == null || targetsToRun.Count == 0)
            {
                var defaultTargets = taskSession.TargetTree.DefaultTargets;
                targetsToRun = new List<string>();
                if (defaultTargets != null && defaultTargets.Count != 0)
                {
                    foreach (var defaultTarget in defaultTargets)
                    {
                        targetsToRun.Add(defaultTarget.TargetName);
                    }
                }
                else
                {
                    targetsToRun.Add("help");
                }
            }

            taskSession.Start(s =>
            {
                var sortedTargets = new SortedList<string, ITarget>();

                foreach (var target in s.TargetTree.EnumerateExecutedTargets())
                    sortedTargets.Add(target.TargetName, target);

                foreach (var target in sortedTargets.Values)
                {
                    var targt = target as Target;

                    if (targt?.TaskStopwatch.ElapsedTicks > 0)
                    {
                        s.LogInfo(
                            $"Target {target.TargetName} took {(int)targt.TaskStopwatch.Elapsed.TotalSeconds} s");
                    }
                }

                s.LogInfo(s.HasFailed ? "BUILD FAILED" : "BUILD SUCCESSFUL");
            });

            //// specific target help
            if (targetsToRun.Count == 2 && targetsToRun[1].Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                taskSession.TargetTree.RunTargetHelp(taskSession, targetsToRun[0]);
                return;
            }

            if (targetsToRun.Count == 1 || !taskSession.Args.ExecuteTargetsInParallel)
            {
                foreach (var targetToRun in targetsToRun) taskSession.TargetTree.RunTarget(taskSession, targetToRun);
            }
            else
            {
                taskSession.LogInfo("Running target's in parallel.");
                var tasks = new List<Task>();
                foreach (var targetToRun in targetsToRun)
                    tasks.Add(taskSession.TargetTree.RunTargetAsync(taskSession, targetToRun));

                Task.WaitAll(tasks.ToArray());
            }

            AssertAllTargetDependenciesWereExecuted(taskSession);
        }

        private void ConfigureDefaultProps(ITaskSession taskSession)
        {
            taskSession.SetBuildVersion(new Version(1, 0, 0, 0));
            taskSession.SetDotnetExecutable(Dotnet.FindDotnetExecutable());

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
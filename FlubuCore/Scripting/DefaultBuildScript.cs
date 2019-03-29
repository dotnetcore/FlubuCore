using System;
using System.Collections.Generic;
#if !NETSTANDARD1_6
using System.Drawing;
#endif
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.IO;
using FlubuCore.Targeting;
using FlubuCore.Tasks.NetCore;
using FlubuCore.WebApi.Client;

namespace FlubuCore.Scripting
{
    public abstract class DefaultBuildScript : IBuildScript
    {
        public int Run(ITaskSession taskSession)
        {
            try
            {
                RunBuild(taskSession);
                taskSession.Complete();
                return 0;
            }
            catch (TargetNotFoundException e)
            {
                taskSession.OnFinish();
                if (taskSession.Args.RethrowOnException)
                    throw;

                taskSession.LogInfo(e.Message);
                return 3;
            }
            catch (WebApiException)
            {
                taskSession.OnFinish();
                if (taskSession.Args.RethrowOnException)
                    throw;

                return 1;
            }
            catch (FlubuException e)
            {
                taskSession.OnFinish();
                if (taskSession.Args.RethrowOnException)
                    throw;

                taskSession.LogInfo(e.Message);
                return 1;
            }
            catch (Exception ex)
            {
                  taskSession.OnFinish();
                if (taskSession.Args.RethrowOnException)
                    throw;

                taskSession.LogInfo(ex.ToString());
                return 2;
            }
        }

        protected abstract void ConfigureBuildProperties(IBuildPropertiesContext context);

        protected abstract void ConfigureTargets(ITaskContext context);

        private static (List<string> targetsToRun, bool unknownTarget, List<string> notFoundTargets) ParseCmdLineArgs(ITaskContextInternal context, TargetTree targetTree)
        {
            if (context.Args.MainCommands == null || context.Args.MainCommands.Count == 0) return (null, false, null);

            if (targetTree.HasAllTargets(context.Args.MainCommands, out var notFoundTargets))
                return (context.Args.MainCommands, false, null);

            return (new List<string> { "help" }, true,  notFoundTargets);
        }

        private void RunBuild(ITaskSession taskSession)
        {
            ConfigureDefaultProps(taskSession);

            ConfigureBuildProperties(taskSession);

            ConfigureDefaultTargets(taskSession);

            ScriptProperties.SetPropertiesFromScriptArg(this,  taskSession);

            TargetCreator.CreateTargetFromMethodAttributes(this, taskSession);

            ConfigureTargets(taskSession);

            var targetsInfo = ParseCmdLineArgs(taskSession, taskSession.TargetTree);

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

            taskSession.Start(s =>
            {
                foreach (var target in s.TargetTree.EnumerateExecutedTargets())
                {
                    var targt = target as Target;

                    if (targt?.TaskStopwatch.ElapsedTicks > 0)
                    {
#if  !NETSTANDARD1_6
                        s.LogInfo($"Target {target.TargetName} took {(int)targt.TaskStopwatch.Elapsed.TotalSeconds} s", Color.DimGray);
#else
                          s.LogInfo($"Target {target.TargetName} took {(int)targt.TaskStopwatch.Elapsed.TotalSeconds} s");
#endif
                    }
                }

                if (taskSession.Args.DryRun)
                {
                    s.LogInfo("DRY RUN PERFORMED");
                }
                else if (!targetsInfo.unknownTarget)
                {
#if  !NETSTANDARD1_6
                    s.LogInfo(s.HasFailed ? "BUILD FAILED" : "BUILD SUCCESSFUL", s.HasFailed ? Color.Red : Color.Green);
#else
                    s.LogInfo(s.HasFailed ? "BUILD FAILED" : "BUILD SUCCESSFUL");
#endif

                }
            });

            //// specific target help
            if (targetsInfo.targetsToRun.Count == 2 && targetsInfo.targetsToRun[1].Equals("help", StringComparison.OrdinalIgnoreCase))
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
                    tasks.Add(taskSession.TargetTree.RunTargetAsync(taskSession, targetToRun));

                Task.WaitAll(tasks.ToArray());
                AfterTargetExecution(taskSession);
            }

            if (targetsInfo.unknownTarget)
            {
                throw new TargetNotFoundException($"Target {string.Join(" and ", targetsInfo.notFoundTargets)} not found.");
            }

            AssertAllTargetDependenciesWereExecuted(taskSession);
        }

        protected virtual void BeforeTargetExecution(ITaskContext context)
        {
        }

        protected virtual void AfterTargetExecution(ITaskContext context)
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
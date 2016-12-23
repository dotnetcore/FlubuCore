using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
            catch (TaskExecutionException e)
            {
                taskSession.LogInfo(e.Message);
                return 1;
            }
            catch (Exception ex)
            {
                taskSession.LogInfo(ex.ToString());
                return 2;
            }
        }

        protected abstract void ConfigureBuildProperties(IBuildPropertiesContext context);

        protected abstract void ConfigureTargets(ITaskContext session);

        private static string ParseCmdLineArgs(ITaskContextInternal context, TargetTree targetTree)
        {
            if (string.IsNullOrEmpty(context.Args.MainCommand))
            {
                return null;
            }

            if (targetTree.HasTarget(context.Args.MainCommand))
            {
                return context.Args.MainCommand;
            }

            context.LogInfo($"ERROR: Target {context.Args.MainCommand} not found.");
            return null;
        }

        private void RunBuild(ITaskSession taskSession)
        {
            ConfigureDefaultProps(taskSession);

            ConfigureBuildProperties(taskSession);

            ConfigureDefaultTargets(taskSession);

            ConfigureTargets(taskSession);

            string targetToRun = ParseCmdLineArgs(taskSession, taskSession.TargetTree);

            if (string.IsNullOrEmpty(targetToRun))
            {
                ITarget defaultTarget = taskSession.TargetTree.DefaultTarget;

                if (defaultTarget == null)
                {
                    throw new InvalidOperationException("The default build target is not defined");
                }

                targetToRun = defaultTarget.TargetName;
            }

            taskSession.Start(s =>
            {
                SortedList<string, ITarget> sortedTargets = new SortedList<string, ITarget>();

                foreach (ITarget target in s.TargetTree.EnumerateExecutedTargets())
                {
                    sortedTargets.Add(target.TargetName, target);
                }

                foreach (ITarget target in sortedTargets.Values)
                {
                    if (target.TaskStopwatch.ElapsedTicks > 0)
                    {
                        s.LogInfo($"Target {target.TargetName} took {(int)target.TaskStopwatch.Elapsed.TotalSeconds} s");
                    }
                }

                s.LogInfo(s.HasFailed ? "BUILD FAILED" : "BUILD SUCCESSFUL");
            });

            taskSession.TargetTree.RunTarget(taskSession, targetToRun);
            AssertAllTargetDependenciesWereExecuted(taskSession);
        }

        private void ConfigureDefaultProps(ITaskSession taskSession)
        {
            taskSession.SetBuildVersion(new Version(1, 0, 0, 0));
            taskSession.SetDotnetExecutable(Dotnet.FindDotnetExecutable());
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            taskSession.SetOSPlatform(isWindows ? OSPlatform.Windows : OSPlatform.Linux);
            taskSession.SetNodeExecutablePath(IOExtensions.GetNodePath(isWindows));
            taskSession.SetProfileFolder(IOExtensions.GetUserProfileFolder(isWindows));
            taskSession.SetNpmPath(IOExtensions.GetNpmPath(isWindows));
            taskSession.SetBuildDir("build");
            taskSession.SetOutputDir("output");
            taskSession.SetProductRootDir(".");

            if (isWindows)
            {
                // do windows specific tasks
            }
            else
            {
                // do linux specific tasks
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
            string buildDir = context.Properties.Get<string>(BuildProps.BuildDir);
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
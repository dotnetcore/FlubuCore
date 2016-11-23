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
        public void Run(ITaskSession taskSession)
        {
            try
            {
                RunBuild(taskSession);
            }
            catch (TaskExecutionException e)
            {
                taskSession.LogInfo(e.Message);
            }
            catch (Exception ex)
            {
                taskSession.LogInfo(ex.ToString());
            }
        }

        protected abstract void ConfigureBuildProperties(IBuildPropertiesContext context);

        protected abstract void ConfigureTargets(ITaskSession session);

        private static string ParseCmdLineArgs(ITaskContext context, TargetTree targetTree)
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
    }
}
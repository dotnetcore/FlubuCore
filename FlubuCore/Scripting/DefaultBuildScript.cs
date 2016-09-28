using System;
using System.Collections.Generic;
using FlubuCore.Context;
using FlubuCore.Targeting;

namespace FlubuCore.Scripting
{
    public abstract class DefaultBuildScript : IBuildScript
    {
        public int Run(ITaskSession taskSession)
        {
            try
            {
                return RunBuild(taskSession);
            }
            catch (TaskExecutionException e)
            {
                taskSession.WriteMessage(e.Message);
                return 1;
            }
            catch (Exception ex)
            {
                taskSession.WriteMessage(ex.ToString());
                return 2;
            }
        }

        protected abstract void ConfigureBuildProperties(ITaskSession session);

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

            context.WriteMessage($"ERROR: Target {context.Args.MainCommand} not found.");
            return null;
        }

        private int RunBuild(ITaskSession taskSession)
        {
            ConfigureTargets(taskSession);

            ConfigureBuildProperties(taskSession);

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

                    foreach (var target in sortedTargets.Values)
                    {
                        if (target.TaskStopwatch.ElapsedTicks > 0)
                        {
                            s.WriteMessage($"Target {target.TargetName} took {(int)target.TaskStopwatch.Elapsed.TotalSeconds} s");
                        }
                    }

                    s.WriteMessage(s.HasFailed ? "BUILD FAILED" : "BUILD SUCCESSFUL");
                });

            return taskSession.TargetTree.RunTarget(taskSession, targetToRun);
        }
    }
}
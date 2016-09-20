using flubu.Targeting;
using System;
using System.Collections.Generic;

namespace flubu.Scripting
{
    public abstract class DefaultBuildScript : IBuildScript
    {
        public int Run(CommandArguments args)
        {
            try
            {
                if (args == null)
                    throw new ArgumentNullException(nameof(args));

                TargetTree targetTree = new TargetTree();

                ConfigureTargets(targetTree, args);

                return RunBuild(args, targetTree);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        protected abstract void ConfigureBuildProperties(TaskSession session);

        protected abstract void ConfigureTargets(TargetTree targetTree, CommandArguments args);

        private int RunBuild(CommandArguments args, TargetTree targetTree)
        {
            if (targetTree == null)
                throw new ArgumentNullException(nameof(targetTree));

            using (TaskSession session = new TaskSession(args, targetTree))
            {
                session.Start(s =>
                {
                    SortedList<string, ITarget> sortedTargets = new SortedList<string, ITarget>();

                    foreach (ITarget target in session.TargetTree.EnumerateExecutedTargets())
                        sortedTargets.Add(target.TargetName, target);

                    foreach (ITarget target in sortedTargets.Values)
                    {
                        if (target.TaskStopwatch.ElapsedTicks > 0)
                        {
                            session.WriteMessage(
                                $"Target {target.TargetName} took {(int)target.TaskStopwatch.Elapsed.TotalSeconds} s");
                        }
                    }

                    if (session.HasFailed)
                        session.WriteMessage("BUILD FAILED");
                    else
                        session.WriteMessage("BUILD SUCCESSFUL");
                });

                ConfigureBuildProperties(session);

                string targetToRun = ParseCmdLineArgs(args, session, targetTree);

                if (string.IsNullOrEmpty(targetToRun))
                {
                    ITarget defaultTarget = targetTree.DefaultTarget;
                    if (defaultTarget == null)
                        throw new InvalidOperationException("The default build target is not defined");

                    return targetTree.RunTarget(session, defaultTarget.TargetName);
                }

                return targetTree.RunTarget(session, targetToRun);
            }
        }

        private static string ParseCmdLineArgs(CommandArguments args, ITaskContext context, TargetTree targetTree)
        {
            if (string.IsNullOrEmpty(args.MainCommand))
                return null;

            if (targetTree.HasTarget(args.MainCommand))
                return args.MainCommand;

            context.WriteMessage($"ERROR: Target {args.MainCommand} not found.");
            return null;
        }
    }
}
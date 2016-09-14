using flubu.Targeting;
using flubu.Tasks;
using Flubu;
using Flubu.Builds;
using System;
using System.Collections.Generic;
using System.Linq;

namespace flubu.Scripting
{
    public abstract class DefaultBuildScript : IBuildScript
    {
        public Func<bool> InteractiveSessionDetectionFunc
        {
            get { return interactiveSessionDetectionFunc; }
            set { interactiveSessionDetectionFunc = value; }
        }

        public int Run(ICollection<string> args)
        {
            try
            {
                if (args == null)
                    throw new ArgumentNullException(nameof(args));

                TargetTree targetTree = new TargetTree();
                BuildTargets.FillBuildTargets(targetTree);

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

        protected abstract void ConfigureTargets(TargetTree targetTree, ICollection<string> args);

        private int RunBuild (ICollection<string> args, TargetTree targetTree)
        {
            if (targetTree == null)
                throw new ArgumentNullException (nameof(targetTree));

            using (TaskSession session = new TaskSession(new SimpleTaskContextProperties(), args, targetTree))
            {
                session.IsInteractive = InteractiveSessionDetectionFunc();

                BuildTargets.FillDefaultProperties(session);
                session.Start(BuildTargets.OnBuildFinished);

                //todo do we need logger??
                //session.AddLogger(new MulticoloredConsoleLogger(Console.Out));

                ConfigureBuildProperties(session);
                bool targetFoundInTargetTree;
                string targetToRun = ParseCmdLineArgs(args.ToList(), session, targetTree, out targetFoundInTargetTree);

                if (targetToRun == null)
                {
                    ITarget defaultTarget = targetTree.DefaultTarget;
                    if (defaultTarget == null)
                        throw new InvalidOperationException("The default build target is not defined");

                    targetTree.RunTarget(session, defaultTarget.TargetName);
                }
                else
                {
                    if (!targetFoundInTargetTree)
                    {
                        session.WriteError("ERROR: The specified target does not exist");
                        targetTree.RunTarget(session, targetToRun);
                        return 2;
                    }

                    targetTree.RunTarget(session, targetToRun);
                }

                session.Complete();

                return 0;
            }
        }

        private static bool DefaultSessionInteractiveSessionDetectionFunc ()
        {
            return Environment.GetEnvironmentVariable ("CI") == null
                   && Environment.GetEnvironmentVariable ("APPVEYOR") == null
                   && Environment.GetEnvironmentVariable ("BUILD_NUMBER") == null;
        }

        private static string ParseCmdLineArgs (IList<string> args, ITaskContext context, TargetTree targetTree, out bool targetFoundInTargetTree)
        {
            string targetToBuild = null;
            if (args.Count == 0)
            {
                targetFoundInTargetTree = false;
                return null;
            }

            foreach (string arg in args)
            {
                //todo how to reenable speech??
                //if (string.Compare (arg, "-speechdisabled", StringComparison.OrdinalIgnoreCase) == 0)
                //    context.Properties.Set (BuildProps.SpeechDisabled, true);
                if (string.IsNullOrEmpty(targetToBuild) && targetTree.HasTarget(arg))
                    targetToBuild = arg;
            }

            if (targetToBuild == null)
            {
                targetFoundInTargetTree = false;
                return "help";
            }

            targetFoundInTargetTree = true;
            return targetToBuild;
        }

        private Func<bool> interactiveSessionDetectionFunc = DefaultSessionInteractiveSessionDetectionFunc;
    }
}

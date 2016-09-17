using flubu.Targeting;
using flubu.Tasks;
using Flubu.Builds;
using System;

namespace flubu.Scripting
{
    public abstract class DefaultBuildScript : IBuildScript
    {
        //public Func<bool> InteractiveSessionDetectionFunc
        //{
        //    get { return interactiveSessionDetectionFunc; }
        //    set { interactiveSessionDetectionFunc = value; }
        //}

        public string Name => nameof(DefaultBuildScript);

        public int Run(CommandArguments args)
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

        protected abstract void ConfigureTargets(TargetTree targetTree, CommandArguments args);

        private int RunBuild(CommandArguments args, TargetTree targetTree)
        {
            if (targetTree == null)
                throw new ArgumentNullException(nameof(targetTree));

            using (TaskSession session = new TaskSession(new SimpleTaskContextProperties(), args, targetTree))
            {
                //session.IsInteractive = InteractiveSessionDetectionFunc();

                BuildTargets.FillDefaultProperties(session);
                session.Start(BuildTargets.OnBuildFinished);

                //todo do we need logger??
                //session.AddLogger(new MulticoloredConsoleLogger(Console.Out));

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

        //private static bool DefaultSessionInteractiveSessionDetectionFunc()
        //{
        //    return Environment.GetEnvironmentVariable("CI") == null
        //           && Environment.GetEnvironmentVariable("APPVEYOR") == null
        //           && Environment.GetEnvironmentVariable("BUILD_NUMBER") == null;
        //}

        private static string ParseCmdLineArgs(CommandArguments args, ITaskContext context, TargetTree targetTree)
        {
            if (string.IsNullOrEmpty(args.MainCommand))
                return null;

            if (targetTree.HasTarget(args.MainCommand))
                return args.MainCommand;

            context.WriteError($"ERROR: Target {args.MainCommand} not found.");
            return null;
        }

        //private Func<bool> interactiveSessionDetectionFunc = DefaultSessionInteractiveSessionDetectionFunc;
    }
}

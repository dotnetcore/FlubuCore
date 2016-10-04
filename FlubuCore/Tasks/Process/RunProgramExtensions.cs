using System;
using FlubuCore.Targeting;

namespace FlubuCore.Tasks.Process
{
    public static class RunProgramExtensions
    {
        public static ITarget RunMultiProgram(this ITarget target, params string[] programs)
        {
            foreach (string program in programs)
            {
                target.AddTask(RunProgram(program, null));
            }

            return target;
        }

        public static ITarget RunProgram(this ITarget target, string program, string workingFolder, params string[] args)
        {
            target.AddTask(RunProgram(program, workingFolder, args));

            return target;
        }

        private static ITask RunProgram(string program, string workingFolder, params string[] args)
        {
            RunProgramTask task = new RunProgramTask(program);

            return task
                .WorkingFolder(workingFolder)
                .WithArguments(args);
        }
    }
}

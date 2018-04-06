using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Context.FluentInterface.TaskExtensions
{
    public static class TaskFluentInterfaceExtensions
    {
        /// <summary>
        /// Run's multiple programs
        /// </summary>
        /// <param name="programs"></param>
        /// <returns></returns>
        public static List<IRunProgramTask> RunMultiProgram(this ITaskFluentInterface taskF, params string[] programs)
        {
            var fluent = (TaskFluentInterface)taskF;
            List<IRunProgramTask> runProgramTasks = new List<IRunProgramTask>();
            foreach (string program in programs)
            {
               runProgramTasks.Add(CreateRunProgram(fluent.Context, program, null));
            }

            return runProgramTasks;
        }

        /// <summary>
        /// Run specified program.
        /// </summary>
        /// <param name="program"></param>
        /// <param name="workingFolder"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IRunProgramTask RunProgram(this ITaskFluentInterface taskF, string program, string workingFolder, params string[] args)
        {
            var fluent = (TaskFluentInterface)taskF;
            return CreateRunProgram(fluent.Context, program, workingFolder, args);
        }

        /// <summary>
        /// Run specified program.
        /// </summary>
        /// <param name="program"></param>
        /// <param name="workingFolder"></param>
        /// <returns></returns>
        public static IRunProgramTask RunProgram(this ITaskFluentInterface taskF, string program, string workingFolder, Action<IRunProgramTask> action = null)
        {
            var fluent = (TaskFluentInterface)taskF;
            var task = CreateRunProgram(fluent.Context, program, workingFolder, null);
            action?.Invoke(task);
            return task;
        }

        private static IRunProgramTask CreateRunProgram(TaskContext context, string program, string workingFolder, params string[] args)
        {
            var task = context.Tasks().RunProgramTask(program);

            return task
                .WorkingFolder(workingFolder)
                .WithArguments(args);
        }
    }
}

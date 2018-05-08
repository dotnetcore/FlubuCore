using System;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Tasks;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Context.FluentInterface.TaskExtensions
{
    public partial class TaskExtensionsFluentInterface
    {
        public ITaskExtensionsFluentInterface RunMultiProgram(params string[] programs)
        {
            foreach (string program in programs)
            {
                Target.Target.AddTask(null, CreateRunProgram(program, null));
            }

            return this;
        }

        public ITaskExtensionsFluentInterface RunProgram(string program, string workingFolder, params string[] args)
        {
            Target.Target.AddTask(null, CreateRunProgram(program, workingFolder, args));

            return this;
        }

        public ITaskExtensionsFluentInterface RunProgram(string program, string workingFolder,
            Action<IRunProgramTask> action = null)
        {
            var task = CreateRunProgram(program, workingFolder, null);
            action?.Invoke(task);
            return this;
        }

        private IRunProgramTask CreateRunProgram(string program, string workingFolder, params string[] args)
        {
            var task = Context.Tasks().RunProgramTask(program);

            return task
                .WorkingFolder(workingFolder)
                .WithArguments(args);
        }
    }
}

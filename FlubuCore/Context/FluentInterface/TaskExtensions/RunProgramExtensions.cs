using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Tasks;

namespace FlubuCore.Context.FluentInterface.TaskExtensions
{
    public partial class TaskExtensionsFluentInterface
    {
        public ITaskExtensionsFluentInterface RunMultiProgram(params string[] programs)
        {
            foreach (string program in programs)
            {
                Target.Target.AddTask(CreateRunProgram(program, null));
            }

            return this;
        }

        public ITaskExtensionsFluentInterface RunProgram(string program, string workingFolder, params string[] args)
        {
            Target.Target.AddTask(CreateRunProgram(program, workingFolder, args));

            return this;
        }

        private ITask CreateRunProgram(string program, string workingFolder, params string[] args)
        {
            var task = Context.Tasks().RunProgramTask(program);

            return task
                .WorkingFolder(workingFolder)
                .WithArguments(args);
        }
    }
}

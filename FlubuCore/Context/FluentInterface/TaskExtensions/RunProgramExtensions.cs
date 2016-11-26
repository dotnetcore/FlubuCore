using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using FlubuCore.Tasks.Process;
using Microsoft.DotNet.Cli.Utils;

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

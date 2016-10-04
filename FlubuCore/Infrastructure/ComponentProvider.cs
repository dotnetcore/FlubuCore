using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Tasks.Process;
using Microsoft.DotNet.Cli.Utils;

namespace FlubuCore.Infrastructure
{
    public class ComponentProvider : IComponentProvider
    {
        private readonly ICommandFactory _commandFactory;

        public ComponentProvider(ICommandFactory commandFactory)
        {
            _commandFactory = commandFactory;
        }

        public IRunProgramTask CreateRunProgramTask(string programToExecute)
        {
            return new RunProgramTask(_commandFactory, programToExecute);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Tasks.NetCore;

namespace FlubuCore.Context
{
    public class CoreTaskFluentInterface
    {
        private readonly TaskContext _context;

        public CoreTaskFluentInterface(TaskContext context)
        {
            _context = context;
        }

        public ExecuteDotnetTask ExecuteDotnetTask(string command)
        {
            return _context.CreateTask<ExecuteDotnetTask>(command);
        }

        public ExecuteDotnetTask ExecuteDotnetTask(StandardDotnetCommands command)
        {
            return _context.CreateTask<ExecuteDotnetTask>(command);
        }
    }
}

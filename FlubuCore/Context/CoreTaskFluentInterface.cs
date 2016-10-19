using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Testing;
using FlubuCore.Tasks.Versioning;

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

        public UpdateNetCoreVersionTask UpdateNetCoreVersionTask(string filePath)
        {
            return _context.CreateTask<UpdateNetCoreVersionTask>(filePath);
        }

        public UpdateNetCoreVersionTask UpdateNetCoreVersionTask(params string[] files)
        {
            return _context.CreateTask<UpdateNetCoreVersionTask>(files);
        }
    }
}

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
    public class CoreTaskFluentInterface : ICoreTaskFluentInterface
    {
        public TaskContext Context { get; set; }

        public ExecuteDotnetTask ExecuteDotnetTask(string command)
        {
            return Context.CreateTask<ExecuteDotnetTask>(command);
        }

        public ExecuteDotnetTask ExecuteDotnetTask(StandardDotnetCommands command)
        {
            return Context.CreateTask<ExecuteDotnetTask>(command);
        }

        public UpdateNetCoreVersionTask UpdateNetCoreVersionTask(string filePath)
        {
            return Context.CreateTask<UpdateNetCoreVersionTask>(filePath);
        }

        public UpdateNetCoreVersionTask UpdateNetCoreVersionTask(params string[] files)
        {
            return Context.CreateTask<UpdateNetCoreVersionTask>(files);
        }
    }
}

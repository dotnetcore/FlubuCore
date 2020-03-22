using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks;
using Microsoft.Extensions.Logging;

namespace FlubuCore.Scripting
{
    public interface IFlubuEngine
    {
        ITaskFactory TaskFactory { get; }

        IServiceProvider ServiceProvider { get; }

        ILoggerFactory LoggerFactory { get; }

        IFlubuSession CreateTaskSession(BuildScriptArguments commandArguments);

        int RunScript<T>(string[] args)
            where T : IBuildScript, new();
    }
}

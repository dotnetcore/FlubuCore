using System.IO;
using Microsoft.DotNet.Cli.Utils;
using NuGet.Frameworks;

namespace Flubu.Tasks.Dotnet
{
    public class ExecuteDotnetTask : TaskBase
    {
        public ExecuteDotnetTask(string command)
        {
            Command = command;
        }

        public override string Description => $"Execute dotnet command";

        public string Command { get; set; }

        protected override void DoExecute(ITaskContext context)
        {
        }
    }
}
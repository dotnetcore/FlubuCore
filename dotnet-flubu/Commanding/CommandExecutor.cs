using System.Threading.Tasks;
using Flubu.Scripting;
using Microsoft.Extensions.Logging;

namespace Flubu.Commanding
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly IBuildScriptLocator locator;
        private readonly ILogger<CommandExecutor> log;
        private readonly IFlubuCommandParser parser;

        public CommandExecutor(
            IFlubuCommandParser parser,
            IBuildScriptLocator locator,
            ILogger<CommandExecutor> log)
        {
            this.parser = parser;
            this.locator = locator;
            this.log = log;
        }

        public async Task<int> Execute(string[] args)
        {
            var commands = parser.Parse(args);

            if (commands.Help)
            {
                return 1;
            }

            var script = await locator.FindBuildScript(commands);

            if (script == null)
            {
                log.LogInformation("Script not found!");
                return -1;
            }

            return script.Run(commands);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Commanding;
using FlubuCore.Scripting;
using FlubuCore.WebApi.Model;
using Microsoft.AspNetCore.Mvc;

namespace FlubuCore.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ScriptsController : Controller
    {
        private ICommandExecutor _commandExecutor;

        private CommandArguments _commandArguments;

        public ScriptsController(ICommandExecutor commandExecutor, CommandArguments commandArguments)
        {
            _commandExecutor = commandExecutor;
            _commandArguments = commandArguments;
        }

        [HttpPost("Execute")]
        public async Task<IActionResult> Execute([FromBody]ExecuteScriptRequest request)
        {
            _commandArguments.MainCommand = request.MainCommand;
            _commandArguments.Script = request.ScriptFilePathLocation;
            _commandArguments.RemainingCommands = request.RemainingCommands;
            await _commandExecutor.ExecuteAsync();

            return Ok();
        }
    }
}
